﻿using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColladaConvert
{
	public class Animator
	{
		//I think this class will need to store all the
		//animation channels per anim, and somehow we need
		//to figure out a way to distinguish different animations.
		//Also need to store the skeleton I think
		//
		//The basic idea behind this right now is to give me the
		//skeleton at time t
		private	Dictionary<string, List<Anim>>	mAnims	=new Dictionary<string, List<Anim>>();


		public Animator(Dictionary<string, Animation> anims, Dictionary<string, SceneNode> roots)
		{
			foreach(KeyValuePair<string, Animation> an in anims)
			{
				List<Anim>	alist	=an.Value.GetAnims(roots);

				mAnims.Add(an.Key, alist);
			}
		}


		public void AnimateAll(float time)
		{
			foreach(KeyValuePair<string, List<Anim>> anlist in mAnims)
			{
				foreach(Anim an in anlist.Value)
				{
					an.Animate(time);
				}
			}
		}


		public void Animate(string name, float time)
		{
			if(mAnims.ContainsKey(name))
			{
				foreach(Anim an in mAnims[name])
				{
					an.Animate(time);
				}
			}
		}


		public List<MeshLib.FloatKeys> BuildGameAnims(MeshLib.Skeleton gs)
		{
			List<MeshLib.FloatKeys>	ret	=new List<MeshLib.FloatKeys>();
			foreach(KeyValuePair<string, List<Anim>> anlist in mAnims)
			{
				//for each anlist, get a full list of times
				//create a set of KeyFrame values and times
				//construct a SubAnim from these

				//get times
				List<float>	times	=new List<float>();
				foreach(Anim an in anlist.Value)
				{
					List<float> anTimes	=an.GetTimes();

					foreach(float time in anTimes)
					{
						if(times.Contains(time))
						{
							continue;
						}
						times.Add(time);
					}
				}

				times.Sort();

				List<MeshLib.KeyFrame>	keys	=new List<MeshLib.KeyFrame>();
				foreach(float time in times)
				{
					foreach(Anim an in anlist.Value)
					{
						an.Animate(time);
					}
				}
				/*
					MeshLib.Channel	gc;

					MeshLib.ChannelTarget	gct;
					
					if(!gs.GetChannelTarget(an.GetNodeName(), an.GetOperandSID(), out gct))
					{
						Debug.WriteLine("GetChannelTarget failed in BuildGameAnims!");
					}

					gc	=new MeshLib.Channel(an.GetNodeName(),
						an.GetOperandSID(), an.GetChannelTarget(), gs);

					MeshLib.FloatKeys	gsa	=new MeshLib.FloatKeys(an.GetNumKeys(),
						an.GetTotalTime(), gc,
						an.GetTimes(), an.GetValues(),
						an.GetControl1(), an.GetControl2());

					ret.Add(gsa);
				}*/
			}
			return	ret;
		}
	}
}