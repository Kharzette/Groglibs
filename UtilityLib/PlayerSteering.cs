﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace UtilityLib
{
	public class PlayerSteering
	{
		public enum SteeringMethod
		{
			None, TwinStick, Fly, ThirdPerson, FirstPerson
		}

		SteeringMethod	mMethod;

		//movement settings
		float	mSpeed				=0.1f;
		float	mMouseSensitivity	=0.01f;
		float	mGamePadSensitivity	=0.05f;
		float	mTurnSpeed			=3.0f;

		//position info
		Vector3	mPosition, mDelta;
		float	mPitch, mYaw, mRoll;

		//for mouselook
		MouseState	mOriginalMS;

		//constants
		const float	PitchClamp	=85.0f;


		public PlayerSteering(float width, float height)
		{
			//set up mouselook on right button
			Mouse.SetPosition((int)(width / 2), (int)(height / 2));
			mOriginalMS	=Mouse.GetState();
		}

		public SteeringMethod Method
		{
			get { return mMethod; }
			set { mMethod = value; }
		}

		public Vector3 Position
		{
			get { return mPosition; }
			set { mPosition = value; }
		}

		public float Speed
		{
			get { return mSpeed; }
			set { mSpeed = value; }
		}

		public Vector3 Delta
		{
			get { return mDelta; }
			set { mDelta = value; }
		}

		public float Pitch
		{
			get { return mPitch; }
			set { mPitch = value; }
		}

		public float Yaw
		{
			get { return mYaw; }
			set { mYaw = value; }
		}

		public float Roll
		{
			get { return mRoll; }
			set { mRoll = value; }
		}


		public void Update(float msDelta, Matrix view, KeyboardState ks, MouseState ms, GamePadState gs)
		{
			if(mMethod == SteeringMethod.None)
			{
				return;
			}

			if(mMethod == SteeringMethod.FirstPerson)
			{
				UpdateGroundMovement(msDelta, view, ks, ms, gs);
			}
			else if(mMethod == SteeringMethod.Fly)
			{
				UpdateFly(msDelta, view, ks, ms, gs);
			}
			else if(mMethod == SteeringMethod.ThirdPerson)
			{
				UpdateGroundMovement(msDelta, view, ks, ms, gs);
			}
			else if(mMethod == SteeringMethod.TwinStick)
			{
				UpdateTwinStick(msDelta, view, ks, ms, gs);
			}
		}


		void UpdateTwinStick(float msDelta, Matrix view, KeyboardState ks, MouseState ms, GamePadState gs)
		{
			Vector3 vup		=Vector3.Zero;
			Vector3 vleft	=Vector3.Zero;
			Vector3 vin		=Vector3.Zero;

			//grab view matrix in vector transpose
			vup.X   =view.M12;
			vup.Y   =view.M22;
			vup.Z   =view.M32;
			vleft.X =view.M11;
			vleft.Y =view.M21;
			vleft.Z =view.M31;
			vin.X   =view.M13;
			vin.Y   =view.M23;
			vin.Z   =view.M33;

			mPitch	=45.0f;
			mRoll	=0.0f;

			Vector3	lastPos	=mPosition;

			if(gs.IsConnected)
			{
				mPosition	-=vleft * (gs.ThumbSticks.Left.X * msDelta * mGamePadSensitivity * mSpeed);
				mPosition	+=vin * (gs.ThumbSticks.Left.Y * msDelta * mGamePadSensitivity * mSpeed);
				mPosition.Y	=0.0f;	//zero out the Y

				mYaw	+=gs.ThumbSticks.Right.X * mGamePadSensitivity * msDelta * mTurnSpeed;
			}
			else
			{
				Vector3	moveDelta	=Vector3.Zero;
				if(ks.IsKeyDown(Keys.A))
				{
					moveDelta	-=vleft;
				}
				else if(ks.IsKeyDown(Keys.D))
				{
					moveDelta	+=vleft;
				}

				if(ks.IsKeyDown(Keys.W))
				{
					moveDelta	-=vin;
				}
				else if(ks.IsKeyDown(Keys.S))
				{
					moveDelta	+=vin;
				}

				moveDelta.Y	=0.0f;	//zero out the Y

				if(moveDelta.LengthSquared() > 0.0f)
				{
					//nix the strafe run
					moveDelta.Normalize();

					mPosition	+=moveDelta * (msDelta * 0.05f * mSpeed);
				}
			}

			mDelta	=mPosition - lastPos;
		}


		void UpdateFly(float msDelta, Matrix view, KeyboardState ks, MouseState ms, GamePadState gs)
		{
			Vector3 vup		=Vector3.Zero;
			Vector3 vleft	=Vector3.Zero;
			Vector3 vin		=Vector3.Zero;

			//grab view matrix in vector transpose
			vup.X   =view.M12;
			vup.Y   =view.M22;
			vup.Z   =view.M32;
			vleft.X =view.M11;
			vleft.Y =view.M21;
			vleft.Z =view.M31;
			vin.X   =view.M13;
			vin.Y   =view.M23;
			vin.Z   =view.M33;

			float	speed	=0.0f;
			if(ks.IsKeyDown(Keys.RightShift) || ks.IsKeyDown(Keys.LeftShift))
			{
				speed	=mSpeed * msDelta * 2.0f;
			}
			else
			{
				speed	=mSpeed * msDelta;
			}
			
			if(ks.IsKeyDown(Keys.Left) || ks.IsKeyDown(Keys.A))
			{
				mPosition	+=vleft * speed;
			}
			if(ks.IsKeyDown(Keys.Right) || ks.IsKeyDown(Keys.D))
			{
				mPosition	-=vleft * speed;
			}
			if(ks.IsKeyDown(Keys.Up) || ks.IsKeyDown(Keys.W))
			{
				mPosition	+=vin * speed;
			}
			if(ks.IsKeyDown(Keys.Down) || ks.IsKeyDown(Keys.S))
			{
				mPosition	-=vin * speed;
			}

			if(ms.RightButton == ButtonState.Pressed)
			{
				Vector2	delta	=Vector2.Zero;
				delta.X	=mOriginalMS.X - ms.X;
				delta.Y	=mOriginalMS.Y - ms.Y;

				Mouse.SetPosition(mOriginalMS.X, mOriginalMS.Y);

				mPitch	-=(delta.Y) * msDelta * mMouseSensitivity;
				mYaw	-=(delta.X) * msDelta * mMouseSensitivity;
			}

			if(gs.IsConnected)
			{
				mPitch	+=gs.ThumbSticks.Right.Y * msDelta * 0.25f;
				mYaw	+=gs.ThumbSticks.Right.X * msDelta * 0.25f;

				mPosition	-=vleft * (gs.ThumbSticks.Left.X * msDelta * 0.25f);
				mPosition	+=vin * (gs.ThumbSticks.Left.Y * msDelta * 0.25f);
			}
		}


		void UpdateGroundMovement(float msDelta, Matrix view, KeyboardState ks, MouseState ms, GamePadState gs)
		{
			Vector3 vup		=Vector3.Zero;
			Vector3 vleft	=Vector3.Zero;
			Vector3 vin		=Vector3.Zero;

			//grab view matrix in vector transpose
			vup.X   =view.M12;
			vup.Y   =view.M22;
			vup.Z   =view.M32;
			vleft.X =view.M11;
			vleft.Y =view.M21;
			vleft.Z =view.M31;
			vin.X   =view.M13;
			vin.Y   =view.M23;
			vin.Z   =view.M33;

			Vector3	moveVec	=Vector3.Zero;
			if(ks.IsKeyDown(Keys.Left) || ks.IsKeyDown(Keys.A))
			{
				moveVec	-=vleft;
			}
			if(ks.IsKeyDown(Keys.Right) || ks.IsKeyDown(Keys.D))
			{
				moveVec	+=vleft;
			}
			if(ks.IsKeyDown(Keys.Up) || ks.IsKeyDown(Keys.W))
			{
				moveVec	-=vin;
			}
			if(ks.IsKeyDown(Keys.Down) || ks.IsKeyDown(Keys.S))
			{
				moveVec	+=vin;
			}

			if(ms.RightButton == ButtonState.Pressed)
			{
				Vector2	delta	=Vector2.Zero;
				delta.X	=mOriginalMS.X - ms.X;
				delta.Y	=mOriginalMS.Y - ms.Y;

				Mouse.SetPosition(mOriginalMS.X, mOriginalMS.Y);

				mPitch	-=(delta.Y) * msDelta * mMouseSensitivity;
				mYaw	-=(delta.X) * msDelta * mMouseSensitivity;
			}

			if(gs.IsConnected)
			{
				mPitch	+=gs.ThumbSticks.Right.Y * msDelta * 0.25f;
				mYaw	+=gs.ThumbSticks.Right.X * msDelta * 0.25f;

				moveVec	=vleft * gs.ThumbSticks.Left.X;
				moveVec	-=vin * gs.ThumbSticks.Left.Y;
			}

			//zero out up/down
			moveVec.Y	=0.0f;

			if(moveVec.LengthSquared() > 0.001f)
			{
				moveVec.Normalize();
				mPosition	+=moveVec * msDelta * mSpeed;
			}

			if(mPitch > PitchClamp)
			{
				mPitch	=PitchClamp;
			}
			else if(mPitch < -PitchClamp)
			{
				mPitch	=-PitchClamp;
			}
		}
	}
}
