﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Linq;
#if !XBOX
using System.Reflection.Emit;
#endif
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;

namespace MeshLib
{
	public static class VertexTypes
	{
		static List<Type>	mTypes	=new List<Type>();

		static VertexTypes()
		{
			VertexTypes.AddType(typeof(VPos));
			VertexTypes.AddType(typeof(VPosNorm));
			VertexTypes.AddType(typeof(VPosBone));
			VertexTypes.AddType(typeof(VPosTex0));
			VertexTypes.AddType(typeof(VPosTex0Tex1));
			VertexTypes.AddType(typeof(VPosTex0Tex1Tex2));
			VertexTypes.AddType(typeof(VPosTex0Tex1Tex2Tex3));
			VertexTypes.AddType(typeof(VPosCol0));
			VertexTypes.AddType(typeof(VPosCol0Col1));
			VertexTypes.AddType(typeof(VPosCol0Col1Col2));
			VertexTypes.AddType(typeof(VPosCol0Col1Col2Col3));
			VertexTypes.AddType(typeof(VPosTex0Col0));
			VertexTypes.AddType(typeof(VPosTex0Col0Col1));
			VertexTypes.AddType(typeof(VPosTex0Col0Col1Col2));
			VertexTypes.AddType(typeof(VPosTex0Col0Col1Col2Col3));
			VertexTypes.AddType(typeof(VPosTex0Tex1Col0));
			VertexTypes.AddType(typeof(VPosTex0Tex1Col0Col1));
			VertexTypes.AddType(typeof(VPosTex0Tex1Col0Col1Col2));
			VertexTypes.AddType(typeof(VPosTex0Tex1Col0Col1Col2Col3));
			VertexTypes.AddType(typeof(VPosTex0Tex1Tex2Col0));
			VertexTypes.AddType(typeof(VPosTex0Tex1Tex2Col0Col1));
			VertexTypes.AddType(typeof(VPosTex0Tex1Tex2Col0Col1Col2));
			VertexTypes.AddType(typeof(VPosTex0Tex1Tex2Col0Col1Col2Col3));
			VertexTypes.AddType(typeof(VPosTex0Tex1Tex2Tex3Col0));
			VertexTypes.AddType(typeof(VPosTex0Tex1Tex2Tex3Col0Col1));
			VertexTypes.AddType(typeof(VPosTex0Tex1Tex2Tex3Col0Col1Col2));
			VertexTypes.AddType(typeof(VPosTex0Tex1Tex2Tex3Col0Col1Col2Col3));
			VertexTypes.AddType(typeof(VPosBoneTex0));
			VertexTypes.AddType(typeof(VPosBoneTex0Tex1));
			VertexTypes.AddType(typeof(VPosBoneTex0Tex1Tex2));
			VertexTypes.AddType(typeof(VPosBoneTex0Tex1Tex2Tex3));
			VertexTypes.AddType(typeof(VPosBoneCol0));
			VertexTypes.AddType(typeof(VPosBoneCol0Col1));
			VertexTypes.AddType(typeof(VPosBoneCol0Col1Col2));
			VertexTypes.AddType(typeof(VPosBoneCol0Col1Col2Col3));
			VertexTypes.AddType(typeof(VPosBoneTex0Col0));
			VertexTypes.AddType(typeof(VPosBoneTex0Col0Col1));
			VertexTypes.AddType(typeof(VPosBoneTex0Col0Col1Col2));
			VertexTypes.AddType(typeof(VPosBoneTex0Col0Col1Col2Col3));
			VertexTypes.AddType(typeof(VPosBoneTex0Tex1Col0));
			VertexTypes.AddType(typeof(VPosBoneTex0Tex1Col0Col1));
			VertexTypes.AddType(typeof(VPosBoneTex0Tex1Col0Col1Col2));
			VertexTypes.AddType(typeof(VPosBoneTex0Tex1Col0Col1Col2Col3));
			VertexTypes.AddType(typeof(VPosBoneTex0Tex1Tex2Col0));
			VertexTypes.AddType(typeof(VPosBoneTex0Tex1Tex2Col0Col1));
			VertexTypes.AddType(typeof(VPosBoneTex0Tex1Tex2Col0Col1Col2));
			VertexTypes.AddType(typeof(VPosBoneTex0Tex1Tex2Col0Col1Col2Col3));
			VertexTypes.AddType(typeof(VPosBoneTex0Tex1Tex2Tex3Col0));
			VertexTypes.AddType(typeof(VPosBoneTex0Tex1Tex2Tex3Col0Col1));
			VertexTypes.AddType(typeof(VPosBoneTex0Tex1Tex2Tex3Col0Col1Col2));
			VertexTypes.AddType(typeof(VPosBoneTex0Tex1Tex2Tex3Col0Col1Col2Col3));
			VertexTypes.AddType(typeof(VPosNormTex0));
			VertexTypes.AddType(typeof(VPosNormTex0Tex1));
			VertexTypes.AddType(typeof(VPosNormTex0Tex1Tex2));
			VertexTypes.AddType(typeof(VPosNormTex0Tex1Tex2Tex3));
			VertexTypes.AddType(typeof(VPosNormCol0));
			VertexTypes.AddType(typeof(VPosNormCol0Col1));
			VertexTypes.AddType(typeof(VPosNormCol0Col1Col2));
			VertexTypes.AddType(typeof(VPosNormCol0Col1Col2Col3));
			VertexTypes.AddType(typeof(VPosNormTex0Col0));
			VertexTypes.AddType(typeof(VPosNormTex0Col0Col1));
			VertexTypes.AddType(typeof(VPosNormTex0Col0Col1Col2));
			VertexTypes.AddType(typeof(VPosNormTex0Col0Col1Col2Col3));
			VertexTypes.AddType(typeof(VPosNormTex0Tex1Col0));
			VertexTypes.AddType(typeof(VPosNormTex0Tex1Col0Col1));
			VertexTypes.AddType(typeof(VPosNormTex0Tex1Col0Col1Col2));
			VertexTypes.AddType(typeof(VPosNormTex0Tex1Col0Col1Col2Col3));
			VertexTypes.AddType(typeof(VPosNormTex0Tex1Tex2Col0));
			VertexTypes.AddType(typeof(VPosNormTex0Tex1Tex2Col0Col1));
			VertexTypes.AddType(typeof(VPosNormTex0Tex1Tex2Col0Col1Col2));
			VertexTypes.AddType(typeof(VPosNormTex0Tex1Tex2Col0Col1Col2Col3));
			VertexTypes.AddType(typeof(VPosNormTex0Tex1Tex2Tex3Col0));
			VertexTypes.AddType(typeof(VPosNormTex0Tex1Tex2Tex3Col0Col1));
			VertexTypes.AddType(typeof(VPosNormTex0Tex1Tex2Tex3Col0Col1Col2));
			VertexTypes.AddType(typeof(VPosNormTex0Tex1Tex2Tex3Col0Col1Col2Col3));
			VertexTypes.AddType(typeof(VPosNormBoneTex0));
			VertexTypes.AddType(typeof(VPosNormBoneTex0Tex1));
			VertexTypes.AddType(typeof(VPosNormBoneTex0Tex1Tex2));
			VertexTypes.AddType(typeof(VPosNormBoneTex0Tex1Tex2Tex3));
			VertexTypes.AddType(typeof(VPosNormBoneCol0));
			VertexTypes.AddType(typeof(VPosNormBoneCol0Col1));
			VertexTypes.AddType(typeof(VPosNormBoneCol0Col1Col2));
			VertexTypes.AddType(typeof(VPosNormBoneCol0Col1Col2Col3));
			VertexTypes.AddType(typeof(VPosNormBoneTex0Col0));
			VertexTypes.AddType(typeof(VPosNormBoneTex0Col0Col1));
			VertexTypes.AddType(typeof(VPosNormBoneTex0Col0Col1Col2));
			VertexTypes.AddType(typeof(VPosNormBoneTex0Col0Col1Col2Col3));
			VertexTypes.AddType(typeof(VPosNormBoneTex0Tex1Col0));
			VertexTypes.AddType(typeof(VPosNormBoneTex0Tex1Col0Col1));
			VertexTypes.AddType(typeof(VPosNormBoneTex0Tex1Col0Col1Col2));
			VertexTypes.AddType(typeof(VPosNormBoneTex0Tex1Col0Col1Col2Col3));
			VertexTypes.AddType(typeof(VPosNormBoneTex0Tex1Tex2Col0));
			VertexTypes.AddType(typeof(VPosNormBoneTex0Tex1Tex2Col0Col1));
			VertexTypes.AddType(typeof(VPosNormBoneTex0Tex1Tex2Col0Col1Col2));
			VertexTypes.AddType(typeof(VPosNormBoneTex0Tex1Tex2Col0Col1Col2Col3));
			VertexTypes.AddType(typeof(VPosNormBoneTex0Tex1Tex2Tex3Col0));
			VertexTypes.AddType(typeof(VPosNormBoneTex0Tex1Tex2Tex3Col0Col1));
			VertexTypes.AddType(typeof(VPosNormBoneTex0Tex1Tex2Tex3Col0Col1Col2));
			VertexTypes.AddType(typeof(VPosNormBoneTex0Tex1Tex2Tex3Col0Col1Col2Col3));
			VertexTypes.AddType(typeof(VPosNormBone));
			VertexTypes.AddType(typeof(VPosNormBlendTex0Tex1Tex2Tex3Tex4));
			VertexTypes.AddType(typeof(VPosNormTanTex0));
			VertexTypes.AddType(typeof(VPosNormTanBiTanTex0));
		}

		public static void AddType(Type t)
		{
			mTypes.Add(t);
		}


		public static Type GetTypeForIndex(int idx)
		{
			return	mTypes[idx];
		}


		public static int GetIndex(Type t)
		{
			return	mTypes.IndexOf(t);
		}


		public static int GetIndexForVertexDeclaration(VertexDeclaration vd)
		{
			Type	t	=GetTypeForVertexDeclaration(vd);

			return	GetIndex(t);
		}


		public static Type GetTypeForVertexDeclaration(VertexDeclaration vd)
		{
			VertexElement	[]elems	=vd.GetVertexElements();

			bool	bPos, bNorm, bBoneIdx, bBoneWeight, bTan, bBiTan;
			int		numTex, numColor;

			bPos	=bNorm	=bBoneIdx	=bTan	=bBoneWeight	=bBiTan	=false;
			numTex	=numColor	=0;

			foreach(VertexElement el in elems)
			{
				if(el.VertexElementFormat == VertexElementFormat.Vector3)
				{
					if(el.VertexElementUsage == VertexElementUsage.Position)
					{
						bPos	=true;
					}
					else if(el.VertexElementUsage == VertexElementUsage.Normal)
					{
						bNorm	=true;
					}
					else if(el.VertexElementUsage == VertexElementUsage.Binormal)
					{
						bBiTan	=true;
					}
				}
				else if(el.VertexElementFormat == VertexElementFormat.Vector4)
				{
					if(el.VertexElementUsage == VertexElementUsage.BlendIndices)
					{
						bBoneIdx	=true;
					}
					else if(el.VertexElementUsage == VertexElementUsage.BlendWeight)
					{
						bBoneWeight	=true;
					}
					else if(el.VertexElementUsage == VertexElementUsage.Color)
					{
						numColor++;
					}
					else if(el.VertexElementUsage == VertexElementUsage.Tangent)
					{
						bTan	=true;
					}
				}
				else if(el.VertexElementFormat == VertexElementFormat.Vector2)
				{
					if(el.VertexElementUsage == VertexElementUsage.TextureCoordinate)
					{
						numTex++;
					}
				}
			}

			return	GetMatch(bPos, bNorm, bBoneIdx, bBoneWeight, bTan, bBiTan, numTex, numColor);
		}


		public static bool HasElement(Type t, string eleName)
		{
			FieldInfo	fi	=t.GetField(eleName);
			return	(fi != null);
		}


		public static int GetSizeForType(Type t)
		{
			int			i			=0;
			int			sizeSoFar	=0;
			FieldInfo	fi;

			fi	=t.GetField("Position");
			if(fi != null)
			{
				sizeSoFar	+=12;
			}
			fi	=t.GetField("Normal");
			if(fi != null)
			{
				sizeSoFar	+=12;
			}
			fi	=t.GetField("Tangent");
			if(fi != null)
			{
				sizeSoFar	+=16;
			}
			fi	=t.GetField("BiTangent");
			if(fi != null)
			{
				sizeSoFar	+=12;
			}
			fi	=t.GetField("BoneIndex");
			if(fi != null)
			{
				sizeSoFar	+=16;
			}
			fi	=t.GetField("BoneWeights");
			if(fi != null)
			{
				sizeSoFar	+=16;
			}
			while(true)
			{
				fi	=t.GetField("TexCoord" + i);
				if(fi == null)
				{
					break;
				}
				i++;
				sizeSoFar	+=8;
			}
			i	=0;
			while(true)
			{
				fi	=t.GetField("Color" + i);
				if(fi == null)
				{
					break;
				}
				i++;
				sizeSoFar	+=16;
			}

			return	sizeSoFar;
		}


		//don't think this is in use
		public static VertexDeclaration GetVertexDeclarationForType(Type t)
		{
			List<VertexElement>	ves			=new List<VertexElement>();
			int					i			=0;
			short				sizeSoFar	=0;
			FieldInfo			fi;

			fi	=t.GetField("Position");
			if(fi != null)
			{
				VertexElement ve	=new VertexElement(
					sizeSoFar, VertexElementFormat.Vector3,
					VertexElementUsage.Position,
					(byte)i);
				ves.Add(ve);
				sizeSoFar	+=12;
			}
			fi	=t.GetField("Normal");
			if(fi != null)
			{
				VertexElement ve	=new VertexElement(
					sizeSoFar, VertexElementFormat.Vector3,
					VertexElementUsage.Normal,
					(byte)i);
				ves.Add(ve);
				sizeSoFar	+=12;
			}
			fi	=t.GetField("Tangent");
			if(fi != null)
			{
				VertexElement ve	=new VertexElement(
					sizeSoFar, VertexElementFormat.Vector4,
					VertexElementUsage.Tangent,
					(byte)i);
				ves.Add(ve);
				sizeSoFar	+=16;
			}
			fi	=t.GetField("BiTangent");
			if(fi != null)
			{
				VertexElement ve	=new VertexElement(
					sizeSoFar, VertexElementFormat.Vector3,
					VertexElementUsage.Binormal,
					(byte)i);
				ves.Add(ve);
				sizeSoFar	+=12;
			}
			fi	=t.GetField("BoneIndex");
			if(fi != null)
			{
				VertexElement ve	=new VertexElement(
					sizeSoFar, VertexElementFormat.Vector4,
					VertexElementUsage.BlendIndices,
					(byte)i);
				ves.Add(ve);
				i++;
				sizeSoFar	+=16;
			}
			fi	=t.GetField("BoneWeights");
			if(fi != null)
			{
				VertexElement ve	=new VertexElement(
					sizeSoFar, VertexElementFormat.Vector4,
					VertexElementUsage.BlendWeight,
					(byte)i);
				ves.Add(ve);
				i++;
				sizeSoFar	+=16;
			}
			i	=0;
			while(true)
			{
				fi	=t.GetField("TexCoord" + i);
				if(fi == null)
				{
					break;
				}
				VertexElement ve	=new VertexElement(
					sizeSoFar, VertexElementFormat.Vector2,
					VertexElementUsage.TextureCoordinate,
					(byte)i);
				ves.Add(ve);
				i++;
				sizeSoFar	+=8;
			}
			i	=0;
			while(true)
			{
				fi	=t.GetField("Color" + i);
				if(fi == null)
				{
					break;
				}
				VertexElement ve	=new VertexElement(
					sizeSoFar, VertexElementFormat.Vector4,
					VertexElementUsage.Color,
					(byte)i);
				ves.Add(ve);
				i++;
				sizeSoFar	+=16;
			}

			VertexElement	[]vel	=new VertexElement[ves.Count];

			for(i=0;i < ves.Count;i++)
			{
				vel[i]	=ves[i];
			}
			
			return	new VertexDeclaration(vel);
		}


		//match up vertex characteristics to one of the
		//structure types in VertexStructures
		public static Type GetMatch(bool bPos, bool bNorm, bool bBoneIdx,
			bool bBoneWeight, bool bTan, bool bBiTan, int numTex, int numColor)
		{
			foreach(Type t in mTypes)
			{
				//only support 1 position
				if(bPos)
				{
					FieldInfo	fi	=t.GetField("Position");
					if(fi == null)
					{
						continue;
					}
				}
				//only support 1 normal set
				if(bNorm)
				{
					FieldInfo	fi	=t.GetField("Normal");
					if(fi == null)
					{
						continue;
					}
				}
				//only support 1 tangent set
				if(bTan)
				{
					FieldInfo	fi	=t.GetField("Tangent");
					if(fi == null)
					{
						continue;
					}
				}
				//only support 1 bitangent set
				if(bBiTan)
				{
					FieldInfo	fi	=t.GetField("BiTangent");
					if(fi == null)
					{
						continue;
					}
				}
				if(bBoneIdx)
				{
					FieldInfo	fi	=t.GetField("BoneIndex");
					if(fi == null)
					{
						continue;
					}
				}
				if(bBoneWeight)
				{
					FieldInfo	fi	=t.GetField("BoneWeights");
					if(fi == null)
					{
						continue;
					}
				}
				bool	bFound	=true;
				for(int i=0;i < numTex;i++)
				{
					FieldInfo	fi	=t.GetField("TexCoord" + i);
					if(fi == null)
					{
						bFound	=false;
						break;
					}
				}
				if(!bFound)
				{
					continue;
				}
				for(int i=0;i < numColor;i++)
				{
					FieldInfo	fi	=t.GetField("Color" + i);
					if(fi == null)
					{
						bFound	=false;
						break;
					}
				}
				if(!bFound)
				{
					continue;
				}
				return	t;
			}
			return	typeof(object);
		}


		public static void SetArrayField(Array a, int idx, string fieldName, object value)
		{
			//grab the struct out of the array
			object	val	=a.GetValue(idx);

			//find the field in the struct
			FieldInfo	fi	=val.GetType().GetField(fieldName);
			if(fi == null)
			{
				return;
			}

			//set the field's value
			fi.SetValue(val, value);

			//put the struct back into the array modified
			a.SetValue(val, idx);
		}


		public static object GetArrayField(Array a, int idx, string fieldName)
		{
			//grab the struct out of the array
			object	val	=a.GetValue(idx);

			//find the field in the struct
			FieldInfo	fi	=val.GetType().GetField(fieldName);
			if(fi == null)
			{
				return	null;
			}

			return	fi.GetValue(val);
		}


		static Array GetVertArray(VertexBuffer vb, int numVerts, int typeIdx)
		{
			Type	vtype	=mTypes[typeIdx];
			Array	verts	=Array.CreateInstance(vtype, numVerts);

			MethodInfo genericMethod =
				typeof (VertexBuffer).GetMethods().Where(
					x => x.Name == "GetData" && x.IsGenericMethod && x.GetParameters().Length == 1).Single();
            
			var typedMethod = genericMethod.MakeGenericMethod(new Type[] {vtype});

			typedMethod.Invoke(vb, new object[] {verts});

			return	verts;
		}


		public static List<Vector3> GetPositions(VertexBuffer vb, int numVerts, int typeIdx)
		{
			List<Vector3>	vecs	=new List<Vector3>();

			Type	vtype	=mTypes[typeIdx];
			Array	verts	=GetVertArray(vb, numVerts, typeIdx);

			FieldInfo	[]finfo	=vtype.GetFields();
			for(int i=0;i < numVerts;i++)
			{
				foreach(FieldInfo fi in finfo)
				{
					//this might not be positional data!
					if(fi.Name == "Position")
					{
						Vector3	vec	=(Vector3)GetArrayField(verts, i, fi.Name);
						vecs.Add(vec);
					}
				}
			}

			return	vecs;
		}


		public static List<Vector3> GetNormals(VertexBuffer vb, int numVerts, int typeIdx)
		{
			List<Vector3>	norms	=new List<Vector3>();

			Type	vtype	=mTypes[typeIdx];
			Array	verts	=GetVertArray(vb, numVerts, typeIdx);

			FieldInfo	[]finfo	=vtype.GetFields();
			for(int i=0;i < numVerts;i++)
			{
				foreach(FieldInfo fi in finfo)
				{
					//this might not be positional data!
					if(fi.Name == "Normal")
					{
						Vector3	vec	=(Vector3)GetArrayField(verts, i, fi.Name);
						norms.Add(vec);
					}
				}
			}

			return	norms;
		}


		public static List<Vector2> GetTexCoord(VertexBuffer vb, int numVerts, int typeIdx, int set)
		{
			Type	vtype	=mTypes[typeIdx];
			Array	verts	=GetVertArray(vb, numVerts, typeIdx);

			List<Vector2>	texs	=new List<Vector2>();

			FieldInfo	[]finfo	=vtype.GetFields();
			for(int i=0;i < numVerts;i++)
			{
				foreach(FieldInfo fi in finfo)
				{
					//this might not be positional data!
					if(fi.Name == "TexCoord" + set)
					{
						Vector2	vec	=(Vector2)GetArrayField(verts, i, fi.Name);
						texs.Add(vec);
					}
				}
			}

			return	texs;
		}


		//create a new vertexbuffer with tangents added
		public static VertexBuffer AddTangents(GraphicsDevice gd, VertexBuffer vb, int numVerts, int typeIdx, Vector4 []tans, out int typeIndex)
		{
			Type	vtype	=mTypes[typeIdx];
			Array	verts	=GetVertArray(vb, numVerts, typeIdx);

			//count texcoords
			int	texCnt	=0;
			while(HasElement(vtype, "TexCoord" + texCnt))
			{
				texCnt++;
			}

			//count colors
			int	colCnt	=0;
			while(HasElement(vtype, "Color" + colCnt))
			{
				colCnt++;
			}

			bool	bPos		=HasElement(vtype, "Position");
			bool	bNorm		=HasElement(vtype, "Normal");
			bool	bBoneIdx	=HasElement(vtype, "BoneIndex");
			bool	bBoneWeight	=HasElement(vtype, "BoneWeights");

			//build the new type
			Type	vtypeNew	=GetMatch(
				bPos,
				bNorm,
				bBoneIdx,
				bBoneWeight,
				true,
				false,
				texCnt,
				colCnt);

			Array	newVerts	=Array.CreateInstance(vtypeNew, numVerts);

			for(int i=0;i < numVerts;i++)
			{
				if(bPos)
				{
					SetArrayField(newVerts, i, "Position", GetArrayField(verts, i, "Position"));
				}
				if(bNorm)
				{
					SetArrayField(newVerts, i, "Normal", GetArrayField(verts, i, "Normal"));
				}
				if(bBoneIdx)
				{
					SetArrayField(newVerts, i, "BoneIndex", GetArrayField(verts, i, "BoneIndex"));
				}
				if(bBoneWeight)
				{
					SetArrayField(newVerts, i, "BoneWeights", GetArrayField(verts, i, "BoneWeights"));
				}
				for(int j=0;j < texCnt;j++)
				{
					SetArrayField(newVerts, i, "TexCoord" + j, GetArrayField(verts, i, "TexCoord" + j));
				}
				for(int j=0;j < colCnt;j++)
				{
					SetArrayField(newVerts, i, "Color" + j, GetArrayField(verts, i, "Color" + j));
				}

				//tangents
				SetArrayField(newVerts, i, "Tangent", tans[i]);
			}

			typeIndex	=GetIndex(vtypeNew);

			VertexDeclaration	dec	=GetVertexDeclarationForType(vtypeNew);

			VertexBuffer vb2	=new VertexBuffer(gd, dec, numVerts, BufferUsage.None);
			
			MethodInfo genericMethod =
				typeof (VertexBuffer).GetMethods().Where(
					x => x.Name == "SetData" && x.IsGenericMethod && x.GetParameters().Length == 1).Single();
            
			var typedMethod = genericMethod.MakeGenericMethod(new Type[] {vtypeNew});

			typedMethod.Invoke(vb2, new object[] {newVerts});

			return	vb2;
		}


		//create a new vertexbuffer with tangents and bitangents added
		public static VertexBuffer AddTangents(GraphicsDevice gd, VertexBuffer vb, int numVerts,
			int typeIdx, Vector3 []tans, Vector3 []bitans, out int typeIndex)
		{
			Type	vtype	=mTypes[typeIdx];
			Array	verts	=GetVertArray(vb, numVerts, typeIdx);

			//count texcoords
			int	texCnt	=0;
			while(HasElement(vtype, "TexCoord" + texCnt))
			{
				texCnt++;
			}

			//count colors
			int	colCnt	=0;
			while(HasElement(vtype, "Color" + colCnt))
			{
				colCnt++;
			}

			bool	bPos		=HasElement(vtype, "Position");
			bool	bNorm		=HasElement(vtype, "Normal");
			bool	bBoneIdx	=HasElement(vtype, "BoneIndex");
			bool	bBoneWeight	=HasElement(vtype, "BoneWeights");

			//build the new type
			Type	vtypeNew	=GetMatch(
				bPos,
				bNorm,
				bBoneIdx,
				bBoneWeight,
				true,
				true,
				texCnt,
				colCnt);

			Array	newVerts	=Array.CreateInstance(vtypeNew, numVerts);

			for(int i=0;i < numVerts;i++)
			{
				if(bPos)
				{
					SetArrayField(newVerts, i, "Position", GetArrayField(verts, i, "Position"));
				}
				if(bNorm)
				{
					SetArrayField(newVerts, i, "Normal", GetArrayField(verts, i, "Normal"));
				}
				if(bBoneIdx)
				{
					SetArrayField(newVerts, i, "BoneIndex", GetArrayField(verts, i, "BoneIndex"));
				}
				if(bBoneWeight)
				{
					SetArrayField(newVerts, i, "BoneWeights", GetArrayField(verts, i, "BoneWeights"));
				}
				for(int j=0;j < texCnt;j++)
				{
					SetArrayField(newVerts, i, "TexCoord" + j, GetArrayField(verts, i, "TexCoord" + j));
				}
				for(int j=0;j < colCnt;j++)
				{
					SetArrayField(newVerts, i, "Color" + j, GetArrayField(verts, i, "Color" + j));
				}

				//tangents
				SetArrayField(newVerts, i, "Tangent", tans[i]);

				//bitangents
				SetArrayField(newVerts, i, "BiTangent", bitans[i]);
			}

			typeIndex	=GetIndex(vtypeNew);

			VertexDeclaration	dec	=GetVertexDeclarationForType(vtypeNew);

			VertexBuffer vb2	=new VertexBuffer(gd, dec, numVerts, BufferUsage.None);
			
			MethodInfo genericMethod =
				typeof (VertexBuffer).GetMethods().Where(
					x => x.Name == "SetData" && x.IsGenericMethod && x.GetParameters().Length == 1).Single();
            
			var typedMethod = genericMethod.MakeGenericMethod(new Type[] {vtypeNew});

			typedMethod.Invoke(vb2, new object[] {newVerts});

			return	vb2;
		}


		public static void GetVertBounds(VertexBuffer vb, int numVerts, int typeIdx, IRayCastable bound)
		{
			AxialBounds	bnd	=new AxialBounds();

			List<Vector3>	points	=GetPositions(vb, numVerts, typeIdx);

			bound.AddPointListToBounds(points);
		}


		public static void WriteVerts(BinaryWriter bw, VertexBuffer vb, int typeIdx)
		{
			Type	vtype	=mTypes[typeIdx];
			Array	verts	=Array.CreateInstance(vtype, vb.VertexCount);

			//save vertex declaration first
			UtilityLib.FileUtil.WriteVertexDeclaration(bw, vb.VertexDeclaration);

			MethodInfo genericMethod =
				typeof (VertexBuffer).GetMethods().Where(
					x => x.Name == "GetData" && x.IsGenericMethod && x.GetParameters().Length == 1).Single();
            
			var typedMethod = genericMethod.MakeGenericMethod(new Type[] {vtype});

			typedMethod.Invoke(vb, new object[] {verts});

			FieldInfo	[]finfo	=vtype.GetFields();
			for(int i=0;i < vb.VertexCount;i++)
			{
				foreach(FieldInfo fi in finfo)
				{
					if(fi.FieldType.Name == "Vector2")
					{
						Vector2	vec	=(Vector2)GetArrayField(verts, i, fi.Name);
						bw.Write(vec.X);
						bw.Write(vec.Y);
					}
					else if(fi.FieldType.Name == "Vector3")
					{
						Vector3	vec	=(Vector3)GetArrayField(verts, i, fi.Name);
						bw.Write(vec.X);
						bw.Write(vec.Y);
						bw.Write(vec.Z);
					}
					else if(fi.FieldType.Name == "Vector4")
					{
						Vector4	vec	=(Vector4)GetArrayField(verts, i, fi.Name);
						bw.Write(vec.X);
						bw.Write(vec.Y);
						bw.Write(vec.Z);
						bw.Write(vec.W);
					}
				}
			}
		}


		public static void ReadVerts(BinaryReader		br,
									GraphicsDevice		gd,
									out VertexBuffer	vb,
									int					numVerts,
									int					typeIdx,
									bool				bEditor)
		{
			Type	vtype	=mTypes[typeIdx];
			Array	verts	=Array.CreateInstance(vtype, numVerts);

			MethodInfo	[]meths	=typeof (VertexBuffer).GetMethods();
			MethodInfo	genericMethod	=null;
			
			//read the vertex declaration
			VertexDeclaration	vd;
			UtilityLib.FileUtil.ReadVertexDeclaration(br, out vd);

			foreach(MethodInfo mi in meths)
			{
				if(mi.Name == "SetData" && mi.IsGenericMethod)
				{
					genericMethod	=mi;

					//get parameters is not supported on generic methods
					//so make a typed method to test
					var testMethod	=genericMethod.MakeGenericMethod(new Type[] {vtype});
					ParameterInfo	[]pi	=testMethod.GetParameters();
					if(pi.Length == 1)
					{
						break;
					}
				}
			}
            
			var typedMethod = genericMethod.MakeGenericMethod(new Type[] {vtype});

			if(bEditor)
			{
				vb	=new VertexBuffer(gd, vd, numVerts, BufferUsage.None);
			}
			else
			{
				vb	=new VertexBuffer(gd, vd, numVerts, BufferUsage.WriteOnly);
			}

			FieldInfo	[]finfo	=vtype.GetFields();
			for(int i=0;i < numVerts;i++)
			{
				foreach(FieldInfo fi in finfo)
				{
					if(fi.FieldType.Name == "Vector2")
					{
						Vector2	vec	=Vector2.Zero;

						vec.X	=br.ReadSingle();
						vec.Y	=br.ReadSingle();

						SetArrayField(verts, i, fi.Name, vec);
					}
					else if(fi.FieldType.Name == "Vector3")
					{
						Vector3	vec	=Vector3.Zero;

						vec.X	=br.ReadSingle();
						vec.Y	=br.ReadSingle();
						vec.Z	=br.ReadSingle();

						SetArrayField(verts, i, fi.Name, vec);
					}
					else if(fi.FieldType.Name == "Vector4")
					{
						Vector4	vec	=Vector4.Zero;

						vec.X	=br.ReadSingle();
						vec.Y	=br.ReadSingle();
						vec.Z	=br.ReadSingle();
						vec.W	=br.ReadSingle();

						SetArrayField(verts, i, fi.Name, vec);
					}
				}
			}

			typedMethod.Invoke(vb, new object[] {verts});
		}
	}
}
