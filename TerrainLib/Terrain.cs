﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

using SharpDX;

using UtilityLib;
using MaterialLib;

using MatLib	=MaterialLib.MaterialLib;


namespace TerrainLib
{
	class ChunkState
	{
		internal int	mChunkX, mChunkY;

		internal GraphicsDevice	mGD;
	}

	class BuildState
	{
		internal int			mDestroyAt;
		internal Point			mCellCoord;
		internal GraphicsDevice	mGD;
	}


	//constructs and stores multiple heightmaps
	//handles the tiling and other such
	public class Terrain
	{
		//dimensions of the vertexbuffer chunks that are frust rejected
		int		mChunkDim, mPolySize;

		//the raw data, might be very large
		float	[,]mHeightData;
		Vector3	[,]mNormals;

		//grid of heights
		//only the nearby cells are kept in memory
		HeightMap[,]	mStreamMaps;

		//texturing data
		List<HeightMap.TexData>	mTexData	=new List<HeightMap.TexData>();

		//blending between layers
		float	mTransitionHeight;

		//locations
		Point	mCellCoord;
		Vector3	mLevelPos;

		//thread stuff
		bool	mbBuilding;
		int		mThreadCounter;
		int		mThreadsActive;

		const int	MaxThreads	=2;


		//set up textures and such
		public Terrain(float [,]data, Vector3 [,]norms,
			int polySize, int chunkDim, int cellGridMax)
		{
			mChunkDim	=chunkDim;
			mHeightData	=data;
			mPolySize	=polySize;
			mNormals	=norms;

			mStreamMaps	=new HeightMap[cellGridMax, cellGridMax];
		}


		public void FreeAll()
		{
			while(mbBuilding)
			{
				Thread.Sleep(1);
			}

			mHeightData	=null;

			foreach(HeightMap hm in mStreamMaps)
			{
				if(hm != null)
				{
					hm.FreeAll();
				}
			}

			mTexData.Clear();
		}


		public List<HeightMap.TexData> GetTextureData(out float transitionHeight)
		{
			transitionHeight	=mTransitionHeight;

			return	mTexData;
		}


		public void SetTextureData(List<HeightMap.TexData> texData, float transHeight)
		{
			mTexData			=texData;
			mTransitionHeight	=transHeight;
		}


		public void SetCellCoord(Point cellCoord)
		{
			mCellCoord	=cellCoord;

			int	cw	=mStreamMaps.GetLength(1);
			int	ch	=mStreamMaps.GetLength(0);

			foreach(HeightMap hm in mStreamMaps)
			{
				if(hm == null)
				{
					continue;
				}

				hm.SetRelativePos(cellCoord, cw, ch, mChunkDim, mPolySize);
			}
		}


		public int GetThreadsActive()
		{
			return	mThreadsActive;
		}


		public int GetThreadCounter()
		{
			return	mThreadCounter;
		}


		//streams in nearby stuff, and nukes stuff at
		//destroyAt and beyond, should be called at a
		//boundary crossing
		public void BuildGrid(GraphicsDevice gd, int destroyAt)
		{
			if(mbBuilding)
			{
				return;
			}
			mbBuilding	=true;

			BuildState	bs	=new BuildState();
			bs.mGD			=gd;
			bs.mDestroyAt	=destroyAt;
			bs.mCellCoord	=mCellCoord;

			ThreadPool.QueueUserWorkItem(BuildGridCB, bs);
		}


		void BuildGridCB(object state)
		{
			BuildState	bs	=state as BuildState;
			if(bs == null)
			{
				return;
			}

			int	cw	=mStreamMaps.GetLength(1);
			int	ch	=mStreamMaps.GetLength(0);

			//blast cells outside range
			for(int y=0;y < ch;y++)
			{
				for(int x=0;x < cw;x++)
				{
					int	xWrapNeg	=x - cw;
					int	yWrapNeg	=y - ch;
					int	xWrapPos	=x + cw;
					int	yWrapPos	=y + ch;

					int	xDist	=Math.Abs(bs.mCellCoord.X - x);
					int	yDist	=Math.Abs(bs.mCellCoord.Y - y);

					int	xDistWN	=Math.Abs(bs.mCellCoord.X - xWrapNeg);
					int	yDistWN	=Math.Abs(bs.mCellCoord.Y - yWrapNeg);

					int	xDistWP	=Math.Abs(bs.mCellCoord.X - xWrapPos);
					int	yDistWP	=Math.Abs(bs.mCellCoord.Y - yWrapPos);

					if(xDist > xDistWN)
					{
						xDist	=xDistWN;
					}
					if(yDist > yDistWN)
					{
						yDist	=yDistWN;
					}
					if(xDist > xDistWP)
					{
						xDist	=xDistWP;
					}
					if(yDist > yDistWP)
					{
						yDist	=yDistWP;
					}

					if(xDist >= bs.mDestroyAt || yDist >= bs.mDestroyAt)
					{
						if(mStreamMaps[y, x] != null)
						{
							mStreamMaps[y, x].FreeAll();
							mStreamMaps[y, x]	=null;
						}
					}
				}
			}

			GC.Collect(1);

			int	w	=mHeightData.GetLength(1);
			int	h	=mHeightData.GetLength(0);

			//set to nearest power of two
			int	pow	=0;
			while(w > 0)
			{
				w	>>=1;
				pow++;
			}
			w	=1 << (pow - 1);

			pow	=0;
			while(h > 0)
			{
				h	>>=1;
				pow++;
			}
			h	=1 << (pow - 1);

			int	inRange	=bs.mDestroyAt - 1;

			//clamp inrange within the size of the grid
			int	smallDim	=Math.Min(w, h);

			smallDim	/=mChunkDim;

			inRange	=Math.Min(inRange, smallDim);

			mThreadCounter	=1 + 2 * inRange;
			mThreadsActive	=0;

			mThreadCounter	*=mThreadCounter;

			for(int cellY = bs.mCellCoord.Y - inRange;cellY <= bs.mCellCoord.Y + inRange;cellY++)
			{
				for(int cellX = bs.mCellCoord.X - inRange;cellX <= bs.mCellCoord.X + inRange;cellX++)
				{
					int	wCellX	=cellX;
					int	wCellY	=cellY;

					//wrap
					if(wCellX >= cw)
					{
						wCellX	-=cw;
					}
					else if(wCellX < 0)
					{
						wCellX	+=cw;
					}

					if(wCellY >= ch)
					{
						wCellY	-=ch;
					}
					else if(wCellY < 0)
					{
						wCellY	+=ch;
					}

					if(mStreamMaps[wCellY, wCellX] != null)
					{
						Interlocked.Decrement(ref mThreadCounter);
						continue;
					}

					ChunkState	cs	=new ChunkState();
					cs.mChunkX		=wCellX;
					cs.mChunkY		=wCellY;
					cs.mGD			=bs.mGD;

					//limit to MaxThreads
					while(mThreadsActive >= MaxThreads)
					{
						Thread.Sleep(2);
						GC.Collect();
					}

					Interlocked.Increment(ref mThreadsActive);
					ThreadPool.QueueUserWorkItem(DoChunk, cs);
				}
			}

			while(!Interlocked.Equals(mThreadCounter, 0))
			{
				Thread.Sleep(2);
			}
			mbBuilding	=false;
		}


		void DoChunk(object state)
		{
			ChunkState	cs	=state as ChunkState;
			if(cs == null)
			{
				return;
			}

			int	w	=mHeightData.GetLength(1);
			int	h	=mHeightData.GetLength(0);

			Point	coord	=new Point(cs.mChunkX, cs.mChunkY);

			HeightMap	map	=new HeightMap(mHeightData, mNormals,
				coord, mChunkDim,
				w, h,
				mChunkDim, mChunkDim,
				mPolySize, mTransitionHeight,
				mTexData,
				cs.mGD);

//			Vector3	pos	=Vector3.Zero;
//			pos.X	=cs.mChunkX * (mChunkDim) * mPolySize;
//			pos.Z	=cs.mChunkY * (mChunkDim) * mPolySize;
//			pos.Y	=0.0f;

//			map.SetPos(pos);

			lock(mStreamMaps)
			{
				mStreamMaps[cs.mChunkY, cs.mChunkX]	=map;
			}

			Interlocked.Decrement(ref mThreadsActive);
			Interlocked.Decrement(ref mThreadCounter);
		}


//		public Vector3 GetGoodColorForHeight(float height)
//		{
//			return	mStreamMaps[0, 0].GetGoodColorForHeight(height);
//		}


		public void GetTimings(out long pos, out long texFact,
			out long index, out long buffer)
		{
			long	posAccum, tfAccum;
			long	indAccum, bufAccum;

			pos	=texFact	=index	=buffer	=0;

			foreach(HeightMap hm in mStreamMaps)
			{
				if(hm == null)
				{
					continue;
				}
				hm.GetTimings(out posAccum, out tfAccum,
					out indAccum, out bufAccum);

				pos		+=posAccum;
				texFact	+=tfAccum;
				index	+=indAccum;
				buffer	+=bufAccum;
			}
		}


		void InitEffect(MatLib mat)
		{
			/*
			mFXTerrain	=cm.Load<Effect>("Shaders/Terrain");

			//set up shader stuff that won't change for now
			Vector4	colr	=Vector4.Zero;
			colr.X	=1.0f;
			colr.Y	=1.0f;
			colr.Z	=1.0f;
			colr.W	=1.0f;
			mFXTerrain.Parameters["mLightColor"].SetValue(colr);

			colr.X	=0.2f;
			colr.Y	=0.2f;
			colr.Z	=0.2f;
			colr.W	=1.0f;
			mFXTerrain.Parameters["mAmbientColor"].SetValue(colr);

			Vector3	dir	=Vector3.Zero;
			dir.X	=0.4f;
			dir.Z	=-0.2f;
			dir.Y	=-0.4f;
			dir.Normalize();

			mFXTerrain.Parameters["mLightDirection"].SetValue(dir);

			if(mTEXAtlas != null)
			{
				mFXTerrain.Parameters["mTexAtlas"].SetValue(mTEXAtlas);
			}

			//fog stuff
			Vector3	fogColor	=Vector3.Zero;
			fogColor.X	=0.8f;
			fogColor.Y	=0.9f;
			fogColor.Z	=1.0f;
			mFXTerrain.Parameters["mFogEnabled"].SetValue(1.0f);
			mFXTerrain.Parameters["mFogStart"].SetValue(500.0f);
			mFXTerrain.Parameters["mFogEnd"].SetValue(1000.0f);
			mFXTerrain.Parameters["mFogColor"].SetValue(fogColor);
			mFXTerrain.Parameters["mEyePos"].SetValue(Vector3.Zero);*/
		}


		public void Draw(GraphicsDevice gd, MatLib mats, BoundingFrustum frust)
		{
			foreach(HeightMap m in mStreamMaps)
			{
				if(m == null)
				{
					continue;
				}
				if(m.InFrustum(frust))
				{
					m.Draw(gd.DC, mats, Matrix.Identity,
						gd.GCam.View, gd.GCam.Projection);
				}
				//for testing bad boundboxes
				/*
				else
				{
					SetFogEnabled(false);
					m.Draw(gd, mFXTerrain);
					SetFogEnabled(true);
				}*/
			}
		}


		public void DrawWorldY(GraphicsDevice gd, BoundingFrustum frust)
		{
			/*
			mFXTerrain.CurrentTechnique	=mFXTerrain.Techniques["WorldY"];

			foreach(HeightMap m in mStreamMaps)
			{
				if(m == null)
				{
					continue;
				}
				if(m.InFrustum(frust))
				{
					m.Draw(gd, mFXTerrain);
				}
			}*/
		}


		//get the peak of the heightmap closest to pos
		public float GetLocalHeight(Vector3 pos)
		{
			HeightMap	closeMap	=null;
			float		closest		=696969.0f;
			foreach(HeightMap m in mStreamMaps)
			{
				if(m == null)
				{
					continue;
				}
				Vector3	dist	=m.GetPos();
				dist	+=mLevelPos;
				dist	+=pos;

				float	len	=dist.Length();

				if(len < closest)
				{
					closest		=len;
					closeMap	=m;
				}
			}
			if(closeMap != null)
			{
				return	closeMap.GetPeak();
			}
			else
			{
				return	6969.0f;
			}
		}


		public void UpdateLightColor(Vector4 lightColor)
		{
//			mFXTerrain.Parameters["mLightColor"].SetValue(lightColor);
		}


		public void UpdateLightDirection(Vector3 lightDir)
		{
//			mFXTerrain.Parameters["mLightDirection"].SetValue(lightDir);
		}


		public void UpdateAmbientColor(Vector4 ambient)
		{
//			mFXTerrain.Parameters["mAmbientColor"].SetValue(ambient);
		}


		public void UpdatePosition(Vector3 pos, MatLib mats)
		{
			Matrix	mat	=Matrix.Translation(pos);

			mLevelPos	=pos;
		}


		public void SetFogDetails(float fogStart, float fogEnd, Vector3 col)
		{
//			mFXTerrain.Parameters["mFogStart"].SetValue(fogStart);
//			mFXTerrain.Parameters["mFogEnd"].SetValue(fogEnd);
//			mFXTerrain.Parameters["mFogColor"].SetValue(col);
		}


		public void SetSkyFogDetails(float fogStart, float fogEnd, Vector3 col0, Vector3 col1)
		{
//			mFXTerrain.Parameters["mFogStart"].SetValue(fogStart);
//			mFXTerrain.Parameters["mFogEnd"].SetValue(fogEnd);
//			mFXTerrain.Parameters["mSkyGradient0"].SetValue(col0);
//			mFXTerrain.Parameters["mSkyGradient1"].SetValue(col1);
		}


		public void SetFogEnabled(bool bFog)
		{
			if(bFog)
			{
//				mFXTerrain.Parameters["mFogEnabled"].SetValue(1.0f);
			}
			else
			{
//				mFXTerrain.Parameters["mFogEnabled"].SetValue(0.0f);
			}
		}
	}
}
