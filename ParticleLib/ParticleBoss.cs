﻿using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpriteMapLib;
using UtilityLib;


namespace ParticleLib
{
	public class ParticleBoss
	{
		//graphics stuff
		GraphicsDevice	mGD;
		Effect			mFX;

		//texture library
		Dictionary<string, Texture2D>	mTextures;

		//indexes
		int	mNextIndex;

		class EmitterData
		{
			internal Emitter			mEmitter;
			internal ParticleViewDynVB	mView;
			internal Vector4			mColor;
		}

		//data
		Dictionary<int, EmitterData>	mEmitters	=new Dictionary<int, EmitterData>();


		public ParticleBoss(GraphicsDevice gd, Effect fx, Dictionary<string, Texture2D> texs)
		{
			mGD			=gd;
			mFX			=fx;
			mTextures	=texs;
		}


		public ParticleBoss(GraphicsDevice gd, ContentManager texCM, ContentManager sCM, string texLibPath)
		{
			mGD	=gd;

			//load particle texture lib
			Dictionary<string, TextureElement>	pTex	=new Dictionary<string, TextureElement>();
			TextureElement.LoadTexLib(texCM.RootDirectory + "/TexLibs/Particles.TexLib", texCM, pTex);

			mTextures	=new Dictionary<string, Texture2D>();
			foreach(KeyValuePair<string, TextureElement> te in pTex)
			{
				mTextures.Add(te.Key, te.Value.GetTexture(0));
			}

			mFX	=sCM.Load<Effect>("Shaders/2D");
		}


		public int CreateEmitter(string texName, Vector4 color, bool bCel,
			Emitter.Shapes shape, float shapeSize,
			int maxParticles, Vector3 pos,
			int gravYaw, int gravPitch, float gravStr,
			float startSize, float startAlpha, float emitMS,
			float rotVelMin, float rotVelMax, float velMin,
			float velMax, float sizeVelMin, float sizeVelMax,
			float alphaVelMin, float alphaVelMax,
			int lifeMin, int lifeMax)
		{
			if(!mTextures.ContainsKey(texName))
			{
				return	-1;
			}

			Emitter	newEmitter	=new Emitter(
				maxParticles, shape, shapeSize, pos,
				gravYaw, gravPitch, gravStr,
				startSize, startAlpha, emitMS,
				rotVelMin, rotVelMax, velMin, velMax,
				sizeVelMin, sizeVelMax, alphaVelMin, alphaVelMax,
				lifeMin, lifeMax);

			newEmitter.Activate(true);
			
			ParticleViewDynVB	pvd	=new ParticleViewDynVB(mGD, mFX, mTextures[texName], maxParticles);

			EmitterData	ed	=new EmitterData();
			ed.mColor		=color;
			ed.mEmitter		=newEmitter;
			ed.mView		=pvd;

			mEmitters.Add(mNextIndex++, ed);

			pvd.SetCel(bCel);

			return	mNextIndex - 1;
		}


		//returns true if emitter count changed
		public void Update(int msDelta)
		{
			foreach(KeyValuePair<int, EmitterData> em in mEmitters)
			{
				int	numParticles	=0;
				Particle	[]parts	=em.Value.mEmitter.Update(msDelta, out numParticles);

				em.Value.mView.Update(parts, numParticles);
			}
		}


		public void DrawDMN(Matrix view, Matrix proj, Vector3 eyePos)
		{
			foreach(KeyValuePair<int, EmitterData> em in mEmitters)
			{
				em.Value.mView.DrawDMN(em.Value.mColor, view, proj, eyePos);
			}
		}


		public void Draw(Matrix view, Matrix proj)
		{
			foreach(KeyValuePair<int, EmitterData> em in mEmitters)
			{
				em.Value.mView.Draw(em.Value.mColor, view, proj);
			}
		}


		public void Draw(MaterialLib.AlphaPool ap, Matrix view, Matrix proj)
		{
			foreach(KeyValuePair<int, EmitterData> em in mEmitters)
			{
				em.Value.mView.Draw(ap, em.Value.mEmitter.mPosition, em.Value.mColor, view, proj);
			}
		}


		public int GetEmitterCount()
		{
			return	mEmitters.Count;
		}


		public Emitter GetEmitterByIndex(int index)
		{
			if(!mEmitters.ContainsKey(index))
			{
				return	null;
			}
			return	mEmitters[index].mEmitter;
		}


		public Vector4 GetColorByIndex(int index)
		{
			if(!mEmitters.ContainsKey(index))
			{
				return	Vector4.One;
			}
			return	mEmitters[index].mColor;
		}


		public void SetColorByIndex(int index, Vector4 col)
		{
			if(!mEmitters.ContainsKey(index))
			{
				return;
			}
			mEmitters[index].mColor	=col;
		}


		public void SetTextureByIndex(int index, Texture2D tex)
		{
			if(!mEmitters.ContainsKey(index))
			{
				return;
			}
			mEmitters[index].mView.SetTexture(tex);
		}


		public void SetCelByIndex(int index, bool bOn)
		{
			if(!mEmitters.ContainsKey(index))
			{
				return;
			}
			mEmitters[index].mView.SetCel(bOn);
		}


		public bool GetCelByIndex(int index)
		{
			if(!mEmitters.ContainsKey(index))
			{
				return	false;
			}
			return	mEmitters[index].mView.GetCel();
		}


		public string GetTexturePathByIndex(int index)
		{
			if(!mEmitters.ContainsKey(index))
			{
				return	"";
			}
			return	mEmitters[index].mView.GetTexturePath();
		}


		public void NukeEmitter(int idx)
		{
			if(!mEmitters.ContainsKey(idx))
			{
				return;
			}
			mEmitters.Remove(idx);
		}


		public void NukeAll()
		{
			mEmitters.Clear();
		}


		internal static void AddField(ref string ent, string fieldName, string value)
		{
			ent	+="    " + fieldName + " = \"" + value + "\"\n";
		}

		
		//convert to a quark entity
		public string GetEmitterEntityString(int index)
		{
			if(!mEmitters.ContainsKey(index))
			{
				return	"";
			}

			EmitterData	ed	=mEmitters[index];

			string	entity	="QQRKSRC1\n{\n  misc_particle_emitter:e =\n  {\n";

			entity	=ed.mEmitter.GetEntityFields(entity);
			entity	=ed.mView.GetEntityFields(entity);

			Vector4	col	=ed.mColor;

			AddField(ref entity, "color", Misc.FloatToString(col.X, 4)
				+ " " + Misc.FloatToString(col.Y, 4)
				+ " " + Misc.FloatToString(col.Z, 4));

			//color's w component goes in alpha
			//quark doesn't like 4 component stuff
			AddField(ref entity, "alpha", "" + Misc.FloatToString(col.W, 4));

			entity	+="  }\n}";

			return	entity;
		}
	}
}
