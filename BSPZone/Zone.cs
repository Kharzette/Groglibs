﻿using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UtilityLib;


namespace BSPZone
{
	internal class ZoneTrigger
	{
		internal ZoneEntity		mEntity;
		internal BoundingBox	mBox, mTransformedBox;
		internal Int32			mModelNum;
		internal bool			mbTriggered;
		internal bool			mbTriggerOnce;
		internal bool			mbTriggerStandIn;
		internal int			mTimeSinceTriggered;
		internal int			mWait;
	}

	public partial class Zone
	{
		class WorldLeaf
		{
//			public Int32	mVisFrame;
			public Int32	mParent;
		}

		//structural
		ZoneModel		[]mZoneModels;
		ZoneNode		[]mZoneNodes;
		ZoneLeaf		[]mZoneLeafs;
		ZoneLeafSide	[]mZoneLeafSides;
		ZonePlane		[]mZonePlanes;
		ZoneEntity		[]mZoneEntities;

		//debug vis stuff
		Int32			[]mDebugLeafFaces;
		DebugFace		[]mDebugFaces;
		Vector3			[]mDebugVerts;
		Int32			[]mDebugIndexes;
		VisCluster		[]mVisClusters;
		VisArea			[]mVisAreas;
		VisAreaPortal	[]mVisAreaPortals;

		//vis stuff
		Int32		[]mClusterVisFrame;
		WorldLeaf	[]mLeafData;
		Int32		[]mNodeParents;
		Int32		[]mNodeVisFrame;
		byte		[]mVisData;
		byte		[]mMaterialVisData;

		//gameplay stuff
		List<ZoneTrigger>	mTriggers	=new List<ZoneTrigger>();
		BoundingBox			mPushable;
		Vector3				mPushableWorldCenter;
		int					mPushableModelOn;

		int	mLightMapGridSize;
		int	mNumVisLeafBytes;
		int	mNumVisMaterialBytes;

		public event EventHandler	eTriggerHit;
		public event EventHandler	eTriggerOutOfRange;
		public event EventHandler	ePushObject;

		const float	GroundAngle				=0.8f;	//how sloped can you be to be considered ground
		const float	RampAngle				=0.7f;	//how steep can we climb?
		const float StepHeight				=18.0f;	//stair step height for bipeds
		const int	MaxMoveBoxIterations	=64;


		#region IO
		void WritePlaneArray(BinaryWriter bw)
		{
			bw.Write(mZonePlanes.Length);
			for(int i=0;i < mZonePlanes.Length;i++)
			{
				mZonePlanes[i].Write(bw);
			}
		}


		void ReadPlaneArray(BinaryReader br)
		{
			int	count	=br.ReadInt32();
			mZonePlanes	=new ZonePlane[count];
			for(int i=0;i < count;i++)
			{
				mZonePlanes[i].Read(br);
			}
		}


		public void Write(string fileName, bool bDebug)
		{
			FileStream	file	=new FileStream(fileName, FileMode.Create, FileAccess.Write);

			BinaryWriter	bw	=new BinaryWriter(file);

			FileUtil.WriteArray(mZoneModels, bw);
			FileUtil.WriteArray(mZoneNodes, bw);
			FileUtil.WriteArray(mZoneLeafs, bw);
			FileUtil.WriteArray(mVisAreas, bw);
			FileUtil.WriteArray(mVisAreaPortals, bw);
			WritePlaneArray(bw);
			FileUtil.WriteArray(mZoneEntities, bw);
			FileUtil.WriteArray(mZoneLeafSides, bw);

			bw.Write(bDebug);
			if(bDebug)
			{
				FileUtil.WriteArray(bw, mDebugLeafFaces);
				FileUtil.WriteArray(mDebugFaces, bw);
				FileUtil.WriteArray(bw, mDebugVerts);
				FileUtil.WriteArray(bw, mDebugIndexes);
			}

			if(mVisData != null && mVisData.Length > 0)
			{
				bw.Write(true);
				FileUtil.WriteArray(mVisData, bw);
			}
			else
			{
				bw.Write(false);
			}

			if(mMaterialVisData != null && mMaterialVisData.Length > 0)
			{
				bw.Write(true);
				FileUtil.WriteArray(mMaterialVisData, bw);
			}
			else
			{
				bw.Write(false);
			}
			FileUtil.WriteArray(mVisClusters, bw);
			bw.Write(mLightMapGridSize);
			bw.Write(mNumVisLeafBytes);
			bw.Write(mNumVisMaterialBytes);

			bw.Close();
			file.Close();
		}


		public void Read(string fileName, bool bTool)
		{
			Stream			file	=null;
			if(bTool)
			{
				file	=new FileStream(fileName, FileMode.Open, FileAccess.Read);
			}
			else
			{
				file	=FileUtil.OpenTitleFile(fileName);
			}

			if(file == null)
			{
				return;
			}
			BinaryReader	br	=new BinaryReader(file);

			mZoneModels		=FileUtil.ReadArray(br, delegate(Int32 count)
							{ return FileUtil.InitArray<ZoneModel>(count); }) as ZoneModel[];
			mZoneNodes		=FileUtil.ReadArray(br, delegate(Int32 count)
							{ return FileUtil.InitArray<ZoneNode>(count); }) as ZoneNode[];
			mZoneLeafs		=FileUtil.ReadArray(br, delegate(Int32 count)
							{ return FileUtil.InitArray<ZoneLeaf>(count); }) as ZoneLeaf[];
			mVisAreas		=FileUtil.ReadArray(br, delegate(Int32 count)
							{ return FileUtil.InitArray<VisArea>(count); }) as VisArea[];
			mVisAreaPortals	=FileUtil.ReadArray(br, delegate(Int32 count)
							{ return FileUtil.InitArray<VisAreaPortal>(count); }) as VisAreaPortal[];
			ReadPlaneArray(br);
			mZoneEntities	=FileUtil.ReadArray(br, delegate(Int32 count)
							{ return FileUtil.InitArray<ZoneEntity>(count); }) as ZoneEntity[];
			mZoneLeafSides	=FileUtil.ReadArray(br, delegate(Int32 count)
							{ return FileUtil.InitArray<ZoneLeafSide>(count); }) as ZoneLeafSide[];

			bool	bDebug	=br.ReadBoolean();
			if(bDebug)
			{
				mDebugLeafFaces	=FileUtil.ReadIntArray(br);
				mDebugFaces		=FileUtil.ReadArray(br, delegate(Int32 count)
							{ return FileUtil.InitArray<DebugFace>(count); }) as DebugFace[];
				mDebugVerts		=FileUtil.ReadVecArray(br);
				mDebugIndexes	=FileUtil.ReadIntArray(br);
			}

			mVisData			=FileUtil.ReadByteArray(br);
			mMaterialVisData	=FileUtil.ReadByteArray(br);

			mVisClusters	=FileUtil.ReadArray(br, delegate(Int32 count)
							{ return FileUtil.InitArray<VisCluster>(count); }) as VisCluster[];

			mLightMapGridSize		=br.ReadInt32();
			mNumVisLeafBytes		=br.ReadInt32();
			mNumVisMaterialBytes	=br.ReadInt32();

			br.Close();
			file.Close();

			//make clustervisframe
			mClusterVisFrame	=new int[mVisClusters.Length];
			mNodeParents		=new int[mZoneNodes.Length];
			mNodeVisFrame		=new int[mZoneNodes.Length];
			mLeafData			=new WorldLeaf[mZoneLeafs.Length];

			//fill in leafdata with blank worldleafs
			for(int i=0;i < mZoneLeafs.Length;i++)
			{
				mLeafData[i]	=new WorldLeaf();
			}

			FindParents_r(mZoneModels[0].mRootNode, -1);

			//grab out triggers
			List<ZoneEntity>	trigs	=GetEntitiesStartsWith("trigger");
			foreach(ZoneEntity ze in trigs)
			{
				if(ze.mData.ContainsKey("Model"))
				{
					ZoneTrigger	zt		=new ZoneTrigger();
					string	modelNum	=ze.mData["Model"];
					zt.mModelNum		=Convert.ToInt32(modelNum);
					zt.mBox				=GetModelBounds(zt.mModelNum);
					zt.mEntity			=ze;
					zt.mbTriggered		=false;
					zt.mbTriggerOnce	=(ze.mData["classname"] == "trigger_once");
					zt.mbTriggerStandIn	=(ze.mData["classname"] == "trigger_gravity");	//hax

					if(ze.mData.ContainsKey("wait"))
					{
						if(Mathery.TryParse(ze.mData["wait"], out zt.mWait))
						{
							//bump to milliseconds
							zt.mWait	*=1000;
						}
					}

					mTriggers.Add(zt);
				}
			}
		}
		#endregion


		#region Entity Related
		public List<ZoneEntity> GetSwitchedOnLights()
		{
			List<ZoneEntity>	ret	=new List<ZoneEntity>();
			foreach(ZoneEntity e in mZoneEntities)
			{
				int	switchNum;
				if(!e.GetInt("LightSwitchNum", out switchNum))
				{
					continue;
				}

				int	spawnFlags;
				if(e.GetInt("spawnflags", out spawnFlags))
				{
					if(Misc.bFlagSet(spawnFlags, 1))
					{
						continue;
					}
				}

				//no flags at all means start on
				ret.Add(e);
			}
			return	ret;
		}


		void CollideModel(int modelIndex, Matrix newTrans, Matrix oldInv)
		{
			//transform into new rotated model space
			Vector3	newCenter	=Vector3.Transform(mPushableWorldCenter, oldInv);

			//transform back to world space vs old matrix
			newCenter	=Vector3.Transform(newCenter, newTrans);

			Vector3	pushedPos		=Vector3.Zero;

			//push the starting point back a bit along the frame of rev vector
			Vector3	dirVec	=newCenter - mPushableWorldCenter;

			if(dirVec.LengthSquared() == 0f)
			{
				return;
			}

			dirVec.Normalize();
			dirVec	*=(mPushable.Max.X * 0.5f);	//to adjust for corner expansion / contraction

			Vector3	targPos	=newCenter + dirVec;

			//collide vs the rotated model with new -> old ray
			BipedModelPush(mPushable, targPos, mPushableWorldCenter, modelIndex, ref pushedPos);

			if(pushedPos != mPushableWorldCenter)
			{
				Misc.SafeInvoke(ePushObject, new Nullable<Vector3>(pushedPos - mPushableWorldCenter));				
			}
		}


		//this is an actual move, not a delta
		public void MoveModelTo(int modelIndex, Vector3 pos)
		{
			if(mZoneModels.Length <= modelIndex)
			{
				return;
			}

			ZoneModel	zm	=mZoneModels[modelIndex];

			Matrix	oldMatInv	=zm.mInvertedTransform;
			Vector3	oldPos		=zm.mPosition;

			//do the actual move
			zm.SetPosition(pos);

			//if player is riding on this model, move them too
			if(mPushableModelOn == modelIndex)
			{
				Misc.SafeInvoke(ePushObject, new Nullable<Vector3>(pos - oldPos));
			}

			Matrix	newMat		=zm.mTransform;

			CollideModel(modelIndex, newMat, oldMatInv);
		}


		public void RotateModelX(int modelIndex, float degrees)
		{
			if(mZoneModels.Length <= modelIndex)
			{
				return;
			}

			ZoneModel	zm	=mZoneModels[modelIndex];

			Matrix	oldMatInv	=zm.mInvertedTransform;

			//do the actual rotation
			zm.RotateX(degrees);

			Matrix	newMat		=zm.mTransform;

			CollideModel(modelIndex, newMat, oldMatInv);
		}


		public void RotateModelY(int modelIndex, float degrees)
		{
			if(mZoneModels.Length <= modelIndex)
			{
				return;
			}

			ZoneModel	zm	=mZoneModels[modelIndex];

			Matrix	oldMatInv	=zm.mInvertedTransform;

			//do the actual rotation
			zm.RotateY(degrees);

			Matrix	newMat		=zm.mTransform;

			CollideModel(modelIndex, newMat, oldMatInv);
		}


		public void RotateModelZ(int modelIndex, float degrees)
		{
			if(mZoneModels.Length <= modelIndex)
			{
				return;
			}

			ZoneModel	zm	=mZoneModels[modelIndex];

			Matrix	oldMatInv	=zm.mInvertedTransform;

			//do the actual rotation
			zm.RotateZ(degrees);

			Matrix	newMat		=zm.mTransform;

			CollideModel(modelIndex, newMat, oldMatInv);
		}


		public Matrix GetModelTransform(int modelIndex)
		{
			if(modelIndex >= mZoneModels.Length)
			{
				return	Matrix.Identity;
			}

			return	mZoneModels[modelIndex].mTransform;
		}


		public Vector3 GetPlayerStartPos()
		{
			foreach(ZoneEntity e in mZoneEntities)
			{
				if(e.mData.ContainsKey("classname"))
				{
					if(e.mData["classname"] != "info_player_start")
					{
						continue;
					}
				}
				else
				{
					continue;
				}

				Vector3	ret	=Vector3.Zero;
				if(e.GetOrigin(out ret))
				{
					return	ret;
				}
			}
			return	Vector3.Zero;
		}


		public BoundingBox GetModelBounds(int modelNum)
		{
			if(modelNum > 0 && modelNum < mZoneModels.Length)
			{
				return	mZoneModels[modelNum].mBounds;
			}
			return	new BoundingBox();
		}


		public List<ZoneEntity> GetEntitiesByTargetName(string targName)
		{
			List<ZoneEntity>	ret	=new List<ZoneEntity>();
			foreach(ZoneEntity ze in mZoneEntities)
			{
				if(ze.mData.ContainsKey("targetname"))
				{
					if(ze.mData["targetname"] == targName)
					{
						ret.Add(ze);
					}
				}
			}
			return	ret;
		}


		public List<ZoneEntity> GetEntities(string className)
		{
			List<ZoneEntity>	ret	=new List<ZoneEntity>();
			foreach(ZoneEntity ze in mZoneEntities)
			{
				if(ze.mData.ContainsKey("classname"))
				{
					if(ze.mData["classname"] == className)
					{
						ret.Add(ze);
					}
				}
			}
			return	ret;
		}


		public List<ZoneEntity> GetEntitiesStartsWith(string startText)
		{
			List<ZoneEntity>	ret	=new List<ZoneEntity>();
			foreach(ZoneEntity ze in mZoneEntities)
			{
				if(ze.mData.ContainsKey("classname"))
				{
					if(ze.mData["classname"].StartsWith(startText))
					{
						ret.Add(ze);
					}
				}
			}
			return	ret;
		}


		//for assigning character lights
		public List<Vector3> GetNearestThreeLightsInLOS(Vector3 pos)
		{
			List<ZoneEntity>	lightsInVis	=new List<ZoneEntity>();

			foreach(ZoneEntity ent in mZoneEntities)
			{
				if(ent.IsLight())
				{
					Vector3	lightPos;
					if(ent.GetOrigin(out lightPos))
					{
						if(IsVisibleFrom(pos, lightPos))
						{
							Vector3	intersection	=Vector3.Zero;
							bool	bHitLeaf		=false;
							Int32	leafHit			=0;
							Int32	nodeHit			=0;
							if(!RayIntersect(pos, lightPos, 0, ref intersection,
								ref bHitLeaf, ref leafHit, ref nodeHit))
							{
								lightsInVis.Add(ent);
							}
						}
					}
				}
			}

			List<Vector3>	positions	=new List<Vector3>();

			foreach(ZoneEntity ent in mZoneEntities)
			{
				Vector3	lightPos;
				if(ent.GetOrigin(out lightPos))
				{
					positions.Add(lightPos);
				}
			}

			positions.Sort();

			if(positions.Count > 3)
			{
				positions.RemoveRange(3, positions.Count - 3);
			}
			return	positions;
		}


		public ZoneEntity GetNearestLightInLOS(Vector3 pos)
		{
			List<ZoneEntity>	lightsInVis	=new List<ZoneEntity>();

			foreach(ZoneEntity ent in mZoneEntities)
			{
				if(ent.IsLight())
				{
					Vector3	lightPos;
					if(ent.GetOrigin(out lightPos))
					{
						if(IsVisibleFrom(pos, lightPos))
						{
							Vector3	intersection	=Vector3.Zero;
							bool	bHitLeaf		=false;
							Int32	leafHit			=0;
							Int32	nodeHit			=0;
							if(!RayIntersect(pos, lightPos, 0, ref intersection,
								ref bHitLeaf, ref leafHit, ref nodeHit))
							{
								lightsInVis.Add(ent);
							}
						}
					}
				}
			}

			float		minDist	=float.MaxValue;
			ZoneEntity	ret		=null;
			foreach(ZoneEntity ent in lightsInVis)
			{
				Vector3	lightPos;
				if(ent.GetOrigin(out lightPos))
				{
					float	dist	=Vector3.DistanceSquared(lightPos, pos);
					if(dist < minDist)
					{
						minDist	=dist;
						ret		=ent;
					}
				}
			}
			return	ret;
		}
		#endregion


		#region Ray Casts and Movement
		public void SetPushable(BoundingBox box, Vector3 center, int modelOn)
		{
			mPushable				=box;
			mPushableWorldCenter	=center;
			mPushableModelOn		=modelOn;
		}


		public bool IsSphereInSolid(Vector3 pnt, float dist)
		{
			bool	bHitLeaf	=false;
			int		leafHit		=0;
			int		nodeHit		=0;
			return	SphereIntersect(pnt, dist, 0, ref bHitLeaf, ref leafHit, ref nodeHit);
		}


		//this is faked with a box for now
		//real capsule cast is broken
		public bool CapsuleCollide(Vector3 start, Vector3 end, float radius,
			ref Vector3 intersect, ref ZonePlane hitPlane)
		{
			return	Trace_WorldCollisionCapsule(start, end, radius, ref intersect, ref hitPlane);
		}


		public bool IsPointInSolid(Vector3 pnt)
		{
			int	node	=FindNodeLandedIn(0, pnt);

			if(node > 0)
			{
				return	true;	//is that right?  Can't remember
			}
			Int32	leaf	=-(node + 1);

			return	((mZoneLeafs[leaf].mContents & Contents.BSP_CONTENTS_SOLID2) != 0);
		}


		public bool RayCollide(Vector3 Front, Vector3 Back,
			ref Vector3 I, ref Int32 leafHit, ref Int32 nodeHit)
		{
			bool	hitLeaf	=false;
			if(RayIntersect(Front, Back, mZoneModels[0].mRootNode,
				ref I, ref hitLeaf, ref leafHit, ref nodeHit))
			{
				return	true;
			}
			return	false;
		}


		bool IsGround(ZonePlane p)
		{
			return	(Vector3.Dot(p.mNormal, Vector3.UnitY) > GroundAngle);
		}


		bool FootCheck(BoundingBox box, Vector3 footPos, float dist, out int modelOn)
		{
			//see if the feet are still on the ground
			Vector3		footCheck	=footPos - Vector3.UnitY * dist;
			ZonePlane	footPlane	=ZonePlane.BlankX;
			Vector3		impVec		=Vector3.Zero;

			modelOn	=-1;

			if(Trace_All(box, footPos, footCheck, ref modelOn, ref impVec, ref footPlane))
			{
				if(IsGround(footPlane))
				{
					return	true;
				}
				else
				{
					modelOn	=-1;
				}
			}
			return	false;
		}


		bool StairMove(BoundingBox box, Vector3 start, Vector3 end, Vector3 stairAxis,
			bool bSlopeOk, float stepHeight, float originalLenSquared, ref Vector3 stepPos, out int modelOn)
		{
			Vector3		impVec		=Vector3.Zero;
			ZonePlane	impPlane	=ZonePlane.BlankX;
			int			modelHit	=0;

			//first trace up from the start point
			//to make sure there's head room
			if(Trace_All(box, start, start + stairAxis * stepHeight, ref modelHit, ref impVec, ref impPlane))
			{
				//hit noggin, just use previous point
				modelOn	=-1;
				return	false;
			}

			//movebox from start step height to end step height
			stepPos	=Vector3.Zero;
			bool	bGroundStep	=MoveBox(box, start + stairAxis * stepHeight,
				end + stairAxis * stepHeight, out stepPos, out modelOn);

			if(!bGroundStep)
			{
				//trace down by step height and make sure
				//we land on a ground surface
				if(Trace_All(box, stepPos, stepPos - Vector3.UnitY * stepHeight,
					ref modelHit, ref impVec, ref impPlane))
				{
					if(IsGround(impPlane))
					{
						//landed on the ground
						stepPos		=impVec;
						bGroundStep	=true;
					}
					else
					{
						if(bSlopeOk)
						{
							//see if the plane has any footing at all
							if(Vector3.Dot(Vector3.UnitY, impPlane.mNormal) > RampAngle)
							{
								stepPos		=impVec;
								bGroundStep	=true;
							}
						}
					}
				}
			}

			Vector3	moveVec	=stepPos - start;
			if(!bGroundStep || moveVec.LengthSquared() <= originalLenSquared)
			{
				//earlier move was better
				return	false;
			}

			return	true;
		}


		//returns true if on ground
		//this one assumes 2 legs, so navigates stairs
		//TODO: This gets a bit strange on gentle slopes
		public bool BipedMoveBox(BoundingBox box, Vector3 start, Vector3 end,
			bool bPrevOnGround, out Vector3 finalPos, out bool bUsedStairs, ref int modelOn)
		{
			bUsedStairs	=false;

			//first check if we are moving at all
			Vector3	moveVec	=end - start;
			float	delt	=moveVec.LengthSquared();
			if(delt < Mathery.ANGLE_EPSILON)
			{
				//didn't move enough to bother
				finalPos	=start;
				return		bPrevOnGround;
			}

			//try the standard box move
			int		firstModelOn;
			bool	bGround	=MoveBox(box, start, end, out finalPos, out firstModelOn);

			//see how far it went
			moveVec	=finalPos - start;

			float	deltMove	=moveVec.LengthSquared();
			if(delt / deltMove < 1.333f)
			{
				//3/4 the movement energy at least was expended
				//good enough
				modelOn	=firstModelOn;
				return	bGround;
			}

			//see if original movement is mostly non vertical
			moveVec	=end - start;
			moveVec.Normalize();
			float	vert	=Vector3.Dot(Vector3.UnitY, moveVec);

			if(vert > RampAngle || vert < -RampAngle)
			{
				//no need to try stairs if just falling or climbing
				modelOn	=firstModelOn;
				return	bGround;
			}

			//only attempt stair stepping if biped was previously on ground
			if(!bPrevOnGround)
			{
				modelOn	=firstModelOn;
				return	bGround;
			}

			//try a step at a quarter stair height
			//this can get us over cracks where the
			//returned plane is one of the extra axials
			Vector3	stairPos	=Vector3.Zero;
			if(StairMove(box, start, end, Vector3.UnitY, true, StepHeight * 0.25f, deltMove, ref stairPos, out modelOn))
			{
				finalPos	=stairPos;
				bUsedStairs	=true;
				return		true;
			}

			//try a full step height
			if(StairMove(box, start, end, Vector3.UnitY, false, StepHeight, deltMove, ref stairPos, out modelOn))
			{
				finalPos	=stairPos;
				bUsedStairs	=true;
				return		true;
			}

			modelOn	=firstModelOn;

			if(bGround)
			{
				return	true;
			}

			//earlier move was better
			return	bGround;
		}


		//returns true if on ground
		//this one assumes 2 legs, so navigates stairs
		//Collides only against modelIndex
		//TODO: This gets a bit strange on gentle slopes
		public void BipedModelPush(BoundingBox box, Vector3 start,
			Vector3 end, int modelIndex, ref Vector3 finalPos)
		{
			//first check if we are moving at all
			Vector3	moveVec	=end - start;
			float	delt	=moveVec.LengthSquared();
			if(delt < Mathery.ANGLE_EPSILON)
			{
				//didn't move enough to bother
				finalPos	=start;
			}

			//try the standard box move
			MoveBoxModelPush(box, start, end, modelIndex, ref finalPos);
		}


		//positions should be in the middle base of the box
		//returns true if on the ground
		public void MoveBoxModelPush(BoundingBox box,
			Vector3 start, Vector3 end, int modelIndex, ref Vector3 finalPos)
		{
			Vector3		impacto		=Vector3.Zero;
			int			i			=0;

			List<ZonePlane>	hitPlanes	=new List<ZonePlane>();

			for(i=0;i < MaxMoveBoxIterations;i++)
			{
				ZonePlane	zp	=ZonePlane.Blank;
				if(!Trace_WorldCollisionFakeOBBox(box, modelIndex, start, end, ref impacto, ref zp))
				{
					break;
				}

				if(zp.mNormal == Vector3.Zero)
				{
					break;	//in solid
				}

				//adjust plane to worldspace
				zp	=ZonePlane.Transform(zp, mZoneModels[modelIndex].mTransform);

				float	startDist	=zp.DistanceFast(start);
				float	dist		=zp.DistanceFast(end);

				Debug.Assert(startDist > 0f && dist < 0f);

				end	-=(zp.mNormal * (dist - Mathery.VCompareEpsilon));
				
				if(!hitPlanes.Contains(zp))
				{
					hitPlanes.Add(zp);
				}
			}

			finalPos	=end;
			if(i == MaxMoveBoxIterations)
			{
				//can't solve!
				finalPos	=start;
			}
		}


		//positions should be in the middle base of the box
		//returns true if on the ground
		public bool MoveBox(BoundingBox box, Vector3 start,
							Vector3 end, out Vector3 finalPos, out int modelOn)
		{
			Vector3		impacto		=Vector3.Zero;
			int			i			=0;
			int			modelHit	=0;

			List<ZonePlane>	hitPlanes	=new List<ZonePlane>();

			for(i=0;i < MaxMoveBoxIterations;i++)
			{
				ZonePlane	zp	=ZonePlane.Blank;
				if(!Trace_All(box, start, end, ref modelHit, ref impacto, ref zp))
				{
					break;
				}

				if(zp.mNormal == Vector3.Zero)
				{
					break;	//in solid
				}

				float	startDist	=zp.DistanceFast(start);
				float	dist		=zp.DistanceFast(end);

				Debug.Assert(startDist > 0f && dist < 0f);

				end	-=(zp.mNormal * (dist - Mathery.VCompareEpsilon));
				
				if(!hitPlanes.Contains(zp))
				{
					hitPlanes.Add(zp);
				}
			}

			finalPos	=end;
			if(i == MaxMoveBoxIterations)
			{
				//can't solve!
				finalPos	=start;
				modelOn		=-1;

				//player is probably stuck
				//give them footing to help break free
				return	true;
			}

			return	FootCheck(box, end, 4.0f, out modelOn);
		}


		//should be called either once a frame or whenever a trigger model moves
		//normally they don't move so could call this once on load if so
		public void UpdateTriggerPositions()
		{
			foreach(ZoneTrigger zt in mTriggers)
			{
				zt.mTransformedBox.Min	=Vector3.Transform(
					zt.mBox.Min, mZoneModels[zt.mModelNum].mTransform);

				zt.mTransformedBox.Max	=Vector3.Transform(
					zt.mBox.Max, mZoneModels[zt.mModelNum].mTransform);
			}
		}


		public void BoxTriggerCheck(BoundingBox box, Vector3 start, Vector3 end, int msDelta)
		{
			//check for new entries
			foreach(ZoneTrigger zt in mTriggers)
			{
				zt.mTimeSinceTriggered	+=msDelta;

				if((zt.mbTriggerOnce || zt.mbTriggerStandIn) && zt.mbTriggered)
				{
					continue;
				}

				if(Trace_TriggerFakeOBBox(box, zt.mModelNum, start, end))
				{
					if(zt.mTimeSinceTriggered > zt.mWait)
					{
						zt.mbTriggered			=true;
						zt.mTimeSinceTriggered	=0;
						Misc.SafeInvoke(eTriggerHit, zt.mEntity);
					}
				}
			}

			//check for expiring standins
			foreach(ZoneTrigger zt in mTriggers)
			{
				if(!zt.mbTriggerStandIn)
				{
					continue;
				}

				if(!zt.mbTriggered)
				{
					continue;
				}

				if(Trace_TriggerFakeOBBox(box, zt.mModelNum, start, end))
				{
					continue;
				}

				zt.mbTriggered			=false;
				zt.mTimeSinceTriggered	=0;
				Misc.SafeInvoke(eTriggerOutOfRange, zt.mEntity);
			}
		}


		public Int32 FindNodeLandedIn(Int32 node, Vector3 pos)
		{
			float		dist;
			ZoneNode	pNode;

			if(node < 0)		// At leaf, no more recursing
			{
				return	node;
			}

			pNode	=mZoneNodes[node];
			
			//Get the distance that the eye is from this plane
			dist	=mZonePlanes[pNode.mPlaneNum].DistanceFast(pos);

			//Go down the side we are on first, then the other side
			Int32	ret	=0;
			ret	=FindNodeLandedIn((dist < 0)? pNode.mBack : pNode.mFront, pos);
			if(ret < 0)
			{
				return	ret;
			}
			ret	=FindNodeLandedIn((dist < 0)? pNode.mFront : pNode.mBack, pos);
			return	ret;
		}
		#endregion


		#region Vis Stuff
		public bool IsMaterialVisibleFromPos(Vector3 pos, int matIndex)
		{
			if(mZoneNodes == null)
			{
				return	true;	//no map data
			}
			Int32	node	=FindNodeLandedIn(0, pos);
			if(node > 0)
			{
				return	true;	//in solid space
			}

			Int32	leaf	=-(node + 1);
			return	IsMaterialVisible(leaf, matIndex);
		}


		//only used for debugging vis
		public int GetVisibleGeometry(Vector3 pos, List<Vector3> verts, List<UInt32> inds)
		{
			if(mDebugFaces == null)
			{
				return	-1;	//no debug info saved
			}

			Int32	posNode	=FindNodeLandedIn(0, pos);
			if(posNode > 0)
			{
				return	0;	//solid
			}

			Int32	leaf	=-(posNode + 1);
			Int32	clust	=mZoneLeafs[leaf].mCluster;

			if(clust == -1 || mVisClusters[clust].mVisOfs == -1)
			{
				return	-69;	//no info for position
			}

			Int32	ofs	=mVisClusters[clust].mVisOfs;

			int	leafsVisible	=0;
			foreach(ZoneLeaf zl in mZoneLeafs)
			{
				Int32	c	=zl.mCluster;

				if(c < 0)
				{
					continue;
				}

				if((mVisData[ofs + (c >> 3)] & (1 << (c & 7))) == 0)
				{
					continue;
				}

				leafsVisible++;

				for(int i=0;i < zl.mNumFaces;i++)
				{
					int		vofs	=verts.Count;
					int		face	=mDebugLeafFaces[zl.mFirstFace + i];
					int		nverts	=mDebugFaces[face].mNumVerts;
					int		fvert	=mDebugFaces[face].mFirstVert;

					for(int j=fvert;j < (fvert + nverts);j++)
					{
						int	idx	=mDebugIndexes[j];
						verts.Add(mDebugVerts[idx]);
					}

					for(int k=1;k < nverts-1;k++)
					{
						//initial vertex
						inds.Add((UInt32)vofs);
						inds.Add((UInt32)(vofs + k));
						inds.Add((UInt32)(vofs + ((k + 1) % nverts)));
					}
				}
			}

			GetModelGeometry(verts, inds);

			return	leafsVisible;
		}


		public void GetModelGeometry(List<Vector3> verts, List<UInt32> inds)
		{
			if(mDebugFaces == null || mZoneModels.Length < 2)
			{
				return;	//no debug info saved
			}

			for(int i=1;i < mZoneModels.Length;i++)
			{
				int	firstFace	=mZoneModels[i].mFirstFace;
				int	numFaces	=mZoneModels[i].mNumFaces;

				for(int j=firstFace;j < (firstFace + numFaces);j++)
				{
					int		vofs	=verts.Count;
					int		face	=j;
					int		nverts	=mDebugFaces[face].mNumVerts;
					int		fvert	=mDebugFaces[face].mFirstVert;

					for(int k=fvert;k < (fvert + nverts);k++)
					{
						int	idx	=mDebugIndexes[k];

						Vector3	transd	=Vector3.Transform(mDebugVerts[idx], mZoneModels[i].mTransform);

						verts.Add(transd);
					}

					for(int z=1;z < nverts-1;z++)
					{
						//initial vertex
						inds.Add((UInt32)vofs);
						inds.Add((UInt32)(vofs + z));
						inds.Add((UInt32)(vofs + ((z + 1) % nverts)));
					}
				}
			}
		}


		public Vector3 GetClusterCenter(int clust)
		{
			Vector3	ret	=Vector3.Zero;
			foreach(ZoneLeaf zl in mZoneLeafs)
			{
				if(zl.mCluster != clust)
				{
					continue;
				}

				ret	+=((zl.mMaxs + zl.mMins) / 2.0f);
			}
			return	ret;
		}


		public bool IsVisibleFrom(Vector3 posA, Vector3 posB)
		{
			Int32	posANode	=FindNodeLandedIn(0, posA);
			if(posANode > 0)
			{
				return	false;	//position in solid
			}

			Int32	posBNode	=FindNodeLandedIn(0, posB);
			if(posBNode > 0)
			{
				return	false;	//position in solid
			}

			Int32	leafA	=-(posANode + 1);
			Int32	leafB	=-(posBNode + 1);

			Int32	clusterA	=mZoneLeafs[leafA].mCluster;
			Int32	clusterB	=mZoneLeafs[leafB].mCluster;

			if(clusterA == -1 || mVisClusters[clusterA].mVisOfs == -1)
			{
				return	false;	//no vis info for position
			}
			if(clusterB == -1 || mVisClusters[clusterB].mVisOfs == -1)
			{
				return	false;	//no vis info for position
			}

			int	ofs	=mVisClusters[clusterA].mVisOfs;

			if((mVisData[ofs + (clusterB >> 3)] & (1 << (clusterB & 7))) != 0)
			{
				return	true;	//A can see B
			}
			return	false;
		}


		bool IsMaterialVisible(int leaf, int matIndex)
		{
			if(mZoneLeafs == null)
			{
				return	false;
			}

			int	clust	=mZoneLeafs[leaf].mCluster;

			if(clust == -1 || mVisClusters[clust].mVisOfs == -1
				|| mMaterialVisData == null)
			{
				return	true;	//this will make everything vis
								//when outside of the map
			}

			//plus one to avoid 0 problem
			matIndex++;

			int	ofs	=leaf * mNumVisMaterialBytes;
			
			return	((mMaterialVisData[ofs + (matIndex >> 3)] & (1 << (matIndex & 7))) != 0);
		}


		void FindParents_r(Int32 Node, Int32 Parent)
		{
			if(Node < 0)		// At a leaf, mark leaf parent and return
			{
				mLeafData[-(Node + 1)].mParent	=Parent;
				return;
			}

			//At a node, mark node parent, and keep going till hitting a leaf
			mNodeParents[Node]	=Parent;

			// Go down front and back markinf parents on the way down...
			FindParents_r(mZoneNodes[Node].mFront, Node);
			FindParents_r(mZoneNodes[Node].mBack, Node);
		}
		#endregion


		//for debugging
		public void GetTriggerGeometry(List<Vector3> verts, List<Int32> inds)
		{
			Int32	curIndex	=0;
			foreach(ZoneTrigger zt in mTriggers)
			{
				if(zt.mbTriggerOnce && zt.mbTriggered)
				{
					continue;
				}

				Vector3	[]corners	=zt.mBox.GetCorners();

				foreach(Vector3 v in corners)
				{
					verts.Add(v);
				}

				//wireframe lines
				//front face
				inds.Add(curIndex);
				inds.Add(curIndex + 1);
				inds.Add(curIndex + 1);
				inds.Add(curIndex + 2);
				inds.Add(curIndex + 2);
				inds.Add(curIndex + 3);
				inds.Add(curIndex + 3);
				inds.Add(curIndex);

				//back face
				inds.Add(curIndex + 4);
				inds.Add(curIndex + 5);
				inds.Add(curIndex + 5);
				inds.Add(curIndex + 6);
				inds.Add(curIndex + 6);
				inds.Add(curIndex + 7);
				inds.Add(curIndex + 7);
				inds.Add(curIndex + 4);

				//connections for sides
				inds.Add(curIndex);
				inds.Add(curIndex + 4);
				inds.Add(curIndex + 1);
				inds.Add(curIndex + 5);
				inds.Add(curIndex + 2);
				inds.Add(curIndex + 6);
				inds.Add(curIndex + 3);
				inds.Add(curIndex + 7);

				curIndex	+=8;
			}
		}


		public void GetBounds(out Vector3 mins, out Vector3 maxs)
		{
			if(mZoneModels.Length <= 0)
			{
				mins	=Vector3.Zero;
				maxs	=Vector3.Zero;
				return;
			}
			mins	=mZoneModels[0].mBounds.Min;
			maxs	=mZoneModels[0].mBounds.Max;
		}
	}
}