﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;
using BSPCore;


namespace BSPVis
{
	class VISPStack
	{
		internal byte		[]mVisBits	=new byte[MAX_TEMP_PORTALS/8];
		internal GBSPPoly	mSource;
		internal GBSPPoly	mPass;
		internal VISPortal	mPortal;
		internal VISLeaf	mLeaf;
		internal VISPStack	mNext;
		internal GBSPPlane	mPortalPlane;

		public const int	MAX_TEMP_PORTALS	=25000;


		internal void FreeAll(VisPools vp, ClipPools cp)
		{
			Free(vp, cp);

			vp.mStacks.FlagFreeItem(this);
		}

		internal void Free(VisPools vp, ClipPools cp)
		{
			if(mSource != null)
			{
				if(mSource.mVerts != null)
				{
					cp.FreeVerts(mSource.mVerts);
				}
				vp.mPolys.FlagFreeItem(mSource);
				mSource	=null;
			}

			if(mPass != null)
			{
				if(mPass.mVerts != null)
				{
					cp.FreeVerts(mPass.mVerts);
				}
				vp.mPolys.FlagFreeItem(mPass);
				mPass	=null;
			}
		}
	}


	internal class VISPortal
	{
		internal GBSPPlane	mPlane;
		internal Vector3	mCenter;
		internal float		mRadius;
		internal GBSPPoly	mPoly;

		internal byte	[]mPortalFront;
		internal byte	[]mPortalFlood;
		internal byte	[]mPortalVis;

		internal Int32	mPortNum;		//index into portal array or portal num for vis
		internal Int32	mLeaf;
		internal Int32	mMightSee;
		internal Int32	mCanSee;
		internal bool	mbDone;


		internal void Read(BinaryReader br, List<int> indexes)
		{
			mPlane.Read(br);
			mCenter.X	=br.ReadSingle();
			mCenter.Y	=br.ReadSingle();
			mCenter.Z	=br.ReadSingle();
			mRadius		=br.ReadSingle();
			mPoly		=new GBSPPoly(0);
			mPoly.Read(br);

			mPortalFront	=UtilityLib.FileUtil.ReadByteArray(br);
			mPortalFlood	=UtilityLib.FileUtil.ReadByteArray(br);
			mPortalVis		=UtilityLib.FileUtil.ReadByteArray(br);

			mPortNum	=br.ReadInt32();
			mLeaf		=br.ReadInt32();
			mMightSee	=br.ReadInt32();
			mCanSee		=br.ReadInt32();
			mbDone		=br.ReadBoolean();
		}


		internal void Write(BinaryWriter bw)
		{
			mPlane.Write(bw);
			bw.Write(mCenter.X);
			bw.Write(mCenter.Y);
			bw.Write(mCenter.Z);
			bw.Write(mRadius);
			mPoly.Write(bw);

			UtilityLib.FileUtil.WriteArray(mPortalFront, bw);
			UtilityLib.FileUtil.WriteArray(mPortalFlood, bw);
			UtilityLib.FileUtil.WriteArray(mPortalVis, bw);

			bw.Write(mPortNum);
			bw.Write(mLeaf);
			bw.Write(mMightSee);
			bw.Write(mCanSee);
			bw.Write(mbDone);
		}


		internal void WriteVisBits(BinaryWriter bw)
		{
			UtilityLib.FileUtil.WriteArray(mPortalVis, bw);
		}


		internal void ReadVisBits(BinaryReader br)
		{
			mPortalVis	=UtilityLib.FileUtil.ReadByteArray(br);
		}


		internal void CalcPortalInfo()
		{
			mCenter	=mPoly.Center();
			mRadius	=mPoly.Radius();
		}


		static internal bool CollectBits(List<VISPortal> ports, byte []portBits)
		{
			//'OR' all portals that this portal can see into one list
			foreach(VISPortal p in ports)
			{
				if(p.mPortalVis != null)
				{
					//Try to use final vis info first
					for(int k=0;k < portBits.Length;k++)
					{
						portBits[k]	|=p.mPortalVis[k];
					}
				}
				else if(p.mPortalFront != null)
				{
					for(int k=0;k < portBits.Length;k++)
					{
						portBits[k]	|=p.mPortalFront[k];
					}
				}
				else
				{
					CoreEvents.Print("No VisInfo for portal.\n");
					return	false;
				}

				p.mPortalFlood	=null;
				p.mPortalFront	=null;
				p.mPortalVis	=null;
			}
			return	true;
		}


		internal bool CanSeePortal(VISPortal port2)
		{
			if(!mPoly.AnyPartBehind(port2.mPlane))
			{
				//No points of Portal1 behind Portal2, can't possibly see
				return	false;
			}
			if(!port2.mPoly.AnyPartInFront(mPlane))
			{
				//No points of Portal2 in front of Portal1, can't possibly see
				return	false;
			}
			return	true;
		}

		/*
		internal void FloodPortalsFast_r(VISPortal destPortal,
			bool []portSeen, VISLeaf []visLeafs,
			int srcLeaf, ref int mightSee)
		{
			Int32	portNum	=destPortal.mPortNum;
			
			if(portSeen[portNum])
			{
				return;
			}

			portSeen[portNum]	=true;

			//Add the portal that we are Flooding into, to the original portals visbits
			Int32	leafNum	=destPortal.mLeaf;
			
			byte	Bit	=(byte)(portNum & 7);
			Bit	=(byte)(1 << Bit);

			if((mVisBits[portNum >> 3] & Bit) == 0)
			{
				mVisBits[portNum>>3]	|=(byte)Bit;
				mMightSee++;
				visLeafs[srcLeaf].mMightSee++;
				mightSee++;
			}

			VISLeaf	leaf	=visLeafs[leafNum];

			//Now, try and Flood into the leafs that this portal touches
			for(VISPortal port=leaf.mPortals;port != null;port=port.mNext)
			{
				//If SrcPortal can see this Portal, flood into it...
				if(CanSeePortal(port))
				{
					FloodPortalsFast_r(port, portSeen, visLeafs, srcLeaf, ref mightSee);
				}
			}
		}*/


		static unsafe UInt32 AndTogetherDWords(byte []src1, byte []src2, byte []src3, byte []dest)
		{
			int		len	=src2.Length;
			UInt32	ret	=0;

			fixed(byte *pSrc1 = src1, pSrc2 = src2, pSrc3 = src3, pDest = dest)
			{
				byte	*pS1	=pSrc1;
				byte	*pS2	=pSrc2;
				byte	*pS3	=pSrc3;
				byte	*pD		=pDest;

				for(int i=0;i < len / 4;i++)
				{
					UInt32	anded=*((UInt32 *)pS1) & *((UInt32 *)pS2);

					*((UInt32 *)pD)	=anded;

					ret	|=anded &~ *((UInt32 *)pS3);

					pS1	+=4;
					pS2	+=4;
					pS3	+=4;
					pD	+=4;
				}

				//get leftover
				for(int i=0;i < len % 4;i++)
				{
					UInt32	anded	=*((UInt32 *)pSrc1) & *((UInt32 *)pSrc2);

					*pD	=(byte)anded;

					ret	|=(byte)(anded &~ *((UInt32 *)pS3));

					pS1++;
					pS2++;
					pS3++;
					pD++;
				}
			}
			return	ret;
		}


		internal static unsafe UInt64 AndTogetherQWords(byte []src1, byte []src2, byte []src3, byte []dest)
		{
			int		len	=src2.Length;
			UInt64	ret	=0;

			fixed(byte *pSrc1 = src1, pSrc2 = src2, pSrc3 = src3, pDest = dest)
			{
				byte	*pS1	=pSrc1;
				byte	*pS2	=pSrc2;
				byte	*pS3	=pSrc3;
				byte	*pD		=pDest;

				for(int i=0;i < len / 8;i++)
				{
					UInt64	anded=*((UInt64 *)pS1) & *((UInt64 *)pS2);

					*((UInt64 *)pD)	=anded;

					ret	|=anded &~ *((UInt64 *)pS3);

					pS1	+=8;
					pS2	+=8;
					pS3	+=8;
					pD	+=8;
				}

				//get leftover
				for(int i=0;i < len % 8;i++)
				{
					UInt32	anded	=*((UInt32 *)pSrc1) & *((UInt32 *)pSrc2);

					*pD	=(byte)anded;

					ret	|=(byte)(anded &~ *((UInt32 *)pS3));

					pS1++;
					pS2++;
					pS3++;
					pD++;
				}
			}
			return	ret;
		}

		/*
		internal bool FloodPortalsSlow_r(VISPortal destPort, VISPStack prevStack, VisPools vPools)
		{
			Int32	portNum	=destPort.mPortNum;

			Interlocked.Increment(ref vPools.mIterations);

			//Add the portal that we are Flooding into, to the original portals visbits
			byte	Bit	=(byte)(portNum & 7);
			Bit	=(byte)(1 << Bit);

			if((mFinalVisBits[portNum >> 3] & Bit) == 0)
			{
				mFinalVisBits[portNum>>3] |= Bit;
				mCanSee++;
				vPools.mVisLeafs[mLeaf].mCanSee++;
				vPools.mCanSee++;
			}

			//Get the leaf that this portal looks into, and flood from there
			Int32		leafNum	=destPort.mLeaf;
			VISLeaf		leaf	=vPools.mVisLeafs[leafNum];
			VISPStack	stack	=null;

			// Now, try and Flood into the leafs that this portal touches
			for(VISPortal port=leaf.mPortals;port != null;port=port.mNext)
			{
				portNum	=port.mPortNum;
				Bit		=(byte)(1<<(portNum&7));

				//GHook.Printf("PrevStack VisBits:  %i\n", PrevStack.mVisBits[PNum>>3]);

				//If might see could'nt see it, then don't worry about it
				if((prevStack.mVisBits[portNum>>3] & Bit) == 0)
				{
					continue;	// Can't possibly see it
				}

				if(stack == null)
				{
					stack	=vPools.mStacks.GetFreeItem();
				}

				//If the portal can't see anything we haven't allready seen, skip it
				UInt64	more	=0;
				if(port.mbDone)
				{
					for(int j=0;j < mPortalVis.Length;j++)
					{
						//there is no & for bytes, can you believe that?
						uint	prevBit		=(uint)prevStack.mVisBits[j];
						uint	portBit		=(uint)port.mPortalVis[j];
						uint	bothBit		=prevBit & portBit;
						stack.mVisBits[j]	=(byte)bothBit;

						prevBit	=stack.mVisBits[j];
						portBit	=mPortalVis[j];

						more	|=prevBit &~ portBit;
					}
				}
				else
				{
					//qwords and dwords seem about the same performancewise
					more	=AndTogetherQWords(prevStack.mVisBits, port.mPortalFront, mPortalVis, stack.mVisBits);
				}
				
				if(more == 0 && ((mPortalVis[portNum>>3] & Bit) != 0))
				{
					//Can't see anything new
					continue;
				}

				//Setup Source/Pass
				stack.mPass			=vPools.mPolys.GetFreeItem();
				stack.mPass.mVerts	=vPools.mClipPools.DupeVerts(port.mPoly.mVerts);

				//Cut away portion of pass portal we can't see through
				if(!stack.mPass.ClipPoly(mPlane, false, vPools.mClipPools))
				{
					stack.FreeAll(vPools, vPools.mClipPools);
					return	false;
				}
				if(stack.mPass.VertCount() < 3)
				{
					stack.Free(vPools, vPools.mClipPools);
					continue;
				}

				stack.mSource			=vPools.mPolys.GetFreeItem();
				stack.mSource.mVerts	=vPools.mClipPools.DupeVerts(prevStack.mSource.mVerts);

				if(!stack.mSource.ClipPoly(port.mPlane, true, vPools.mClipPools))
				{
					stack.FreeAll(vPools, vPools.mClipPools);
					return	false;
				}
				if(stack.mSource.VertCount() < 3)
				{
					stack.Free(vPools, vPools.mClipPools);
					continue;
				}

				//If we don't have a PrevStack.mPass, then we don't have enough to look through.
				//This portal can only be blocked by VisBits (Above test)...
				if(prevStack.mPass == null)
				{
					if(!FloodPortalsSlow_r(port, stack, vPools))
					{
						stack.FreeAll(vPools, vPools.mClipPools);
						return	false;
					}
					stack.Free(vPools, vPools.mClipPools);
					continue;
				}

				if(!stack.mPass.SeperatorClip(stack.mSource, prevStack.mPass, false, vPools.mClipPools))
				{
					stack.FreeAll(vPools, vPools.mClipPools);
					return	false;
				}
				if(stack.mPass == null || stack.mPass.VertCount() < 3)
				{
					stack.Free(vPools, vPools.mClipPools);
					continue;
				}

				if(!stack.mPass.SeperatorClip(prevStack.mPass, stack.mSource, true, vPools.mClipPools))
				{
					stack.FreeAll(vPools, vPools.mClipPools);
					return	false;
				}
				if(stack.mPass == null || stack.mPass.VertCount() < 3)
				{
					stack.Free(vPools, vPools.mClipPools);
					continue;
				}

				//Flood into it...
				if(!FloodPortalsSlow_r(port, stack, vPools))
				{
					stack.FreeAll(vPools, vPools.mClipPools);
					return	false;
				}

				stack.Free(vPools, vPools.mClipPools);
			}

			if(stack != null)
			{
				stack.FreeAll(vPools, vPools.mClipPools);
			}

			return	true;
		}*/
	}


	class VisLeafComparer : IComparer<VISLeaf>
	{
		public int Compare(VISLeaf x, VISLeaf y)
		{
			if(x.mMightSee == y.mMightSee)
			{
				return	0;
			}
			if(x.mMightSee < y.mMightSee)
			{
				return	-1;
			}
			return	1;
		}
	}


	internal class VisPortalComparer : IComparer<VISPortal>
	{
		public int Compare(VISPortal x, VISPortal y)
		{
			if(x.mMightSee == y.mMightSee)
			{
				return	0;
			}
			if(x.mMightSee < y.mMightSee)
			{
				return	-1;
			}
			return	1;
		}
	}


	internal class VISLeaf
	{
		internal List<VISPortal>	mPortals	=new List<VISPortal>();
		internal Int32				mMightSee;
		internal Int32				mCanSee;


		internal void Write(BinaryWriter bw)
		{
			bw.Write(mPortals.Count);
			foreach(VISPortal vp in mPortals)
			{
				bw.Write(vp.mPortNum);
			}
			bw.Write(mMightSee);
			bw.Write(mCanSee);
		}


		internal void Read(BinaryReader br, VISPortal[] ports)
		{
			Int32	numPorts	=br.ReadInt32();
			for(int i=0;i < numPorts;i++)
			{
				int	idx	=br.ReadInt32();
				mPortals.Add(ports[idx]);
			}
			mMightSee	=br.ReadInt32();
			mCanSee		=br.ReadInt32();
		}
	}
}
