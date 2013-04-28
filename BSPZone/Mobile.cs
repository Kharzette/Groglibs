﻿using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UtilityLib;


namespace BSPZone
{
	public class Mobile
	{
		public enum LocomotionState
		{
			Idle, Walk, WalkBack, WalkLeft, WalkRight
		}

		//zone to collide against
		Zone	mZone;

		//trigger helper
		TriggerHelper	mTHelper;

		//model index standing on, -1 for midair or sliding down
		int	mModelOn;

		//true if there is solid footing underneath
		bool	mbOnGround;

		//true if this object can be pushed by models
		bool	mbPushable;

		//position and momentum
		Vector3	mPosition;
		Vector3	mVelocity;

		//collision box, sized for this mobile
		BoundingBox	mBox;

		//offset from the boundingbox center to the eye position
		Vector3		mEyeHeight;

		//camera stuff if needed
		BoundingBox	mCamBox;

		//constants
		const float MidAirMoveScale	=0.03f;
		const float	JumpVelocity	=1.5f;
		const float	Friction		=0.6f;
		const float	MinCamDist		=10f;


		public Mobile(float boxWidth, float boxHeight, float eyeHeight, bool bPushable, TriggerHelper th)
		{
			mBox			=Misc.MakeBox(boxWidth, boxHeight);
			mEyeHeight		=Vector3.UnitY * (eyeHeight + mBox.Min.Y);
			mbPushable		=bPushable;
			mTHelper		=th;

			//small box for camera collision
			mCamBox	=Misc.MakeBox(4f, 4f);
		}


		public void SetZone(Zone z)
		{
			mZone	=z;
			mZone.RegisterPushable(this, mBox, mPosition, mModelOn);
		}


		//for initial start pos and teleports
		public void SetPosition(Vector3 pos)
		{
			mPosition	=pos;
		}


		public BoundingBox GetTransformedBound()
		{
			BoundingBox	ret	=mBox;

			ret.Min	+=mPosition;
			ret.Max	+=mPosition;

			return	ret;
		}


		public void Jump()
		{
			if(mbOnGround)
			{
				mVelocity	+=Vector3.UnitY * JumpVelocity;
			}
		}


		public LocomotionState DetermineLocoState(Vector3 moveDelta, Vector3 camForward)
		{
			LocomotionState	ls	=LocomotionState.Idle;

			if(moveDelta.LengthSquared() > 0.001f)
			{
				//need a leveled out forward direction
				Vector3	dir		=camForward;
				Vector3	side	=Vector3.Cross(dir, Vector3.Up);
				dir				=Vector3.Cross(side, Vector3.Up);

				//check the direction moving vs axis
				moveDelta.Normalize();
				float	forwardDot	=Vector3.Dot(dir, moveDelta);
				float	leftDot		=Vector3.Dot(side, moveDelta);

				if(Math.Abs(forwardDot) > Math.Abs(leftDot))
				{
					if(forwardDot < 0f)
					{
						ls	=LocomotionState.Walk;
					}
					else
					{
						ls	=LocomotionState.WalkBack;
					}
				}
				else
				{
					if(leftDot < 0f)
					{
						ls	=LocomotionState.WalkLeft;
					}
					else
					{
						ls	=LocomotionState.WalkRight;
					}
				}
			}

			return	ls;
		}


		//returns true if any actual movement
		public bool Orient(PlayerSteering ps, Vector3 pos, Vector3 camPos, Vector3 camForward,
			out Vector3 mobForward, out Vector3 mobCamPos, out bool bFirstPerson)
		{
			Matrix	orientation	=
				Matrix.CreateRotationY(MathHelper.ToRadians(ps.Yaw)) *
				Matrix.CreateRotationX(MathHelper.ToRadians(ps.Pitch)) *
				Matrix.CreateRotationZ(MathHelper.ToRadians(ps.Roll));

			//grab transpose forward
			Vector3	forward;

			forward.X	=orientation.M13;
			forward.Y	=orientation.M23;
			forward.Z	=orientation.M33;

			//camera positions are always negated
			camPos	=-camPos;

			//for the third person camera, back the position out
			//along the updated forward vector
			mobCamPos	=camPos + (forward * ps.Zoom);

			Vector3		impacto		=Vector3.Zero;
			ZonePlane	planeHit	=ZonePlane.Blank;
			if(mZone.Trace_WorldCollisionBBox(mCamBox, 0, camPos, mobCamPos, ref impacto, ref planeHit))
			{
				mobCamPos	=impacto;
			}

			Vector3	camRay	=mobCamPos - camPos;
			float	len		=camRay.Length();
				
			//if really short, just use first person
			if(len < MinCamDist)
			{
				mobCamPos		=camPos;
				bFirstPerson	=true;
			}
			else
			{
				bFirstPerson	=false;
			}

			Vector3	delt	=ps.Position - pos;
			if(delt.LengthSquared() > 0.001f)
			{
				//need a leveled out forward direction
				Vector3	dir		=camForward;
				Vector3	side	=Vector3.Cross(dir, Vector3.Up);
				dir				=Vector3.Cross(side, Vector3.Up);

				mobForward	=dir;

				return	true;
			}

			mobForward	=Vector3.Zero;	//don't use

			return	false;
		}


		public void Move(Vector3 endPos, int msDelta,
			bool bAffectVelocity, bool bFly, bool bTriggerCheck,
			out Vector3 retPos, out Vector3 camPos)
		{
			retPos	=Vector3.Zero;
			camPos	=Vector3.Zero;

			if(mZone == null)
			{
				return;
			}

			Vector3	moveDelta	=endPos - mPosition;

			if(bFly)
			{
				retPos	=mPosition	=endPos;
				camPos	=-mPosition;
				if(mbPushable)
				{
					mZone.UpdatePushable(this, mPosition, mModelOn);
				}
				return;
			}

			//if not on the ground, limit midair movement
			if(!mbOnGround && bAffectVelocity)
			{
				moveDelta.X	*=MidAirMoveScale;
				moveDelta.Z	*=MidAirMoveScale;
				mVelocity.Y	-=((9.8f / 1000.0f) * msDelta);	//gravity
			}

			//get ideal final position
			if(bAffectVelocity)
			{
				endPos	=mPosition + mVelocity + moveDelta;
			}
			else
			{
				endPos	=mPosition + moveDelta;
			}

			//move it through the bsp
			bool	bUsedStairs	=false;
			if(mZone.BipedMoveBox(mBox, mPosition, endPos, mbOnGround, out endPos, out bUsedStairs, ref mModelOn))
			{
				mbOnGround	=true;

				//on ground, friction velocity
				if(bAffectVelocity)
				{
					mVelocity	=endPos - mPosition;
					mVelocity	*=Friction;

					//clamp really small velocities
					Mathery.TinyToZero(ref mVelocity);

					//prevent stairsteps from launching the velocity
					if(bUsedStairs)
					{
						mVelocity.Y	=0.0f;
					}
				}
			}
			else
			{
				if(bAffectVelocity)
				{
					mVelocity	=endPos - mPosition;
				}
				mbOnGround	=false;
			}

			retPos	=endPos;

			//pop up to eye height, and negate
			camPos	=-(endPos + mEyeHeight);

			//do a trigger check if requested
			if(bTriggerCheck)
			{
				mTHelper.CheckPlayer(mBox, mPosition, endPos, msDelta);
			}

			mPosition	=endPos;
			if(mbPushable)
			{
				mZone.UpdatePushable(this, mPosition, mModelOn);
			}
		}


		public bool IsOnGround()
		{
			return	mbOnGround;
		}


		public bool TryMoveTo(Vector3 tryPos)
		{
			if(!mbOnGround)
			{
				return	false;
			}

			int			modelHit	=0;
			Vector3		impacto		=Vector3.Zero;
			ZonePlane	planeHit	=ZonePlane.Blank;

			bool	bHit	=mZone.Trace_All(mBox, mPosition,
				tryPos, ref modelHit, ref impacto, ref planeHit);

			return	!bHit;
		}
	}
}
