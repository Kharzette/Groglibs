﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace UtilityLib
{
	public static class Misc
	{
		public static void SafeInvoke(this EventHandler eh, object sender)
		{
			if(eh != null)
			{
				eh(sender, EventArgs.Empty);
			}
		}


		public static void SafeInvoke(this EventHandler eh, object sender, EventArgs ea)
		{
			if(eh != null)
			{
				eh(sender, ea);
			}
		}


		public static void SafeInvole<T>(this EventHandler<T> eh, object sender, T ea) where T : EventArgs
		{
			if(eh != null)
			{
				eh(sender, ea);
			}
		}


		public static Vector4 ARGBToVector4(int argb)
		{
			Vector4	ret	=Vector4.Zero;

			ret.X	=((float)((argb & 0x00ff0000) >> 16) / 255f);
			ret.Y	=((float)((argb & 0x0000ff00) >> 8) / 255f);
			ret.Z	=((float)(argb & 0x000000ff) / 255f);
			ret.W	=((float)((argb & 0xff000000) >> 24) / 255f);

			return	ret;
		}


		public static int Vector4ToARGB(Vector4 vecColor)
		{
			int	argb	=(int)(vecColor.W * 255f) << 24;
			argb		|=(int)(vecColor.X * 255f) << 16;
			argb		|=(int)(vecColor.Y * 255f) << 8;
			argb		|=(int)(vecColor.Z * 255f);

			return	argb;
		}


		public static Vector3 ColorNormalize(Vector3 inVec)
		{
			float	mag	=-696969;

			if(inVec.X > mag)
			{
				mag	=inVec.X;
			}
			if(inVec.Y > mag)
			{
				mag	=inVec.Y;
			}
			if(inVec.Z > mag)
			{
				mag	=inVec.Z;
			}
			return	inVec / mag;
		}


		static string AddCountStuffToString(int num, string stuff)
		{
			string	ret	="";

			for(int i=0;i < num;i++)
			{
				ret	+=stuff;
			}
			return	ret;
		}


		public static string FloatToString(float f, int numDecimalPlaces)
		{
			//this I think prevents scientific notation on small numbers
			decimal	d	=Convert.ToDecimal(f);

			return	string.Format("{0:0." + AddCountStuffToString(numDecimalPlaces, "#") + "}",
				d.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}


		public static string VectorToString(Vector3 vec)
		{
			return	vec.X.ToString(System.Globalization.CultureInfo.InvariantCulture)
				+ " " + vec.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)
				+ " " + vec.Z.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}


		public static string VectorToString(Vector2 vec)
		{
			return	vec.X.ToString(System.Globalization.CultureInfo.InvariantCulture)
				+ " " + vec.Y.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}


		public static string VectorToString(Vector4 vec)
		{
			return	vec.X.ToString(System.Globalization.CultureInfo.InvariantCulture)
				+ " " + vec.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)
				+ " " + vec.Z.ToString(System.Globalization.CultureInfo.InvariantCulture)
				+ " " + vec.W.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}


		public static string AssignValue(string val)
		{
			if(val == null)
			{
				return	"";
			}
			return	val;
		}


#if !X64
		public static Color ModulateColour(Color a, Color b)
		{
			int	A	=a.A * b.A;
			int	R	=a.R * b.R;
			int	G	=a.G * b.G;
			int	B	=a.B * b.B;

			Color	ret	=Color.White;

			ret.A	=(byte)(A >> 8);
			ret.R	=(byte)(R >> 8);
			ret.G	=(byte)(G >> 8);
			ret.B	=(byte)(B >> 8);

			return	ret;
		}
#endif


		//returns a centered box
		public static BoundingBox MakeBox(float width, float height)
		{
			BoundingBox	ret;

			float	halfWidth	=width * 0.5f;
			float	halfHeight	=height * 0.5f;

			ret.Min.X	=-halfWidth;
			ret.Max.X	=halfWidth;
			
			ret.Min.Y	=-halfHeight;
			ret.Max.Y	=halfHeight;

			ret.Min.Z	=-halfWidth;
			ret.Max.Z	=halfWidth;

			return	ret;
		}


		public static bool bFlagSet(UInt32 val, UInt32 flag)
		{
			return	((val & flag) != 0);
		}


		public static bool bFlagSet(Int32 val, Int32 flag)
		{
			return	((val & flag) != 0);
		}


		public static void ClearFlag(ref Int32 val, Int32 flag)
		{
			val	&=(~flag);
		}


		public static void ClearFlag(ref UInt32 val, UInt32 flag)
		{
			val	&=(~flag);
		}
	}
}
