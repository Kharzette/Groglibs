﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;


namespace BSPCore
{
	public class BSPBuildParams
	{
		public int	mMaxThreads;
		public bool	mbVerbose;
		public bool	mbEntityVerbose;
		public bool mbFixTJunctions;
		public bool	mbSlickAsGouraud;
		public bool mbWarpAsMirror;
	}


	public class LightParams
	{
		public bool		mbSeamCorrection;
		public bool		mbRadiosity;
		public bool		mbFastPatch;
		public int		mPatchSize;
		public int		mNumBounces;
		public int		mNumSamples;	//1 to 5
		public float	mLightScale;
		public Vector3	mMinLight;
		public float	mSurfaceReflect;
		public int		mMaxIntensity;
		public int		mLightGridSize;
		public int		mAtlasSize;
	}


	public class VisParams
	{
		public bool	mbFullVis;
		public bool	mbSortPortals;
		public bool mbDistribute;
		public bool mbResume;
	}
}
