using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using Microsoft.Xna.Framework.Storage;


namespace BSPLib
{
	public partial class Map
	{
		List<MapEntity>	mEntities;

		//models
		internal List<GBSPModel>	mModels	=new List<GBSPModel>();

		//planes
		PlanePool	mPlanePool	=new PlanePool();

		//texinfos
		TexInfoPool	mTIPool	=new TexInfoPool();

		//stuff for debug draw
		Int32		mCurrentLeaf;
		Int32		mCurFrameStatic;
		Int32		[]mClusterVisFrame;
		WorldLeaf	[]mLeafData;
		Int32		[]mNodeParents;
		Int32		[]mNodeVisFrame;
		internal static List<MapBrush>	TroubleBrushes	=new List<MapBrush>();

		//gfx data
		GFXModel		[]mGFXModels;
		GFXNode			[]mGFXNodes;
		GFXLeaf			[]mGFXLeafs;
		GFXCluster		[]mGFXClusters;
		GFXArea			[]mGFXAreas;
		GFXAreaPortal	[]mGFXAreaPortals;
		GFXPlane		[]mGFXPlanes;
		GFXFace			[]mGFXFaces;
		Int32			[]mGFXLeafFaces;
		GFXLeafSide		[]mGFXLeafSides;
		Vector3			[]mGFXVerts;
		Int32			[]mGFXVertIndexes;
		Vector3			[]mGFXRGBVerts;
		GFXTexInfo		[]mGFXTexInfos;
		MapEntity		[]mGFXEntities;
		byte			[]mGFXLightData;
		byte			[]mGFXVisData;
		byte			[]mGFXMaterialVisData;
		int				mLightMapGridSize;

		//build settings
//		BSPBuildParams	mBSPParms;
//		LightParams		mLightParams;
//		VisParams		mVisParams;

		public event EventHandler	eNumPlanesChanged;
		public event EventHandler	eNumVertsChanged;
		public event EventHandler	eNumClustersChanged;
		public event EventHandler	eNumPortalsChanged;
		public event EventHandler	eGBSPSaveDone;
		public event EventHandler	eVisDone;
		public event EventHandler	eLightDone;
		public event EventHandler	eBuildDone;

		static public event EventHandler	ePrint;


		//method delegates
		internal delegate GBSPModel ModelForLeafNode(GBSPNode n);
		internal delegate bool IsPointInSolid(Vector3 pos);
		internal delegate bool RayCollision(Vector3 front, Vector3 back, ref Vector3 Impacto);
		internal delegate Int32 GetNodeLandedIn(Int32 node, Vector3 pos);
		public delegate Vector3 GetEmissiveForMaterial(string matName);


		public Map() { }


		#region Queries
		public void GetTriangles(Vector3 pos, List<Vector3> verts, List<UInt32> indexes, string drawChoice)
		{
			if(drawChoice == "Map Brushes")
			{
				foreach(MapEntity ent in mEntities)
				{
					ent.GetTriangles(verts, indexes);
				}
			}
			else if(drawChoice == "Trouble Brushes")
			{
				lock(TroubleBrushes)
				{
					foreach(MapBrush mb in TroubleBrushes)
					{
						mb.GetTriangles(verts, indexes, false);
					}
				}
			}
			else if(drawChoice == "Draw Brushes")
			{
				if(mGFXModels != null && mGFXModels.Length > 0)
				{
					int	root	=mGFXModels[0].mRootNode[0];

					RenderBSPFrontBack_r2(root, pos, verts, indexes, false);

					for(int i=1;i < mGFXModels.Length;i++)
					{
						RenderModelBSPFrontBack_r2(mGFXModels[i].mRootNode[0], pos, verts, indexes);
					}
				}
				else
				{
					Print("No GFXModels to draw!\n");
				}
//				foreach(GBSPBrush b in mGBSPBrushes)
//				{
//					b.GetTriangles(verts, indexes, true);
//				}
			}
			else if(drawChoice == "Collision Brushes")
			{
			}
			else if(drawChoice == "Draw Tree")
			{
				if(mGFXModels != null && mGFXModels.Length > 0)
				{
					int	root	=mGFXModels[0].mRootNode[0];

					VisWorld(root, pos);

					RenderBSPFrontBack_r2(root, pos, verts, indexes, true);

					for(int i=1;i < mGFXModels.Length;i++)
					{
						RenderModelBSPFrontBack_r2(mGFXModels[i].mRootNode[0], pos, verts, indexes);
					}
				}
				else
				{
					Print("No GFXModels to draw!\n");
				}
			}
			else if(drawChoice == "Collision Tree")
			{
			}
			else if(drawChoice == "Portals")
			{
			}
			else if(drawChoice == "Portal Tree")
			{
			}
		}


		public bool ClassifyPoint(Vector3 pnt)
		{
			return	false;
		}


		public Vector3 GetPlayerStartPos()
		{
			foreach(MapEntity e in mGFXEntities)
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


		public Vector3 GetFirstLightPos()
		{
			foreach(MapEntity e in mEntities)
			{
				if(e == GetWorldSpawnEntity())
				{
					continue;
				}
				float dist;
				if(e.GetLightValue(out dist))
				{
					Vector3	ret;
					e.GetOrigin(out ret);
					return	ret;
				}
			}
			return	Vector3.Zero;
		}


		public MapEntity GetWorldSpawnEntity()
		{
			foreach(MapEntity e in mEntities)
			{
				if(e.mData.ContainsKey("classname"))
				{
					if(e.mData["classname"] == "worldspawn")
					{
						return	e;
					}
				}
			}
			return	null;
		}
		#endregion


		#region IO
		public void Write(string fileName)
		{
			FileStream	file	=UtilityLib.FileUtil.OpenTitleFile(fileName,
									FileMode.OpenOrCreate, FileAccess.Write);

			BinaryWriter	bw	=new BinaryWriter(file);

			SaveArray(mGFXModels, bw);
			SaveArray(mGFXNodes, bw);
			SaveArray(mGFXLeafs, bw);
			SaveArray(mGFXClusters, bw);
			SaveArray(mGFXAreas, bw);
			SaveArray(mGFXAreaPortals, bw);
			SaveArray(mGFXPlanes, bw);
			SaveArray(mGFXEntities, bw);
			SaveArray(mGFXVisData, bw);
			SaveArray(mGFXMaterialVisData, bw);
			bw.Write(mLightMapGridSize);
			bw.Write(mNumVisLeafBytes);
			bw.Write(mNumVisMaterialBytes);

			bw.Close();
			file.Close();
		}


		public void Read(string fileName)
		{
			FileStream	file	=UtilityLib.FileUtil.OpenTitleFile(fileName,
									FileMode.Open, FileAccess.Read);

			BinaryReader	br	=new BinaryReader(file);

			mGFXModels		=LoadArray(br, delegate(Int32 count)
							{ return InitArray<GFXModel>(count); }) as GFXModel[];
			mGFXNodes		=LoadArray(br, delegate(Int32 count)
							{ return InitArray<GFXNode>(count); }) as GFXNode[];
			mGFXLeafs		=LoadArray(br, delegate(Int32 count)
							{ return InitArray<GFXLeaf>(count); }) as GFXLeaf[];
			mGFXClusters	=LoadArray(br, delegate(Int32 count)
							{ return InitArray<GFXCluster>(count); }) as GFXCluster[];
			mGFXAreas		=LoadArray(br, delegate(Int32 count)
							{ return InitArray<GFXArea>(count); }) as GFXArea[];
			mGFXAreaPortals	=LoadArray(br, delegate(Int32 count)
							{ return InitArray<GFXAreaPortal>(count); }) as GFXAreaPortal[];
			mGFXPlanes		=LoadArray(br, delegate(Int32 count)
							{ return InitArray<GFXPlane>(count); }) as GFXPlane[];
			mGFXEntities	=LoadArray(br, delegate(Int32 count)
							{ return InitArray<MapEntity>(count); }) as MapEntity[];
			LoadGFXVisData(br);
			LoadGFXMaterialVisData(br);
			mLightMapGridSize		=br.ReadInt32();
			mNumVisLeafBytes		=br.ReadInt32();
			mNumVisMaterialBytes	=br.ReadInt32();

			//make clustervisframe
			mClusterVisFrame	=new int[mGFXClusters.Length];
			mNodeParents		=new int[mGFXNodes.Length];
			mNodeVisFrame		=new int[mGFXNodes.Length];
			mLeafData			=new WorldLeaf[mGFXLeafs.Length];

			//fill in leafdata with blank worldleafs
			for(int i=0;i < mGFXLeafs.Length;i++)
			{
				mLeafData[i]	=new WorldLeaf();
			}

			FindParents_r(mGFXModels[0].mRootNode[0], -1);

			br.Close();
			file.Close();
		}
		#endregion


		public void LoadBrushFile(string mapFileName)
		{
			mEntities	=new List<MapEntity>();

			int	numSolids	=0;
			int	numDetails	=0;
			int	numTotal	=0;

			if(File.Exists(mapFileName))
			{
				using(StreamReader sr = File.OpenText(mapFileName))
				{
					string	s	="";

					//see if this is a .map or a .vmf
					if(mapFileName.EndsWith(".map"))
					{
						while((s = sr.ReadLine()) != null)
						{
							s	=s.Trim();
							if(s == "{")
							{
								MapEntity	e	=new MapEntity();
								e.ReadFromMap(sr, mPlanePool, mTIPool, mEntities.Count);
								mEntities.Add(e);

								if(eNumPlanesChanged != null)
								{
									eNumPlanesChanged(mPlanePool.mPlanes.Count, null);
								}
							}
						}
					}
					else
					{
						while((s = sr.ReadLine()) != null)
						{
							s	=s.Trim();
							if(s == "entity")
							{
								MapEntity	e	=new MapEntity();
								e.ReadVMFEntBlock(sr, mEntities.Count, mPlanePool, mTIPool);
								mEntities.Add(e);
								if(eNumPlanesChanged != null)
								{
									eNumPlanesChanged(mPlanePool.mPlanes.Count, null);
								}
							}
							else if(s == "world")
							{
								MapEntity	e	=new MapEntity();
								e.ReadVMFWorldBlock(sr, mEntities.Count, mPlanePool, mTIPool);
								mEntities.Add(e);
								if(eNumPlanesChanged != null)
								{
									eNumPlanesChanged(mPlanePool.mPlanes.Count, null);
								}
							}
							else if(s == "cameras")
							{
								MapEntity.SkipVMFEditorBlock(sr);
							}
							else if(s == "cordon")
							{
								MapEntity.SkipVMFEditorBlock(sr);
							}
						}
					}
				}
			}

			foreach(MapEntity e in mEntities)
			{
				e.CountBrushes(ref numDetails, ref numSolids, ref numTotal);
			}

			InsertModelNumbers();

			Print("Brush file load complete\n");
			Print("" + numSolids + " solid brushes\n");
			Print("" + numDetails + " detail brushes\n");
			Print("" + numTotal + " total brushes\n");
		}


		bool ProcessEntities(bool bVerbose, bool bEntityVerbose)
		{
			int	index	=0;

			foreach(MapEntity me in mEntities)
			{
				List<MapBrush>	brushery	=me.GetBrushes();
				if(brushery.Count == 0)
				{
					index++;
					continue;
				}

				GBSPModel	mod	=new GBSPModel();

				Vector3	org;
				me.GetOrigin(out org);

				mod.SetOrigin(org);

				if(index == 0)
				{
					if(!mod.ProcessWorldModel(brushery, mEntities,
						mPlanePool, mTIPool, bVerbose, eNumPlanesChanged))
					{
						return	false;
					}
				}
				else
				{
					if(!mod.ProcessSubModel(brushery, mPlanePool,
						mTIPool, bEntityVerbose))
					{
						return	false;
					}
				}
				mModels.Add(mod);
				index++;
			}
			return	true;
		}


		internal void UpdateNumPortals(int numPortals)
		{
			if(eNumPortalsChanged != null)
			{
				eNumPortalsChanged(numPortals, null);
			}
		}


		static internal void Print(string str)
		{
			EventHandler	evt	=ePrint;
			if(evt != null)
			{
				evt(str, null);
			}
		}


		internal GBSPModel GetModelForLeafNode(GBSPNode Node)
		{
			if(Node.IsLeaf())
			{
				Print("ModelForLeafNode:  Node not a leaf!\n");
				return	null;
			}

			int	entNum	=Node.GetOriginalEntityNum();
			if(entNum == -1)
			{
				return	null;
			}

			return	mModels[mEntities[entNum].mModelNum];
		}


		bool InsertModelNumbers()
		{
			Int32	i, NumModels	=0;

			for(i=0;i < mEntities.Count;i++)
			{
				if(mEntities[i].GetBrushCount() == 0)	//No model if no brushes
				{
					continue;
				}
				
				mEntities[i].mModelNum	=NumModels;

				if(i != 0)
				{
					mEntities[i].mData.Add("Model", "" + NumModels);
				}
				NumModels++;
			}
			return	true;
		}


		void BuildTreeCB(object threadContext)
		{
			BSPBuildParams	bp	=threadContext as BSPBuildParams;

			mTIPool.AssignMaterials();

			if(ProcessEntities(bp.mbVerbose, bp.mbEntityVerbose))
			{
				Print("Build GBSP Complete\n");
				if(eBuildDone != null)
				{
					eBuildDone(true, null);
				}
			}
			else
			{
				Print("Compilation failed\n");
				if(eBuildDone != null)
				{
					eBuildDone(false, null);
				}
			}
		}


		public void BuildTree(BSPBuildParams prms)
		{
			ThreadPool.QueueUserWorkItem(BuildTreeCB, prms);
		}


		bool FixModelTJunctions(FaceFixer ff, bool bFixTJunctions, bool bVerbose)
		{
			Print(" --- Weld Model Verts --- \n");

			for(int i=0;i < mModels.Count;i++)
			{
				if(!mModels[i].GetFaceVertIndexNumbers(ff))
				{
					return	false;
				}
			}

			//Skip if asked to do so...
			if(!bFixTJunctions)
			{
				return	true;
			}


			Map.Print(" --- Fix Model TJunctions --- \n");

			for(int i=0;i < mModels.Count;i++)
			{
				if(!mModels[i].FixTJunctions(ff, mTIPool))
				{
					return false;
				}
			}

			if(bVerbose)
			{
				Print(" Num TJunctions        : " + ff.NumTJunctions + "\n");
				Print(" Num Fixed Faces       : " + ff.NumFixedFaces + "\n");
			}
			return true;
		}


		bool PrepAllGBSPModels(string visFile, NodeCounter nc, bool bVerbose, bool bEntityVerbose)
		{
			Int32	i;

			List<GFXLeafSide>	leafSides	=new List<GFXLeafSide>();
			for(i=0;i < mModels.Count;i++)
			{
				if(!mModels[i].PrepGBSPModel(visFile, i == 0,
					(i == 0)? bVerbose : bEntityVerbose,
					mPlanePool,
					ref nc.mNumLeafClusters,
					leafSides))
				{
					Map.Print("PrepAllGBSPModels:  Could not prep model " + i + "\n");
					return	false;
				}

				//create areas
				if(i == 0)
				{
					CreateAreas(mModels[i], nc);
				}
			}

			mGFXLeafSides	=leafSides.ToArray();

			return	true;
		}


		void ConvertGBSPToFileCB(object threadContext)
		{
			GBSPSaveParameters sp	=threadContext as GBSPSaveParameters;

			FileStream	file	=UtilityLib.FileUtil.OpenTitleFile(sp.mFileName,
									FileMode.OpenOrCreate, FileAccess.Write);

			if(file == null)
			{
				Map.Print("ConvertGBSPToFile:  geVFile_OpenNewSystem failed.\n");
				if(eGBSPSaveDone != null)
				{
					eGBSPSaveDone(false, null);
				}
				return;
			}

			string	VisFile	=sp.mFileName;

			FaceFixer	ff	=new FaceFixer();

			if(!FixModelTJunctions(ff, sp.mBSPParams.mbFixTJunctions, sp.mBSPParams.mbVerbose))
			{
				Map.Print("ConvertGBSPToFile:  FixModelTJunctions failed.\n");
				if(eGBSPSaveDone != null)
				{
					eGBSPSaveDone(false, null);
				}
				return;
			}

			mGFXVerts		=ff.GetWeldedVertArray();

			if(eNumVertsChanged != null)
			{
				eNumVertsChanged(mGFXVerts.Length, null);
			}

			NodeCounter	nc	=new NodeCounter();

			if(!PrepAllGBSPModels(VisFile, nc, sp.mBSPParams.mbVerbose, sp.mBSPParams.mbEntityVerbose))
			{
				Print("ConvertGBSPToFile:  Could not prep models.\n");
				if(eGBSPSaveDone != null)
				{
					eGBSPSaveDone(false, null);
				}
				return;
			}

			if(eNumClustersChanged != null)
			{
				eNumClustersChanged(nc.mNumLeafClusters, null);
			}

			mGFXVertIndexes	=nc.GetIndexArray();

			BinaryWriter	bw	=new BinaryWriter(file);

			GFXHeader	header	=new GFXHeader();

			header.mTag				=0x47425350;	//"GBSP"
			header.mbHasLight		=false;
			header.mbHasVis			=false;
			header.mbHasMaterialVis	=false;
			header.Write(bw);

			SaveGFXModelDataFromList(bw);
			SaveGFXNodes(bw, nc);
			SaveGFXLeafs(bw, nc);
			SaveEmptyGFXClusters(bw, nc);

			//set gfx area stuff from lists
			mGFXAreas		=mAreas.ToArray();
			mGFXAreaPortals	=mAreaPorts.ToArray();

			SaveGFXAreasAndPortals(bw);
			SaveGFXLeafSides(bw);
			SaveGFXFaces(bw, nc);

			mGFXPlanes	=mPlanePool.GetGFXArray();

			SaveGFXPlanes(bw);
			SaveGFXVerts(bw);
			SaveGFXVertIndexes(bw);
			mTIPool.Write(bw);

			SaveGFXEntDataList(bw);
			
			bw.Close();
			file.Close();

			Map.Print(" --- Save GBSP File --- \n");
		 	
			Map.Print("Num Models           : " + mModels.Count + "\n");
			Map.Print("Num Nodes            : " + nc.mNumGFXNodes + "\n");
			Map.Print("Num Solid Leafs      : " + nc.mNumSolidLeafs + "\n");
			Map.Print("Num Total Leafs      : " + nc.mNumGFXLeafs + "\n");
			Map.Print("Num Clusters         : " + nc.mNumLeafClusters + "\n");
			Map.Print("Num Areas            : " + (mGFXAreas.Length - 1) + "\n");
			Map.Print("Num Area Portals     : " + mGFXAreaPortals.Length + "\n");
			Map.Print("Num Leafs Sides      : " + mGFXLeafSides.Length + "\n");
			Map.Print("Num Planes           : " + mPlanePool.mPlanes.Count + "\n");
			Map.Print("Num Faces            : " + nc.mNumGFXFaces + "\n");
			Map.Print("Num Leaf Faces       : " + nc.mNumGFXLeafFaces + "\n");
			Map.Print("Num Vert Index       : " + nc.VertIndexListCount + "\n");
			Map.Print("Num Verts            : " + mGFXVerts.Length + "\n");
			Map.Print("Num FaceInfo         : " + mTIPool.mTexInfos.Count + "\n");

			FreeGBSPFile();

			if(eGBSPSaveDone != null)
			{
				eGBSPSaveDone(true, null);
			}
		}


		void FreeGBSPFile()
		{
			mGFXModels			=null;
			mGFXNodes			=null;
			mGFXLeafs			=null;
			mGFXClusters		=null;		// CHANGE: CLUSTER
			mGFXAreas			=null;
			mGFXPlanes			=null;
			mGFXFaces			=null;
			mGFXLeafFaces		=null;
			mGFXLeafSides		=null;
			mGFXVerts			=null;
			mGFXVertIndexes		=null;
			mGFXRGBVerts		=null;
			mGFXEntities		=null;			
			mGFXTexInfos		=null;
			mGFXLightData		=null;
			mGFXVisData			=null;
			mGFXMaterialVisData	=null;
		}


		Vector3	[]MakeVertNormals()
		{
			Vector3	[]ret	=new Vector3[mGFXVerts.Length];
			if(ret == null)
			{
				Print("MakeVertNormals:  Out of memory for normals.\n");
				return	null;
			}

			for(int i=0;i < mGFXFaces.Length;i++)
			{
				GFXFace		f	=mGFXFaces[i];
				GFXTexInfo	tex	=mGFXTexInfos[f.mTexInfo];

				//grab face normal
				Vector3	Normal	=mGFXPlanes[f.mPlaneNum].mNormal;

				if(f.mPlaneSide != 0)
				{
					Normal	=-Normal;
				}

				//adds adjacent faces, not sure what happens
				//if you get stuff that is two sided
				//this smooths normals for gouraud
				for(int v=0;v < f.mNumVerts;v++)
				{
					Int32	vn	=f.mFirstVert + v;

					Int32	Index	=mGFXVertIndexes[vn];

					ret[Index]	=ret[Index] + Normal;
				}
			}

			for(int i=0;i < mGFXVerts.Length;i++)
			{
				ret[i].Normalize();
			}
			return	ret;
		}


		bool IsPointInSolidSpace(Vector3 pos)
		{
			Int32	node	=FindNodeLandedIn(0, pos);

			Int32	Leaf	=-(node + 1);

			return	((mGFXLeafs[Leaf].mContents & Contents.BSP_CONTENTS_SOLID2) != 0);
		}


		bool RayIntersect(Vector3 Front, Vector3 Back, Int32 Node, ref Vector3 intersectionPoint, ref bool hitLeaf)
		{
			float	Fd, Bd, Dist;
			Int32	Side;
			Vector3	I;

			if(Node < 0)						
			{
				Int32	Leaf	=-(Node+1);

				if((mGFXLeafs[Leaf].mContents
					& Contents.BSP_CONTENTS_SOLID2) != 0)
				{
					return	true;	//Ray collided with solid space
				}
				else 
				{
					return	false;	//Ray collided with empty space
				}
			}
			GFXNode		n	=mGFXNodes[Node];
			GFXPlane	p	=mGFXPlanes[n.mPlaneNum];

			Fd	=p.DistanceFast(Front);
			Bd	=p.DistanceFast(Back);

			if(Fd >= -1 && Bd >= -1)
			{
				return(RayIntersect(Front, Back, n.mChildren[0], ref intersectionPoint, ref hitLeaf));
			}
			if(Fd < 1 && Bd < 1)
			{
				return(RayIntersect(Front, Back, n.mChildren[1], ref intersectionPoint, ref hitLeaf));
			}

			Side	=(Fd < 0)? 1 : 0;
			Dist	=Fd / (Fd - Bd);

			I	=Front + Dist * (Back - Front);

			//Work our way to the front, from the back side.  As soon as there
			//is no more collisions, we can assume that we have the front portion of the
			//ray that is in empty space.  Once we find this, and see that the back half is in
			//solid space, then we found the front intersection point...
			if(RayIntersect(Front, I, n.mChildren[Side], ref intersectionPoint, ref hitLeaf))
			{
				return	true;
			}
			else if(RayIntersect(I, Back, n.mChildren[(Side == 0)? 1 : 0], ref intersectionPoint, ref hitLeaf))
			{
				if(!hitLeaf)
				{
					intersectionPoint	=I;
					hitLeaf				=true;
				}
				return	true;
			}
			return	false;
		}


		bool RayCollide(Vector3 Front, Vector3 Back, ref Vector3 I)
		{
			bool	hitLeaf	=false;
			if(RayIntersect(Front, Back, mGFXModels[0].mRootNode[0], ref I, ref hitLeaf))
			{
				return	true;
			}
			return	false;
		}


		public Int32 FindNodeLandedIn(Int32 node, Vector3 pos)
		{
			float		Dist1;
			GFXNode		pNode;
			Int32		Side;

			if(node < 0)		// At leaf, no more recursing
			{
				return	node;
			}

			pNode	=mGFXNodes[node];
			
			//Get the distance that the eye is from this plane
			Dist1	=mGFXPlanes[pNode.mPlaneNum].DistanceFast(pos);

			if(Dist1 < 0)
			{
				Side	=1;
			}
			else
			{
				Side	=0;
			}
			
			//Go down the side we are on first, then the other side
			Int32	ret	=0;
			ret	=FindNodeLandedIn(pNode.mChildren[Side], pos);
			if(ret < 0)
			{
				return	ret;
			}
			ret	=FindNodeLandedIn(pNode.mChildren[(Side == 0)? 1 : 0], pos);
			return	ret;
		}


		bool VisWorld(Int32 rootNode, Vector3 pos)
		{
			Int32	i, Area;
			Int32	Leaf, Cluster;
			GFXLeaf	pLeaf;

			Int32	node	=FindNodeLandedIn(rootNode, -pos);

			Leaf	=-(node + 1);
			Area	=mGFXLeafs[Leaf].mArea;

			mCurrentLeaf	=Leaf;
			mCurFrameStatic++;			// Make all old vis info obsolete

			Cluster	=mGFXLeafs[Leaf].mCluster;

			if(Cluster == -1 || mGFXClusters[Cluster].mVisOfs == -1)
			{
				return	true;
			}

			/*
			if (Area)
				Vis_FloodAreas_r(World, Area);

			World->VisInfo = GE_TRUE;
			*/

			//VisData = &GFXVisData[GFXClusters[Cluster].VisOfs];

			int	ofs	=mGFXClusters[Cluster].mVisOfs;

			// Mark all visible clusters
			for(i=0;i < mGFXModels[0].mNumClusters;i++)
			{
				if((mGFXVisData[ofs + (i >> 3)] & (1 << (i & 7))) != 0)
				{
					mClusterVisFrame[i]	=mCurFrameStatic;
				}
			}

			//Go through and find all visible leafs based on the visible clusters the leafs are in
			for(i=0;i < mGFXModels[0].mNumLeafs;i++)
			{
				pLeaf	=mGFXLeafs[mGFXModels[0].mFirstLeaf + i];

				Cluster	=pLeaf.mCluster;

				if(Cluster == -1)	// No cluster info for this leaf (must be solid)
				{
					continue;
				}

				//If the cluster is not visible, then the leaf is not visible
				if(mClusterVisFrame[Cluster] != mCurFrameStatic)
				{
					continue;
				}
				
				//If the area is not visible, then the leaf is not visible
//				if (World->CurrentBSP->AreaVisFrame[pLeaf->Area] != World->CurFrameStatic)
//					continue;

				//Mark all visible nodes by bubbling up the tree from the leaf
				MarkVisibleParents(i);

				//Mark the leafs vis frame to worlds current frame
				mLeafData[i].mVisFrame	=mCurFrameStatic;
					
//				pFace = &GFXLeafFaces[pLeaf->FirstFace];

				// Go ahead and vis surfaces here...
//				for (k=0; k< pLeaf->NumFaces; k++)
//				{
					// Update each surface infos visframe thats touches each visible leaf
//					SurfInfo[*pFace++].VisFrame = World->CurFrameStatic;
//				}
			}
			return	true;
		}


		void MarkVisibleParents(Int32 Leaf)
		{
			Int32		Node;

			Debug.Assert(Leaf >= 0);
			Debug.Assert(Leaf < mGFXLeafs.Length);

			//Find the leafs parent
			Node	=mLeafData[Leaf].mParent;

			// Bubble up the tree from the current node, marking them as visible
			while(Node >= 0)
			{
				mNodeVisFrame[Node]	=mCurFrameStatic;
				Node	=mNodeParents[Node];
			}
		}


		void RenderBSPFrontBack_r2(Int32 Node, Vector3 pos,
			List<Vector3> verts, List<uint> indexes, bool bCheck)
		{
			float		Dist1;
			GFXNode		pNode;
			Int32		Side;

			if(Node < 0)		// At leaf, no more recursing
			{
				Int32		Leaf;

				Leaf	=-(Node+1);

				if(bCheck && mLeafData[Leaf].mVisFrame != mCurFrameStatic)
				{
					return;
				}

				Debug.Assert(Leaf >= 0 && Leaf < mGFXLeafs.Length);

				for(int i=0;i < mGFXLeafs[Leaf].mNumFaces;i++)
				{
					int		ofs		=verts.Count;
					UInt32	offset	=(UInt32)ofs;
					int		face	=mGFXLeafFaces[mGFXLeafs[Leaf].mFirstFace + i];
					int		nverts	=mGFXFaces[face].mNumVerts;
					int		fvert	=mGFXFaces[face].mFirstVert;

					for(int j=fvert;j < (fvert + nverts);j++)
					{
						int	idx	=mGFXVertIndexes[j];
						verts.Add(mGFXVerts[idx]);
					}

					int k	=0;
					for(k=1;k < nverts-1;k++)
					{
						//initial vertex
						indexes.Add(offset);
						indexes.Add((UInt32)(offset + k));
						indexes.Add((UInt32)(offset + ((k + 1) % nverts)));
					}
				}
				return;
			}

//			if(NodeVisFrame[Node] != CurFrameStatic)
//			{
//				return;
//			}

			pNode	=mGFXNodes[Node];
			
			//Get the distance that the eye is from this plane
			Dist1	=mGFXPlanes[pNode.mPlaneNum].DistanceFast(pos);

			if(Dist1 < 0)
			{
				Side	=1;
			}
			else
			{
				Side	=0;
			}
			
			//Go down the side we are on first, then the other side
			RenderBSPFrontBack_r2(pNode.mChildren[Side], pos, verts, indexes, bCheck);
			RenderBSPFrontBack_r2(pNode.mChildren[(Side == 0)? 1 : 0], pos, verts, indexes, bCheck);
		}


		void RenderModelBSPFrontBack_r2(Int32 Node, Vector3 pos,
						List<Vector3> verts, List<uint> indexes)
		{
			float		Dist1;
			GFXNode		pNode;
			Int32		Side;

			if(Node < 0)		// At leaf, no more recursing
			{
				Int32		Leaf;

				Leaf	=-(Node+1);

				Debug.Assert(Leaf >= 0 && Leaf < mGFXLeafs.Length);

				for(int i=0;i < mGFXLeafs[Leaf].mNumFaces;i++)
				{
					int		ofs		=verts.Count;
					UInt32	offset	=(UInt32)ofs;
					int		face	=mGFXLeafFaces[mGFXLeafs[Leaf].mFirstFace + i];
					int		nverts	=mGFXFaces[face].mNumVerts;
					int		fvert	=mGFXFaces[face].mFirstVert;

					for(int j=fvert;j < (fvert + nverts);j++)
					{
						int	idx	=mGFXVertIndexes[j];
						verts.Add(mGFXVerts[idx]);
					}

					int k	=0;
					for(k=1;k < nverts-1;k++)
					{
						//initial vertex
						indexes.Add(offset);
						indexes.Add((UInt32)(offset + k));
						indexes.Add((UInt32)(offset + ((k + 1) % nverts)));
					}
				}
				return;
			}

			pNode	=mGFXNodes[Node];
			
			for(int i=0;i < pNode.mNumFaces;i++)
			{
				int		ofs		=verts.Count;
				UInt32	offset	=(UInt32)ofs;
				int		face	=pNode.mFirstFace + i;
				int		nverts	=mGFXFaces[face].mNumVerts;
				int		fvert	=mGFXFaces[face].mFirstVert;

				for(int j=fvert;j < (fvert + nverts);j++)
				{
					int	idx	=mGFXVertIndexes[j];
					verts.Add(mGFXVerts[idx]);
				}

				int k	=0;
				for(k=1;k < nverts-1;k++)
				{
					//initial vertex
					indexes.Add(offset);
					indexes.Add((UInt32)(offset + k));
					indexes.Add((UInt32)(offset + ((k + 1) % nverts)));
				}
			}
			//Get the distance that the eye is from this plane
			Dist1	=mGFXPlanes[pNode.mPlaneNum].DistanceFast(pos);

			if(Dist1 < 0)
			{
				Side	=1;
			}
			else
			{
				Side	=0;
			}
			
			//Go down the side we are on first, then the other side
			RenderModelBSPFrontBack_r2(pNode.mChildren[Side], pos, verts, indexes);
			RenderModelBSPFrontBack_r2(pNode.mChildren[(Side == 0)? 1 : 0], pos, verts, indexes);
		}


		public List<MaterialLib.Material> GetMaterials()
		{
			MapGrinder	mg	=new MapGrinder(null, mGFXTexInfos, mGFXFaces, mLightMapGridSize, 1);

			return	mg.GetMaterials();
		}


		//an intermediate step to generate a set of materials
		//so the user can set up emissives for radiosity
		public List<MaterialLib.Material> GenerateMaterials(string fileName)
		{
			LoadGBSPFile(fileName);

			List<MaterialLib.Material>	ret	=GetMaterials();

			FreeGBSPFile();

			return	ret;
		}


		public bool BuildLMRenderData(GraphicsDevice g,
			//lightmap stuff
			out VertexBuffer lmVB,
			out IndexBuffer lmIB,
			out VertexDeclaration lmVD,
			out Int32 []matOffsets,
			out Int32 []matNumVerts,
			out Int32 []matNumTris,

			//animated lightmap stuff
			out VertexBuffer lmAnimVB,
			out IndexBuffer lmAnimIB,
			out VertexDeclaration lmAnimVD,
			out Int32 []matAnimOffsets,
			out Int32 []matAnimNumVerts,
			out Int32 []matAnimNumTris,

			//lightmapped alpha stuff
			out VertexBuffer lmaVB,
			out IndexBuffer lmaIB,
			out VertexDeclaration lmaVD,
			out Int32 []amatOffsets,
			out Int32 []amatNumVerts,
			out Int32 []amatNumTris,
			out Vector3 []amatSortPoints,

			//animated alpha lightmap stuff
			out VertexBuffer lmaAnimVB,
			out IndexBuffer lmaAnimIB,
			out VertexDeclaration lmaAnimVD,
			out Int32 []amatAnimOffsets,
			out Int32 []amatAnimNumVerts,
			out Int32 []amatAnimNumTris,
			out Vector3 []amatAnimSortPoints,

			int lightAtlasSize,
			out MaterialLib.TexAtlas lightAtlas)
		{
			MapGrinder	mg	=new MapGrinder(g, mGFXTexInfos, mGFXFaces, mLightMapGridSize, lightAtlasSize);

			if(!mg.BuildLMFaceData(mGFXVerts, mGFXVertIndexes, mGFXLightData))
			{
				lmVB	=null;	lmIB	=null;	lmVD	=null;	matOffsets	=null;
				matNumVerts	=null;	matNumTris	=null;	lmAnimVB	=null;
				lmAnimIB	=null;	lmAnimVB	=null;	lmAnimVD	=null;
				matAnimOffsets	=null;	matAnimNumVerts	=null;	matAnimNumTris	=null;
				lmaVB	=null;	lmaIB	=null;	lmaVD	=null;	amatOffsets	=null;
				amatNumVerts	=null;	amatNumTris	=null;	lmaAnimVB	=null;
				lmaAnimIB	=null;	lmaAnimVB	=null;	lmaAnimVD	=null;
				amatAnimOffsets	=null;	amatAnimNumVerts	=null;	amatAnimNumTris	=null;
				amatSortPoints	=null;	amatAnimSortPoints	=null;	lightAtlas	=null;
				return	false;
			}
			mg.GetLMBuffers(out lmVB, out lmIB, out lmVD);

			if(!mg.BuildLMAnimFaceData(mGFXVerts, mGFXVertIndexes, mGFXLightData))
			{
				lmVB	=null;	lmIB	=null;	lmVD	=null;	matOffsets	=null;
				matNumVerts	=null;	matNumTris	=null;	lmAnimVB	=null;
				lmAnimIB	=null;	lmAnimVB	=null;	lmAnimVD	=null;
				matAnimOffsets	=null;	matAnimNumVerts	=null;	matAnimNumTris	=null;
				lmaVB	=null;	lmaIB	=null;	lmaVD	=null;	amatOffsets	=null;
				amatNumVerts	=null;	amatNumTris	=null;	lmaAnimVB	=null;
				lmaAnimIB	=null;	lmaAnimVB	=null;	lmaAnimVD	=null;
				amatAnimOffsets	=null;	amatAnimNumVerts	=null;	amatAnimNumTris	=null;
				amatSortPoints	=null;	amatAnimSortPoints	=null;	lightAtlas	=null;
				return	false;
			}
			mg.GetLMAnimBuffers(out lmAnimVB, out lmAnimIB, out lmAnimVD);

			if(!mg.BuildLMAFaceData(mGFXVerts, mGFXVertIndexes, mGFXLightData))
			{
				lmVB	=null;	lmIB	=null;	lmVD	=null;	matOffsets	=null;
				matNumVerts	=null;	matNumTris	=null;	lmAnimVB	=null;
				lmAnimIB	=null;	lmAnimVB	=null;	lmAnimVD	=null;
				matAnimOffsets	=null;	matAnimNumVerts	=null;	matAnimNumTris	=null;
				lmaVB	=null;	lmaIB	=null;	lmaVD	=null;	amatOffsets	=null;
				amatNumVerts	=null;	amatNumTris	=null;	lmaAnimVB	=null;
				lmaAnimIB	=null;	lmaAnimVB	=null;	lmaAnimVD	=null;
				amatAnimOffsets	=null;	amatAnimNumVerts	=null;	amatAnimNumTris	=null;
				amatSortPoints	=null;	amatAnimSortPoints	=null;	lightAtlas	=null;
				return	false;
			}
			mg.GetLMABuffers(out lmaVB, out lmaIB, out lmaVD);

			if(!mg.BuildLMAAnimFaceData(mGFXVerts, mGFXVertIndexes, mGFXLightData))
			{
				lmVB	=null;	lmIB	=null;	lmVD	=null;	matOffsets	=null;
				matNumVerts	=null;	matNumTris	=null;	lmAnimVB	=null;
				lmAnimIB	=null;	lmAnimVB	=null;	lmAnimVD	=null;
				matAnimOffsets	=null;	matAnimNumVerts	=null;	matAnimNumTris	=null;
				lmaVB	=null;	lmaIB	=null;	lmaVD	=null;	amatOffsets	=null;
				amatNumVerts	=null;	amatNumTris	=null;	lmaAnimVB	=null;
				lmaAnimIB	=null;	lmaAnimVB	=null;	lmaAnimVD	=null;
				amatAnimOffsets	=null;	amatAnimNumVerts	=null;	amatAnimNumTris	=null;
				amatSortPoints	=null;	amatAnimSortPoints	=null;	lightAtlas	=null;
				return	false;
			}
			mg.GetLMAAnimBuffers(out lmaAnimVB, out lmaAnimIB, out lmaAnimVD);

			lightAtlas	=mg.GetLightMapAtlas();

			mg.GetLMMaterialData(out matOffsets, out matNumVerts, out matNumTris);
			mg.GetLMAnimMaterialData(out matAnimOffsets, out matAnimNumVerts, out matAnimNumTris);
			mg.GetLMAMaterialData(out amatOffsets, out amatNumVerts, out amatNumTris, out amatSortPoints);
			mg.GetLMAAnimMaterialData(out amatAnimOffsets, out amatAnimNumVerts, out amatAnimNumTris, out amatAnimSortPoints);

			return	true;
		}


		public void BuildVLitRenderData(GraphicsDevice g, out VertexBuffer vb,
			out IndexBuffer ib, out VertexDeclaration vd, out Int32 []matOffsets,
			out Int32 []matNumVerts, out Int32 []matNumTris)
		{
			MapGrinder	mg	=new MapGrinder(g, mGFXTexInfos, mGFXFaces, mLightMapGridSize, 1);

			Vector3	[]vnorms	=MakeVertNormals();

			mg.BuildVLitFaceData(mGFXVerts, mGFXRGBVerts, vnorms, mGFXVertIndexes);

			mg.GetVLitBuffers(out vb, out ib, out vd);

			mg.GetVLitMaterialData(out matOffsets, out matNumVerts, out matNumTris);
		}


		public void BuildAlphaRenderData(GraphicsDevice g, out VertexBuffer vb,
			out IndexBuffer ib, out VertexDeclaration vd, out Int32 []matOffsets,
			out Int32 []matNumVerts, out Int32 []matNumTris, out Vector3 []matSortPoints)
		{
			MapGrinder	mg	=new MapGrinder(g, mGFXTexInfos, mGFXFaces, mLightMapGridSize, 1);

			Vector3	[]vnorms	=MakeVertNormals();

			mg.BuildAlphaFaceData(mGFXVerts, mGFXRGBVerts, vnorms, mGFXVertIndexes);

			mg.GetAlphaBuffers(out vb, out ib, out vd);

			mg.GetAlphaMaterialData(out matOffsets, out matNumVerts, out matNumTris, out matSortPoints);
		}


		public void BuildFullBrightRenderData(GraphicsDevice g, out VertexBuffer vb,
			out IndexBuffer ib, out VertexDeclaration vd, out Int32 []matOffsets,
			out Int32 []matNumVerts, out Int32 []matNumTris)
		{
			MapGrinder	mg	=new MapGrinder(g, mGFXTexInfos, mGFXFaces, mLightMapGridSize, 1);

			mg.BuildFullBrightFaceData(mGFXVerts, mGFXVertIndexes);

			mg.GetFullBrightBuffers(out vb, out ib, out vd);

			mg.GetFullBrightMaterialData(out matOffsets, out matNumVerts, out matNumTris);
		}


		public void BuildMirrorRenderData(GraphicsDevice g, out VertexBuffer vb,
			out IndexBuffer ib, out VertexDeclaration vd, out Int32 []matOffsets,
			out Int32 []matNumVerts, out Int32 []matNumTris,
			out Vector3 []matSortPoints, out List<List<Vector3>> mirrorPolys)
		{
			MapGrinder	mg	=new MapGrinder(g, mGFXTexInfos, mGFXFaces, mLightMapGridSize, 1);

			Vector3	[]vnorms	=MakeVertNormals();

			mg.BuildMirrorFaceData(mGFXVerts, mGFXRGBVerts, vnorms, mGFXVertIndexes);

			mg.GetMirrorBuffers(out vb, out ib, out vd);

			mg.GetMirrorMaterialData(out matOffsets, out matNumVerts,
				out matNumTris, out matSortPoints, out mirrorPolys);
		}


		public void BuildSkyRenderData(GraphicsDevice g, out VertexBuffer vb,
			out IndexBuffer ib, out VertexDeclaration vd, out Int32 []matOffsets,
			out Int32 []matNumVerts, out Int32 []matNumTris)
		{
			MapGrinder	mg	=new MapGrinder(g, mGFXTexInfos, mGFXFaces, mLightMapGridSize, 1);

			mg.BuildSkyFaceData(mGFXVerts, mGFXVertIndexes);

			mg.GetSkyBuffers(out vb, out ib, out vd);

			mg.GetSkyMaterialData(out matOffsets, out matNumVerts, out matNumTris);
		}


		void FindParents_r(Int32 Node, Int32 Parent)
		{
			if(Node < 0)		// At a leaf, mark leaf parent and return
			{
				mLeafData[-(Node+1)].mParent	=Parent;
				return;
			}

			//At a node, mark node parent, and keep going till hitting a leaf
			mNodeParents[Node]	=Parent;

			// Go down front and back markinf parents on the way down...
			FindParents_r(mGFXNodes[Node].mChildren[0], Node);
			FindParents_r(mGFXNodes[Node].mChildren[1], Node);
		}
	}
}
