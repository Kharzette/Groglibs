﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace UtilityLib
{
	public class Mathery
	{
		public const float		NORMAL_EPSILON	=0.00001f;
		public const float		DIST_EPSILON	=0.01f;
		public const float		ANGLE_EPSILON	=0.00001f;
		public const float		VCompareEpsilon	=0.001f;
		public static Vector3	[]AxialNormals	=new Vector3[6];


		static Mathery()
		{
			AxialNormals[0]	=Vector3.UnitX;
			AxialNormals[1]	=Vector3.UnitY;
			AxialNormals[2]	=Vector3.UnitZ;
			AxialNormals[3]	=-Vector3.UnitX;
			AxialNormals[4]	=-Vector3.UnitY;
			AxialNormals[5]	=-Vector3.UnitZ;
		}


		public static Color RandomColor(Random rnd)
		{
			Microsoft.Xna.Framework.Graphics.Color	randColor
				=new Microsoft.Xna.Framework.Graphics.Color(
						Convert.ToByte(rnd.Next(255)),
						Convert.ToByte(rnd.Next(255)),
						Convert.ToByte(rnd.Next(255)));
			return	randColor;
		}


		public static bool IsAxial(Vector3 v)
		{
			foreach(Vector3 ax in AxialNormals)
			{
				if(CompareVectorEpsilon(ax, v, 0.001f))
				{
					return	true;
				}
			}
			return	false;
		}


		public static float VecIdx(Vector3 v, int idx)
		{
			if(idx == 0)
			{
				return	v.X;
			}
			else if(idx == 1)
			{
				return	v.Y;
			}
			return	v.Z;
		}


		public static float VecIdx(Vector3 v, UInt32 idx)
		{
			if(idx == 0)
			{
				return	v.X;
			}
			else if(idx == 1)
			{
				return	v.Y;
			}
			return	v.Z;
		}


		public static bool CompareVectorEpsilon(Vector3 v1, Vector3 v2, float epsilon)
		{
			if((v1.X - v2.X) < -epsilon || (v1.X - v2.X) > epsilon)
			{
				return	false;
			}
			if((v1.Y - v2.Y) < -epsilon || (v1.Y - v2.Y) > epsilon)
			{
				return	false;
			}
			if((v1.Z - v2.Z) < -epsilon || (v1.Z - v2.Z) > epsilon)
			{
				return	false;
			}
			return	true;
		}


		public static bool CompareVectorABS(Vector3 v1, Vector3 v2, float epsilon)
		{
			v1.X	=Math.Abs(v1.X);
			v1.Y	=Math.Abs(v1.Y);
			v1.Z	=Math.Abs(v1.Z);
			v2.X	=Math.Abs(v2.X);
			v2.Y	=Math.Abs(v2.Y);
			v2.Z	=Math.Abs(v2.Z);

			return	CompareVectorEpsilon(v1, v2, epsilon);
		}


		public static void VecIdxAssign(ref Vector3 v, int idx, float val)
		{
			if(idx == 0)
			{
				v.X	=val;
			}
			else if(idx == 1)
			{
				v.Y	=val;
			}
			else
			{
				v.Z	=val;
			}
		}


		public static void SnapVector(ref Vector3 vec)
		{
			for(int i=0;i < 3;i++)
			{
				float	vecElement	=VecIdx(vec, i);
				vecElement	=Math.Abs(vecElement - 1.0f);
				if(vecElement < ANGLE_EPSILON)
				{
					vec	=Vector3.Zero;
					VecIdxAssign(ref vec, i, 1.0f);
					break;
				}

				vecElement	=VecIdx(vec, i);
				vecElement	=Math.Abs(vecElement - -1.0f);
				if(vecElement < ANGLE_EPSILON)
				{
					vec	=Vector3.Zero;
					VecIdxAssign(ref vec, i, -1.0f);
					break;
				}
			}
		}
	}
}
