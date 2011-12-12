﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;


namespace BSPZone
{
	internal class ZoneNode : UtilityLib.IReadWriteable
	{
		internal Int32		mFront, mBack;
		internal Int32		mNumFaces;
		internal Int32		mFirstFace;
		internal Int32		mPlaneNum;
		internal Vector3	mMins, mMaxs;


		public void Write(BinaryWriter bw)
		{
			bw.Write(mFront);
			bw.Write(mBack);
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
			mFront			=br.ReadInt32();
			mBack			=br.ReadInt32();
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
