﻿using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColladaConvert
{
	public class Accessor
	{
		private string		mSource;
		private int			mCount;
		private int			mStride;
		private List<Param>	mParams	=new List<Param>();


		public bool IsTexCoord()
		{
			return	(mParams[0].mName == "S"
				&& mParams[1].mName == "T");
		}


		public bool IsPosition()
		{
			if(mStride == 3 && mParams.Count == 3)
			{
				if(mParams[0].mName == "X"
					&& mParams[1].mName == "Y"
					&& mParams[2].mName == "Z")
				{
					return	true;
				}
			}
			return	false;
		}


		public bool IsNormal()
		{
			if(mStride == 3 && mParams.Count == 3)
			{
				if(mParams[0].mName == "X"
					&& mParams[1].mName == "Y"
					&& mParams[2].mName == "Z")
				{
					return	true;
				}
			}
			return	false;
		}


		public void Load(XmlReader r)
		{
			int	attCnt	=r.AttributeCount;
			if(attCnt > 0)
			{
				r.MoveToFirstAttribute();
				while(attCnt > 0)
				{
					if(r.Name == "source")
					{
						mSource	=r.Value;
					}
					else if(r.Name == "count")
					{
						int.TryParse(r.Value, out mCount);
					}
					else if(r.Name == "stride")
					{
						int.TryParse(r.Value, out mStride);
					}
					r.MoveToNextAttribute();
					attCnt--;
				}
			}
			while(r.Read())
			{
				if(r.NodeType == XmlNodeType.Whitespace)
				{
					continue;	//skip whitey
				}
				if(r.Name == "accessor")
				{
					return;
				}
				else if(r.Name == "param")
				{
					if(r.AttributeCount > 0)
					{
						Param	p	=new Param();
						p.Load(r);
						mParams.Add(p);
					}
				}
			}
		}
	}
}