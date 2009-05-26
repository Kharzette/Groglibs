﻿using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColladaConvert
{
	public class Controller
	{
		private Skin			mSkin;
		private List<Matrix>	mBones	=new List<Matrix>();

		private Dictionary<string, Int32>	mBIdxMap	=new Dictionary<string,int>();


		public void Load(XmlReader r)
		{
			//read controller stuff
			while(r.Read())
			{
				if(r.NodeType == XmlNodeType.Whitespace)
				{
					continue;	//skip whitey
				}

				if(r.Name == "controller")
				{
					return;
				}
				else if(r.Name == "skin")
				{
					mSkin	=new Skin();
					mSkin.Load(r);
				}
			}
		}


		public Skin	GetSkin()
		{
			return	mSkin;
		}


		public void CopyBonesTo(ref Matrix []bones)
		{
			bones	=new Matrix[mBones.Count];

			for(int i=0;i < mBones.Count;i++)
			{
				bones[i]	=mBones[i];
			}
		}

		
		public void BuildBones(GraphicsDevice g, Dictionary<string, SceneNode> nodes)
		{
			//grab the list of bones from the skin
			List<string>	jointNames	=mSkin.GetJointNameArray();

			//grab the inverse bind pose matrix list
			List<Matrix>	ibps	=mSkin.GetInverseBindPoses();

			Matrix	bind		=mSkin.GetBindShapeMatrix();

			//find each bone and place it in our arrays
			for(int i=0;i < jointNames.Count;i++)
			{
				string	jn	=jointNames[i];
				foreach(KeyValuePair<string, SceneNode> sn in nodes)
				{
					Matrix	mat;
					if(sn.Value.GetMatrixForBone(jn, out mat))
					{
//						Matrix	invinv	=Matrix.Invert(ibps[i]);
//						mBones.Add(mat * invinv);
//						mBones.Add(invinv * mat);

						mBones.Add(ibps[i] * mat);

//						mBones.Add(ibps[curMat++] * mat);
//						mBones.Add(ibps[curMat++]);
//						mBones.Add(mat * ibps[curMat++]);
//						mBones.Add(bind * invinv);
//						mBones.Add(invinv * mat);
						break;
					}
				}
			}
		}


		public void ChangeCoordinateSystemMAX(Dictionary<string, SceneNode> nodes)
		{
			//grab the list of bones from the skin
			List<string>	jointNames	=mSkin.GetJointNameArray();

			//grab the inverse bind pose matrix list
			List<Matrix>	ibps	=mSkin.GetInverseBindPoses();

			for(int i=0;i < ibps.Count;i++)
			{
				ibps[i]	=Collada.ConvertMatrixCoordinateSystemMAX(ibps[i]);
			}

			mSkin.ConvertBindShapeMatrixCoordinateSystemMAX();

			foreach(KeyValuePair<string, SceneNode> sn in nodes)
			{
				sn.Value.ConvertBoneCoordinateSystemMAX();
			}
		}
	}
}