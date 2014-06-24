﻿using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms;
using SharpDX.RawInput;
using SharpDX.Multimedia;
using SharpDX.XInput;

using System.Runtime.InteropServices;

using FormsKeys	=System.Windows.Forms.Keys;


namespace InputLib
{
	public class Input
	{
		//need these for translating keys
		[DllImport("user32.dll")] static extern int MapVirtualKey(uint uCode, uint uMapType);
		[DllImport("user32", SetLastError=true, CharSet=CharSet.Unicode)]
			static extern int GetKeyNameTextW(uint lParam, StringBuilder lpString, int nSize);

		public class InputAction
		{
			public float	mMultiplier;	//time or analog amount
			public Enum		mAction;		//user specified thing?

			internal InputAction(long timeHeld, Enum act)
			{
				mMultiplier	=(float)timeHeld / ((float)Stopwatch.Frequency / 1000f);
				mAction		=act;
			}
		}

		internal class KeyHeldInfo
		{
			internal IntPtr	mDevice;
			internal long	mInitialPressTime;
			internal long	mTimeHeld;
			internal Keys	mKey;
			internal int	mCode;


			internal KeyHeldInfo(){}

			internal KeyHeldInfo(KeyHeldInfo copyMe)
			{
				mDevice				=copyMe.mDevice;
				mInitialPressTime	=copyMe.mInitialPressTime;
				mTimeHeld			=copyMe.mTimeHeld;
				mKey				=copyMe.mKey;
				mCode				=copyMe.mCode;
			}
		}

		internal class MouseMovementInfo
		{
			internal IntPtr	mDevice;
			internal int	mXMove, mYMove;
		}

		public enum MoveAxis
		{
			MouseXAxis			=1000,
			MouseYAxis			=1002,
			GamePadLeftXAxis	=1003,
			GamePadLeftYAxis	=1004,
			GamePadRightXAxis	=1005,
			GamePadRightYAxis	=1006,
			GamePadLeftTrigger	=1007,
			GamePadRightTrigger	=1008
		};

		public enum VariousButtons
		{
			LeftMouseButton			=2000,
			RightMouseButton		=2001,
			MiddleMouseButton		=2002,
			MouseButton4			=2003,
			MouseButton5			=2004,
			MouseButton6			=2005,
			MouseButton7			=2006,
			MouseButton8			=2007,
			MouseButton9			=2008,
			MouseButton10			=2009,
			GamePadA				=2100,
			GamePadB				=2101,
			GamePadX				=2102,
			GamePadY				=2103,
			GamePadLeftAnalog		=2104,
			GamePadRightAnalog		=2105,
			GamePadLeftShoulder		=2106,
			GamePadRightShoulder	=2107,
			GamePadStart			=2108,
			GamePadBack				=2109,
			GamePadDPadUp			=2110,
			GamePadDPadDown			=2111,
			GamePadDPadLeft			=2112,
			GamePadDPadRight		=2113
		}

		//this stuff piles up between updates
		//via the handlers
		Dictionary<int, KeyHeldInfo>	mKeysHeld	=new Dictionary<int, KeyHeldInfo>();
		Dictionary<int, KeyHeldInfo>	mKeysUp		=new Dictionary<int, KeyHeldInfo>();
		List<MouseMovementInfo>			mMouseMoves	=new List<MouseMovementInfo>();

		//mappings to controllers / keys / mice / whatever
		Dictionary<UInt32, ActionMapping>	mActionMap	=new Dictionary<uint, ActionMapping>();

		//active toggles
		List<int>	mActiveToggles	=new List<int>();

		//fired actions (for once per press activations)
		List<int>	mOnceActives	=new List<int>();

		//for a press & release action, to ensure the combo was down
		List<UInt32>	mWasHeld	=new List<UInt32>();

		//buffer for winapi key name
		StringBuilder	mNameBuf	=new StringBuilder(260);

		//list of all possible modifiers for checking held key combos
		List<UInt32>	mModCombos	=new List<UInt32>();

		//sticks/pads
		Controller	[]mXControllers;

		//keep track of buttons held on controllers
		List<int>	[]mXButtonsHeld	=new List<int>[4];

		System.Windows.Forms.KeysConverter	mKeyConverter	=new KeysConverter();

		//mouse pos at last update
		int		mLastMouseX, mLastMouseY;
		bool	mbResetPos;
		long	mLastUpdateTime;

		const float	DeadZone	=5000f;


		public Input()
		{
			List<DeviceInfo>	devs	=Device.GetDevices();

			Device.RegisterDevice(UsagePage.Generic, UsageId.GenericKeyboard, DeviceFlags.None);
			Device.RegisterDevice(UsagePage.Generic, UsageId.GenericMouse, DeviceFlags.None);
			Device.RegisterDevice(UsagePage.Generic, UsageId.GenericGamepad, DeviceFlags.None);

			mbResetPos		=true;
			mLastUpdateTime	=Stopwatch.GetTimestamp();

			Device.KeyboardInput	+=OnKeyInput;
			Device.MouseInput		+=OnMouseInput;

			mXControllers	=new Controller[4];

			mXControllers[0]	=new Controller(UserIndex.One);
			mXControllers[1]	=new Controller(UserIndex.Two);
			mXControllers[2]	=new Controller(UserIndex.Three);
			mXControllers[3]	=new Controller(UserIndex.Four);

			for(int i=0;i < 4 ;i++)
			{
				mXButtonsHeld[i]	=new List<int>();
			}

			//modifier combos pre shifted
			mModCombos.Add((UInt32)Modifiers.None << 29);
			mModCombos.Add((UInt32)Modifiers.ShiftHeld << 29);
			mModCombos.Add((UInt32)(Modifiers.ControlHeld) << 29);
			mModCombos.Add((UInt32)(Modifiers.ShiftHeld | Modifiers.ControlHeld) << 29);
			mModCombos.Add((UInt32)(Modifiers.AltHeld) << 29);
			mModCombos.Add((UInt32)(Modifiers.ShiftHeld | Modifiers.AltHeld) << 29);
			mModCombos.Add((UInt32)(Modifiers.ControlHeld | Modifiers.AltHeld) << 29);
			mModCombos.Add((UInt32)(Modifiers.ShiftHeld | Modifiers.ControlHeld | Modifiers.AltHeld) << 29);
		}


		public void FreeAll()
		{
			Device.KeyboardInput	-=OnKeyInput;
			Device.MouseInput		-=OnMouseInput;
		}


		void OnMouseInput(object sender, MouseInputEventArgs miea)
		{
			if(mbResetPos)
			{
				mLastMouseX	=miea.X;
				mLastMouseY	=miea.Y;
				mbResetPos	=false;
			}

			if(miea.X != 0 || miea.Y != 0)
			{
				MouseMovementInfo	mmi	=new MouseMovementInfo();
				mmi.mDevice				=miea.Device;
				mmi.mXMove				=miea.X;// - mLastMouseX;
				mmi.mYMove				=miea.Y;// - mLastMouseY;

				mMouseMoves.Add(mmi);
			}

			List<int>	buttonsDown	=new List<int>();
			List<int>	buttonsUp	=new List<int>();
			if(miea.ButtonFlags != MouseButtonFlags.None)
			{
				if((miea.ButtonFlags & MouseButtonFlags.LeftButtonDown) != 0)
				{
					buttonsDown.Add((int)VariousButtons.LeftMouseButton);
				}
				if((miea.ButtonFlags & MouseButtonFlags.RightButtonDown) != 0)
				{
					buttonsDown.Add((int)VariousButtons.RightMouseButton);
				}
				if((miea.ButtonFlags & MouseButtonFlags.MiddleButtonDown) != 0)
				{
					buttonsDown.Add((int)VariousButtons.MiddleMouseButton);
				}
				if((miea.ButtonFlags & MouseButtonFlags.Button4Down) != 0)
				{
					buttonsDown.Add((int)VariousButtons.MouseButton4);
				}
				if((miea.ButtonFlags & MouseButtonFlags.Button5Down) != 0)
				{
					buttonsDown.Add((int)VariousButtons.MouseButton5);
				}

				if((miea.ButtonFlags & MouseButtonFlags.LeftButtonUp) != 0)
				{
					buttonsUp.Add((int)VariousButtons.LeftMouseButton);
				}
				if((miea.ButtonFlags & MouseButtonFlags.RightButtonUp) != 0)
				{
					buttonsUp.Add((int)VariousButtons.RightMouseButton);
				}
				if((miea.ButtonFlags & MouseButtonFlags.MiddleButtonUp) != 0)
				{
					buttonsUp.Add((int)VariousButtons.MiddleMouseButton);
				}
				if((miea.ButtonFlags & MouseButtonFlags.Button4Up) != 0)
				{
					buttonsUp.Add((int)VariousButtons.MouseButton4);
				}
				if((miea.ButtonFlags & MouseButtonFlags.Button5Up) != 0)
				{
					buttonsUp.Add((int)VariousButtons.MouseButton5);
				}
			}

			long	ts	=Stopwatch.GetTimestamp();
			foreach(int down in buttonsDown)
			{
				AddHeldKey(down, Keys.NoName, ts, false, miea.Device);
			}
			foreach(int up in buttonsUp)
			{
				AddHeldKey(up, Keys.NoName, ts, true, miea.Device);
			}

			if(!mbResetPos)
			{
				mLastMouseX	=miea.X;
				mLastMouseY	=miea.Y;
			}
		}


		void AddHeldKey(int code, Keys key, long ts, bool bUp, IntPtr dev)
		{
			if(mKeysHeld.ContainsKey(code))
			{
				KeyHeldInfo	kh	=mKeysHeld[code];
				kh.mTimeHeld	=ts - kh.mInitialPressTime;
				if(bUp)
				{
					if(mKeysUp.ContainsKey(code))
					{
						mKeysUp[code].mTimeHeld	=kh.mTimeHeld;
					}
					else
					{
						mKeysUp.Add(code, new KeyHeldInfo(kh));
					}
					mKeysHeld.Remove(code);
				}
			}
			else
			{
				if(bUp)
				{
					//not really sure how this happens
					return;
				}
				KeyHeldInfo	kh			=new KeyHeldInfo();
				kh.mDevice				=dev;
				kh.mInitialPressTime	=ts;
				kh.mTimeHeld			=0;
				kh.mKey					=key;
				kh.mCode				=code;

				mKeysHeld.Add(code, kh);
			}
		}


		public string GetKeyName(int makeCode, ScanCodeFlags scf)
		{
			uint	e0Thing	=0;
			
			if(((int)scf & (int)ScanCodeFlags.E0) != 0)
			{
				e0Thing	=(1 << 24);
			}

			uint	bigKey	=((uint)makeCode << 16) | e0Thing;

			int ret	=GetKeyNameTextW(bigKey, mNameBuf, 260);

			if(ret != 0)
			{
				return	mNameBuf.ToString();
			}
			return	"Unknown";
		}


		void OnKeyInput(object sender, EventArgs ea)
		{
			KeyboardInputEventArgs	kiea	=ea as KeyboardInputEventArgs;
			if(kiea == null)
			{
				return;
			}

			if((int)kiea.Key == 255)
			{
				return;	//fake key
			}

			long	ts	=Stopwatch.GetTimestamp();

			AddHeldKey(kiea.MakeCode, kiea.Key, ts,
				kiea.State == KeyState.KeyUp, kiea.Device);
		}


		void Update()
		{
			long	ts	=Stopwatch.GetTimestamp();

			foreach(KeyValuePair<int, KeyHeldInfo> keys in mKeysHeld)
			{
				keys.Value.mTimeHeld	=ts - keys.Value.mInitialPressTime;
			}
		}


		public List<InputAction> GetAction()
		{
			Update();

			List<InputAction>	ret	=ComputeActions();

			mLastUpdateTime	=Stopwatch.GetTimestamp();

			return	ret;
		}


		UInt32 KeyPlusMod(int keyCode, Modifiers mod)
		{
			return	(UInt32)(keyCode & 0x1FFFFFFF) | ((UInt32)mod << 29);
		}


		public void MapAction(Enum action, ActionTypes mode,
			Modifiers mod, int keyCode)
		{
			Debug.Assert(mode != ActionTypes.Toggle);

			UInt32	code	=KeyPlusMod(keyCode, mod);

			if(mActionMap.ContainsKey(code))
			{
				//overwrite existing?
				mActionMap[code].mAction		=action;
				mActionMap[code].mActionType	=mode;
				mActionMap[code].mModifier		=mod;
			}
			else
			{
				ActionMapping	amap	=new ActionMapping();

				amap.mAction		=action;
				amap.mActionType	=mode;
				amap.mKeyCode		=keyCode;
				amap.mModifier		=mod;

				mActionMap.Add(code, amap);
			}
		}


		public void MapAction(Enum action, ActionTypes mode,
			Modifiers mod, VariousButtons button)
		{
			MapAction(action, mode, mod, (int)button);
		}


		public void MapAction(Enum Action, ActionTypes mode,
			Modifiers mod, FormsKeys key)
		{
			int	keyCode	=MapVirtualKey((uint)key, 0);

			MapAction(Action, mode, mod, keyCode);
		}


		public void MapToggleAction(Enum action, Enum actionOff, Modifiers mod, int keyCode)
		{
			UInt32	code	=KeyPlusMod(keyCode, mod);

			if(mActionMap.ContainsKey(code))
			{
				//overwrite existing?
				mActionMap[code].mAction		=action;
				mActionMap[code].mModifier		=mod;
				mActionMap[code].mActionOff		=actionOff;
				mActionMap[code].mActionType	=ActionTypes.Toggle;
			}
			else
			{
				ActionMapping	amap	=new ActionMapping();

				amap.mAction		=action;
				amap.mModifier		=mod;
				amap.mActionOff		=actionOff;
				amap.mActionType	=ActionTypes.Toggle;
				amap.mKeyCode		=keyCode;

				mActionMap.Add(code, amap);
			}
		}


		public void MapToggleAction(Enum action, Enum actionOff,
			Modifiers mod, VariousButtons button)
		{
			MapToggleAction(action, actionOff, mod, (int)button);
		}


		public void MapToggleAction(Enum action, Enum actionOff,
			Modifiers mod, FormsKeys key)
		{
			int	keyCode	=MapVirtualKey((uint)key, 0);

			MapToggleAction(action, actionOff, mod, keyCode);
		}


		public void UnMapAxisAction(Enum action, MoveAxis ma)
		{
			UInt32	moveCode	=(UInt32)ma;

			if(mActionMap.ContainsKey(moveCode))
			{
				mActionMap.Remove(moveCode);
			}
		}


		public void MapAxisAction(Enum action, MoveAxis ma)
		{
			UInt32	moveCode	=(UInt32)ma;

			if(mActionMap.ContainsKey(moveCode))
			{
				//overwrite existing?
				mActionMap[moveCode].mAction		=action;
				mActionMap[moveCode].mActionType	=ActionTypes.AnalogAmount;
			}
			else
			{
				ActionMapping	amap	=new ActionMapping();

				amap.mAction		=action;
				amap.mActionType	=ActionTypes.AnalogAmount;
				amap.mKeyCode		=(int)moveCode;

				mActionMap.Add(moveCode, amap);
			}
		}


		bool IsModifierHeld(UInt32 mod)
		{
			//test shift/ctrl/alt
			bool	bShiftHeld	=false;
			bool	bCtrlHeld	=false;
			bool	bAltHeld	=false;

			foreach(KeyValuePair<int, KeyHeldInfo> held in mKeysHeld)
			{
				FormsKeys	key	=(FormsKeys)MapVirtualKey((uint)held.Key, 1);
				if(key == FormsKeys.ShiftKey)
				{
					bShiftHeld	=true;
				}
				else if(key == FormsKeys.ControlKey)
				{
					bCtrlHeld	=true;
				}
				else if(key == FormsKeys.Menu)
				{
					bAltHeld	=true;
				}
			}

			if(mod == (UInt32)Modifiers.None)
			{
				//none wants no modifier keys held
				return	(!bShiftHeld && !bCtrlHeld && !bAltHeld);
			}

			bool	bRet	=true;

			if((mod & ((UInt32)Modifiers.ShiftHeld << 29)) != 0)
			{
				bRet	&=bShiftHeld;
			}

			if((mod & ((UInt32)Modifiers.ControlHeld << 29)) != 0)
			{
				bRet	&=bCtrlHeld;
			}

			if((mod & ((UInt32)Modifiers.AltHeld << 29)) != 0)
			{
				bRet	&=bAltHeld;
			}

			return	bRet;
		}


		List<InputAction> ComputeActions()
		{
			List<InputAction>	acts	=new List<InputAction>();

			long	ts	=Stopwatch.GetTimestamp();

			for(int i=0;i < 4;i++)
			{
				if(!mXControllers[i].IsConnected)
				{
					continue;
				}

				State	state	=mXControllers[i].GetState();

				if(mActionMap.ContainsKey((int)MoveAxis.GamePadLeftXAxis))
				{
					if(state.Gamepad.LeftThumbX < -Gamepad.LeftThumbDeadZone
						|| state.Gamepad.LeftThumbX > Gamepad.LeftThumbDeadZone)
					{
						ActionMapping	map	=mActionMap[(int)MoveAxis.GamePadLeftXAxis];
						InputAction	ma	=new InputAction(
							(long)(0.05f * (float)state.Gamepad.LeftThumbX),
							map.mAction);
						acts.Add(ma);
					}
				}

				if(mActionMap.ContainsKey((int)MoveAxis.GamePadLeftYAxis))
				{
					if(state.Gamepad.LeftThumbY < -Gamepad.LeftThumbDeadZone
						|| state.Gamepad.LeftThumbY > Gamepad.LeftThumbDeadZone)
					{
						ActionMapping	map	=mActionMap[(int)MoveAxis.GamePadLeftYAxis];
						InputAction	ma	=new InputAction(
							(long)(-0.05f * (float)state.Gamepad.LeftThumbY),
							map.mAction);
						acts.Add(ma);
					}
				}

				if(mActionMap.ContainsKey((int)MoveAxis.GamePadRightXAxis))
				{
					if(state.Gamepad.RightThumbX < -Gamepad.RightThumbDeadZone
						|| state.Gamepad.RightThumbX > Gamepad.RightThumbDeadZone)
					{
						ActionMapping	map	=mActionMap[(int)MoveAxis.GamePadRightXAxis];
						InputAction	ma	=new InputAction(
							(long)(0.05f * (float)state.Gamepad.RightThumbX),
							map.mAction);
						acts.Add(ma);
					}
				}

				if(mActionMap.ContainsKey((int)MoveAxis.GamePadRightYAxis))
				{
					if(state.Gamepad.RightThumbY < -Gamepad.RightThumbDeadZone
						|| state.Gamepad.RightThumbY > Gamepad.RightThumbDeadZone)
					{
						ActionMapping	map	=mActionMap[(int)MoveAxis.GamePadRightYAxis];
						InputAction	ma	=new InputAction(
							(long)(-0.05f * (float)state.Gamepad.RightThumbY),
							map.mAction);
						acts.Add(ma);
					}
				}

				if(mActionMap.ContainsKey((int)MoveAxis.GamePadLeftTrigger))
				{
					if(state.Gamepad.LeftTrigger > Gamepad.TriggerThreshold)
					{
						ActionMapping	map	=mActionMap[(int)MoveAxis.GamePadLeftTrigger];
						InputAction	ma	=new InputAction(
							(long)(0.5f * (float)state.Gamepad.LeftTrigger),
							map.mAction);
						acts.Add(ma);
					}
				}

				if(mActionMap.ContainsKey((int)MoveAxis.GamePadRightTrigger))
				{
					if(state.Gamepad.RightTrigger > Gamepad.TriggerThreshold)
					{
						ActionMapping	map	=mActionMap[(int)MoveAxis.GamePadRightTrigger];
						InputAction	ma	=new InputAction(
							(long)(0.5f * (float)state.Gamepad.RightTrigger),
							map.mAction);
						acts.Add(ma);
					}
				}

				List<int>	toNuke	=new List<int>();

				//check for up events
				foreach(int vb in mXButtonsHeld[i])
				{
					if(CheckButtonUp(state, ts, i, (VariousButtons)vb))
					{
						toNuke.Add(vb);
					}
				}

				foreach(int nuke in toNuke)
				{
					mXButtonsHeld[i].Remove(nuke);
				}

				ProcessXButton(state, ts, i, VariousButtons.GamePadA);
				ProcessXButton(state, ts, i, VariousButtons.GamePadB);
				ProcessXButton(state, ts, i, VariousButtons.GamePadX);
				ProcessXButton(state, ts, i, VariousButtons.GamePadY);
				ProcessXButton(state, ts, i, VariousButtons.GamePadBack);
				ProcessXButton(state, ts, i, VariousButtons.GamePadDPadDown);
				ProcessXButton(state, ts, i, VariousButtons.GamePadDPadLeft);
				ProcessXButton(state, ts, i, VariousButtons.GamePadDPadRight);
				ProcessXButton(state, ts, i, VariousButtons.GamePadDPadUp);
				ProcessXButton(state, ts, i, VariousButtons.GamePadLeftShoulder);
				ProcessXButton(state, ts, i, VariousButtons.GamePadRightShoulder);
				ProcessXButton(state, ts, i, VariousButtons.GamePadLeftAnalog);
				ProcessXButton(state, ts, i, VariousButtons.GamePadRightAnalog);
				ProcessXButton(state, ts, i, VariousButtons.GamePadStart);
			}

			foreach(KeyValuePair<int, KeyHeldInfo> heldKey in mKeysHeld)
			{
				foreach(UInt32 combo in mModCombos)
				{
					UInt32	modKey	=combo | (UInt32)(heldKey.Key & 0x1FFFFFFF);

					if(mActionMap.ContainsKey(modKey) && IsModifierHeld(combo))
					{
						KeyHeldInfo	khi	=heldKey.Value;

						//make sure at least some time has passed
						if(khi.mInitialPressTime == ts || khi.mTimeHeld == 0)
						{
							continue;
						}

						ActionMapping	map	=mActionMap[modKey];

						if(map.mActionType == ActionTypes.ContinuousHold)
						{
							InputAction	act	=new InputAction(heldKey.Value.mTimeHeld, map.mAction);
							acts.Add(act);

							//reset time
							heldKey.Value.mInitialPressTime	=ts;
							heldKey.Value.mTimeHeld			=0;
						}
						else if(map.mActionType == ActionTypes.PressAndRelease)
						{
							if(!mWasHeld.Contains(modKey))
							{
								mWasHeld.Add(modKey);
							}
						}
						else if(map.mActionType == ActionTypes.Toggle)
						{
							if(!mActiveToggles.Contains(heldKey.Key))
							{
								//toggle on
								InputAction	act	=new InputAction(heldKey.Value.mTimeHeld, map.mAction);
								acts.Add(act);

								mActiveToggles.Add(heldKey.Key);
							}
						}
						else if(map.mActionType == ActionTypes.ActivateOnce)
						{
							if(!mOnceActives.Contains(heldKey.Key))
							{
								//not yet fired
								InputAction	act	=new InputAction(heldKey.Value.mTimeHeld, map.mAction);
								acts.Add(act);

								mOnceActives.Add(heldKey.Key);
							}
						}
						else
						{
							Debug.Assert(false);
						}
					}
				}
			}

			foreach(KeyValuePair<int, KeyHeldInfo> heldKey in mKeysUp)
			{
				foreach(UInt32 combo in mModCombos)
				{
					UInt32	modKey	=combo | (UInt32)(heldKey.Key & 0x1FFFFFFF);

					if(mActionMap.ContainsKey(modKey))
					{
						KeyHeldInfo	khi	=heldKey.Value;

						//make sure at least some time has passed
						if(khi.mInitialPressTime == ts || khi.mTimeHeld == 0)
						{
							continue;
						}

						ActionMapping	map	=mActionMap[modKey];

						if(map.mActionType == ActionTypes.ContinuousHold)
						{
							//nothing to do here
						}
						else if(map.mActionType == ActionTypes.PressAndRelease)
						{
							if(mWasHeld.Contains(modKey))
							{
								//release action
								InputAction	act	=new InputAction(heldKey.Value.mTimeHeld, map.mAction);
								acts.Add(act);
								mWasHeld.Remove(modKey);
							}
						}
						else if(map.mActionType == ActionTypes.Toggle)
						{
							if(mActiveToggles.Contains(heldKey.Key))
							{
								//toggle off
								InputAction	act	=new InputAction(heldKey.Value.mTimeHeld, map.mActionOff);
								acts.Add(act);

								mActiveToggles.Remove(heldKey.Key);
							}
						}
						else if(map.mActionType == ActionTypes.ActivateOnce)
						{
							if(mOnceActives.Contains(heldKey.Key))
							{
								mOnceActives.Remove(heldKey.Key);
							}
						}
						else
						{
							Debug.Assert(false);
						}
					}
				}
			}

			foreach(MouseMovementInfo mmi in mMouseMoves)
			{
				if(mmi.mXMove != 0)
				{
					if(mActionMap.ContainsKey((int)MoveAxis.MouseXAxis))
					{
						ActionMapping	map	=mActionMap[(int)MoveAxis.MouseXAxis];

						InputAction	ma	=new InputAction(mmi.mXMove * 300, map.mAction);

						acts.Add(ma);
					}
				}

				if(mmi.mYMove != 0)
				{
					if(mActionMap.ContainsKey((int)MoveAxis.MouseYAxis))
					{
						ActionMapping	map	=mActionMap[(int)MoveAxis.MouseYAxis];

						InputAction	ma	=new InputAction(mmi.mYMove * 300, map.mAction);

						acts.Add(ma);
					}
				}
			}

			mKeysUp.Clear();
			mMouseMoves.Clear();

			return	acts;
		}


		GamepadButtonFlags	VariousToGPBF(VariousButtons vb)
		{
			switch(vb)
			{
				case	VariousButtons.GamePadA:
					return	GamepadButtonFlags.A;
				case	VariousButtons.GamePadB:
					return	GamepadButtonFlags.B;
				case	VariousButtons.GamePadX:
					return	GamepadButtonFlags.X;
				case	VariousButtons.GamePadY:
					return	GamepadButtonFlags.Y;
				case	VariousButtons.GamePadBack:
					return	GamepadButtonFlags.Back;
				case	VariousButtons.GamePadDPadDown:
					return	GamepadButtonFlags.DPadDown;
				case	VariousButtons.GamePadDPadLeft:
					return	GamepadButtonFlags.DPadLeft;
				case	VariousButtons.GamePadDPadRight:
					return	GamepadButtonFlags.DPadRight;
				case	VariousButtons.GamePadDPadUp:
					return	GamepadButtonFlags.DPadUp;
				case	VariousButtons.GamePadLeftAnalog:
					return	GamepadButtonFlags.LeftThumb;
				case	VariousButtons.GamePadLeftShoulder:
					return	GamepadButtonFlags.LeftShoulder;
				case	VariousButtons.GamePadRightAnalog:
					return	GamepadButtonFlags.RightThumb;
				case	VariousButtons.GamePadRightShoulder:
					return	GamepadButtonFlags.RightShoulder;
				case	VariousButtons.GamePadStart:
					return	GamepadButtonFlags.Start;
			}
			return	GamepadButtonFlags.None;
		}


		bool CheckButtonUp(State state, long ts, int index, VariousButtons vb)
		{
			GamepadButtonFlags	gpbf	=VariousToGPBF(vb);

			if(!state.Gamepad.Buttons.HasFlag(gpbf))
			{
				AddHeldKey((int)vb, Keys.NoName, ts, true, IntPtr.Zero);
				return	true;
			}
			return	false;
		}


		void ProcessXButton(State state, long ts, int index, VariousButtons button)
		{
			if(state.Gamepad.Buttons.HasFlag(VariousToGPBF(button)))
			{
				AddHeldKey((int)button, Keys.NoName, ts, false, IntPtr.Zero);
				if(!mXButtonsHeld[index].Contains((int)button))
				{
					mXButtonsHeld[index].Add((int)button);
				}
			}
		}
	}
}
