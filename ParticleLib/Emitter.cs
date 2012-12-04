using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UtilityLib;


namespace ParticleLib
{
	public class Emitter
	{
		public enum Shapes
		{
			Point, Sphere, Box, Line, Plane
		}

		//randomizer
		Random	mRand	=new Random();

		//basic info
		public Vector3	mPosition;
		public float	mStartSize, mStartAlpha;
		public float	mEmitMS;
		public Shapes	mShape;
		public float	mShapeSize;

		int	mMaxParticles;
		int	mCurNumParticles;

		//particle behaviour
		//velocities in units per second
		public float	mRotationalVelocityMin, mRotationalVelocityMax;
		public float	mVelocityMin, mVelocityMax;
		public float	mSizeVelocityMin, mSizeVelocityMax;
		public float	mAlphaVelocityMin, mAlphaVelocityMax;
		public int		mLifeMin, mLifeMax;
		public int		mGravityYaw, mGravityPitch, mGravityRoll;
		public float	mGravityStrength;
		Vector3			mGravity;

		//state data
		public bool	mbOn;
		bool		mbBuffer;
		float		mEmitFrac;
		int			mEmitted;

		//particle work buffers
		Particle	[]mParticles1;
		Particle	[]mParticles2;


		public Emitter(int maxParticles, Shapes shape, float shapeSize,
			Vector3 pos, int gy, int gp, int gr, float gs,
			float startSize, float startAlpha, float emitMS,
			float rotVelMin, float rotVelMax, float velMin,
			float velMax, float sizeVelMin, float sizeVelMax,
			float alphaVelMin, float alphaVelMax,
			int lifeMin, int lifeMax)
		{
			mShape					=shape;
			mShapeSize				=shapeSize;
			mPosition				=pos;
			mGravityYaw				=gy;
			mGravityPitch			=gp;
			mGravityRoll			=gr;
			mGravityStrength		=gs;
			mStartSize				=startSize;
			mStartAlpha				=startAlpha;
			mEmitMS					=emitMS;
			mRotationalVelocityMin	=rotVelMin;
			mRotationalVelocityMax	=rotVelMax;
			mVelocityMin			=velMin;
			mVelocityMax			=velMax;
			mSizeVelocityMin		=sizeVelMin;
			mSizeVelocityMax		=sizeVelMax;
			mAlphaVelocityMin		=alphaVelMin;
			mAlphaVelocityMax		=alphaVelMax;
			mLifeMin				=lifeMin;
			mLifeMax				=lifeMax;

			mMaxParticles	=maxParticles;

			mParticles1	=new Particle[maxParticles];
			mParticles2	=new Particle[maxParticles];

			UpdateGravity();
		}


		internal void Activate(bool bOn)
		{
			mbOn		=bOn;
			mEmitFrac	=0f;
		}


		internal Particle []Update(int msDelta, out int numParticles)
		{
			numParticles	=0;

			//update existing
			Particle	[]buf	=(mbBuffer)? mParticles1 : mParticles2;
			Particle	[]buf2	=(mbBuffer)? mParticles2 : mParticles1;

			mbBuffer	=!mbBuffer;

			int	idx	=0;
			for(int i=0;i < mCurNumParticles;i++)
			{
				if(!buf[i].Update(msDelta, mGravity))
				{
					buf2[idx++]	=buf[i];
				}
			}

			if(mbOn)
			{
				float	emitCount	=msDelta * mEmitMS;

				//fractional progress
				mEmitFrac	+=emitCount;

				int	newParticles	=(int)(mEmitFrac);

				//subtract off int amount
				mEmitFrac	-=(int)mEmitFrac;

				if((newParticles + idx) >= mMaxParticles)
				{
					newParticles	=mMaxParticles - idx;
				}

				mEmitted	+=newParticles;

				for(int i=0;i < newParticles;i++)
				{
					buf2[idx++]	=Emit();
				}
			}

			numParticles		=idx;
			mCurNumParticles	=idx;

			if(idx == 0)
			{
				return	null;
			}

			return	buf2;
		}


		Vector3	PositionForShape()
		{
			if(mShape == Shapes.Point)
			{
				return	mPosition;
			}

			Vector3	ret			=Vector3.Zero;
			float	sizeOverTwo	=mShapeSize * 0.5f;

			if(mShape == Shapes.Box)
			{
				ret.X	=Mathery.RandomFloatNext(mRand, -sizeOverTwo, sizeOverTwo);
				ret.Y	=Mathery.RandomFloatNext(mRand, -sizeOverTwo, sizeOverTwo);
				ret.Z	=Mathery.RandomFloatNext(mRand, -sizeOverTwo, sizeOverTwo);
			}
			else if(mShape == Shapes.Line)
			{
				ret.X	=Mathery.RandomFloatNext(mRand, -sizeOverTwo, sizeOverTwo);
			}
			else if(mShape == Shapes.Plane)
			{
				ret.X	=Mathery.RandomFloatNext(mRand, -sizeOverTwo, sizeOverTwo);
				ret.Z	=Mathery.RandomFloatNext(mRand, -sizeOverTwo, sizeOverTwo);
			}
			else if(mShape == Shapes.Sphere)
			{
				ret	=Mathery.RandomDirection(mRand);
				ret	*=Mathery.RandomFloatNext(mRand, -sizeOverTwo, sizeOverTwo);
			}

			ret	+=mPosition;

			return	ret;
		}


		Particle Emit()
		{
			Particle	ret	=new Particle();

			ret.mPosition		=PositionForShape();
			ret.mSize			=mStartSize;
			ret.mRotation		=0;
			ret.mAlpha			=mStartAlpha;
			ret.mLifeRemaining	=mRand.Next(mLifeMin, mLifeMax);

			ret.mVelocity	=Mathery.RandomDirection(mRand)
				* Mathery.RandomFloatNext(mRand, mVelocityMin, mVelocityMax);

			ret.mRotationalVelocity	=Mathery.RandomFloatNext(mRand, mRotationalVelocityMin, mRotationalVelocityMax);

			ret.mSizeVelocity	=Mathery.RandomFloatNext(mRand, mSizeVelocityMin, mSizeVelocityMax);

			ret.mAlphaVelocity	=Mathery.RandomFloatNext(mRand, mAlphaVelocityMin, mAlphaVelocityMax);

			return	ret;
		}


		public void UpdateGravity()
		{
			float	yaw		=mGravityYaw;
			float	pitch	=mGravityPitch;
			float	roll	=mGravityRoll;
			float	str		=mGravityStrength;

			Mathery.WrapAngleDegrees(ref yaw);
			Mathery.WrapAngleDegrees(ref pitch);
			Mathery.WrapAngleDegrees(ref roll);

			yaw		=MathHelper.ToRadians(yaw);
			pitch	=MathHelper.ToRadians(pitch);
			roll	=MathHelper.ToRadians(roll);

			Matrix	gravMat	=Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);

			mGravity	=Vector3.TransformNormal(Vector3.UnitX, gravMat);
			mGravity	*=str;
		}
	}
}
