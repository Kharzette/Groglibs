﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Diagnostics;
#if !XBOX
using System.Reflection.Emit;
#endif
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;


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
			VertexTypes.AddType(typeof(VPosNormTex04));
			VertexTypes.AddType(typeof(VPosNormTex04Col0));
			VertexTypes.AddType(typeof(VPosNormBlendTex04Tex14Tex24));
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


		public static bool HasElement(Type t, Type subType, string eleName)
		{
			FieldInfo	fi	=t.GetField(eleName);
			if(fi == null)
			{
				return	false;
			}
			return	(fi.FieldType == subType);
		}


		public static int GetSizeForTypeIndex(int index)
		{
			return	GetSizeForType(GetTypeForIndex(index));
		}


		public static int GetSizeForType(Type t)
		{
			FieldInfo	[]fields	=t.GetFields();

			//packed vectors sometimes don't return any fields
			if(fields.Length == 0)
			{
				if(t == typeof(Half4))
				{
					return	8;
				}
				Debug.Assert(false);
			}

			int	size	=0;
			foreach(FieldInfo fi in fields)
			{
				if(fi.FieldType == typeof(Single))
				{
					size	+=4;
				}
				else if(fi.FieldType == typeof(Int32))
				{
					size	+=4;
				}
				else if(fi.FieldType == typeof(Vector2))
				{
					size	+=8;
				}
				else if(fi.FieldType == typeof(Vector3))
				{
					size	+=12;
				}
				else if(fi.FieldType == typeof(Vector4))
				{
					size	+=16;
				}
				else if(fi.FieldType == typeof(Half4))
				{
					size	+=8;
				}
				else if(fi.FieldType == typeof(Int4))
				{
					size	+=16;
				}
				else if(fi.FieldType == typeof(Color))
				{
					size	+=4;
				}
				else
				{
					Debug.Assert(false);	//unknown
				}
			}
			return	size;
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

				if(numTex > 0)
				{
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
				}
				else
				{
					FieldInfo	fi	=t.GetField("TexCoord0");
					if(fi != null)
					{
						continue;
					}
				}

				if(numColor > 0)
				{
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
				}
				else
				{
					FieldInfo	fi	=t.GetField("Color0");
					if(fi != null)
					{
						continue;
					}
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


		public static Buffer BuildABuffer(Device gd, Array verts, int typeIdx)
		{
			return	BuildABuffer(gd, verts, GetTypeForIndex(typeIdx));
		}


		public static Buffer BuildABuffer(Device gd, Array verts, Type vtype)
		{
			int	vertSize	=VertexTypes.GetSizeForType(vtype);

			BufferDescription	bDesc	=new BufferDescription(
				vertSize * verts.Length,
				ResourceUsage.Default, BindFlags.VertexBuffer,
				CpuAccessFlags.None, ResourceOptionFlags.None, 0);

//			IEnumerable<MethodInfo>	meths	=typeof(Buffer).GetMethods().Where(x => x.Name == "Create");

			MethodInfo genericMethod =
				typeof (Buffer).GetMethods().Where(
					x => x.Name == "Create" && x.IsGenericMethod
						&& x.GetParameters().Length == 3).Last();
            
			var typedMethod = genericMethod.MakeGenericMethod(new Type[] {vtype});

			return	typedMethod.Invoke(null, new object[] {gd, verts, bDesc}) as Buffer;
		}


		public static List<Vector3> GetPositions(Array verts, int typeIdx)
		{
			List<Vector3>	vecs	=new List<Vector3>();

			Type	vtype	=mTypes[typeIdx];

			FieldInfo	[]finfo	=vtype.GetFields();
			for(int i=0;i < verts.Length;i++)
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


		public static List<Half4> GetWeights(Array verts, int typeIdx)
		{
			List<Half4>	weights	=new List<Half4>();

			Type	vtype	=mTypes[typeIdx];

			FieldInfo	[]finfo	=vtype.GetFields();
			for(int i=0;i < verts.Length;i++)
			{
				foreach(FieldInfo fi in finfo)
				{
					//this might not be positional data!
					if(fi.Name == "BoneWeights")
					{
						Half4	vec	=(Half4)GetArrayField(verts, i, fi.Name);
						weights.Add(vec);
					}
				}
			}

			return	weights;
		}


		public static List<Color> GetBoneIndexes(Array verts, int typeIdx)
		{
			Type	vtype	=mTypes[typeIdx];

			List<Color>	idxs	=new List<Color>();

			FieldInfo	[]finfo	=vtype.GetFields();
			for(int i=0;i < verts.Length;i++)
			{
				foreach(FieldInfo fi in finfo)
				{
					//this might not be positional data!
					if(fi.Name == "BoneIndex")
					{
						Color	vec	=(Color)GetArrayField(verts, i, fi.Name);
						idxs.Add(vec);
					}
				}
			}

			return	idxs;
		}


		public static List<Vector3> GetNormals(Array verts, int typeIdx)
		{
			List<Vector3>	norms	=new List<Vector3>();

			Type	vtype	=mTypes[typeIdx];

			FieldInfo	[]finfo	=vtype.GetFields();
			for(int i=0;i < verts.Length;i++)
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


		public static List<Vector2> GetTexCoord(Array verts, int typeIdx, int set)
		{
			Type	vtype	=mTypes[typeIdx];

			List<Vector2>	texs	=new List<Vector2>();

			FieldInfo	[]finfo	=vtype.GetFields();
			for(int i=0;i < verts.Length;i++)
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


		//blasts the vert element at index
		//returning a new array and type index
		public static Array NukeElements(Array verts, int typeIdx,
			List<int> indexes, out int typeIndex)
		{
			Type	vtype	=mTypes[typeIdx];

			FieldInfo	[]fis	=vtype.GetFields();
			
			AssemblyName	asmName	=new AssemblyName();
			
			asmName.Name	="FakeAsm";
			
			AssemblyBuilder	asmBuild	=System.Threading.Thread.GetDomain()
				.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);

			ModuleBuilder	modBuild	=asmBuild.DefineDynamicModule("ModuleOne", "FakeAsm.dll");

			TypeBuilder	tb	=modBuild.DefineType("VertStuff",
				TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.SequentialLayout,
				typeof(ValueType));

			for(int i=0;i < fis.Length;i++)
			{
				if(indexes.Contains(i))
				{
					continue;
				}
				tb.DefineField(fis[i].Name, fis[i].FieldType, FieldAttributes.Public);
			}

			Type	newType	=tb.CreateType();

			//count texcoords
			int	texCnt	=0;
			while(HasElement(newType, typeof(Vector2), "TexCoord" + texCnt))
			{
				texCnt++;
			}

			//count colors
			int	colCnt	=0;
			while(HasElement(newType, typeof(Vector4), "Color" + colCnt))
			{
				colCnt++;
			}

			bool	bPos		=HasElement(newType, typeof(Vector3), "Position");
			bool	bNorm		=HasElement(newType, typeof(Vector3), "Normal");
			bool	bBoneIdx	=HasElement(newType, typeof(Color), "BoneIndex");
			bool	bBoneWeight	=HasElement(newType, typeof(Half4), "BoneWeights");
			bool	bTan		=HasElement(newType, typeof(Vector3), "Tangent");
			bool	bBiTan		=HasElement(newType, typeof(Vector3), "BiTangent");

			//build the new type
			Type	vtypeNew	=GetMatch(
				bPos,
				bNorm,
				bBoneIdx,
				bBoneWeight,
				bTan, bBiTan,
				texCnt,
				colCnt);

			Array	newVerts	=Array.CreateInstance(vtypeNew, verts.Length);

			for(int i=0;i < newVerts.Length;i++)
			{
				for(int j=0;j < fis.Length;j++)
				{
					if(indexes.Contains(j))
					{
						continue;
					}
					SetArrayField(newVerts, i, fis[j].Name, GetArrayField(verts, i, fis[j].Name));
				}
			}

			typeIndex	=GetIndex(vtypeNew);

			return	newVerts;
		}


		//replace the weights in array verts
		public static void ReplaceWeights(Array verts, Half4 []newWeights)
		{
			Debug.Assert(verts.Length == newWeights.Length);

			for(int i=0;i < verts.Length;i++)
			{
				SetArrayField(verts, i, "BoneWeights", newWeights[i]);
			}
		}


		public static void ReplaceBoneIndexes(Array verts, Color []newInds)
		{
			Debug.Assert(verts.Length == newInds.Length);

			for(int i=0;i < verts.Length;i++)
			{
				SetArrayField(verts, i, "BoneIndex", newInds[i]);
			}
		}


		//create a new vert array with tangents added
		public static Array AddTangents(Array verts, int typeIdx,
			Vector4 []tans, out int typeIndex)
		{
			Type	vtype	=mTypes[typeIdx];

			//count texcoords
			int	texCnt	=0;
			while(HasElement(vtype, typeof(Vector2), "TexCoord" + texCnt))
			{
				texCnt++;
			}

			//count colors
			int	colCnt	=0;
			while(HasElement(vtype, typeof(Vector4), "Color" + colCnt))
			{
				colCnt++;
			}

			bool	bPos		=HasElement(vtype, typeof(Vector3), "Position");
			bool	bNorm		=HasElement(vtype, typeof(Vector3), "Normal");
			bool	bBoneIdx	=HasElement(vtype, typeof(Color), "BoneIndex");
			bool	bBoneWeight	=HasElement(vtype, typeof(Half4), "BoneWeights");

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

			Array	newVerts	=Array.CreateInstance(vtypeNew, verts.Length);

			for(int i=0;i < verts.Length;i++)
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

			return	newVerts;
		}


		public static void GetVertBounds(Array verts, int typeIdx,
			out BoundingBox box, out BoundingSphere sphere)
		{
			List<Vector3>	points	=GetPositions(verts, typeIdx);

			box		=BoundingBox.FromPoints(points.ToArray());
			sphere	=UtilityLib.Mathery.SphereFromPoints(points);
		}


		public static void WriteVerts(BinaryWriter bw, Array verts, int typeIdx)
		{
			Type	vtype	=mTypes[typeIdx];

			bw.Write(typeIdx);
			bw.Write(verts.Length);

			FieldInfo	[]finfo	=vtype.GetFields();
			for(int i=0;i < verts.Length;i++)
			{
				foreach(FieldInfo fi in finfo)
				{
					if(fi.FieldType.Name == "Single")
					{
						Single	vec	=(Single)GetArrayField(verts, i, fi.Name);
						bw.Write(vec);
					}
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
					else if(fi.FieldType.Name == "Half4")
					{
						Half4	vec	=(Half4)GetArrayField(verts, i, fi.Name);
						bw.Write(vec.X);
						bw.Write(vec.Y);
						bw.Write(vec.Z);
						bw.Write(vec.W);
					}
					else if(fi.FieldType.Name == "Color")
					{
						Color	col	=(Color)GetArrayField(verts, i, fi.Name);
						bw.Write(col.ToRgba());
					}
					else
					{
						Debug.Assert(false);
					}
				}
			}
		}


		public static void ReadVerts(BinaryReader	br,
									 Device			gd,
									 out Array		outVerts)
		{
			int	typeIdx		=br.ReadInt32();
			int	numVerts	=br.ReadInt32();

			Type	vtype	=mTypes[typeIdx];

			outVerts	=Array.CreateInstance(vtype, numVerts);

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

						SetArrayField(outVerts, i, fi.Name, vec);
					}
					else if(fi.FieldType.Name == "Vector3")
					{
						Vector3	vec	=Vector3.Zero;

						vec.X	=br.ReadSingle();
						vec.Y	=br.ReadSingle();
						vec.Z	=br.ReadSingle();

						SetArrayField(outVerts, i, fi.Name, vec);
					}
					else if(fi.FieldType.Name == "Vector4")
					{
						Vector4	vec	=Vector4.Zero;

						vec.X	=br.ReadSingle();
						vec.Y	=br.ReadSingle();
						vec.Z	=br.ReadSingle();
						vec.W	=br.ReadSingle();

						SetArrayField(outVerts, i, fi.Name, vec);
					}
					else if(fi.FieldType.Name == "Half4")
					{
						Half4	vec	=new Half4();

						vec.X	=br.ReadSingle();
						vec.Y	=br.ReadSingle();
						vec.Z	=br.ReadSingle();
						vec.W	=br.ReadSingle();

						SetArrayField(outVerts, i, fi.Name, vec);
					}
					else if(fi.FieldType.Name == "Color")
					{
						int	packedCol	=br.ReadInt32();

						Color	col	=new Color(packedCol);

						SetArrayField(outVerts, i, fi.Name, col);
					}
					else
					{
						Debug.Assert(false);
					}
				}
			}
		}
	}
}
