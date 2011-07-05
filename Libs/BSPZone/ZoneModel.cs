﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;


namespace BSPZone
{
	internal class ZoneModel : UtilityLib.IReadWriteable
	{
		internal Int32			[]mRootNode	=new Int32[2];	// Top level Node in GFXNodes/GFXBNodes
		internal BoundingBox	mBounds;
		internal Vector3		mOrigin;					// Center of model
		internal Int32			mFirstFace;					// First face in GFXFaces
		internal Int32			mNumFaces;					// Number of faces
		internal Int32			mFirstLeaf;					// First leaf in GFXLeafs;
		internal Int32			mNumLeafs;					// Number of leafs (not including solid leaf)
		internal Int32			mFirstCluster;
		internal Int32			mNumClusters;
		internal Int32			[]mAreas	=new Int32[2];	// Area on each side of the model


		public void Write(BinaryWriter bw)
		{
			bw.Write(mRootNode[0]);
			bw.Write(mRootNode[1]);
			bw.Write(mBounds.Min.X);
			bw.Write(mBounds.Min.Y);
			bw.Write(mBounds.Min.Z);
			bw.Write(mBounds.Max.X);
			bw.Write(mBounds.Max.Y);
			bw.Write(mBounds.Max.Z);
			bw.Write(mOrigin.X);
			bw.Write(mOrigin.Y);
			bw.Write(mOrigin.Z);
			bw.Write(mFirstFace);
			bw.Write(mNumFaces);
			bw.Write(mFirstLeaf);
			bw.Write(mNumLeafs);
			bw.Write(mFirstCluster);
			bw.Write(mNumClusters);
			bw.Write(mAreas[0]);
			bw.Write(mAreas[1]);
		}

		public void Read(BinaryReader br)
		{
			mRootNode[0]	=br.ReadInt32();
			mRootNode[1]	=br.ReadInt32();
			mBounds.Min.X	=br.ReadSingle();
			mBounds.Min.Y	=br.ReadSingle();
			mBounds.Min.Z	=br.ReadSingle();
			mBounds.Max.X	=br.ReadSingle();
			mBounds.Max.Y	=br.ReadSingle();
			mBounds.Max.Z	=br.ReadSingle();
			mOrigin.X		=br.ReadSingle();
			mOrigin.Y		=br.ReadSingle();
			mOrigin.Z		=br.ReadSingle();
			mFirstFace		=br.ReadInt32();
			mNumFaces		=br.ReadInt32();
			mFirstLeaf		=br.ReadInt32();
			mNumLeafs		=br.ReadInt32();
			mFirstCluster	=br.ReadInt32();
			mNumClusters	=br.ReadInt32();
			mAreas[0]		=br.ReadInt32();
			mAreas[1]		=br.ReadInt32();
		}
	}
}