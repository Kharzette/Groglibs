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
			None, TwinStick, Fly, ThirdPerson, FirstPerson, Platformer
		}

		SteeringMethod	mMethod;

		//movement settings
		float	mSpeed				=0.1f;
		float	mMouseSensitivity	=0.01f;
		float	mGamePadSensitivity	=0.25f;
		float	mTurnSpeed			=3.0f;
		float	mWheelScrollSpeed	=0.04f;
		bool	mbInvertYAxis		=false;
		bool	mbUsePadIfPossible	=true;

		//position info
		Vector3	mPosition, mDelta;
		float	mPitch, mYaw, mRoll;
		float	mZoom	=80f;		//default
		bool	mbMovedThisFrame;	//true if the player gave movement input

		//for mouselook
		MouseState	mOriginalMS;
		int			mLastWheel;
		bool		mbRightClickToTurn;

		//constants
		const float	PitchClamp	=80.0f;


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

		public bool RightClickToTurn
		{
			get { return mbRightClickToTurn; }
			set { mbRightClickToTurn = value; }
		}

		public bool MovedThisFrame
		{
			get { return mbMovedThisFrame; }
			set { mbMovedThisFrame = value; }
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

		public float Zoom
		{
			get { return mZoom; }
			set { mZoom = value; }
		}

		public float MouseSensitivity
		{
			get { return mMouseSensitivity; }
			set { mMouseSensitivity = value; }
		}

		public float GamePadSensitivity
		{
			get { return mGamePadSensitivity; }
			set { mGamePadSensitivity = value; }
		}

		public bool InvertYAxis
		{
			get { return mbInvertYAxis; }
			set { mbInvertYAxis = value; }
		}

		public bool UseGamePadIfPossible
		{
			get { return mbUsePadIfPossible; }
			set { mbUsePadIfPossible = value; }
		}


		public Vector3 Update(int msDelta, Vector3 pos, GameCamera gc,
			KeyboardState ks, MouseState ms, GamePadState gs)
		{
			mbMovedThisFrame	=false;

			mPosition	=pos;
			if(mMethod == SteeringMethod.None)
			{
				return	pos;
			}

			if(mMethod == SteeringMethod.FirstPerson
				|| mMethod == SteeringMethod.ThirdPerson)
			{
				UpdateGroundMovement(msDelta, gc, ks, ms, gs);
			}
			else if(mMethod == SteeringMethod.Fly)
			{
				UpdateFly(msDelta, gc, ks, ms, gs);
			}
			else if(mMethod == SteeringMethod.TwinStick)
			{
				UpdateTwinStick(msDelta, gc, ks, ms, gs);
			}
			else if(mMethod == SteeringMethod.Platformer)
			{
				UpdatePlatformer(msDelta, gc, ks, ms, gs);
			}


			if(!UseGamePadIfPossible || !gs.IsConnected)
			{
				if(ms.ScrollWheelValue != mLastWheel)
				{
					int	swChange	=ms.ScrollWheelValue - mLastWheel;

					mLastWheel	=ms.ScrollWheelValue;

					mZoom	-=(swChange * 0.04f);
					mZoom	=MathHelper.Clamp(mZoom, 0f, 100f);
				}
			}

			return	mPosition;
		}


		void UpdateTwinStick(int msDelta, GameCamera gc, KeyboardState ks, MouseState ms, GamePadState gs)
		{
			Vector3 vup		=gc.Up;
			Vector3 vleft	=gc.Left;
			Vector3 vin		=gc.Forward;

			mPitch	=45.0f;
			mRoll	=0.0f;

			Vector3	lastPos	=mPosition;

			if(gs.IsConnected)
			{
				if(gs.ThumbSticks.Left != Vector2.Zero)
				{
					mbMovedThisFrame	=true;

					mPosition	+=vleft * (gs.ThumbSticks.Left.X * msDelta * mGamePadSensitivity * mSpeed);
					mPosition	-=vin * (gs.ThumbSticks.Left.Y * msDelta * mGamePadSensitivity * mSpeed);
					mPosition.Y	=0.0f;	//zero out the Y
				}
				mYaw	+=gs.ThumbSticks.Right.X * mGamePadSensitivity * msDelta * mTurnSpeed;
			}
			else
			{
				Vector3	moveDelta	=Vector3.Zero;
				if(ks.IsKeyDown(Keys.A))
				{
					mbMovedThisFrame	=true;
					moveDelta			-=vleft;
				}
				else if(ks.IsKeyDown(Keys.D))
				{
					mbMovedThisFrame	=true;
					moveDelta			+=vleft;
				}

				if(ks.IsKeyDown(Keys.W))
				{
					mbMovedThisFrame	=true;
					moveDelta			-=vin;
				}
				else if(ks.IsKeyDown(Keys.S))
				{
					mbMovedThisFrame	=true;
					moveDelta			+=vin;
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


		void UpdateFly(int msDelta, GameCamera gc, KeyboardState ks, MouseState ms, GamePadState gs)
		{
			GetTurn(msDelta, gs, ms);

			Vector3	moveVec;
			GetMove(msDelta, gc, gs, ms, ks, out moveVec);

			if(moveVec.LengthSquared() > 0.001f)
			{
				moveVec.Normalize();
				mPosition	+=moveVec * msDelta * mSpeed;
			}
		}


		void UpdateGroundMovement(int msDelta, GameCamera gc, KeyboardState ks, MouseState ms, GamePadState gs)
		{
			GetTurn(msDelta, gs, ms);

			Vector3	moveVec;
			GetMove(msDelta, gc, gs, ms, ks, out moveVec);

			//zero out up/down
			moveVec.Y	=0.0f;

			if(moveVec.LengthSquared() > 0.001f)
			{
				moveVec.Normalize();
				mPosition	+=moveVec * msDelta * mSpeed;
			}
		}


		void UpdatePlatformer(int msDelta, GameCamera gc, KeyboardState ks, MouseState ms, GamePadState gs)
		{
			Vector3 vup		=Vector3.Up;
			Vector3 vleft	=Vector3.Left;
			Vector3 vin		=Vector3.Forward;

			Vector3	moveVec	=Vector3.Zero;
			if(ks.IsKeyDown(Keys.Left) || ks.IsKeyDown(Keys.A))
			{
				mbMovedThisFrame	=true;
				moveVec				-=vleft;
			}
			if(ks.IsKeyDown(Keys.Right) || ks.IsKeyDown(Keys.D))
			{
				mbMovedThisFrame	=true;
				moveVec				+=vleft;
			}

			if(gs.IsConnected && mbUsePadIfPossible)
			{
				if(gs.ThumbSticks.Left != Vector2.Zero)
				{
					mbMovedThisFrame	=true;
					moveVec				=vleft * gs.ThumbSticks.Left.X;
				}
			}

			//zero out up/down
			moveVec.Y	=0.0f;

			if(moveVec.LengthSquared() > 0.001f)
			{
				moveVec.Normalize();
				mPosition	+=moveVec * msDelta * mSpeed;
			}
		}


		void GetMove(int msDelta, GameCamera gc,
			GamePadState gs, MouseState ms, KeyboardState ks,
			out Vector3 moveVec)
		{
			Vector3 vup		=gc.Up;
			Vector3 vleft	=gc.Left;
			Vector3 vin		=gc.Forward;

			moveVec	=Vector3.Zero;
			if(ks.IsKeyDown(Keys.Left) || ks.IsKeyDown(Keys.A))
			{
				mbMovedThisFrame	=true;
				moveVec				-=vleft;
			}
			if(ks.IsKeyDown(Keys.Right) || ks.IsKeyDown(Keys.D))
			{
				mbMovedThisFrame	=true;
				moveVec				+=vleft;
			}
			if(ks.IsKeyDown(Keys.Up) || ks.IsKeyDown(Keys.W))
			{
				mbMovedThisFrame	=true;
				moveVec				-=vin;
			}
			if(ks.IsKeyDown(Keys.Down) || ks.IsKeyDown(Keys.S))
			{
				mbMovedThisFrame	=true;
				moveVec				+=vin;
			}

			if(gs.IsConnected && mbUsePadIfPossible)
			{
				if(gs.ThumbSticks.Left != Vector2.Zero)
				{
					mbMovedThisFrame	=true;

					moveVec	=vleft * gs.ThumbSticks.Left.X;
					moveVec	-=vin * gs.ThumbSticks.Left.Y;
				}
			}
		}


		void GetTurn(int msDelta, GamePadState gs, MouseState ms)
		{
			if(gs.IsConnected && mbUsePadIfPossible)
			{
				float	pitchAmount	=gs.ThumbSticks.Right.Y * msDelta * mGamePadSensitivity;

				if(mbInvertYAxis)
				{
					mPitch	+=pitchAmount;
				}
				else
				{
					mPitch	-=pitchAmount;
				}

				mYaw	+=gs.ThumbSticks.Right.X * msDelta * mGamePadSensitivity;
			}
			else if((ms.RightButton == ButtonState.Pressed && mbRightClickToTurn)
				|| !mbRightClickToTurn)
			{
				Vector2	delta	=Vector2.Zero;
				delta.X	=mOriginalMS.X - ms.X;
				delta.Y	=mOriginalMS.Y - ms.Y;

				Mouse.SetPosition(mOriginalMS.X, mOriginalMS.Y);

				if(mbInvertYAxis)
				{
					mPitch	+=(delta.Y) * msDelta * mMouseSensitivity;
				}
				else
				{
					mPitch	-=(delta.Y) * msDelta * mMouseSensitivity;
				}

				mYaw	-=(delta.X) * msDelta * mMouseSensitivity;
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
