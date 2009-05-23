﻿using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColladaConvert
{
	public class PolyList
	{
		private string		mMaterial;
		private int			mCount;

		private List<Input>		mInputs		=new List<Input>();
		private List<List<int>>	mIndexs		=new List<List<int>>();
		private	List<int>		mVertCounts	=new List<int>();


		public List<int> GetVertCounts()
		{
			return	mVertCounts;
		}


		public List<int> GetPositionIndexs()
		{
			//find which index is pos
			for(int i=0;i < mInputs.Count;i++)
			{
				if(mInputs[i].IsVertex())
				{
					return	mIndexs[i];
				}
			}
			return	null;
		}


		public List<int> GetNormalIndexs()
		{
			//find which index is norm
			for(int i=0;i < mInputs.Count;i++)
			{
				if(mInputs[i].IsNormal())
				{
					return	mIndexs[i];
				}
			}
			return	null;
		}


		public List<int> GetTexCoordIndexs()
		{
			//find which index is tex
			for(int i=0;i < mInputs.Count;i++)
			{
				if(mInputs[i].IsTexCoord())
				{
					return	mIndexs[i];
				}
			}
			return	null;
		}


		public void Load(XmlReader r)
		{
			int	attCnt	=r.AttributeCount;
			if(attCnt > 0)
			{
				r.MoveToFirstAttribute();
				while(attCnt > 0)
				{
					if(r.Name == "material")
					{
						mMaterial	=r.Value;
					}
					else if(r.Name == "count")
					{
						int.TryParse(r.Value, out mCount);
					}
					r.MoveToNextAttribute();
					attCnt--;
				}

				while(r.Read())
				{
					if(r.NodeType == XmlNodeType.Whitespace)
					{
						continue;	//skip whitey
					}
					if(r.Name == "input")
					{
						if(r.NodeType == XmlNodeType.EndElement)
						{
							continue;
						}
						Input	inp	=new Input();
						inp.Load(r);
						mInputs.Add(inp);
					}
					else if(r.Name == "vcount")
					{
						if(r.NodeType == XmlNodeType.EndElement)
						{
							continue;
						}
						//go to values
						r.Read();

						string	[]tokens	=r.Value.Split(' ', '\n');
						foreach(string tok in tokens)
						{
							int	i;

							if(int.TryParse(tok, out i))
							{
								mVertCounts.Add(i);
							}
						}
					}
					else if(r.Name == "p")
					{
						if(r.NodeType == XmlNodeType.EndElement)
						{
							continue;
						}
						//figure out how many semantics we have
						int	numSem	=mInputs.Count;
						int	curList	=0;

						//there will be an index for every semantic
						for(int i=0;i < numSem;i++)
						{
							mIndexs.Add(new List<int>());
						}

						//go to values
						r.Read();

						string	[]tokens	=r.Value.Split(' ', '\n');
						foreach(string tok in tokens)
						{
							int	i;

							if(int.TryParse(tok, out i))
							{
								mIndexs[curList].Add(i);
								curList++;
								//hit each semantic list once
								if(curList >= numSem)
								{
									curList	=0;
								}
							}
						}
					}
					else if(r.Name == "polylist")
					{
						return;
					}
				}
			}
		}
	}
}