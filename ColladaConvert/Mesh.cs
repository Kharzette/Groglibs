﻿using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColladaConvert
{
	public class Mesh
	{
		private Dictionary<string, Source>		mSources	=new Dictionary<string,Source>();
		private Dictionary<string, Vertices>	mVerts		=new Dictionary<string,Vertices>();
		private Polygons						mPolys;


		public Mesh()	{}


		public List<float> GetBaseVerts()
		{
			//find key
			string	key	=mPolys.GetPositionSourceKey();

			//strip #
			key	=key.Substring(1);

			//use key to look up in mVerts
			Vertices v	=mVerts[key];

			key	=v.GetPositionKey();

			//strip #
			key	=key.Substring(1);

			return	mSources[key].GetFloatArray();
		}


		public List<float> GetNormals()
		{
			//find key
			string	key	=mPolys.GetNormalSourceKey();

			//strip #
			key	=key.Substring(1);

			return	mSources[key].GetFloatArray();
		}


		public List<float> GetTexCoords(int set)
		{
			//find texcoord key
			string	key	=mPolys.GetTexCoordSourceKey(set);

			if(key == "")
			{
				return	null;
			}

			//strip #
			key	=key.Substring(1);

			return	mSources[key].GetFloatArray();
		}


		public List<int> GetPositionIndexs()
		{
			return	mPolys.GetPositionIndexs();
		}


		public List<int> GetNormalIndexs()
		{
			return	mPolys.GetNormalIndexs();
		}


		public List<int> GetTexCoordIndexs(int set)
		{
			return	mPolys.GetTexCoordIndexs(set);
		}


		public List<int> GetVertCounts()
		{
			return	mPolys.GetVertCounts();
		}


		public void Load(XmlReader r)
		{
			while(r.Read())
			{
				if(r.NodeType == XmlNodeType.Whitespace)
				{
					continue;	//skip whitey
				}

				if(r.Name == "source")
				{
					r.MoveToFirstAttribute();

					string	srcID	=r.Value;

					Source	src	=new Source();
					src.Load(r);

					mSources.Add(srcID, src);
				}
				else if(r.Name == "mesh")
				{
					return;
				}
				else if(r.Name == "vertices")
				{
					r.MoveToFirstAttribute();
					string	vertID	=r.Value;

					Vertices	vert	=new Vertices();
					vert.Load(r);
					mVerts.Add(vertID, vert);					
				}
				else if(r.Name == "polygons")
				{
					mPolys	=new Polygons();
					mPolys.Load(r);
				}
				else if(r.Name == "polylist")
				{
					mPolys	=new Polygons();
					mPolys.LoadList(r);
				}
			}
		}
	}
}