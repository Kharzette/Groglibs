﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;


namespace BSPLib
{
	public struct VPosTex0Tex1Norm0
	{
		public Vector3	Position;
		public Vector2	TexCoord0;
		public Vector2	TexCoord1;
		public Vector3	Normal0;
	}


	public struct VPosTex0Tex1Norm0Col0
	{
		public Vector3	Position;
		public Vector2	TexCoord0;
		public Vector2	TexCoord1;
		public Vector3	Normal0;
		public Vector4	Color0;
	}


	public struct VPosTex0
	{
		public Vector3	Position;
		public Vector2	TexCoord0;
	}


	public struct VPosTex0Norm0Col0
	{
		public Vector3	Position;
		public Vector2	TexCoord0;
		public Vector3	Normal;
		public Vector4	Color0;
	}


	public struct VPosTex0Col0
	{
		public Vector3	Position;
		public Vector2	TexCoord0;
		public Vector4	Color0;
	}


	public struct VPosNorm0Tex0Tex1Tex2Tex3Tex4Style4
	{
		public Vector3	Position;
		public Vector3	Normal;
		public Vector2	TexCoord0;
		public Vector2	TexCoord1;
		public Vector2	TexCoord2;
		public Vector2	TexCoord3;
		public Vector2	TexCoord4;
		public Vector4	StyleIndex;
	}
}