﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Xna.Framework;
using BSPCore;


namespace BSPVis
{
	public class VisParameters
	{
		public BSPBuildParams	mBSPParams;
		public VisParams		mVisParams;
		public string			mFileName;
		public BinaryWriter		mBSPFile;

		public ConcurrentQueue<MapVisClient>	mClients;
	}

	public class WorldLeaf
	{
		public Int32	mVisFrame;
		public Int32	mParent;
	}

	class WorkDivided
	{
		public Int32		mStartPort, mEndPort;
		public bool			mbAttempted;
		public bool			mbFinished;
		public DateTime		mTicTime;
		public int			mPollSeconds;
		public MapVisClient	mCruncher;
	}

	public class VisState
	{
		public byte	[]mVisData;
		public int	mStartPort;
		public int	mEndPort;
		public int	mTotalPorts;
	}

	public class VisMap
	{
		//bspmap
		Map	mMap	=new Map();

		//q2 vis related testing
		Q2Leaf		[]mQ2Leafs;
		Q2Portal	[]mQ2Portals;
		Q2Portal	[]mQ2SortedPortals;
		Q2VisLeaf	[]mQ2VisLeafs;

		//vis related stuff
		GFXLeaf		[]mGFXLeafs;
		GFXCluster	[]mGFXClusters;
		VISPortal	[]mVisPortals;
		VISPortal	[]mVisSortedPortals;
		VISLeaf		[]mVisLeafs;
		Int32		mNumVisLeafBytes, mNumVisPortalBytes;
		Int32		mNumVisMaterialBytes;

		//compiled vis data
		byte	[]mGFXVisData;
		byte	[]mGFXMaterialVisData;

		//area stuff
		List<GFXArea>		mAreas		=new List<GFXArea>();
		List<GFXAreaPortal>	mAreaPorts	=new List<GFXAreaPortal>();

		//threading
		TaskScheduler	mTaskSched	=TaskScheduler.Default;

		//done event for slow flood
		public static event EventHandler	eSlowFloodPartDone;


		void ThreadVisCB(object threadContext)
		{
			VisParameters	vp	=threadContext as VisParameters;

			GFXHeader	header	=mMap.LoadGBSPFile(vp.mFileName);
			if(header == null)
			{
				CoreEvents.Print("PvsGBSPFile:  Could not load GBSP file: " + vp.mFileName + "\n");
				CoreEvents.FireVisDoneEvent(false, null);
				return;
			}

			//copy out vis related stuff
			mGFXLeafs		=mMap.GetGFXLeafs();
			mGFXClusters	=mMap.GetGFXClusters();

			//Clean out any old vis data
			FreeFileVisData();

			string	visExt	=UtilityLib.FileUtil.StripExtension(vp.mFileName);

			visExt	+=".VisData";

			//Open the vis file for writing
			FileStream	fs	=new FileStream(visExt,
				FileMode.OpenOrCreate, FileAccess.Write);

			BinaryWriter	bw	=null;

			if(fs == null)
			{
				CoreEvents.Print("VisGBSPFile:  Could not open VisData file for writing: " + visExt + "\n");
				goto	ExitWithError;
			}

			//Prepare the portal file name
			string	PFile;
			int	extPos	=vp.mFileName.LastIndexOf(".");
			PFile		=vp.mFileName.Substring(0, extPos);
			PFile		+=".gpf";
			
			//Load the portal file
			if(!LoadPortalFile(PFile))
			{
				goto	ExitWithError;
			}

			CoreEvents.FireNumPortalsChangedEvent(mVisPortals.Length, null);

			CoreEvents.Print("NumPortals           : " + mVisPortals.Length + "\n");

			DateTime	startTime	=DateTime.Now;

			CoreEvents.Print("Starting vis at " + startTime + "\n");
			
			//Vis'em
			if(!VisAllLeafs(vp.mClients, vp.mFileName, vp))
			{
				goto	ExitWithError;
			}

			bw	=new BinaryWriter(fs);

			WriteVis(bw);

			//Free all the vis stuff
			FreeAllVisData();

			//Free any remaining leftover bsp data
			mMap.FreeGBSPFile();

			bw.Close();
			fs.Close();
			bw	=null;
			fs	=null;

			DateTime	done	=DateTime.Now;

			CoreEvents.Print("Finished vis at " + done + "\n");
			CoreEvents.Print(done - startTime + " elapsed\n");

			CoreEvents.FireVisDoneEvent(true, null);
			return;

			// ==== ERROR ====
			ExitWithError:
			{
				CoreEvents.Print("PvsGBSPFile:  Could not vis the file: " + vp.mFileName + "\n");

				if(bw != null)
				{
					bw.Close();
				}
				if(fs != null)
				{
					fs.Close();
				}

				FreeAllVisData();
				mMap.FreeGBSPFile();

				CoreEvents.FireVisDoneEvent(false, null);
				return;
			}
		}


		void ThreadQ2VisCB(object threadContext)
		{
			VisParameters	vp	=threadContext as VisParameters;

			LoadQ2Leaves(vp.mFileName);

			//Clean out any old vis data
			FreeFileVisData();

			string	visExt	=UtilityLib.FileUtil.StripExtension(vp.mFileName);

			visExt	+=".VisData";

			//Open the vis file for writing
			FileStream	fs	=new FileStream(visExt,
				FileMode.OpenOrCreate, FileAccess.Write);

			BinaryWriter	bw	=null;

			if(fs == null)
			{
				CoreEvents.Print("VisGBSPFile:  Could not open VisData file for writing: " + visExt + "\n");
				goto	ExitWithError;
			}

			//Prepare the portal file name
			string	PFile;
			int	extPos	=vp.mFileName.LastIndexOf(".");
			PFile		=vp.mFileName.Substring(0, extPos);
			PFile		+=".prt";
			
			//Load the portal file
			if(!LoadQ2PortalFile(PFile))
			{
				goto	ExitWithError;
			}

			DateTime	startTime	=DateTime.Now;
			CoreEvents.Print("Starting vis at " + startTime + "\n");
			
			//Vis'em
			if(!VisAllLeafs(vp.mClients, vp.mFileName, vp))
			{
				goto	ExitWithError;
			}

//			bw	=new BinaryWriter(fs);

//			WriteVis(bw);

			//Free all the vis stuff
			FreeAllVisData();

			//Free any remaining leftover bsp data
			mMap.FreeGBSPFile();

//			bw.Close();
			fs.Close();
			bw	=null;
			fs	=null;

			DateTime	done	=DateTime.Now;

			CoreEvents.Print("Finished vis at " + done + "\n");
			CoreEvents.Print(done - startTime + " elapsed\n");

			CoreEvents.FireVisDoneEvent(true, null);
			return;

			// ==== ERROR ====
			ExitWithError:
			{
				CoreEvents.Print("PvsGBSPFile:  Could not vis the file: " + vp.mFileName + "\n");

				if(bw != null)
				{
					bw.Close();
				}
				if(fs != null)
				{
					fs.Close();
				}

				FreeAllVisData();
				mMap.FreeGBSPFile();

				CoreEvents.FireVisDoneEvent(false, null);
				return;
			}
		}


		public void VisGBSPFile(string fileName, VisParams prms, BSPBuildParams prms2)
		{
			VisParameters	vp	=new VisParameters();
			vp.mBSPParams	=prms2;
			vp.mVisParams	=prms;
			vp.mFileName	=fileName;

			ThreadPool.QueueUserWorkItem(ThreadVisCB, vp);
		}


		public void VisGBSPFile(string fileName, VisParams prms,
			BSPBuildParams prms2, ConcurrentQueue<MapVisClient> clients)
		{
			VisParameters	vp	=new VisParameters();
			vp.mBSPParams	=prms2;
			vp.mVisParams	=prms;
			vp.mFileName	=fileName;
			vp.mClients		=clients;

			ThreadPool.QueueUserWorkItem(ThreadVisCB, vp);
		}


		public void SetMap(Map map)
		{
			mMap	=map;

			//copy out vis related stuff
			mGFXLeafs		=mMap.GetGFXLeafs();
			mGFXClusters	=mMap.GetGFXClusters();
		}


		public void SaveVisZoneData(BinaryWriter bw)
		{
			UtilityLib.FileUtil.WriteArray(mGFXVisData, bw);
			UtilityLib.FileUtil.WriteArray(mGFXMaterialVisData, bw);
			UtilityLib.FileUtil.WriteArray(mGFXClusters, bw); 
		}


		public void LoadVisData(string fileName)
		{
			string	visExt	=UtilityLib.FileUtil.StripExtension(fileName);

			visExt	+=".VisData";

			FileStream		fs	=new FileStream(visExt, FileMode.Open, FileAccess.Read);
			BinaryReader	br	=new BinaryReader(fs);

			UInt32	magic	=br.ReadUInt32();

			if(magic != 0x715da7aa)
			{
				return;
			}

			mGFXVisData	=UtilityLib.FileUtil.ReadByteArray(br);
			mGFXMaterialVisData	=UtilityLib.FileUtil.ReadByteArray(br);

			//load clusters
			mGFXClusters	=UtilityLib.FileUtil.ReadArray(br, delegate(Int32 count)
							{ return UtilityLib.FileUtil.InitArray<GFXCluster>(count); }) as GFXCluster[];

			br.Close();
			fs.Close();
		}


#if !X64
		public bool MaterialVisGBSPFile(string fileName)
		{
			CoreEvents.Print(" --- Material Vis GBSP File --- \n");

			GFXHeader	header	=mMap.LoadGBSPFile(fileName);
			if(header == null)
			{
				CoreEvents.Print("PvsGBSPFile:  Could not load GBSP file: " + fileName + "\n");
				return	false;
			}

			//make sure it is lit
			if(!mMap.HasLightData())
			{
				CoreEvents.Print("Map needs to be lit before material vis can work properly.\n");
				return	false;
			}

			//copy out vis related stuff
			mGFXLeafs		=mMap.GetGFXLeafs();
			mGFXClusters	=mMap.GetGFXClusters();

			LoadVisData(fileName);

			string	visExt	=UtilityLib.FileUtil.StripExtension(fileName);

			visExt	+=".VisData";

			//Open the bsp file for writing
			FileStream	fs	=new FileStream(visExt,
				FileMode.OpenOrCreate, FileAccess.Write);

			BinaryWriter	bw	=null;

			if(fs == null)
			{
				CoreEvents.Print("MatVisGBSPFile:  Could not open VisData file for writing: " + fileName + "\n");
				goto	ExitWithError;
			}

			//make a material vis, what materials
			//can be seen from each leaf
			VisMaterials();

			//Save the leafs, clusters, vis data, etc
			bw	=new BinaryWriter(fs);
			WriteVis(bw);

			//Free all the vis stuff
			FreeAllVisData();

			//Free any remaining leftover bsp data
			mMap.FreeGBSPFile();

			bw.Close();
			fs.Close();
			bw	=null;
			fs	=null;
			
			return	true;

			// ==== ERROR ====
			ExitWithError:
			{
				CoreEvents.Print("MatPvsGBSPFile:  Could not vis the file: " + fileName + "\n");

				if(bw != null)
				{
					bw.Close();
				}
				if(fs != null)
				{
					fs.Close();
				}

				FreeAllVisData();
				mMap.FreeGBSPFile();

				return	false;
			}
		}


		public bool IsMaterialVisibleFromPos(Vector3 pos, int matIndex)
		{
			Int32	node	=mMap.FindNodeLandedIn(0, pos);
			if(node > 0)
			{
				return	true;	//in solid space
			}

			Int32	leaf	=-(node + 1);
			return	IsMaterialVisible(leaf, matIndex);
		}


		public bool IsMaterialVisible(int leaf, int matIndex)
		{
			if(mGFXLeafs == null)
			{
				return	false;
			}

			int	clust	=mGFXLeafs[leaf].mCluster;

			if(clust == -1 || mGFXClusters[clust].mVisOfs == -1
				|| mGFXMaterialVisData == null)
			{
				return	true;	//this will make everything vis
								//when outside of the map
			}

			//plus one to avoid 0 problem
			matIndex++;

			int	ofs	=leaf * mNumVisMaterialBytes;
			
			return	((mGFXMaterialVisData[ofs + (matIndex >> 3)] & (1 << (matIndex & 7))) != 0);
		}


		public void VisMaterials()
		{
			Dictionary<Int32, List<string>>	visibleMaterials
				=new Dictionary<Int32, List<string>>();

			if(mGFXLeafs == null)
			{
				return;
			}

			CoreEvents.Print("Computing visible materials from each leaf...\n");

			//Grab map stuff needed to compute this
			int			firstLeaf	=mMap.GetWorldFirstLeaf();

			//make a temporary mapgrinder to help sync
			//up material names and indexes and such
			MapGrinder	mg	=mMap.MakeMapGrinder();

			object	prog	=ProgressWatcher.RegisterProgress(0, mGFXLeafs.Length, 0);

			for(int leaf=0;leaf < mGFXLeafs.Length;leaf++)
			{
				ProgressWatcher.UpdateProgress(prog, leaf);

				int	clust	=mGFXLeafs[leaf].mCluster;
				if(clust == -1)
				{
					continue;
				}

				int	ofs		=mGFXClusters[clust].mVisOfs;
				if(ofs == -1)
				{
					continue;
				}

				visibleMaterials.Add(leaf, new List<string>());

				List<int>	visibleClusters	=new List<int>();

				//Mark all visible clusters
				for(int i=0;i < mGFXClusters.Length;i++)
				{
					if((mGFXVisData[ofs + (i >> 3)] & (1 << (i & 7))) != 0)
					{
						visibleClusters.Add(i);
					}
				}

				for(int i=0;i < mGFXLeafs.Length;i++)
				{
					GFXLeaf	checkLeaf	=mGFXLeafs[firstLeaf + i];
					int		checkClust	=checkLeaf.mCluster;

					if(checkClust == -1 || !visibleClusters.Contains(checkClust))
					{
						continue;
					}
/*					for(int k=0;k < checkLeaf.mNumFaces;k++)
					{
						string	matName	=mMap.GetMaterialNameForLeafFace(k + checkLeaf.mFirstFace);

						if(!visibleMaterials[leaf].Contains(matName))
						{
							visibleMaterials[leaf].Add(matName);
						}
					}*/
				}
			}

			ProgressWatcher.Clear();

			//grab list of material names
			List<string>	matNames	=mg.GetMaterialNames();

			//alloc compressed bytes
			mNumVisMaterialBytes	=((matNames.Count + 63) & ~63) >> 3;

			mGFXMaterialVisData	=new byte[mGFXLeafs.Length * mNumVisMaterialBytes];

			//compress
			foreach(KeyValuePair<Int32, List<string>> visMat in visibleMaterials)
			{
				foreach(string mname in visMat.Value)
				{
					//zero doesn't or very well, so + 1 here
					int	idx	=matNames.IndexOf(mname) + 1;
					mGFXMaterialVisData[visMat.Key * mNumVisMaterialBytes + (idx >> 3)]
						|=(byte)(1 << (idx & 7));
				}
			}
			CoreEvents.Print("Material Vis Complete:  " + mGFXMaterialVisData.Length + " bytes.\n");
		}
#endif


		void WriteVis(BinaryWriter bw)
		{
			//write out vis file tag
			UInt32	magic	=0x715da7aa;
			bw.Write(magic);

			SaveVisZoneData(bw);
		}


		bool ClientHasPortals(MapVisClient amvc, int numPorts, out bool bRealFailure)
		{
			bRealFailure	=false;

			VisState	vs	=new VisState();

			vs.mVisData		=null;
			vs.mStartPort	=0;
			vs.mEndPort		=0;
			vs.mTotalPorts	=numPorts;

			bool	bHasSuccess	=false;

			try
			{
				bHasSuccess	=amvc.HasPortals(vs);
			}
			catch(Exception e)
			{
				//check for normal no worries exceptions
				if(e is System.AggregateException)
				{
					System.AggregateException	ae	=e as System.AggregateException;
					foreach(Exception ee in ae.InnerExceptions)
					{
						if(ee is System.ServiceModel.EndpointNotFoundException)
						{
						}
						else
						{
							bRealFailure	=true;
						}
					}
				}
				else if(e is System.ServiceModel.EndpointNotFoundException)
				{
				}
				else
				{
					bRealFailure	=true;
				}

				if(bRealFailure)
				{
					CoreEvents.Print("Exception: " + e.Message + " for HasPortals.  Will requeue...\n");
				}

				return	false;
			}
			return	bHasSuccess;
		}


		bool FeedPortalsToRemote(MapVisClient amvc, out bool bRealFailure)
		{
			MemoryStream	ms	=new MemoryStream();
			BinaryWriter	bw	=new BinaryWriter(ms);

			bw.Write(mVisSortedPortals.Length);
			foreach(VISPortal vp in mVisPortals)
			{
				vp.Write(bw);
			}

			bw.Write(mVisLeafs.Length);
			foreach(VISLeaf vl in mVisLeafs)
			{
				vl.Write(bw);
			}

			bw.Write(mNumVisPortalBytes);

			BinaryReader	br	=new BinaryReader(ms);
			br.BaseStream.Seek(0, SeekOrigin.Begin);

			byte	[]visDat	=br.ReadBytes((int)ms.Length);

			bw.Close();
			br.Close();
			ms.Close();

			bRealFailure	=false;

			VisState	vs	=new VisState();

			vs.mVisData		=visDat;
			vs.mStartPort	=0;
			vs.mEndPort		=0;
			vs.mTotalPorts	=mVisPortals.Length;

			bool	bReadSuccess	=false;

			try
			{
				bReadSuccess	=amvc.ReadPortals(vs);
			}
			catch(Exception e)
			{
				//check for normal no worries exceptions
				if(e is System.AggregateException)
				{
					System.AggregateException	ae	=e as System.AggregateException;
					foreach(Exception ee in ae.InnerExceptions)
					{
						if(ee is System.ServiceModel.EndpointNotFoundException)
						{
						}
						else
						{
							bRealFailure	=true;
						}
					}
				}
				else if(e is System.ServiceModel.EndpointNotFoundException)
				{
				}
				else
				{
					bRealFailure	=true;
				}

				if(bRealFailure)
				{
					CoreEvents.Print("Exception: " + e.Message + " for ReadPortals.  Will requeue...\n");
				}

				return	false;
			}

			return	bReadSuccess;
		}


		bool ProcessWork(WorkDivided wrk, MapVisClient amvc, out bool bRealFailure, string fileName)
		{
			bRealFailure	=false;

			VisState	vs	=new VisState();

			vs.mVisData		=null;
			vs.mStartPort	=wrk.mStartPort;
			vs.mEndPort		=wrk.mEndPort;			

			try
			{
				bool	bStarted	=amvc.BeginFloodPortalsSlow(vs);
			}
			catch(Exception e)
			{
				//check for normal no worries exceptions
				if(e is System.AggregateException)
				{
					System.AggregateException	ae	=e as System.AggregateException;
					foreach(Exception ee in ae.InnerExceptions)
					{
						if(ee is System.ServiceModel.EndpointNotFoundException)
						{
						}
						else
						{
							bRealFailure	=true;
						}
					}
				}
				else if(e is System.ServiceModel.EndpointNotFoundException)
				{
				}
				else
				{
					bRealFailure	=true;
				}

				if(bRealFailure)
				{
					CoreEvents.Print("Exception: " + e.Message + " for portals " + wrk.mStartPort + " to " + wrk.mEndPort + ".  Will requeue...\n");
				}

				return	false;
			}

			return	true;
		}


		List<WorkDivided> ResumeFromBitsFiles(string fileName)
		{
			int		dirPos		=fileName.LastIndexOf('\\');
			string	baseName	=UtilityLib.FileUtil.StripExtension(fileName);
			string	dirName		=baseName.Substring(0, dirPos);

			baseName	=baseName.Substring(dirPos + 1);

			DirectoryInfo	di	=new DirectoryInfo(dirName);
			FileInfo[]		fis	=di.GetFiles(baseName + "bits*", SearchOption.TopDirectoryOnly);

			foreach(FileInfo fi in fis)
			{
				int	postBits	=fi.Name.IndexOf("bits") + 4;

				string	justNums	=fi.Name.Substring(postBits);

				int	underPos	=justNums.IndexOf('_');

				string	leftNum		=justNums.Substring(0, underPos);
				string	rightNum	=justNums.Substring(underPos + 1);

				int	left, right;
				if(!int.TryParse(leftNum, out left))
				{
					continue;
				}
				if(!int.TryParse(rightNum, out right))
				{
					continue;
				}

				FileStream		fs	=fi.OpenRead();
				BinaryReader	br	=new BinaryReader(fs);

				for(int i=left;i < right;i++)
				{
					mVisPortals[i].ReadVisBits(br);
					mVisPortals[i].mbDone	=true;	//use to mark
				}

				br.Close();
				fs.Close();
			}

			//figure out what is left to do
			List<WorkDivided>	workRemaining	=new List<WorkDivided>();
			WorkDivided			current			=null;
			for(int i=0;i < mVisPortals.Length;i++)
			{
				if(!mVisPortals[i].mbDone && current == null)
				{
					current	=new WorkDivided();
					current.mStartPort	=i;
				}
				else if(mVisPortals[i].mbDone && current != null)
				{
					current.mEndPort	=i;
					workRemaining.Add(current);
					current	=null;
				}
			}

			if(current != null)
			{
				//never reached the end
				current.mEndPort	=mVisPortals.Length - 1;
				workRemaining.Add(current);
			}

			//reset mbDone
			for(int i=0;i < mVisPortals.Length;i++)
			{
				mVisPortals[i].mbDone	=false;
			}

			return	workRemaining;
		}


		void BytesToVisBits(byte []ports, int startPort, int endPort, string fileName)
		{
			string	saveChunk	=UtilityLib.FileUtil.StripExtension(fileName);

			saveChunk	+="bits" + startPort + "_" + endPort;

			MemoryStream	ms	=new MemoryStream();
			BinaryWriter	bw	=new BinaryWriter(ms);

			bw.Write(ports, 0, ports.Length);

			BinaryReader	br	=new BinaryReader(ms);
			br.BaseStream.Seek(0, SeekOrigin.Begin);

			for(int j=startPort;j < endPort;j++)
			{
				mVisPortals[j].ReadVisBits(br);
			}

			bw.Close();
			br.Close();
			ms.Close();

			//save to file too
			FileStream	fs	=new FileStream(saveChunk, FileMode.Create, FileAccess.Write);
			bw	=new BinaryWriter(fs);

			bw.Write(ports, 0, ports.Length);

			bw.Close();
			fs.Close();
		}


		void DistributedVis(ConcurrentQueue<MapVisClient> clients, string fileName, bool bResume, bool bVerbose)
		{
			if(clients == null || clients.Count == 0)
			{
				return;
			}

			int	granularity	=1;
			//choose a granularity, which is a size of how much
			//to split up the work
			if(mVisPortals.Length > 20000)
			{
				granularity	=50;
			}
			else if(mVisPortals.Length > 10000)
			{
				granularity	=100;
			}
			else
			{
				granularity	=500;
			}

			//make a list of work to be done
			ConcurrentQueue<WorkDivided>	work	=new ConcurrentQueue<WorkDivided>();

			if(bResume)
			{
				//load in the previously calculated stuff
				List<WorkDivided>	remainingWork	=ResumeFromBitsFiles(fileName);

				foreach(WorkDivided wd in remainingWork)
				{
					if(((wd.mEndPort - wd.mStartPort) / granularity) == 0)
					{
						WorkDivided	wdd		=new WorkDivided();
						wdd.mStartPort		=wd.mStartPort;
						wdd.mEndPort		=wd.mEndPort;
						wdd.mPollSeconds	=5;

						work.Enqueue(wdd);
						continue;
					}

					for(int i=0;i < ((wd.mEndPort - wd.mStartPort) / granularity);i++)
					{
						WorkDivided	wdd		=new WorkDivided();
						wdd.mStartPort		=i * granularity + wd.mStartPort;
						wdd.mEndPort		=wd.mStartPort + ((i + 1) * granularity);
						wdd.mPollSeconds	=5;

						work.Enqueue(wdd);
					}
				}
			}
			else
			{
				for(int i=0;i < (mVisPortals.Length / granularity);i++)
				{
					WorkDivided	wd	=new WorkDivided();
					wd.mStartPort	=i * granularity;
					wd.mEndPort		=(i + 1) * granularity;
					wd.mPollSeconds	=5;

					work.Enqueue(wd);
				}

				if(((mVisPortals.Length / granularity) * granularity) != mVisPortals.Length)
				{
					WorkDivided	remainder	=new WorkDivided();
					remainder.mStartPort	=(mVisPortals.Length / granularity) * granularity;
					remainder.mEndPort	=mVisPortals.Length;

					work.Enqueue(remainder);
				}
			}

			List<MapVisClient>	working		=new List<MapVisClient>();
			List<WorkDivided>	workingOn	=new List<WorkDivided>();

			CoreEvents.Print("Beginning distributed visibility with " + clients.Count + " possible work machines\n");

			DateTime	startTime	=DateTime.Now;

			object	prog	=ProgressWatcher.RegisterProgress(0, work.Count, 0);
			while(!work.IsEmpty || working.Count != 0)
			{
				MapVisClient	amvc	=null;

				//see if any clients are unbusy
				if(clients.TryDequeue(out amvc))
				{
					if(amvc == null)	//shouldn't happen
					{
						CoreEvents.Print("Null client in client queue!\n");
						continue;
					}

					if(bVerbose)
					{
						CoreEvents.Print("DeQueue of " + amvc.Endpoint.Address.ToString() + "\n");
					}

					bool	bRecreate;
					if(amvc.IsReadyOrTrashed(out bRecreate))
					{
						if(bVerbose)
						{
							CoreEvents.Print(amvc.Endpoint.Address.ToString() + " shows to be ready to go\n");
						}

						//add this client to the working list
						lock(working) {	working.Add(amvc); }

						Task	task	=Task.Factory.StartNew(() =>
						{
							bool	bWorking	=false;

							WorkDivided	wrk;
							if(work.TryDequeue(out wrk))
							{
								bool	bRealFailure;

								//see if client has portals
								bool	bHasPortals	=ClientHasPortals(amvc, mVisPortals.Length, out bRealFailure);

								if(bVerbose)
								{
									CoreEvents.Print(amvc.Endpoint.Address.ToString() + " HasPortals " + bHasPortals + " and RealFailure " + bRealFailure + "\n");
								}

								if(!bRealFailure)
								{
									bool	bFed	=false;
									if(!bHasPortals)
									{
										bFed	=FeedPortalsToRemote(amvc, out bRealFailure);
									}

									if(bVerbose)
									{
										CoreEvents.Print(amvc.Endpoint.Address.ToString() + " FedPortals " + bFed + " and RealFailure " + bRealFailure + "\n");
									}

									if(!bRealFailure && (bFed || bHasPortals))
									{
										wrk.mbAttempted	=true;

										if(!ProcessWork(wrk, amvc, out bRealFailure, fileName))
										{
											//failed, requeue
											work.Enqueue(wrk);
										}
										else
										{
											//client has begun work
											if(bVerbose)
											{
												CoreEvents.Print(amvc.Endpoint.Address.ToString() + " beginning work\n");
											}
											bWorking		=true;
											wrk.mTicTime	=DateTime.Now;
											wrk.mCruncher	=amvc;
											lock(workingOn) { workingOn.Add(wrk); }
										}
									}

									if(!bFed && !bHasPortals && !bRealFailure)
									{
										//something went wrong in the portal send stage
										if(bVerbose)
										{
											CoreEvents.Print(amvc.Endpoint.Address.ToString() + " had a problem\n");
										}
										work.Enqueue(wrk);
									}
								}

								if(bRealFailure)
								{
									if(bVerbose)
									{
										CoreEvents.Print("Build Farm Node : " + amvc.Endpoint.Address + " failed a work unit.  Requeueing it.\n");
									}
									amvc.mNumFailures++;
									work.Enqueue(wrk);
								}
							}
							if(!bWorking)
							{
								if(bVerbose)
								{
									CoreEvents.Print(amvc.Endpoint.Address.ToString() + " notworking, going back in client queue\n");
								}
								lock(working) { working.Remove(amvc); }
								clients.Enqueue(amvc);
							}
						});
					}
					else
					{
						if(bVerbose)
						{
							CoreEvents.Print(amvc.Endpoint.Address.ToString() + " not ready, bRecreate is " + bRecreate + "\n");
						}
						if(bRecreate)
						{
							//existing client hozed, make a new one
							//this will probably go on a lot if the endpoint is down
							MapVisClient	newMVC	=new MapVisClient("WSHttpBinding_IMapVis", amvc.mEndPointURI);
							clients.Enqueue(newMVC);
						}
						else
						{
							clients.Enqueue(amvc);
						}
					}
				}

				Thread.Sleep(1000);

				//check on the work in progress
				List<WorkDivided>	toCheck	=new List<WorkDivided>();
				lock(workingOn)
				{
					foreach(WorkDivided wrk in workingOn)
					{
						TimeSpan	elapsed	=DateTime.Now - wrk.mTicTime;

						if(elapsed.Seconds >= wrk.mPollSeconds)
						{
							toCheck.Add(wrk);
						}
					}
				}

				foreach(WorkDivided wrk in toCheck)
				{
					VisState	vs	=new VisState();
					vs.mStartPort	=wrk.mStartPort;
					vs.mEndPort		=wrk.mEndPort;

					byte	[]ports	=null;

					try
					{
						ports	=wrk.mCruncher.IsFinished(vs);
					}
					catch
					{
					}

					if(ports != null)
					{
						//finished a work unit!
						CoreEvents.Print(wrk.mCruncher.Endpoint.Address.ToString() + " finished a work unit\n");
						BytesToVisBits(ports, wrk.mStartPort, wrk.mEndPort, fileName);
						wrk.mbFinished	=true;
						ProgressWatcher.UpdateProgressIncremental(prog);
						lock(workingOn) { workingOn.Remove(wrk); }
						lock(working) { working.Remove(wrk.mCruncher); }
						clients.Enqueue(wrk.mCruncher);
					}

					wrk.mTicTime	=DateTime.Now;
				}
			}

			CoreEvents.Print("Finished vis in " + (DateTime.Now - startTime) + "\n");
			foreach(MapVisClient mvc in clients)
			{
				CoreEvents.Print("Freeing client portals\n");

				try
				{
					mvc.FreePortals();
				}
				catch {	}

				CoreEvents.Print(mvc.Endpoint.Address.ToString() + " with " + mvc.mNumFailures + " failures.\n");
			}
		}
		
		
		void RecursiveLeafFlowGenesis(VISPortal destPort, int leafNum, VISPStack prevStack)
		{
			VISLeaf	leaf	=mVisLeafs[leafNum];
			
			VISPStack	stack	=new VISPStack();

			prevStack.mNext	=stack;
			
			stack.mNext		=null;
			stack.mLeaf		=leaf;
			stack.mPortal	=null;
			
			byte	[]might	=stack.mVisBits;
			byte	[]vis	=destPort.mPortalVis;
			
			//check all portals for flowing into other leafs
			for(int i=0;i < leaf.mPortals.Count;i++)
			{
				VISPortal	p	=leaf.mPortals[i];

				int	pnum	=p.mPortNum;

				if((prevStack.mVisBits[pnum >> 3] & (1 << (pnum & 7))) == 0)
				{
					continue;	// can't possibly see it
				}
				
				//if the portal can't see anything we haven't allready seen, skip it
				byte	[]test	=null;
				if(p.mbDone)
				{
					test	=p.mPortalVis;
				}
				else
				{
					test	=p.mPortalFlood;
				}

				Int32	more	=0;
				for(int j=0;j < mNumVisPortalBytes;j++)
				{
					might[j]	=(byte)(prevStack.mVisBits[j] & test[j]);
					more		|=(might[j] & ~vis[j]);
				}
		
				if((more == 0) && (vis[pnum >> 3] & (1 << (pnum & 7))) != 0)
				{	//can't see anything new
					continue;
				}

				//get plane of portal, point normal into the neighbor leaf
				stack.mPortalPlane	=p.mPlane;
				stack.mPortal		=p;
				stack.mNext			=null;

				stack.mPass	=new GBSPPoly(p.mPoly);
				if(!stack.mPass.ClipPoly(destPort.mPlane, false))
				{
					continue;
				}
				if(stack.mPass.mVerts == null)
				{
					stack.mPass	=null;
					continue;
				}

				stack.mSource	=new GBSPPoly(prevStack.mSource);
				if(!stack.mSource.ClipPoly(p.mPlane, true))
				{
					continue;
				}
				if(stack.mSource.mVerts == null)
				{
					stack.mSource	=null;
					continue;
				}

				if(prevStack.mPass == null)
				{	//the second leaf can only be blocked if coplanar

					//mark the portal as visible
					vis[pnum >> 3]	|=(byte)(1 << (pnum & 7));

					RecursiveLeafFlowGenesis(destPort, p.mLeaf, stack);
					continue;
				}

				if(!stack.mPass.SeperatorClip(stack.mSource, prevStack.mPass, false))
				{
					continue;
				}
				if(stack.mPass.mVerts == null)
				{
					stack.mPass	=null;
					continue;
				}
				if(!stack.mPass.SeperatorClip(prevStack.mPass, stack.mSource, true))
				{
					continue;
				}
				if(stack.mPass.mVerts == null)
				{
					stack.mPass	=null;
					continue;
				}

				//mark the portal as visible
				vis[pnum >> 3]	|=(byte)(1 << (pnum & 7));

				//flow through it for real
				RecursiveLeafFlowGenesis(destPort, p.mLeaf, stack);
			}	
		}


		void RecursiveLeafFlow(Q2Portal destPort, int leafNum, Q2Stack prevStack)
		{
			Q2VisLeaf	leaf	=mQ2VisLeafs[leafNum];
			
			Q2Stack	stack	=new Q2Stack();

			prevStack.mNext	=stack;
			
			stack.mNext		=null;
			stack.mLeaf		=leaf;
			stack.mPortal	=null;
			
			byte	[]might	=stack.mVisBits;
			byte	[]vis	=destPort.mPortalVis;
			
			//check all portals for flowing into other leafs
			for(int i=0;i < leaf.mNumPortals;i++)
			{
				Q2Portal	p	=leaf.mPortals[i];

				int	pnum	=p.mPortNum;

				if((prevStack.mVisBits[pnum >> 3] & (1<<(pnum&7))) == 0)
				{
					continue;	// can't possibly see it
				}
				
				//if the portal can't see anything we haven't allready seen, skip it
				byte	[]test	=null;
				if(p.mbDone)
				{
					test	=p.mPortalVis;
				}
				else
				{
					test	=p.mPortalFlood;
				}

				Int32	more	=0;
				for(int j=0;j < mNumVisPortalBytes;j++)
				{
					might[j]	=(byte)(prevStack.mVisBits[j] & test[j]);
					more		|=(might[j] & ~vis[j]);
				}
		
				if((more == 0) && (vis[pnum >> 3] & (1 << (pnum & 7))) != 0)
				{	//can't see anything new
					continue;
				}

				//get plane of portal, point normal into the neighbor leaf
				stack.mPortalPlane	=p.mPlane;
				stack.mPortal		=p;
				stack.mNext			=null;

				stack.mPass	=new GBSPPoly(p.mWinding);
				if(!stack.mPass.ClipPoly(destPort.mPlane, true))
				{
					continue;
				}
				if(stack.mPass.mVerts == null)
				{
					stack.mPass	=null;
					continue;
				}

				stack.mSource	=new GBSPPoly(prevStack.mSource);
				if(!stack.mSource.ClipPoly(p.mPlane, false))
				{
					continue;
				}
				if(stack.mSource.mVerts == null)
				{
					stack.mSource	=null;
					continue;
				}

				if(prevStack.mPass == null)
				{	//the second leaf can only be blocked if coplanar

					//mark the portal as visible
					vis[pnum >> 3]	|=(byte)(1 << (pnum & 7));

					RecursiveLeafFlow(destPort, p.mLeaf, stack);
					continue;
				}

				if(!stack.mPass.SeperatorClip(stack.mSource, prevStack.mPass, false))
				{
					continue;
				}
				if(stack.mPass.mVerts == null)
				{
					stack.mPass	=null;
					continue;
				}
				if(!stack.mPass.SeperatorClip(prevStack.mPass, stack.mSource, true))
				{
					continue;
				}
				if(stack.mPass.mVerts == null)
				{
					stack.mPass	=null;
					continue;
				}

				//mark the portal as visible
				vis[pnum >> 3]	|=(byte)(1 << (pnum & 7));

				//flow through it for real
				RecursiveLeafFlow(destPort, p.mLeaf, stack);
			}	
		}


		void	PortalFlow(int portalnum)
		{
			Q2Portal	p	=mQ2SortedPortals[portalnum];

			p.mbDone	=false;

			int	c_might	=CountBits(p.mPortalFlood, mQ2SortedPortals.Length);

			Q2Stack	vps	=new Q2Stack();
			vps.mSource	=p.mWinding;
			p.mPortalFlood.CopyTo(vps.mVisBits, 0);

			RecursiveLeafFlow(p, p.mLeaf, vps);

			p.mbDone	=true;

			int	c_can	=CountBits(p.mPortalVis, mQ2SortedPortals.Length);
			CoreEvents.Print("portal:" + p.mPortNum + " mightsee:" + c_might +
				" cansee:" + c_can + "\n");
		}


		void	PortalFlowGenesis(int portalnum)
		{
			VISPortal	p	=mVisSortedPortals[portalnum];

			p.mbDone	=false;

			int	c_might	=CountBits(p.mPortalFlood, mVisSortedPortals.Length);

			VISPStack	vps	=new VISPStack();
			vps.mSource		=p.mPoly;
			p.mPortalFlood.CopyTo(vps.mVisBits, 0);

			RecursiveLeafFlowGenesis(p, p.mLeaf, vps);

			p.mbDone	=true;

			int	c_can	=CountBits(p.mPortalVis, mVisSortedPortals.Length);
			CoreEvents.Print("portal:" + p.mPortNum + " mightsee:" + c_might +
				" cansee:" + c_can + "\n");
		}


		static void	PortalFlowGenesis(int portalnum, VISPortal []visPortals, VISLeaf []visLeafs)
		{
			VISPortal	p	=visPortals[portalnum];

			p.mbDone	=false;

			int	c_might	=CountBits(p.mPortalFlood, visPortals.Length);

			VISPStack	vps	=new VISPStack();
			vps.mSource		=p.mPoly;
			p.mPortalFlood.CopyTo(vps.mVisBits, 0);

//			RecursiveLeafFlowGenesis(p, p.mLeaf, vps);

			p.mbDone	=true;

			int	c_can	=CountBits(p.mPortalVis, visPortals.Length);
			CoreEvents.Print("portal:" + p.mPortNum + " mightsee:" + c_might +
				" cansee:" + c_can + "\n");
		}


		bool VisAllLeafs(ConcurrentQueue<MapVisClient> clients, string fileName, VisParameters vp)
		{
			CoreEvents.Print("Quick vis for " + mVisPortals.Length + " portals...\n");

			object	prog	=ProgressWatcher.RegisterProgress(0, mVisPortals.Length, 0);

#if false
			for(int i=0;i < mQ2Portals.Length;i++)
			{
				BasePortalVis(i);
				ProgressWatcher.UpdateProgress(prog, i);
			}
			ProgressWatcher.Clear();
#else
			//Flood all the leafs with the fast method first...
			for(int i=0;i < mVisPortals.Length; i++)
			{
				BasePortalVisGenesis(i);
				ProgressWatcher.UpdateProgress(prog, i);
			}
			ProgressWatcher.Clear();
#endif
			//Sort the portals with MightSee
			SortPortals();

			if(vp.mVisParams.mbFullVis)
			{
				if(vp.mVisParams.mbDistribute)
				{
					DistributedVis(clients, fileName, vp.mVisParams.mbResume, vp.mBSPParams.mbVerbose);
				}
				else
				{
					prog	=ProgressWatcher.RegisterProgress(0, mVisPortals.Length, 0);
					if(!FloodPortalsSlowGenesis(0, mVisPortals.Length, vp.mBSPParams.mbVerbose, prog))
					{
						return	false;
					}
				}
			}
			ProgressWatcher.Clear();

			mGFXVisData	=new byte[mVisLeafs.Length * mNumVisLeafBytes];
			if(mGFXVisData == null)
			{
				CoreEvents.Print("VisAllLeafs:  Out of memory for LeafVisBits.\n");
				goto	ExitWithError;
			}

			//null out full vis arrays if no full vis
			if(!vp.mVisParams.mbFullVis)
			{
				foreach(VISPortal vsp in mVisPortals)
				{
					vsp.mPortalVis	=null;
				}
			}
			int	TotalVisibleLeafs	=0;

			for(int i=0;i < mVisLeafs.Length;i++)
			{
				int	leafSee	=0;

				
				if(!CollectLeafVisBits(i, ref leafSee))
				{
					goto	ExitWithError;
				}
				TotalVisibleLeafs	+=leafSee;
			}

			CoreEvents.Print("Total visible areas           : " + TotalVisibleLeafs + "\n");
			CoreEvents.Print("Average visible from each area: " + TotalVisibleLeafs / mVisLeafs.Length + "\n");

			return	true;

			// ==== ERROR ====
			ExitWithError:
			{
				// Free all the global vis data
				FreeAllVisData();

				return	false;
			}
		}


		bool FloodPortalsSlowQ2(int startPort, int endPort, bool bVerbose, object prog)
		{
			for(int k=startPort;k < endPort;k++)
			{
				mQ2Portals[k].mbDone	=false;
			}

			int	count	=startPort;
			for(int k=startPort;k < endPort;k++)
//			Parallel.For(startPort, endPort, (k) =>
			{
				Q2Portal	port	=mQ2SortedPortals[k];
				
				//This portal can't see anyone yet...
				for(int i=0;i < mNumVisPortalBytes;i++)
				{
					port.mPortalVis[i]	=0;
				}
				PortalFlow(k);

				port.mbDone			=true;

				Interlocked.Increment(ref count);
				if(count % 10 == 0 && prog != null)
				{
					ProgressWatcher.UpdateProgress(prog, count);
				}

				if(bVerbose)
				{
//					CoreEvents.Print("Portal: " + (k + 1) + " - Fast Vis: "
//						+ port.mMightSee + ", Full Vis: "
//						+ port.mCanSee + ", remaining: "
//						+ (endPort - count) + "\n");
//					CoreEvents.Print("Portal: " + (k + 1) + " - Fast Vis: "
//						+ port.mNumMightSee + ", Full Vis: "
//						+ vPools.mCanSee + ", iterations: "
//						+ vPools.mIterations + "\n");
				}
			}//);
			return	true;
		}


		bool FloodPortalsSlowGenesis(int startPort, int endPort, bool bVerbose, object prog)
		{
			for(int k=startPort;k < endPort;k++)
			{
				mVisPortals[k].mbDone	=false;
			}

			int	count	=startPort;
//			for(int k=startPort;k < endPort;k++)
			Parallel.For(startPort, endPort, (k) =>
			{
				VISPortal	port	=mVisSortedPortals[k];
				
				//This portal can't see anyone yet...
				for(int i=0;i < mNumVisPortalBytes;i++)
				{
					port.mPortalVis[i]	=0;
				}
				PortalFlowGenesis(k);

				port.mbDone			=true;

				Interlocked.Increment(ref count);
				if(count % 10 == 0 && prog != null)
				{
					ProgressWatcher.UpdateProgress(prog, count);
				}

				if(bVerbose)
				{
//					CoreEvents.Print("Portal: " + (k + 1) + " - Fast Vis: "
//						+ port.mMightSee + ", Full Vis: "
//						+ port.mCanSee + ", remaining: "
//						+ (endPort - count) + "\n");
//					CoreEvents.Print("Portal: " + (k + 1) + " - Fast Vis: "
//						+ port.mNumMightSee + ", Full Vis: "
//						+ vPools.mCanSee + ", iterations: "
//						+ vPools.mIterations + "\n");
				}
			});
			return	true;
		}


		//used by the external distributed vis
		public static byte []FloodPortalsSlow(byte []visData, int startPort, int endPort)
		{
			/*
			DateTime	visTime	=DateTime.Now;

			//convert the bytes to something usable
			MemoryStream	ms	=new MemoryStream();
			BinaryWriter	bw	=new BinaryWriter(ms);

			bw.Write(visData, 0, visData.Length);

			BinaryReader	br	=new BinaryReader(ms);
			br.BaseStream.Seek(0, SeekOrigin.Begin);

			int	portCount	=br.ReadInt32();

			List<Int32>	indexes			=new List<Int32>();
			VISPortal	[]visPortals	=new VISPortal[portCount];
			for(int i=0;i < portCount;i++)
			{
				visPortals[i]	=new VISPortal();
				visPortals[i].Read(br, indexes);
			}

			//rebuild indexes
			for(int i=0;i < portCount;i++)
			{
				if(indexes[i] >= 0)
				{
					visPortals[i].mNext	=visPortals[indexes[i]];
				}
			}

			//read visleafs
			int	leafCount	=br.ReadInt32();
			VISLeaf	[]visLeafs	=new VISLeaf[leafCount];
			for(int i=0;i < leafCount;i++)
			{
				visLeafs[i]	=new VISLeaf();
				visLeafs[i].Read(br, visPortals);
			}

			//read numbytes
			int	numVisPortalBytes	=br.ReadInt32();

			for(int k=startPort;k < endPort;k++)
			{
				visPortals[k].mbDone	=false;
			}

			bw.Close();
			br.Close();
			ms.Close();

			int	count	=startPort;

//			for(int i=0;i < startPort;i++)
//			{
//				VisLoop(visPortals, visLeafs, numVisPortalBytes, i, endPort, ref count);
//			}

			Parallel.For(startPort, endPort, (k) =>
				{
					VisLoop(visPortals, visLeafs, numVisPortalBytes, k, endPort, ref count);
				});*/

/*
			List<Task>	tasks	=new List<Task>();

			for(int i=startPort;i < endPort;i++)
			{
				int	temp	=i;

				Task	task	=new Task(() => VisLoop(visPortals, visLeafs, numVisPortalBytes, temp, endPort, ref count));

				tasks.Add(task);
				task.Start();
			}

			Task.WaitAll(tasks.ToArray());*/
			/*
			//put vis bits in return data
			ms	=new MemoryStream();
			bw	=new BinaryWriter(ms);

			for(int i=startPort;i < endPort;i++)
			{
				visPortals[i].WriteVisBits(bw);
			}

			//read into memory
			br	=new BinaryReader(ms);
			br.BaseStream.Seek(0, SeekOrigin.Begin);

			byte	[]returnBytes	=br.ReadBytes((int)ms.Length);

			bw.Close();
			br.Close();
			ms.Close();

			VisState	vs	=new VisState();
			vs.mStartPort	=startPort;
			vs.mEndPort		=endPort;
			vs.mTotalPorts	=0;
			vs.mVisData		=returnBytes;

			UtilityLib.Misc.SafeInvoke(eSlowFloodPartDone, vs);

			return	returnBytes;*/
			return	null;
		}


		static void VisLoop(VISPortal []visPortals, VISLeaf []visLeafs,
			int numVisPortalBytes, int k, int endPort, ref int count)
		{
			VISPortal	port	=visPortals[k];

			//This portal can't see anyone yet...
			for(int i=0;i < numVisPortalBytes;i++)
			{
				port.mPortalVis[i]	=0;
			}
			PortalFlowGenesis(k, visPortals, visLeafs);

			port.mbDone			=true;

			Interlocked.Increment(ref count);

			Console.WriteLine("Portal: " + (k + 1) + " - Fast Vis: "
				+ port.mMightSee + ", Full Vis: "
				+ port.mCanSee + ", iterations: ");
//				+ vPools.mIterations);
//			Console.WriteLine("Portal: " + (k + 1) + " - Fast Vis: "
//				+ port.mMightSee + ", Full Vis: "
//				+ port.mCanSee + ", remaining: " + (endPort - count));
		}


		bool CollectLeafVisBits(int leafNum, ref int leafSee)
		{
			VISPortal	sport;
			VISLeaf		Leaf;
			Int32		k, Bit, SLeaf, LeafBitsOfs;
			
			Leaf	=mVisLeafs[leafNum];

			LeafBitsOfs	=leafNum * mNumVisLeafBytes;

			byte	[]portalBits	=new byte[mNumVisPortalBytes];

			if(!VISPortal.CollectBits(Leaf.mPortals, portalBits))
			{
				return	false;
			}

			// Take this list, and or all leafs that each visible portal looks in to
			for(k=0;k < mVisPortals.Length;k++)
			{
				if((portalBits[k >> 3] & (1 << (k & 7))) != 0)
				{
					sport	=mVisPortals[k];
					SLeaf	=sport.mLeaf;
					Debug.Assert((1 << (SLeaf & 7)) < 256);
					mGFXVisData[LeafBitsOfs + (SLeaf >> 3)]	|=(byte)(1 << (SLeaf & 7));
				}
			}
					
			Bit	=1 << (leafNum & 7);

			Debug.Assert(Bit < 256);

			//He should not have seen himself (yet...)
			if((mGFXVisData[LeafBitsOfs + (leafNum >> 3)] & Bit) != 0)
			{
				CoreEvents.Print("*WARNING* CollectLeafVisBits:  Leaf:" + leafNum + " can see himself!\n");
			}
			mGFXVisData[LeafBitsOfs + (leafNum >> 3)]	|=(byte)Bit;

			for(k=0;k < mVisLeafs.Length;k++)
			{
				Bit	=(1 << (k & 7));

				if((mGFXVisData[LeafBitsOfs + (k>>3)] & Bit) != 0)
				{
					leafSee++;
				}
			}

			if(leafSee == 0)
			{
				CoreEvents.Print("CollectLeafVisBits:  Leaf can see nothing.\n");
				return	false;
			}

			mGFXClusters[leafNum].mVisOfs	=LeafBitsOfs;

			return	true;
		}


		void SortPortals()
		{
#if true
			List<VISPortal>	sortMe	=new List<VISPortal>(mVisPortals);

			sortMe.Sort(new VisPortalComparer());

			mVisSortedPortals	=sortMe.ToArray();
#else
			List<Q2Portal>	sortMe	=new List<Q2Portal>(mQ2Portals);

			sortMe.Sort(new Q2PortalComparer());

			mQ2SortedPortals	=sortMe.ToArray();
#endif
		}


		void BasePortalVis(int portNum)
		{
			Q2Portal	p	=mQ2Portals[portNum];

			p.mPortalFront	=new byte[mNumVisPortalBytes];
			p.mPortalFlood	=new byte[mNumVisPortalBytes];
			p.mPortalVis	=new byte[mNumVisPortalBytes];

			for(int i=0;i < mQ2Portals.Length;i++)
			{
				if(i == portNum)
				{
					continue;
				}

				Q2Portal	tp	=mQ2Portals[i];

				int	k=0;
				for(k=0;k < tp.mWinding.mVerts.Length;k++)
				{
					float	d	=Vector3.Dot(tp.mWinding.mVerts[k], p.mPlane.mNormal)
									- p.mPlane.mDist;
					if(d < -UtilityLib.Mathery.ON_EPSILON)
					{
						break;
					}
				}
				if(k == tp.mWinding.mVerts.Length)
				{
					continue;
				}

				for(k=0;k < p.mWinding.mVerts.Length;k++)
				{
					float	d	=Vector3.Dot(p.mWinding.mVerts[k], tp.mPlane.mNormal)
									- tp.mPlane.mDist;
					if(d > UtilityLib.Mathery.ON_EPSILON)
					{
						break;
					}
				}
				if(k == p.mWinding.mVerts.Length)
				{
					continue;
				}

				p.mPortalFront[i >> 3]	|=(byte)(1 << (i & 7));
			}

			SimpleFlood(p, p.mLeaf);

			p.mNumMightSee	=CountBits(p.mPortalFlood, mQ2Portals.Length);
		}
		
		
		void BasePortalVisGenesis(int portNum)
		{
			VISPortal	p	=mVisPortals[portNum];

			p.mPortalFront	=new byte[mNumVisPortalBytes];
			p.mPortalFlood	=new byte[mNumVisPortalBytes];
			p.mPortalVis	=new byte[mNumVisPortalBytes];

			for(int i=0;i < mVisPortals.Length;i++)
			{
				if(i == portNum)
				{
					continue;
				}

				VISPortal	tp	=mVisPortals[i];

				int	k=0;
				for(k=0;k < tp.mPoly.mVerts.Length;k++)
				{
					float	d	=Vector3.Dot(tp.mPoly.mVerts[k], p.mPlane.mNormal)
									- p.mPlane.mDist;
					if(d > UtilityLib.Mathery.ON_EPSILON)
					{
						break;
					}
				}
				if(k == tp.mPoly.mVerts.Length)
				{
					continue;
				}

				for(k=0;k < p.mPoly.mVerts.Length;k++)
				{
					float	d	=Vector3.Dot(p.mPoly.mVerts[k], tp.mPlane.mNormal)
									- tp.mPlane.mDist;
					if(d < -UtilityLib.Mathery.ON_EPSILON)
					{
						break;
					}
				}
				if(k == p.mPoly.mVerts.Length)
				{
					continue;
				}

				p.mPortalFront[i >> 3]	|=(byte)(1 << (i & 7));
			}

			SimpleFloodGenesis(p, p.mLeaf);

			p.mMightSee	=CountBits(p.mPortalFlood, mVisPortals.Length);
		}
		
		
		static int CountBits(byte []bits, int numbits)
		{
			int		i;
			int		c = 0;
			
			for(i=0 ; i<numbits ; i++)
			{
				if((bits[i>>3] & (1<<(i&7))) != 0)
				{
					c++;
				}
			}
			return	c;
		}


		void SimpleFlood(Q2Portal src, int leafNum)
		{
			Q2VisLeaf	leaf	=mQ2VisLeafs[leafNum];

			for(int i=0;i < leaf.mNumPortals;i++)
			{
				Q2Portal	port	=leaf.mPortals[i];

				int	pNum	=port.mPortNum;

				if((src.mPortalFront[pNum >> 3] & (1 << (pNum & 7))) == 0)
				{
					continue;
				}

				if((src.mPortalFlood[pNum >> 3] & (1 << (pNum & 7))) != 0)
				{
					continue;
				}

				src.mPortalFlood[pNum >> 3]	|=(byte)(1 << (pNum & 7));

				SimpleFlood(src, port.mLeaf);
			}
		}


		void SimpleFloodGenesis(VISPortal src, int leafNum)
		{
			VISLeaf	leaf	=mVisLeafs[leafNum];

			for(int i=0;i < leaf.mPortals.Count;i++)
			{
				VISPortal	port	=leaf.mPortals[i];

				int	pNum	=port.mPortNum;

				if((src.mPortalFront[pNum >> 3] & (1 << (pNum & 7))) == 0)
				{
					continue;
				}

				if((src.mPortalFlood[pNum >> 3] & (1 << (pNum & 7))) != 0)
				{
					continue;
				}

				src.mPortalFlood[pNum >> 3]	|=(byte)(1 << (pNum & 7));

				SimpleFloodGenesis(src, port.mLeaf);
			}
		}


		/*
		void FloodLeafPortalsFast(int leafNum, bool []portSeen)
		{
			VISLeaf	leaf	=mVisLeafs[leafNum];

			if(leaf.mPortals == null)
			{
				CoreEvents.Print("*WARNING* FloodLeafPortalsFast:  Leaf with no portals.\n");
				return;
			}
			
			int	srcLeaf	=leafNum;

			for(VISPortal port=leaf.mPortals;port != null;port=port.mNext)
			{
				port.mVisBits	=new byte[mNumVisPortalBytes];

				//This portal can't see anyone yet...
				for(int i=0;i < mNumVisPortalBytes;i++)
				{
					port.mVisBits[i]	=0;
				}
				for(int i=0;i < mVisPortals.Length;i++)
				{
					portSeen[i]	=false;
				}

				int	mightSee	=0;
				
				port.FloodPortalsFast_r(port, portSeen, mVisLeafs, srcLeaf, ref mightSee);
			}
		}*/


		void SaveGFXVisData(BinaryWriter bw)
		{
			bw.Write(mGFXVisData.Length);
			bw.Write(mGFXVisData, 0, mGFXVisData.Length);
		}


		void LoadGFXVisData(BinaryReader br)
		{
			int	count	=br.ReadInt32();
			mGFXVisData	=br.ReadBytes(count);
		}


		void SaveGFXMaterialVisData(BinaryWriter bw)
		{
			bw.Write(mGFXMaterialVisData.Length);
			bw.Write(mGFXMaterialVisData, 0, mGFXMaterialVisData.Length);
		}


		void LoadGFXMaterialVisData(BinaryReader br)
		{
			int	count			=br.ReadInt32();
			mGFXMaterialVisData	=br.ReadBytes(count);
		}


		public void FreeFileVisData()
		{
			mGFXVisData			=null;
			mGFXMaterialVisData	=null;
		}


		void FreeAllVisData()
		{
			FreeFileVisData();

			if(mVisPortals != null)
			{
				for(int i=0;i < mVisPortals.Length;i++)
				{
					mVisPortals[i].mPoly		=null;
					mVisPortals[i].mPortalFlood	=null;
					mVisPortals[i].mPortalFront	=null;
					mVisPortals[i].mPortalVis	=null;
				}

				mVisPortals	=null;
			}
			mVisPortals			=null;
			mVisSortedPortals	=null;
			mVisLeafs			=null;

			mMap.FreeGBSPFile();	//Free rest of GBSP GFX data
		}


		bool LoadQ2Leaves(string fileName)
		{
			FileStream	fs	=new FileStream(fileName, FileMode.Open, FileAccess.Read);
			if(fs == null)
			{
				return	false;
			}

			BinaryReader	br	=new BinaryReader(fs);

			UInt32	magic	=br.ReadUInt32();
			UInt32	ver		=br.ReadUInt32();

			List<int>	offsets	=new List<int>();
			List<int>	lengths	=new List<int>();

			for(int i=0;i < 19;i++)
			{
				offsets.Add(br.ReadInt32());
				lengths.Add(br.ReadInt32());
			}

			//leafs are index 8
			br.BaseStream.Seek(offsets[8], SeekOrigin.Begin);

			int	numLeaves	=lengths[8] / 28;	//q2leaf size 28

//			mGFXLeafs	=new GFXLeaf[numLeaves];
			mQ2Leafs	=new Q2Leaf[numLeaves];

			for(int i=0;i < numLeaves;i++)
			{
#if false
				mGFXLeafs[i]	=new GFXLeaf();

				UInt32	contents	=br.ReadUInt32();

				//invert x and swap y and z
				mGFXLeafs[i].mContents	=contents;//Contents.FixContents(contents);
				mGFXLeafs[i].mCluster	=br.ReadInt16();
				mGFXLeafs[i].mArea		=br.ReadInt16();
				mGFXLeafs[i].mMins.X	=-br.ReadInt16();
				mGFXLeafs[i].mMins.Z	=br.ReadInt16();
				mGFXLeafs[i].mMins.Y	=br.ReadInt16();
				mGFXLeafs[i].mMaxs.X	=-br.ReadInt16();
				mGFXLeafs[i].mMaxs.Z	=br.ReadInt16();
				mGFXLeafs[i].mMaxs.Y	=br.ReadInt16();
				mGFXLeafs[i].mFirstFace	=br.ReadUInt16();
				mGFXLeafs[i].mNumFaces	=br.ReadUInt16();
				mGFXLeafs[i].mFirstSide	=br.ReadUInt16();
				mGFXLeafs[i].mNumSides	=br.ReadUInt16();
#else
				mQ2Leafs[i]		=new Q2Leaf();

				Int32	contents	=br.ReadInt32();

				//invert x and swap y and z
				mQ2Leafs[i].mContents		=contents;//Contents.FixContents(contents);
				mQ2Leafs[i].mCluster		=br.ReadInt16();
				mQ2Leafs[i].mArea			=br.ReadInt16();
				mQ2Leafs[i].mMins.X			=-br.ReadInt16();
				mQ2Leafs[i].mMins.Z			=br.ReadInt16();
				mQ2Leafs[i].mMins.Y			=br.ReadInt16();
				mQ2Leafs[i].mMaxs.X			=-br.ReadInt16();
				mQ2Leafs[i].mMaxs.Z			=br.ReadInt16();
				mQ2Leafs[i].mMaxs.Y			=br.ReadInt16();
				mQ2Leafs[i].mFirstLeafFace	=br.ReadUInt16();
				mQ2Leafs[i].mNumLeafFaces	=br.ReadUInt16();
				mQ2Leafs[i].mFirstLeafBrush	=br.ReadUInt16();
				mQ2Leafs[i].mNumLeafBrushes	=br.ReadUInt16();
#endif
			}
			return	true;
		}


		bool LoadQ2PortalFile(string portFile)
		{
			FileStream		fs	=new FileStream(portFile, FileMode.Open, FileAccess.Read);
			StreamReader	sr	=null;

			if(fs == null)		// opps
			{
				CoreEvents.Print("LoadQ2PortalFile:  Could not open " + portFile + " for reading.\n");
				goto	ExitWithError;
			}

			sr	=new StreamReader(fs);
			
			string	TAG	=sr.ReadLine();
			if(TAG != "PRT1")
			{
				CoreEvents.Print("LoadPortalFile:  " + portFile + " is not a GBSP Portal file.\n");
				goto	ExitWithError;
			}

			string	tmp				=sr.ReadLine();
			int		NumVisLeafs		=0;
			int		NumVisPortals	=0;

			if(!Int32.TryParse(tmp, out NumVisLeafs))
			{
				goto	ExitWithError;
			}

			tmp	=sr.ReadLine();
			if(!Int32.TryParse(tmp, out NumVisPortals))
			{
				goto	ExitWithError;
			}
			
			mQ2Portals	=new Q2Portal[NumVisPortals * 2];
//			mVisPortals	=new VISPortal[NumVisPortals * 2];
//			if(mVisPortals == null)
//			{
//				CoreEvents.Print("LoadPortalFile:  Out of memory for VisPortals.\n");
//				goto	ExitWithError;
//			}
			
//			mVisSortedPortals	=new VISPortal[NumVisPortals * 2];
//			if(mVisSortedPortals == null)
//			{
//				CoreEvents.Print("LoadPortalFile:  Out of memory for VisSortedPortals.\n");
//				goto ExitWithError;
//			}
	
			mQ2VisLeafs	=new Q2VisLeaf[NumVisLeafs];
//			mVisLeafs	=new VISLeaf[NumVisLeafs];
//			if(mVisLeafs == null)
//			{
//				CoreEvents.Print("LoadPortalFile:  Out of memory for VisLeafs.\n");
//				goto ExitWithError;
//			}

			//fill arrays with blank objects
			for(int i=0;i < NumVisLeafs;i++)
			{
//				mVisLeafs[i]	=new VISLeaf();
				mQ2VisLeafs[i]	=new Q2VisLeaf();
			}

			//
			//	Load in the portals
			//
			for(int i=0;i < NumVisPortals * 2;i+=2)
			{
				//alloc blank portals
//				mVisPortals[i]		=new VISPortal();
//				mVisPortals[i + 1]	=new VISPortal();
				mQ2Portals[i]		=new Q2Portal();
				mQ2Portals[i + 1]	=new Q2Portal();

				tmp	=sr.ReadLine();

				string	[]tokens	=tmp.Split(' ');

				int	numPoint	=0;
				int	leaf0		=0;
				int	leaf1		=0;

				if(!Int32.TryParse(tokens[0], out numPoint))
				{
					goto ExitWithError;
				}
				if(!Int32.TryParse(tokens[1], out leaf0))
				{
					goto ExitWithError;
				}
				if(!Int32.TryParse(tokens[2], out leaf1))
				{
					goto ExitWithError;
				}
/*
				if(leafFrom >= NumVisLeafs || leafFrom < 0)
				{
					CoreEvents.Print("LoadPortalFile:  Invalid LeafFrom: " + leafFrom + "\n");
					goto	ExitWithError;
				}

				if(leafTo >= NumVisLeafs || leafTo < 0)
				{
					CoreEvents.Print("LoadPortalFile:  Invalid LeafTo: " + leafTo + "\n");
					goto	ExitWithError;
				}*/

				List<Vector3>	points	=new List<Vector3>();
				for(int j=0;j < numPoint;j++)
				{
					float	v0, v1, v2;

					string	stripPer	=tokens[3 + (j * 4)].TrimStart('(');

					if(!Single.TryParse(stripPer, out v0))
					{
						goto ExitWithError;
					}

					if(!Single.TryParse(tokens[4 + (j * 4)], out v1))
					{
						goto ExitWithError;
					}
					if(!Single.TryParse(tokens[5 + (j * 4)], out v2))
					{
						goto ExitWithError;
					}

					//invert x and swap y and z
					points.Add(new Vector3(-v0, v2, v1));
				}

				GBSPPoly	poly	=new GBSPPoly(numPoint);

				poly.mVerts	=points.ToArray();
				

				GBSPPlane	pln	=new GBSPPlane(poly);

//				VISLeaf		fleaf	=mVisLeafs[leafFrom];
//				VISLeaf		bleaf	=mVisLeafs[leafTo];
//				VISPortal	fport	=mVisPortals[i];
//				VISPortal	bport	=mVisPortals[i + 1];

				Q2VisLeaf	fleaf	=mQ2VisLeafs[leaf0];
				Q2VisLeaf	bleaf	=mQ2VisLeafs[leaf1];
				Q2Portal	fport	=mQ2Portals[i];
				Q2Portal	bport	=mQ2Portals[i + 1];

				fport.mPortNum	=i;
				fport.mWinding	=poly;
				fport.mLeaf		=leaf1;
				fport.mPlane	=pln;
				fleaf.mPortals[fleaf.mNumPortals++]	=fport;
//				fport.CalcPortalInfo();

				bport.mPortNum	=i + 1;
				bport.mWinding	=new GBSPPoly(poly);
				bport.mLeaf		=leaf0;
				bport.mPlane	=pln;
				bleaf.mPortals[bleaf.mNumPortals++]	=bport;

				bport.mWinding.Reverse();
				bport.mPlane.Inverse();

//				bport.CalcPortalInfo();
			}
			
			mNumVisLeafBytes	=((NumVisLeafs+63)&~63) >> 3;
			mNumVisPortalBytes	=(((NumVisPortals * 2)+63)&~63) >> 3;

			sr.Close();
			fs.Close();
			sr	=null;
			fs	=null;

			return	true;

			// ==== ERROR ===
			ExitWithError:
			{
				if(sr != null)
				{
					sr.Close();
				}
				if(fs != null)
				{
					fs.Close();
				}

				mVisPortals			=null;
				mVisSortedPortals	=null;
				mVisLeafs			=null;

				return	false;
			}
		}


		bool LoadPortalFile(string portFile)
		{
			FileStream	fs	=new FileStream(portFile,
				FileMode.Open, FileAccess.Read);

			BinaryReader	br	=null;

			if(fs == null)		// opps
			{
				CoreEvents.Print("LoadPortalFile:  Could not open " + portFile + " for reading.\n");
				goto	ExitWithError;
			}

			br	=new BinaryReader(fs);
			
			// 
			//	Check the TAG
			//
			string	TAG	=br.ReadString();
			if(TAG != "GBSP_PRTFILE")
			{
				CoreEvents.Print("LoadPortalFile:  " + portFile + " is not a GBSP Portal file.\n");
				goto	ExitWithError;
			}

			//
			//	Get the number of portals
			//
			int	NumVisPortals	=br.ReadInt32();
			if(NumVisPortals >= VISPStack.MAX_TEMP_PORTALS)
			{
				CoreEvents.Print("LoadPortalFile:  Max portals for temp buffers.\n");
				goto	ExitWithError;
			}
			
			mVisPortals	=new VISPortal[NumVisPortals * 2];
			if(mVisPortals == null)
			{
				CoreEvents.Print("LoadPortalFile:  Out of memory for VisPortals.\n");
				goto	ExitWithError;
			}
			
			mVisSortedPortals	=new VISPortal[NumVisPortals * 2];
			if(mVisSortedPortals == null)
			{
				CoreEvents.Print("LoadPortalFile:  Out of memory for VisSortedPortals.\n");
				goto ExitWithError;
			}

			//
			//	Get the number of leafs
			//
			int	NumVisLeafs	=br.ReadInt32();
			if(NumVisLeafs > mGFXLeafs.Length)
			{
				goto	ExitWithError;
			}
			
			mVisLeafs	=new VISLeaf[NumVisLeafs];
			if(mVisLeafs == null)
			{
				CoreEvents.Print("LoadPortalFile:  Out of memory for VisLeafs.\n");
				goto ExitWithError;
			}

			//fill arrays with blank objects
			for(int i=0;i < NumVisLeafs;i++)
			{
				mVisLeafs[i]	=new VISLeaf();
			}

			//
			//	Load in the portals
			//
			for(int i=0;i < NumVisPortals * 2;i+=2)
			{
				//alloc blank portals
				mVisPortals[i]		=new VISPortal();
				mVisPortals[i + 1]	=new VISPortal();

				GBSPPoly	poly	=new GBSPPoly(0);
				poly.Read(br);

				int	leafFrom	=br.ReadInt32();
				int	leafTo		=br.ReadInt32();
				
				if(leafFrom >= NumVisLeafs || leafFrom < 0)
				{
					CoreEvents.Print("LoadPortalFile:  Invalid LeafFrom: " + leafFrom + "\n");
					goto	ExitWithError;
				}

				if(leafTo >= NumVisLeafs || leafTo < 0)
				{
					CoreEvents.Print("LoadPortalFile:  Invalid LeafTo: " + leafTo + "\n");
					goto	ExitWithError;
				}

				GBSPPlane	pln	=new GBSPPlane(poly);

				VISLeaf		fleaf	=mVisLeafs[leafFrom];
				VISLeaf		bleaf	=mVisLeafs[leafTo];
				VISPortal	fport	=mVisPortals[i];
				VISPortal	bport	=mVisPortals[i + 1];

				fport.mPortNum	=i;
				fport.mPoly		=poly;
				fport.mLeaf		=leafTo;
				fport.mPlane	=pln;
				fleaf.mPortals.Add(fport);
				fport.CalcPortalInfo();

				bport.mPortNum	=i + 1;
				bport.mPoly		=new GBSPPoly(poly);
				bport.mLeaf		=leafFrom;
				bport.mPlane	=pln;
				bleaf.mPortals.Add(bport);

				bport.mPoly.Reverse();
				bport.mPlane.Inverse();

				bport.CalcPortalInfo();
			}
			
			mNumVisLeafBytes	=((NumVisLeafs+63)&~63) >> 3;
			mNumVisPortalBytes	=(((NumVisPortals * 2)+63)&~63) >> 3;

			br.Close();
			fs.Close();
			br	=null;
			fs	=null;

			return	true;

			// ==== ERROR ===
			ExitWithError:
			{
				if(br != null)
				{
					br.Close();
				}
				if(fs != null)
				{
					fs.Close();
				}

				mVisPortals			=null;
				mVisSortedPortals	=null;
				mVisLeafs			=null;

				return	false;
			}
		}
	}
}
