﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;


namespace BSPZone
{
	internal class ZoneModel : UtilityLib.IReadWriteable
	{
		internal Int32			mRootNode;		// Top level Node in GFXNodes/GFXBNodes
		internal BoundingBox	mBounds;
		internal Vector3		mOrigin;		// Center of model
		internal Int32			mFirstFace;		// First face in GFXFaces
		internal Int32			mNumFaces;		// Number of faces
		internal Int32			mFirstLeaf;		// First leaf in GFXLeafs;
		internal Int32			mNumLeafs;		// Number of leafs (not including solid leaf)
		internal Int32			mFirstCluster;
		internal Int32			mNumClusters;
		internal Int32			mAreaFront, mAreaBack;	// Area on each side of the model

		//transform data, game changes these
		internal float		mPitch, mYaw, mRoll;
		internal Vector3	mPosition;

		//these are updated whenever the above changes
		internal Matrix	mTransform, mInvertedTransform;


		public void Write(BinaryWriter bw)
		{
			bw.Write(mRootNode);
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
			bw.Write(mAreaFront);
			bw.Write(mAreaBack);
		}

		public void Read(BinaryReader br)
		{
			mRootNode		=br.ReadInt32();
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
			mAreaFront		=br.ReadInt32();
			mAreaBack		=br.ReadInt32();

			mPosition	=mOrigin;
			UpdateTransforms();
		}


		internal void SetPosition(Vector3 newPos)
		{
			mPosition	=newPos;

			UpdateTransforms();
		}


		internal void RotateX(float deltaDegrees)
		{
			mPitch	+=deltaDegrees;

			UtilityLib.Mathery.WrapAngleDegrees(ref mPitch);

			UpdateTransforms();
		}


		internal void RotateY(float deltaDegrees)
		{
			mYaw	+=deltaDegrees;

			UtilityLib.Mathery.WrapAngleDegrees(ref mYaw);

			UpdateTransforms();
		}


		internal void RotateZ(float deltaDegrees)
		{
			mRoll	+=deltaDegrees;

			UtilityLib.Mathery.WrapAngleDegrees(ref mRoll);

			UpdateTransforms();
		}


		internal void UpdateTransforms()
		{
			mTransform	=Matrix.CreateRotationZ(MathHelper.ToRadians(mRoll)) *
				Matrix.CreateRotationX(MathHelper.ToRadians(mPitch)) *
				Matrix.CreateRotationY(MathHelper.ToRadians(mYaw)) *
				Matrix.CreateTranslation(mPosition);

			mInvertedTransform	=Matrix.Invert(mTransform);
		}
	}
}