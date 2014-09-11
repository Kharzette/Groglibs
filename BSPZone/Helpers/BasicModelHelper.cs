﻿using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using SharpDX;
using UtilityLib;


namespace BSPZone
{
	internal class BasicModelHelper
	{
		Zone	mZone;

		class ModelStages
		{
			internal int	mCurStage;
			internal bool	mbForward;
			internal bool	mbActive;
			internal bool	mbBlocked;	//smashed into a solid object

			internal List<ModelMoveStage>	mStages	=new List<ModelMoveStage>();
		}

		class ModelMoveStage
		{
			internal int		mModelIndex;
			internal Vector3	mOrigin;
			internal bool		mbForward;
			internal Vector3	mMoveAxis;
			internal float		mMoveAmount;
			internal Vector3	mRotationTarget, mRotationRate;	//xyz
			internal bool		mbRotateToTarget;
			internal float		mStageInterval;
			internal float		mEaseIn, mEaseOut;

//			internal SoundEffectInstance	mSoundForward, mSoundBackward;

//			internal AudioEmitter	mEmitter;

			internal Mover3	mMover		=new Mover3();
			internal Mover3	mRotator	=new Mover3();

			internal void Fire(bool bForward)
			{
				if(mbForward == bForward)
				{
					return;
				}
				mbForward	=bForward;

				if(mbForward)
				{
//					if(mSoundForward != null)
//					{
//						mSoundForward.Play();
//					}

					if(mMover.Done())
					{
						mMover.SetUpMove(mOrigin, mOrigin + mMoveAxis * mMoveAmount,
							mStageInterval, mEaseIn, mEaseOut);
					}
					else
					{
						mMover.SetUpMove(mMover.GetPos(), mOrigin + mMoveAxis * mMoveAmount,
							mStageInterval, mEaseIn, mEaseOut);
					}

					if(mbRotateToTarget)
					{
						if(mRotator.Done())
						{
							mRotator.SetUpMove(Vector3.Zero, mRotationTarget,
								mStageInterval, mEaseIn, mEaseOut);
						}
						else
						{
							mRotator.SetUpMove(mRotator.GetPos(), mRotationTarget,
								mStageInterval, mEaseIn, mEaseOut);
						}
					}
				}
				else
				{
//					if(mSoundBackward != null)
//					{
//						mSoundBackward.Play();
//					}
					if(mMover.Done())
					{
						mMover.SetUpMove(mOrigin + mMoveAxis * mMoveAmount,
							mOrigin, mStageInterval, mEaseIn, mEaseOut);
					}
					else
					{
						mMover.SetUpMove(mMover.GetPos(), mOrigin,
							mStageInterval, mEaseIn, mEaseOut);
					}

					if(mbRotateToTarget)
					{
						if(mRotator.Done())
						{
							mRotator.SetUpMove(mRotationTarget, Vector3.Zero,
								mStageInterval, mEaseIn, mEaseOut);
						}
						else
						{
							mRotator.SetUpMove(mRotationTarget, mRotator.GetPos(),
								mStageInterval, mEaseIn, mEaseOut);
						}
					}
				}
			}


			internal bool Update(int msDelta, Zone z)//, AudioListener lis)
			{
				if(mMover.Done())
				{
					if(mbRotateToTarget && mRotator.Done())
					{
						return	true;
					}
					if(!mbRotateToTarget && mRotationRate == Vector3.Zero)
					{
						return	true;
					}
				}

				if(!mMover.Done())
				{
					mMover.Update(msDelta);

					//do the move
					z.MoveModelTo(mModelIndex, mMover.GetPos());
				}

				//update rotation if any
				if(mbRotateToTarget)
				{
					if(!mRotator.Done())
					{
						Vector3	rotPreUpdate	=mRotator.GetPos();

						mRotator.Update(msDelta);

						Vector3	rotPostUpdate	=mRotator.GetPos();

						z.RotateModelX(mModelIndex, rotPostUpdate.X - rotPreUpdate.X);
						z.RotateModelY(mModelIndex, rotPostUpdate.Y - rotPreUpdate.Y);
						z.RotateModelZ(mModelIndex, rotPostUpdate.Z - rotPreUpdate.Z);
					}
				}
				else
				{
					Vector3	rotAmount	=(mRotationRate / 1000f) * msDelta;

					if(rotAmount != Vector3.Zero)
					{
						z.RotateModelX(mModelIndex, rotAmount.X);
						z.RotateModelY(mModelIndex, rotAmount.Y);
						z.RotateModelZ(mModelIndex, rotAmount.Z);
					}
				}

//				mEmitter.Position	=mMover.GetPos() * Audio.InchWorldScale;

//				Apply3DToSound(mSoundForward, lis, mEmitter);
//				Apply3DToSound(mSoundBackward, lis, mEmitter);

				return	false;
			}
		}

		Dictionary<int, ModelStages>	mModelStages	=new Dictionary<int, ModelStages>();

		bool	mbWiredToFunc;

		
		internal BasicModelHelper(){}


		internal void Initialize(Zone zone, TriggerHelper thelp)//, Audio aud, AudioListener lis)
		{
			mZone	=zone;

			if(!mbWiredToFunc)
			{
				thelp.eFunc	+=OnFunc;

				mbWiredToFunc	=true;
			}

			mModelStages.Clear();

			//grab doors and lifts and such
			List<ZoneEntity>	funcs	=zone.GetEntitiesStartsWith("func_");

			foreach(ZoneEntity ze in funcs)
			{
				int		model;
				Vector3	org;

				if(!ze.GetInt("Model", out model))
				{
					continue;
				}
				if(!ze.GetVectorNoConversion("ModelOrigin", out org))
				{
					continue;
				}

				GetMoveStages(ze, model, org);//, aud, lis);
			}
		}


		void UpdateSingle(int msDelta, int modelIndex)//, AudioListener lis)
		{
			if(!mModelStages.ContainsKey(modelIndex))
			{
				return;
			}

			ModelStages	ms	=mModelStages[modelIndex];
			if(!ms.mbActive)
			{
				return;
			}

			ModelMoveStage	mms	=ms.mStages[ms.mCurStage];

			bool	bDone	=mms.Update(msDelta, mZone);//, lis);

			if(bDone)
			{
				ms.mbBlocked	=false;	//release blocked state if set

				if(ms.mbForward)
				{
					if(ms.mStages.Count > (ms.mCurStage + 1))
					{
						ms.mCurStage++;
						ms.mStages[ms.mCurStage].Fire(ms.mbForward);
					}
					else
					{
						ms.mbActive	=false;
					}
				}
				else
				{
					if((ms.mCurStage - 1) >= 0)
					{
						ms.mCurStage--;
						ms.mStages[ms.mCurStage].Fire(ms.mbForward);
					}
					else
					{
						ms.mbActive	=false;
					}
				}
			}
		}


		internal void Update(int msDelta)//, AudioListener lis)
		{
			foreach(KeyValuePair<int, ModelStages> mss in mModelStages)
			{
				UpdateSingle(msDelta, mss.Key);//, lis);
			}
		}


		internal bool GetState(int modelIndex)
		{
			if(!mModelStages.ContainsKey(modelIndex))
			{
				return	false;
			}

			ModelStages	ms	=mModelStages[modelIndex];

			return	ms.mbForward;
		}


		internal void SetBlocked(int modelIndex)
		{
			if(!mModelStages.ContainsKey(modelIndex))
			{
				return;
			}

			ModelStages	ms	=mModelStages[modelIndex];

			if(ms.mbBlocked)
			{
				return;	//already marked
			}

			ms.mbBlocked	=true;

			//reverse direction
			ms.mbForward	=!ms.mbForward;

			//call fire
			ms.mStages[ms.mCurStage].Fire(ms.mbForward);
		}


		internal void SetState(int modelIndex, bool bOpen)
		{
			if(!mModelStages.ContainsKey(modelIndex))
			{
				return;
			}

			ModelStages	ms	=mModelStages[modelIndex];

			ms.mbActive		=true;
			ms.mbForward	=bOpen;

			ms.mStages[ms.mCurStage].Fire(bOpen);
		}


//		internal static void Apply3DToSound(SoundEffectInstance sei,
//			AudioListener al, AudioEmitter em)
//		{
//			if(sei != null)
//			{
//				sei.Apply3D(al, em);
//			}
//		}


		void GetMoveStages(ZoneEntity ze, int modelIdx, Vector3 org)//, Audio aud, AudioListener lis)
		{
			if(ze == null)
			{
				return;
			}

			string	moveTarg	=ze.GetValue("target");

			if(moveTarg == "")
			{
				return;
			}

			if(moveTarg == null || moveTarg == "")
			{
				return;
			}

			List<ZoneEntity>	targs	=mZone.GetEntitiesByTargetName(moveTarg);

			Debug.Assert(targs.Count < 2);

			if(targs.Count != 1)
			{
				return;
			}

			ZoneEntity	targ	=targs[0];

			if(!mModelStages.ContainsKey(modelIdx))
			{
				ModelStages	stages	=new ModelStages();
				mModelStages.Add(modelIdx, stages);
			}

			ModelMoveStage	mms	=new ModelMoveStage();
			mms.mModelIndex		=modelIdx;
			mms.mOrigin			=org;
//			mms.mEmitter		=new AudioEmitter();

//			string	forward	=targ.GetValue("sound_forward");
//			string	back	=targ.GetValue("sound_backward");

//			mms.mSoundForward	=aud.GetInstance(forward, false);
//			mms.mSoundBackward	=aud.GetInstance(back, false);

//			mms.mEmitter.Position	=org;

//			Apply3DToSound(mms.mSoundForward, lis, mms.mEmitter);
//			Apply3DToSound(mms.mSoundBackward, lis, mms.mEmitter);

			targ.GetVectorNoConversion("rotation_target", out mms.mRotationTarget);
			targ.GetVectorNoConversion("rotation_rate", out mms.mRotationRate);	//degrees per second

			targ.GetBool("rotate_to_target", out mms.mbRotateToTarget);

			targ.GetFloat("move_amount", out mms.mMoveAmount);
			targ.GetFloat("stage_interval", out mms.mStageInterval);
			targ.GetFloat("ease_in", out mms.mEaseIn);
			targ.GetFloat("ease_out", out mms.mEaseOut);
			targ.GetDirectionFromAngles("move_axis", out mms.mMoveAxis);

			mModelStages[modelIdx].mStages.Add(mms);

			//movers work in seconds
			mms.mStageInterval	/=1000f;

			//recurse, offsetting by move amount
			//TODO: rotation amount too
			GetMoveStages(targ, modelIdx, org + (mms.mMoveAmount * mms.mMoveAxis));//, aud, lis);
		}


		void OnFunc(object sender, EventArgs ea)
		{
			ZoneEntity	ze	=sender as ZoneEntity;
			if(ze == null)
			{
				return;
			}

			TriggerHelper.FuncEventArgs	fea	=ea as TriggerHelper.FuncEventArgs;
			if(fea == null)
			{
				return;
			}

			Mobile	mob	=fea.mTCEA.mContext as Mobile;
			if(mob == null)
			{
				return;
			}

			int	modIdx;
			ze.GetInt("Model", out modIdx);

			Vector3	org;
			if(ze.GetVectorNoConversion("ModelOrigin", out org))
			{
				org	=mZone.DropToGround(org, false);
			}

			SetState(modIdx, fea.mbTriggerState);
		}
	}
}
