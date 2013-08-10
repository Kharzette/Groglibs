﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UtilityLib;


namespace MeshLib
{
	//this is now a master skin, one per character
	//all mesh parts will index in the same way
	//The tool will need to make sure the inverse bind poses
	//are all the same for each bone
	public class Skin
	{
		List<string>	mBoneNames			=new List<string>();
		List<Matrix>	mInverseBindPoses	=new List<Matrix>();
		Matrix			mMaxAdjust;	//coordinate system stuff


		public Skin()
		{
			mMaxAdjust	=Matrix.CreateFromYawPitchRoll(0,
										MathHelper.ToRadians(-90),
										MathHelper.ToRadians(180));
		}


		internal List<string> GetBoneNames()
		{
			return	mBoneNames;
		}


		public void SetBoneNamesAndPoses(Dictionary<string, Matrix> invBindPoses)
		{
			mBoneNames.Clear();
			mInverseBindPoses.Clear();

			foreach(KeyValuePair<string, Matrix> bp in invBindPoses)
			{
				if(mBoneNames.Contains(bp.Key))
				{
					continue;
				}
				mBoneNames.Add(bp.Key);
				mInverseBindPoses.Add(bp.Value);
			}
		}


		public int GetNumBones()
		{
			return	mBoneNames.Count;
		}


		public Matrix GetBoneByIndex(int idx, Skeleton sk)
		{
			Matrix	ret	=Matrix.Identity;

			sk.GetMatrixForBone(mBoneNames[idx], out ret);

			//multiply by inverse bind pose
			ret	=mInverseBindPoses[idx] * ret * mMaxAdjust;

			return	ret;
		}


		public void Read(BinaryReader br)
		{
			mBoneNames.Clear();
			mInverseBindPoses.Clear();

			int	numNames	=br.ReadInt32();
			for(int i=0;i < numNames;i++)
			{
				string	name	=br.ReadString();

				mBoneNames.Add(name);
			}

			int	numInvs	=br.ReadInt32();
			for(int i=0;i < numInvs;i++)
			{
				Matrix	mat	=FileUtil.ReadMatrix(br);
				mInverseBindPoses.Add(mat);
			}
		}


		public void Write(BinaryWriter bw)
		{
			bw.Write(mBoneNames.Count);
			foreach(string name in mBoneNames)
			{
				bw.Write(name);
			}

			bw.Write(mInverseBindPoses.Count);
			foreach(Matrix m in mInverseBindPoses)
			{
				FileUtil.WriteMatrix(bw, m);
			}
		}
	}
}