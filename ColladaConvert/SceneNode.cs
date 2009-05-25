﻿using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColladaConvert
{
	public class SceneNode
	{
		private	string			mName, mSID, mType;
		private	Matrix			mMat;

		private	Dictionary<string, SceneNode>	mChildren	=new Dictionary<string, SceneNode>();

		//skin instance stuff
		private string	mInstanceControllerURL;
		private string	mSkeleton;
		private string	mInstanceGeometryURL;

		private	List<InstanceMaterial>	mBindMaterials;


		public SceneNode()
		{
			mBindMaterials	=new List<InstanceMaterial>();
			mMat			=Matrix.Identity;
		}


		public bool GetMatrixForBone(string boneName, out Matrix outMat)
		{
			if(mName == boneName)
			{
				outMat	=GetMatrix();
				return	true;
			}
			foreach(KeyValuePair<string, SceneNode> sn in mChildren)
			{
				if(sn.Value.GetMatrixForBone(boneName, out outMat))
				{
					//mul by parent
					outMat	*=GetMatrix();
					return	true;
				}
			}
			outMat	=Matrix.Identity;
			return	false;
		}


		public string GetInstanceControllerURL()
		{
			return	mInstanceControllerURL;
		}


		public Matrix GetMatrix()
		{
			return	mMat;
		}


		public void LoadNode(XmlReader r)
		{
			r.MoveToFirstAttribute();

			int attcnt	=r.AttributeCount;

			while(attcnt > 0)
			{
				//look for valid attributes for nodes
				if(r.Name == "name")
				{
					mName	=r.Value;
				}
				else if(r.Name == "sid")
				{
					mSID	=r.Value;
				}
				else if(r.Name == "type")
				{
					mType	=r.Value;
				}

				attcnt--;
				r.MoveToNextAttribute();
			}

			InstanceMaterial	m		=null;
			bool				bEmpty	=false;
			
			while(r.Read())
			{
				if(r.NodeType == XmlNodeType.Whitespace)
				{
					continue;
				}

				if(r.Name == "translate")
				{
					int attcnt2	=r.AttributeCount;

					if(attcnt2 > 0)
					{
						//skip to the next element, the actual value
						r.Read();

						Vector3	trans;

						Collada.GetVectorFromString(r.Value, out trans);

						mMat	*=Matrix.CreateTranslation(trans);
					}
				}
				else if(r.Name == "instance_geometry")
				{
					if(r.AttributeCount > 0)
					{
						r.MoveToFirstAttribute();
						mInstanceGeometryURL	=r.Value;
					}
				}
				else if(r.Name == "scale")
				{
					int attcnt2	=r.AttributeCount;

					if(attcnt2 > 0)
					{
						//skip to the next element, the actual value
						r.Read();

						Vector3	scale;

						Collada.GetVectorFromString(r.Value, out scale);

						mMat	*=Matrix.CreateScale(scale);
					}
				}
				else if(r.Name == "instance_material")
				{
					if(r.AttributeCount > 0)
					{
						bEmpty	=r.IsEmptyElement;

						r.MoveToFirstAttribute();

						m	=new InstanceMaterial();

						if(r.Name == "symbol")
						{
							m.mSymbol	=r.Value;
						}
						else if(r.Name == "target")
						{
							m.mTarget	=r.Value;
						}

						r.MoveToNextAttribute();

						if(r.Name == "symbol")
						{
							m.mSymbol	=r.Value;
						}
						else if(r.Name == "target")
						{
							m.mTarget	=r.Value;
						}
						if(bEmpty)
						{
							mBindMaterials.Add(m);
						}
					}
					else
					{
						if(!bEmpty)
						{
							mBindMaterials.Add(m);
						}
					}
				}
				else if(r.Name == "bind")
				{
					r.MoveToFirstAttribute();

					if(r.Name == "semantic")
					{
						m.mBindSemantic	=r.Value;
					}
					else if(r.Name == "target")
					{
						m.mBindTarget	=r.Value;
					}

					r.MoveToNextAttribute();

					if(r.Name == "semantic")
					{
						m.mBindSemantic	=r.Value;
					}
					else if(r.Name == "target")
					{
						m.mBindTarget	=r.Value;
					}
				}
				else if(r.Name == "instance_controller")
				{
					if(r.AttributeCount > 0)
					{
						r.MoveToFirstAttribute();

						mInstanceControllerURL	=r.Value;
					}
				}
				else if(r.Name == "skeleton")
				{
					r.Read();
					mSkeleton	=r.Value;
				}
				else if(r.Name == "rotate")
				{
					int attcnt2	=r.AttributeCount;

					if(attcnt2 > 0)
					{
						r.MoveToFirstAttribute();

						//skip to the next element, the actual value
						r.Read();

						//these are a 3 element axis + a rotation in degrees
						Vector4	axisRot;
						Collada.GetVectorFromString(r.Value, out axisRot);

						//bust out the axis into a vec3
						Vector3	axis;
						axis.X	=axisRot.X;
						axis.Y	=axisRot.Y;
						axis.Z	=axisRot.Z;

						mMat	*=Matrix.CreateFromAxisAngle(axis, MathHelper.ToRadians(axisRot.W));
					}
				}
				else if(r.Name == "node")
				{
					int attcnt2	=r.AttributeCount;

					if(attcnt2 > 0)
					{
						r.MoveToFirstAttribute();
						string	id	=r.Value;

						SceneNode child	=new SceneNode();
						child.LoadNode(r);

						mChildren.Add(id, child);
					}
					else
					{
						return;
					}
				}
			}
		}
	}
}