﻿using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using UtilityLib;

using SharpDX;
using SharpDX.DXGI;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX.IO;

//ambiguous stuff
using Color		=SharpDX.Color;
using Device	=SharpDX.Direct3D11.Device;
using Resource	=SharpDX.Direct3D11.Resource;
using wic		=SharpDX.WIC;


namespace MaterialLib
{
	//hangs on to stuff like shaders and textures
	public class StuffKeeper
	{
		internal class IncludeFX : CallbackBase, Include
		{
			string	mRootDir;

			internal IncludeFX(string rootDir)
			{
				mRootDir	=rootDir;
			}

			static string includeDirectory = "Shaders\\";
			public void Close(Stream stream)
			{
				stream.Close();
				stream.Dispose();
			}

			public Stream Open(IncludeType type, string fileName, Stream parentStream)
			{
				return	new FileStream(mRootDir + "\\" + includeDirectory + fileName, FileMode.Open);
			}
		}

		IncludeFX	mIFX;

		//for texture loading
		wic.ImagingFactory	mIF	=new wic.ImagingFactory();

		//game directory
		string	mGameRootDir;

		//list of shaders available
		Dictionary<string, Effect>	mFX	=new Dictionary<string, Effect>();

		//texture 2ds
		Dictionary<string, Texture2D>	mTexture2s	=new Dictionary<string, Texture2D>();

		//font texture 2ds
		Dictionary<string, Texture2D>	mFontTexture2s	=new Dictionary<string, Texture2D>();

		//list of texturey things
		Dictionary<string, Resource>	mResources	=new Dictionary<string, Resource>();

		//list of shader resource views for stuff like textures
		Dictionary<string, ShaderResourceView>	mSRVs	=new Dictionary<string, ShaderResourceView>();

		//shader resource views for fonts
		Dictionary<string, ShaderResourceView>	mFontSRVs	=new Dictionary<string, ShaderResourceView>();

		//list of parameter variables per shader
		Dictionary<string, List<EffectVariable>>	mVars	=new Dictionary<string, List<EffectVariable>>();

		//data driven set of ignored and hidden parameters (see text file)
		Dictionary<string, List<string>>	mIgnoreData	=new Dictionary<string,List<string>>();
		Dictionary<string, List<string>>	mHiddenData	=new Dictionary<string,List<string>>();

		//list of font data
		Dictionary<string, Font>	mFonts	=new Dictionary<string, Font>();

		public enum ShaderModel
		{
			SM5, SM41, SM4, SM2
		};

		public event EventHandler	eCompileNeeded;	//shader compiles needed, passes the number
		public event EventHandler	eCompileDone;		//fired as each compile finishes


		public StuffKeeper() { }

		public void Init(GraphicsDevice gd, string gameRootDir)
		{
			mGameRootDir	=gameRootDir;
			mIFX			=new IncludeFX(gameRootDir);

			switch(gd.GD.FeatureLevel)
			{
				case	FeatureLevel.Level_11_0:
					Construct(gd, ShaderModel.SM5);
					break;
				case	FeatureLevel.Level_10_1:
					Construct(gd, ShaderModel.SM41);
					break;
				case	FeatureLevel.Level_10_0:
					Construct(gd, ShaderModel.SM4);
					break;
				case	FeatureLevel.Level_9_3:
					Construct(gd, ShaderModel.SM2);
					break;
				default:
					Debug.Assert(false);	//only support the above
					Construct(gd, ShaderModel.SM2);
					break;
			}
		}


		void Construct(GraphicsDevice gd, ShaderModel sm)
		{
			LoadShaders(gd.GD, sm);
			SaveHeaderTimeStamps(sm);
			LoadResources(gd);
			LoadFonts(gd);
			LoadParameterData();

			GrabVariables();
		}


		internal Dictionary<string, List<string>> GetIgnoreData()
		{
			return	mIgnoreData;
		}


		internal Dictionary<string, List<string>> GetHideData()
		{
			return	mHiddenData;
		}


		internal Texture2D GetTexture2D(string name)
		{
			if(!mTexture2s.ContainsKey(name))
			{
				return	null;
			}
			return	mTexture2s[name];
		}


		internal Font GetFont(string name)
		{
			if(!mFonts.ContainsKey(name))
			{
				return	null;
			}
			return	mFonts[name];
		}


		internal List<EffectVariable> GetEffectVariables(string fxName)
		{
			if(!mVars.ContainsKey(fxName))
			{
				return	null;
			}

			return	mVars[fxName];
		}


		internal EffectVariable GetVariable(string fxName, string varName)
		{
			if(!mFX.ContainsKey(fxName))
			{
				return	null;
			}

			return	mVars[fxName].FirstOrDefault(v => v.Description.Name == varName);
		}


		internal ShaderResourceView GetSRV(string name)
		{
			if(!mSRVs.ContainsKey(name))
			{
				return	null;
			}
			return	mSRVs[name];
		}


		internal ShaderResourceView GetFontSRV(string name)
		{
			if(!mFontSRVs.ContainsKey(name))
			{
				return	null;
			}
			return	mFontSRVs[name];
		}


		internal Effect EffectForName(string fxName)
		{
			if(mFX.ContainsKey(fxName))
			{
				return	mFX[fxName];
			}
			return	null;
		}


		internal ShaderResourceView ResourceForName(string fxName)
		{
			if(mSRVs.ContainsKey(fxName))
			{
				return	mSRVs[fxName];
			}
			return	null;
		}


		internal string NameForEffect(Effect fx)
		{
			if(mFX.ContainsValue(fx))
			{
				foreach(KeyValuePair<string, Effect> ef in mFX)
				{
					if(ef.Value == fx)
					{
						return	ef.Key;
					}
				}
			}
			return	"";
		}


		internal List<EffectVariable> GrabVariables(string fx)
		{
			if(mVars.ContainsKey(fx))
			{
				return	mVars[fx];
			}
			return	new List<EffectVariable>();
		}


		//for tools mainly
		public List<string> GetTexture2DList()
		{
			List<string>	ret	=new List<string>();

			foreach(KeyValuePair<string, Texture2D> tex in mTexture2s)
			{
				ret.Add(tex.Key);
			}
			return	ret;
		}


		public List<string> GetFontList()
		{
			List<string>	ret	=new List<string>();

			//sort fonts by height
			IOrderedEnumerable<KeyValuePair<string, Font>>	sorted
				=mFonts.OrderBy(fnt => fnt.Value.GetCharacterHeight());

			foreach(KeyValuePair<string, Font> font in sorted)
			{
				ret.Add(font.Key);
			}
			return	ret;
		}


		public List<string> GetEffectList()
		{
			List<string>	ret	=new List<string>();

			foreach(KeyValuePair<string, Effect> fx in mFX)
			{
				ret.Add(fx.Key);
			}
			return	ret;
		}


		public void AddMap(string name, ShaderResourceView srv)
		{
			if(mSRVs.ContainsKey(name))
			{
				mSRVs.Remove(name);
			}
			mSRVs.Add(name, srv);
		}


		public void AddTex(Device dev, string name, Color []texData, int w, int h)
		{
			if(mTexture2s.ContainsKey(name))
			{
				return;
			}

			Texture2D	tex	=MakeTexture(dev, texData, w, h);

			mResources.Add(name, tex as Resource);
			mTexture2s.Add(name, tex);

			ShaderResourceView	srv	=new ShaderResourceView(dev, tex);

			srv.DebugName	=name;

			mSRVs.Add(name, srv);
		}


		public bool AddTexToAtlas(TexAtlas atlas, string texName, GraphicsDevice gd,
			out double scaleU, out double scaleV, out double uoffs, out double voffs)
		{
			scaleU	=scaleV	=uoffs	=voffs	=0.0;

			if(!mTexture2s.ContainsKey(texName))
			{
				return	false;
			}

			//even though we already have this loaded, there appears
			//to be no way to get at the bits, so load it again
			int		w, h;
			Color	[]colArray	=LoadPNGWIC(mIF, mGameRootDir + "\\Textures\\" + texName + ".png",
									out w, out h);

			return	atlas.Insert(colArray, w, h,
				out scaleU, out scaleV, out uoffs, out voffs);
		}


		public void FreeAll()
		{
			mIF.Dispose();

			foreach(KeyValuePair<string, Texture2D> tex in mTexture2s)
			{
				tex.Value.Dispose();
			}
			mTexture2s.Clear();

			foreach(KeyValuePair<string, Texture2D> tex in mFontTexture2s)
			{
				tex.Value.Dispose();
			}
			mFontTexture2s.Clear();

			foreach(KeyValuePair<string, Resource> res in mResources)
			{
				res.Value.Dispose();
			}
			mResources.Clear();

			foreach(KeyValuePair<string, ShaderResourceView> srv in mSRVs)
			{
				srv.Value.Dispose();
			}
			mSRVs.Clear();

			foreach(KeyValuePair<string, ShaderResourceView> srv in mFontSRVs)
			{
				srv.Value.Dispose();
			}
			mFontSRVs.Clear();

			foreach(KeyValuePair<string, List<EffectVariable>> effList in mVars)
			{
				foreach(EffectVariable efv in effList.Value)
				{
					efv.Dispose();
				}
				effList.Value.Clear();
			}
			mVars.Clear();

			mIgnoreData.Clear();
			mHiddenData.Clear();

			foreach(KeyValuePair<string, Effect> fx in mFX)
			{
				fx.Value.Dispose();
			}
			mFX.Clear();
		}


		//sometimes dx11 thinks a rendertarget is still bound as a resource
		//setting that resource to null and then calling this will give it
		//a kick in the pants to actually free the resource up
		public void HackyTechniqueRefresh(DeviceContext dc, string fx, string tech)
		{
			if(!mFX.ContainsKey(fx))
			{
				return;
			}

			EffectTechnique	et	=mFX[fx].GetTechniqueByName(tech);

			if(et == null || !et.IsValid)
			{
				return;
			}

			EffectPass	ep	=et.GetPassByIndex(0);

			ep.Apply(dc);

			ep.Dispose();
			et.Dispose();
		}


		void LoadResources(GraphicsDevice gd)
		{
			//see if Textures folder exists in Content
			if(Directory.Exists(mGameRootDir + "/Textures"))
			{
				DirectoryInfo	di	=new DirectoryInfo(mGameRootDir + "/Textures/");

				FileInfo[]		fi	=di.GetFiles("*.png", SearchOption.AllDirectories);
				foreach(FileInfo f in fi)
				{
					LoadTexture(gd, f.DirectoryName, f.Name);
				}
			}
		}


		void LoadFonts(GraphicsDevice gd)
		{
			//see if Fonts folder exists in Content
			if(!Directory.Exists(mGameRootDir + "/Fonts"))
			{
				return;
			}

			DirectoryInfo	di	=new DirectoryInfo(mGameRootDir + "/Fonts/");

			FileInfo[]		fi	=di.GetFiles("*.png", SearchOption.AllDirectories);
			foreach(FileInfo f in fi)
			{
				LoadFontTexture(gd, f.DirectoryName, f.Name);

				string	extLess	=FileUtil.StripExtension(f.Name);

				Font	font	=new Font(f.DirectoryName + "/" + extLess + ".dat");
				mFonts.Add(extLess, font);
			}
		}


		//load all shaders in the shaders folder
		void LoadShaders(Device dev, ShaderModel sm)
		{
			ShaderMacro []macs	=new ShaderMacro[1];

			macs[0]	=new ShaderMacro(sm.ToString(), 1);

			//see if Shader folder exists in Content
			if(Directory.Exists(mGameRootDir + "/Shaders"))
			{
				DirectoryInfo	di	=new DirectoryInfo(mGameRootDir + "/Shaders/");

				bool	bHeaderSame	=false;
				if(Directory.Exists(mGameRootDir + "/CompiledShaders"))
				{
					//see if a precompiled exists
					if(Directory.Exists(mGameRootDir + "/CompiledShaders/" + macs[0].Name))
					{
						DirectoryInfo	preDi	=new DirectoryInfo(
							mGameRootDir + "/CompiledShaders/" + macs[0].Name);

						bHeaderSame	=CheckHeaderTimeStamps(preDi, di);
					}
				}

				List<string>	dirNames	=new List<string>();
				List<string>	fileNames	=new List<string>();

				FileInfo[]		fi	=di.GetFiles("*.fx", SearchOption.AllDirectories);
				foreach(FileInfo f in fi)
				{
					if(bHeaderSame)
					{
						if(Directory.Exists(mGameRootDir + "/CompiledShaders"))
						{
							//see if a precompiled exists
							if(Directory.Exists(mGameRootDir + "/CompiledShaders/" + macs[0].Name))
							{
								DirectoryInfo	preDi	=new DirectoryInfo(
									mGameRootDir + "/CompiledShaders/" + macs[0].Name);

								FileInfo[]	preFi	=preDi.GetFiles(f.Name + ".Compiled", SearchOption.TopDirectoryOnly);

								if(preFi.Length == 1)
								{
									if(f.LastWriteTime <= preFi[0].LastWriteTime)
									{
										LoadCompiledShader(dev, preFi[0].DirectoryName, preFi[0].Name, macs);
										continue;
									}
								}
							}
						}
					}
					dirNames.Add(f.DirectoryName);
					fileNames.Add(f.Name);
				}

				if(fileNames.Count > 0)
				{
					Debug.Assert(fileNames.Count == dirNames.Count);

					Misc.SafeInvoke(eCompileNeeded, fileNames.Count);

					for(int i=0;i < fileNames.Count;i++)
					{
						LoadShader(dev, dirNames[i], fileNames[i], macs);
						Misc.SafeInvoke(eCompileDone, i + 1);
					}
				}
			}
		}


		void LoadCompiledShader(Device dev, string dir, string file, ShaderMacro []macs)
		{
			string	fullPath	=dir + "\\" + file;

			FileStream		fs	=new FileStream(fullPath, FileMode.Open, FileAccess.Read);
			BinaryReader	br	=new BinaryReader(fs);

			int	len	=br.ReadInt32();

			byte	[]code	=br.ReadBytes(len);

			//if you get an unable to find dll in path error here, make
			//sure materiallib's sharpdx effect dlls are marked as
			//content with copy if newer
			Effect	fx	=new Effect(dev, code);
			if(fx != null)
			{
				mFX.Add(file.Substring(0, file.Length - 9), fx);
			}

			br.Close();
			fs.Close();
		}


		void SaveHeaderTimeStamps(ShaderModel sm)
		{
			if(!Directory.Exists(mGameRootDir + "/CompiledShaders"))
			{
				return;
			}

			DirectoryInfo	src	=new DirectoryInfo(mGameRootDir + "/Shaders/");

			if(!src.Exists)
			{
				return;
			}

			DirectoryInfo	di	=new DirectoryInfo(mGameRootDir + "/CompiledShaders/" + sm.ToString() + "/");

			FileStream	fs	=new FileStream(
				di.FullName + "Header.TimeStamps",
				FileMode.Create, FileAccess.Write);

			Debug.Assert(fs != null);

			BinaryWriter	bw	=new BinaryWriter(fs);

			Dictionary<string, DateTime>	stamps	=GetHeaderTimeStamps(src);

			bw.Write(stamps.Count);
			foreach(KeyValuePair<string, DateTime> time in stamps)
			{
				bw.Write(time.Key);
				bw.Write(time.Value.Ticks);
			}

			bw.Close();
			fs.Close();
		}


		Dictionary<string, DateTime> GetHeaderTimeStamps(DirectoryInfo di)
		{
			FileInfo[]		fi	=di.GetFiles("*.fxh", SearchOption.AllDirectories);

			Dictionary<string, DateTime>	ret	=new Dictionary<string, DateTime>();

			foreach(FileInfo f in fi)
			{
				ret.Add(f.Name, f.LastWriteTime);
			}
			return	ret;
		}


		//returns true if headers haven't changed
		bool CheckHeaderTimeStamps(DirectoryInfo preDi, DirectoryInfo srcDi)
		{
			//see if there is a binary file here that contains the
			//timestamps of the fxh files
			FileInfo[]	hTime	=preDi.GetFiles("Header.TimeStamps", SearchOption.TopDirectoryOnly);
			if(hTime.Length != 1)
			{
				return	false;
			}

			FileStream	fs	=new FileStream(hTime[0].DirectoryName + "\\" + hTime[0].Name, FileMode.Open, FileAccess.Read);
			if(fs == null)
			{
				return	false;
			}

			BinaryReader	br	=new BinaryReader(fs);

			Dictionary<string, DateTime>	times	=new Dictionary<string, DateTime>();

			int	count	=br.ReadInt32();
			for(int i=0;i < count;i++)
			{
				string	fileName	=br.ReadString();
				long	time		=br.ReadInt64();

				DateTime	t	=new DateTime(time);

				times.Add(fileName, t);
			}

			br.Close();
			fs.Close();

			Dictionary<string, DateTime>	onDisk	=GetHeaderTimeStamps(srcDi);

			//check the timestamp data against the dates
			if(onDisk.Count != times.Count)
			{
				return	false;
			}

			foreach(KeyValuePair<string, DateTime> tstamp in onDisk)
			{
				if(!times.ContainsKey(tstamp.Key))
				{
					return	false;
				}

				if(times[tstamp.Key] < tstamp.Value)
				{
					return	false;
				}
			}
			return	true;
		}


		void LoadShader(Device dev, string dir, string file, ShaderMacro []macs)
		{
			if(!Directory.Exists(mGameRootDir + "/CompiledShaders"))
			{
				Directory.CreateDirectory(mGameRootDir + "/CompiledShaders");
			}

			if(!Directory.Exists(mGameRootDir + "/CompiledShaders/" + macs[0].Name))
			{
				Directory.CreateDirectory(mGameRootDir + "/CompiledShaders/" + macs[0].Name);
			}

			string	fullPath	=dir + "\\" + file;

			CompilationResult	shdRes	=ShaderBytecode.CompileFromFile(
				fullPath, "fx_5_0", ShaderFlags.Debug, EffectFlags.None, macs, mIFX);

			Effect	fx	=new Effect(dev, shdRes);
			if(fx == null)
			{
				return;
			}

			Debug.Assert(fx.IsValid);

			//do a validity check on all techniques and passes
			for(int i=0;i < fx.Description.TechniqueCount;i++)
			{
				EffectTechnique	et	=fx.GetTechniqueByIndex(i);

				Debug.Assert(et.IsValid);

				for(int j=0;j < et.Description.PassCount;j++)
				{
					EffectPass	ep	=et.GetPassByIndex(j);

					Debug.Assert(ep.IsValid);

					ep.Dispose();
				}

				et.Dispose();
			}

			FileStream	fs	=new FileStream(mGameRootDir + "/CompiledShaders/"
				+ macs[0].Name + "/" + file + ".Compiled",
				FileMode.Create, FileAccess.Write);

			BinaryWriter	bw	=new BinaryWriter(fs);

			bw.Write(shdRes.Bytecode.Data.Length);
			bw.Write(shdRes.Bytecode.Data, 0, shdRes.Bytecode.Data.Length);

			bw.Close();
			fs.Close();

			mFX.Add(file, fx);
		}


		void LoadTexture(GraphicsDevice gd, string path, string fileName)
		{
			int	texIndex	=path.LastIndexOf("Textures");

			string	afterTex	="";

			if((texIndex + 9) < path.Length)
			{
				afterTex	=path.Substring(texIndex + 9);
			}
			string	extLess	="";

			if(afterTex != "")
			{
				extLess	=afterTex + "\\" + FileUtil.StripExtension(fileName);
			}
			else
			{
				extLess	=FileUtil.StripExtension(fileName);
			}

			int	w, h;
			Color	[]colArray	=LoadPNGWIC(mIF, path + "\\" + fileName, out w, out h);

			PreMultAndLinear(colArray, w, h);

			Texture2D	finalTex	=MakeTexture(gd.GD, colArray, w, h);

			mResources.Add(extLess, finalTex as Resource);
			mTexture2s.Add(extLess, finalTex);

			ShaderResourceView	srv	=new ShaderResourceView(gd.GD, finalTex);

			srv.DebugName	=extLess;

			mSRVs.Add(extLess, srv);
		}


		void LoadFontTexture(GraphicsDevice gd, string path, string fileName)
		{
			int	texIndex	=path.LastIndexOf("Fonts");

			string	afterTex	="";

			if((texIndex + 6) < path.Length)
			{
				afterTex	=path.Substring(texIndex + 6);
			}
			string	extLess	="";

			if(afterTex != "")
			{
				extLess	=afterTex + "\\" + FileUtil.StripExtension(fileName);
			}
			else
			{
				extLess	=FileUtil.StripExtension(fileName);
			}

			int	w,h;
			Color	[]colors	=LoadPNGWIC(mIF, path + "\\" + fileName, out w, out h);

			PreMultAndLinear(colors, w, h);

			Texture2D	finalTex	=MakeTexture(gd.GD, colors, w, h);

			mResources.Add(extLess, finalTex as Resource);
			mFontTexture2s.Add(extLess, finalTex);

			ShaderResourceView	srv	=new ShaderResourceView(gd.GD, finalTex);

			srv.DebugName	=extLess;

			mFontSRVs.Add(extLess, srv);
		}


		Texture2D MakeTexture(Device dev, Color []colors, int width, int height)
		{
			Texture2DDescription	texDesc	=new Texture2DDescription();
			texDesc.ArraySize				=1;
			texDesc.BindFlags				=BindFlags.ShaderResource;
			texDesc.CpuAccessFlags			=CpuAccessFlags.None;
			texDesc.MipLevels				=1;
			texDesc.OptionFlags				=ResourceOptionFlags.None;
			texDesc.Usage					=ResourceUsage.Immutable;
			texDesc.Width					=width;
			texDesc.Height					=height;
			texDesc.Format					=Format.R8G8B8A8_UNorm;
			texDesc.SampleDescription		=new SampleDescription(1, 0);

			DataStream	ds	=DataStream.Create<Color>(colors, true, true, 0);

			ds.Position	=0;

			DataBox	[]dbs	=new DataBox[1];

			dbs[0]	=new DataBox(ds.DataPointer, width * 4, width * height * 4);

			Texture2D	tex	=new Texture2D(dev, texDesc, dbs);

			ds.Dispose();

			return	tex;
		}


		void GrabVariables()
		{
			foreach(KeyValuePair<string, Effect> fx in mFX)
			{
				for(int i=0;;i++)
				{
					EffectVariable	ev	=fx.Value.GetVariableByIndex(i);
					if(ev == null)
					{
						break;
					}
					if(!ev.IsValid)
					{
						ev.Dispose();
						break;
					}

					if(!mVars.ContainsKey(fx.Key))
					{
						mVars.Add(fx.Key, new List<EffectVariable>());
					}
					mVars[fx.Key].Add(ev);
				}
			}
		}


		void PreMultAndLinear(Color []colors, int width, int height)
		{
			float	oo255	=1.0f / 255.0f;

			for(int y=0;y < height;y++)
			{
				//divide pitch by sizeof(Color)
				int	ofs	=y * width;

				for(int x=0;x < width;x++)
				{
					Color	c	=colors[ofs + x];

					float	xc	=c.R * oo255;
					float	yc	=c.G * oo255;
					float	zc	=c.B * oo255;
					float	wc	=c.A * oo255;

					//convert to linear
					xc	=(float)Math.Pow(xc, 2.2);
					yc	=(float)Math.Pow(yc, 2.2);
					zc	=(float)Math.Pow(zc, 2.2);

					//premultiply alpha
					xc	*=wc;
					yc	*=wc;
					zc	*=wc;

					colors[ofs + x].R	=(byte)(xc * 255.0f);
					colors[ofs + x].G	=(byte)(yc * 255.0f);
					colors[ofs + x].B	=(byte)(zc * 255.0f);
				}
			}
		}


		public static Color[] LoadPNGWIC(wic.ImagingFactory wif, string path, out int w, out int h)
		{
			wic.BitmapDecoder	pbd	=new wic.BitmapDecoder(wif, path,
					NativeFileAccess.Read, wic.DecodeOptions.CacheOnDemand);

			wic.BitmapFrameDecode	bfd	=pbd.GetFrame(0);

			wic.FormatConverter	conv	=new wic.FormatConverter(wif);

			conv.Initialize(bfd, wic.PixelFormat.Format32bppRGBA);

			w	=bfd.Size.Width;
			h	=bfd.Size.Height;

			Color	[]colArray	=new Color[w * h];

			conv.CopyPixels<Color>(colArray);

			conv.Dispose();
			bfd.Dispose();
			pbd.Dispose();

			return	colArray;
		}


		void LoadParameterData()
		{
			if(!Directory.Exists(mGameRootDir + "/Shaders"))
			{
				return;
			}

			FileStream		fs	=new FileStream(mGameRootDir + "/Shaders/ParameterData.txt", FileMode.Open, FileAccess.Read);
			StreamReader	sr	=new StreamReader(fs);

			string			curTechnique	="";
			string			curCategory		="";
			List<string>	curStuff		=new List<string>();
			for(;;)
			{
				string	line	=sr.ReadLine();
				if(line.StartsWith("//"))
				{
					continue;
				}

				//python style!
				if(line.StartsWith("\t\t"))
				{
					Debug.Assert(curTechnique != "");
					Debug.Assert(curCategory != "");

					if(curCategory == "Ignored")
					{
						curStuff.Add(line.Trim());
					}
					else if(curCategory == "Hidden")
					{
						curStuff.Add(line.Trim());
					}
					else
					{
						Debug.Assert(false);
					}
				}
				else if(line.StartsWith("\t"))
				{
					if(curStuff.Count > 0)
					{
						if(curCategory == "Ignored")
						{
							mIgnoreData.Add(curTechnique, curStuff);
						}
						else if(curCategory == "Hidden")
						{
							mHiddenData.Add(curTechnique, curStuff);
						}
						else
						{
							Debug.Assert(false);
						}
						curStuff	=new List<string>();
					}
					curCategory	=line.Trim();
				}
				else
				{
					if(curStuff.Count > 0)
					{
						if(curCategory == "Ignored")
						{
							mIgnoreData.Add(curTechnique, curStuff);
						}
						else if(curCategory == "Hidden")
						{
							mHiddenData.Add(curTechnique, curStuff);
						}
						else
						{
							Debug.Assert(false);
						}
						curStuff	=new List<string>();
					}
					curCategory		="";
					curTechnique	=line.Trim();
				}

				if(sr.EndOfStream)
				{
					if(curStuff.Count > 0)
					{
						if(curCategory == "Ignored")
						{
							mIgnoreData.Add(curTechnique, curStuff);
						}
						else if(curCategory == "Hidden")
						{
							mHiddenData.Add(curTechnique, curStuff);
						}
						else
						{
							Debug.Assert(false);
						}
						curStuff	=new List<string>();
					}
					break;
				}
			}

			sr.Close();
			fs.Close();
		}
	}
}