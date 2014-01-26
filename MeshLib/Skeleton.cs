﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MeshLib
{
	public class GSNode
	{
		internal string	mName;
		List<GSNode>	mChildren	=new List<GSNode>();

		//current pos / rot / scale
		KeyFrame	mKeyValue	=new KeyFrame();


		public GSNode()
		{
		}


		public void AddChild(GSNode kid)
		{
			mChildren.Add(kid);
		}


		public void SetName(string name)
		{
			mName	=UtilityLib.Misc.AssignValue(name);
		}


		public Matrix GetMatrix()
		{
			Matrix	mat	=Matrix.CreateScale(mKeyValue.mScale) *
				Matrix.CreateFromQuaternion(mKeyValue.mRotation) *
				Matrix.CreateTranslation(mKeyValue.mPosition);

			return	mat;
		}


		public bool GetMatrixForBone(string boneName, out Matrix ret)
		{
			if(boneName == mName)
			{
				ret	=GetMatrix();
				return	true;
			}

			foreach(GSNode n in mChildren)
			{
				if(n.GetMatrixForBone(boneName, out ret))
				{
					ret	*=GetMatrix();
					return	true;
				}
			}
			ret	=Matrix.Identity;
			return	false;
		}


		internal void NukeBone(string boneName)
		{
			foreach(GSNode n in mChildren)
			{
				if(n.mName == boneName)
				{
					mChildren.Remove(n);
					return;
				}
				n.NukeBone(boneName);
			}
		}


		public void Read(BinaryReader br)
		{
			mName	=br.ReadString();

			mKeyValue.Read(br);

			int	numChildren	=br.ReadInt32();
			for(int i=0;i < numChildren;i++)
			{
				GSNode	n	=new GSNode();
				n.Read(br);

				mChildren.Add(n);
			}
		}


		public void Write(BinaryWriter bw)
		{
			bw.Write(mName);

			mKeyValue.Write(bw);

			bw.Write(mChildren.Count);
			foreach(GSNode n in mChildren)
			{
				n.Write(bw);
			}
		}


		internal void GetBoneNames(List<string> names)
		{
			foreach(GSNode n in mChildren)
			{
				n.GetBoneNames(names);
			}
			names.Add(mName);
		}


		internal bool GetBoneParentName(string boneName, out string parent)
		{
			if(boneName == mName)
			{
				parent	="";
				return	true;
			}

			foreach(GSNode n in mChildren)
			{
				if(n.GetBoneParentName(boneName, out parent))
				{
					if(parent == "")
					{
						parent	=mName;
					}
					return	true;
				}
			}
			parent	=null;
			return	false;
		}


		internal bool GetBoneKey(string bone, out KeyFrame ret)
		{
			if(mName == bone)
			{
				ret	=mKeyValue;
				return	true;
			}

			foreach(GSNode n in mChildren)
			{
				if(n.GetBoneKey(bone, out ret))
				{
					return	true;
				}
			}
			ret	=null;
			return	false;
		}


		public void SetKey(KeyFrame keyFrame)
		{
			mKeyValue.mPosition	=keyFrame.mPosition;
			mKeyValue.mRotation	=keyFrame.mRotation;
			mKeyValue.mScale	=keyFrame.mScale;
		}


		internal void IterateStructure(Skeleton.IterateStruct ist)
		{
			foreach(GSNode gsn in mChildren)
			{
				ist(gsn.mName, mName);
			}

			foreach(GSNode gsn in mChildren)
			{
				gsn.IterateStructure(ist);
			}
		}
	}


	public class Skeleton
	{
		List<GSNode>	mRoots	=new List<GSNode>();

		public delegate void IterateStruct(string name, string parent);


		public Skeleton()
		{
		}


		public void AddRoot(GSNode gsn)
		{
			mRoots.Add(gsn);
		}


		public void GetBoneNames(List<string> names)
		{
			foreach(GSNode n in mRoots)
			{
				n.GetBoneNames(names);
			}
		}


		public bool GetMatrixForBone(string boneName, out Matrix ret)
		{
			foreach(GSNode n in mRoots)
			{
				if(n.GetMatrixForBone(boneName, out ret))
				{
					return	true;
				}
			}
			ret	=Matrix.Identity;
			return	false;
		}


		public void NukeBone(string name)
		{
			foreach(GSNode n in mRoots)
			{
				if(n.mName == name)
				{
					mRoots.Remove(n);
					return;
				}
				n.NukeBone(name);
			}
		}


		public void Read(BinaryReader br)
		{
			int	numRoots	=br.ReadInt32();

			for(int i=0;i < numRoots;i++)
			{
				GSNode	n	=new GSNode();

				n.Read(br);

				mRoots.Add(n);
			}
		}


		public void Write(BinaryWriter bw)
		{
			bw.Write(mRoots.Count);

			foreach(GSNode n in mRoots)
			{
				n.Write(bw);
			}
		}


		internal bool GetBoneParentName(string boneName, out string parent)
		{
			foreach(GSNode n in mRoots)
			{
				if(n.GetBoneParentName(boneName, out parent))
				{
					return	true;
				}
			}
			parent	=null;
			return	false;
		}


		public KeyFrame GetBoneKey(string bone)
		{
			KeyFrame	ret	=null;
			foreach(GSNode n in mRoots)
			{
				if(n.GetBoneKey(bone, out ret))
				{
					return	ret;
				}
			}
			return	ret;
		}


		public void IterateStructure(IterateStruct ist)
		{
			//do the roots
			foreach(GSNode gsn in mRoots)
			{
				ist(gsn.mName, null);
			}

			//recurse
			foreach(GSNode gsn in mRoots)
			{
				gsn.IterateStructure(ist);
			}
		}
	}
}