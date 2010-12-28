﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;


namespace BSPLib
{
	public class GFXNode : IReadWriteable
	{
		public Int32	[]mChildren	=new Int32[2];
		public Int32	mNumFaces;
		public Int32	mFirstFace;
		public Int32	mPlaneNum;
		public Vector3	mMins, mMaxs;


		public void Write(BinaryWriter bw)
		{
			bw.Write(mChildren[0]);
			bw.Write(mChildren[1]);
			bw.Write(mNumFaces);
			bw.Write(mFirstFace);
			bw.Write(mPlaneNum);
			bw.Write(mMins.X);
			bw.Write(mMins.Y);
			bw.Write(mMins.Z);
			bw.Write(mMaxs.X);
			bw.Write(mMaxs.Y);
			bw.Write(mMaxs.Z);
		}

		public void Read(BinaryReader br)
		{
			mChildren[0]	=br.ReadInt32();
			mChildren[1]	=br.ReadInt32();
			mNumFaces		=br.ReadInt32();
			mFirstFace		=br.ReadInt32();
			mPlaneNum		=br.ReadInt32();
			mMins.X			=br.ReadSingle();
			mMins.Y			=br.ReadSingle();
			mMins.Z			=br.ReadSingle();
			mMaxs.X			=br.ReadSingle();
			mMaxs.Y			=br.ReadSingle();
			mMaxs.Z			=br.ReadSingle();
		}
	}
}
