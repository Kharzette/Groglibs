﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace MeshLib
{
	public class StaticMeshObject
	{
		List<StaticMesh>	mMeshParts	=new List<StaticMesh>();

		//refs to anim and material libs
		MaterialLib.MaterialLib	mMatLib;

		//transform
		Matrix	mTransform;


		public StaticMeshObject(MaterialLib.MaterialLib ml)
		{
			mMatLib		=ml;
		}


		public Matrix GetTransform()
		{
			return	mTransform;
		}


		public void SetTransform(Matrix mat)
		{
			mTransform	=mat;
		}


		public void AddMeshPart(Mesh m)
		{
			StaticMesh	sm	=m as StaticMesh;

			if(sm != null)
			{
				mMeshParts.Add(sm);
			}
		}


		public Mesh GetMeshPart(string name)
		{
			foreach(Mesh m in mMeshParts)
			{
				if(m.Name == name)
				{
					return	m;
				}
			}
			return	null;
		}


		public void NukeMesh(Mesh m)
		{
			StaticMesh	sm	=m as StaticMesh;

			if(sm != null)
			{
				if(mMeshParts.Contains(sm))
				{
					mMeshParts.Remove(sm);
				}
			}
		}


		//for gui
		public List<StaticMesh> GetMeshPartList()
		{
			return	mMeshParts;
		}


		public void SaveToFile(string fileName)
		{
			FileStream	file	=new FileStream(fileName, FileMode.Create, FileAccess.Write);
			BinaryWriter	bw	=new BinaryWriter(file);

			//write a magic number identifying a static
			UInt32	magic	=0x57A71C35;

			bw.Write(magic);

			//save mesh parts
			bw.Write(mMeshParts.Count);
			foreach(StaticMesh m in mMeshParts)
			{
				m.Write(bw);
			}

			bw.Close();
			file.Close();
		}


		//set bEditor if you want the buffers set to readable
		//so they can be resaved if need be
		public bool ReadFromFile(string fileName, GraphicsDevice gd, bool bEditor)
		{
			Stream	file	=null;
			if(!bEditor)
			{
				file	=UtilityLib.FileUtil.OpenTitleFile(fileName);
			}
			else
			{
				file	=new FileStream(fileName, FileMode.Open, FileAccess.Read);
			}

			if(file == null)
			{
				return	false;
			}

			BinaryReader	br	=new BinaryReader(file);

			//clear existing data
			mMeshParts.Clear();

			//read magic number
			UInt32	magic	=br.ReadUInt32();

			if(magic != 0x57A71C35)
			{
				return	false;
			}

			int	numMesh	=br.ReadInt32();
			for(int i=0;i < numMesh;i++)
			{
				StaticMesh	m	=new StaticMesh();

				m.Read(br, gd, bEditor);
				mMeshParts.Add(m);
			}

			br.Close();
			file.Close();

			mTransform	=Matrix.Identity;

			return	true;
		}


		public void Draw(GraphicsDevice gd)
		{
			foreach(StaticMesh m in mMeshParts)
			{
				if(!m.Visible)
				{
					continue;
				}
				m.Draw(gd, mMatLib, mTransform, "");
			}
		}


		public void SetSecondVertexBufferBinding(VertexBufferBinding v2)
		{
			foreach(StaticMesh m in mMeshParts)
			{
				if(!m.Visible)
				{
					continue;
				}
				m.SetSecondVertexBufferBinding(v2);
			}
		}


		//instanced
		public void Draw(GraphicsDevice gd, int numInstances)
		{
			foreach(StaticMesh m in mMeshParts)
			{
				if(!m.Visible)
				{
					continue;
				}
				m.Draw(gd, mMatLib, numInstances);
			}
		}


		public void Draw(GraphicsDevice gd, string altMatName)
		{
			foreach(StaticMesh m in mMeshParts)
			{
				if(!m.Visible)
				{
					continue;
				}

				m.Draw(gd, mMatLib, mTransform, altMatName);
			}
		}


		//draw instanced
		public void Draw(GraphicsDevice gd, int numInstances, string altMatName)
		{
			foreach(StaticMesh m in mMeshParts)
			{
				if(!m.Visible)
				{
					continue;
				}

				string	temp	=m.MaterialName;

				m.MaterialName	=altMatName;
				m.Draw(gd, mMatLib, numInstances);
				m.MaterialName	=temp;
			}
		}


		public void UpdateBounds()
		{
			foreach(StaticMesh m in mMeshParts)
			{
				m.Bound();
			}
		}


		public BoundingBox GetBoxBound()
		{
			List<Vector3>	pnts	=new List<Vector3>();
			foreach(StaticMesh m in mMeshParts)
			{
				BoundingBox	b	=m.GetBoxBounds();

				//internal part transforms
				Vector3	transMin	=Vector3.Transform(b.Min, m.GetTransform());
				Vector3	transMax	=Vector3.Transform(b.Max, m.GetTransform());

				pnts.Add(transMin);
				pnts.Add(transMax);
			}

			return	BoundingBox.CreateFromPoints(pnts);
		}


		public BoundingSphere GetSphereBound()
		{
			BoundingSphere	merged;
			merged.Center	=Vector3.Zero;
			merged.Radius	=0.0f;
			foreach(StaticMesh m in mMeshParts)
			{
				BoundingSphere	s	=m.GetSphereBounds();

				s	=s.Transform(m.GetTransform());

				merged	=BoundingSphere.CreateMerged(merged, s);
			}
			return	merged;
		}


		public float? RayIntersect(Vector3 start, Vector3 end, bool bBox, out StaticMesh partHit)
		{
			//find which piece was hit
			float		minDist	=float.MaxValue;
			partHit				=null;

			foreach(StaticMesh m in mMeshParts)
			{
				if(!m.Visible)
				{
					continue;
				}
				Nullable<float>	dist	=m.RayIntersect(start, end, bBox);
				if(dist != null)
				{
					if(dist.Value < minDist)
					{
						partHit	=m;
						minDist	=dist.Value;
					}
				}
			}

			if(partHit == null)
			{
				return	null;
			}
			return	minDist;
		}


		public static Dictionary<string, StaticMeshObject> LoadAllMeshes(string dir,
			GraphicsDevice gd, ContentManager cm, MaterialLib.MaterialLib mats)
		{
			Dictionary<string, StaticMeshObject>	ret	=new Dictionary<string, StaticMeshObject>();

			if(Directory.Exists(cm.RootDirectory + "/" + dir))
			{
				DirectoryInfo	di	=new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory
					+ cm.RootDirectory + "/" + dir + "/");

				FileInfo[]		fi	=di.GetFiles("*.Static", SearchOption.TopDirectoryOnly);
				foreach(FileInfo f in fi)
				{
					//strip back
					string	path	=f.DirectoryName.Substring(
						f.DirectoryName.LastIndexOf(cm.RootDirectory));

					StaticMeshObject	smo	=new StaticMeshObject(mats);
					bool	bWorked	=smo.ReadFromFile(path + "\\" + f.Name, gd, false);

					if(bWorked)
					{
						ret.Add(f.Name, smo);
					}
				}
			}

			return	ret;
		}
	}
}