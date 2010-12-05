﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;


namespace BSPLib
{
	public class GBSPBrush
	{
		public GBSPBrush	mNext;
		public Bounds		mBounds	=new Bounds();
		public UInt32		mSide, mTestSide;
		public MapBrush		mOriginal;

		public List<GBSPSide>	mSides	=new List<GBSPSide>();

		public const UInt32 BSP_CONTENTS_SOLID2			=(1<<0);		// Solid (Visible)
		public const UInt32 BSP_CONTENTS_WINDOW2		=(1<<1);		// Window (Visible)
		public const UInt32 BSP_CONTENTS_EMPTY2			=(1<<2);		// Empty but Visible (water, lava, etc...)

		public const UInt32 BSP_CONTENTS_TRANSLUCENT2	=(1<<3);		// Vis will see through it
		public const UInt32 BSP_CONTENTS_WAVY2			=(1<<4);		// Wavy (Visible)
		public const UInt32 BSP_CONTENTS_DETAIL2		=(1<<5);		// Won't be included in vis oclusion

		public const UInt32 BSP_CONTENTS_CLIP2			=(1<<6);		// Structural but not visible
		public const UInt32 BSP_CONTENTS_HINT2			=(1<<7);		// Primary splitter (Non-Visible)
		public const UInt32 BSP_CONTENTS_AREA2			=(1<<8);		// Area seperator leaf (Non-Visible)

		public const UInt32 BSP_CONTENTS_FLOCKING		=(1<<9);		// flocking flag.  Not really a contents type
		public const UInt32 BSP_CONTENTS_SHEET			=(1<<10);
		public const UInt32 RESERVED3					=(1<<11);
		public const UInt32 RESERVED4					=(1<<12);
		public const UInt32 RESERVED5					=(1<<13);
		public const UInt32 RESERVED6					=(1<<14);
		public const UInt32 RESERVED7					=(1<<15);

		//16-31 reserved for user contents
		public const UInt32 BSP_CONTENTS_USER1			=(1<<16);	//I'm using this for lava
		public const UInt32 BSP_CONTENTS_USER2			=(1<<17);	//slime
		public const UInt32 BSP_CONTENTS_USER3			=(1<<18);	//water
		public const UInt32 BSP_CONTENTS_USER4			=(1<<19);	//mist
		public const UInt32 BSP_CONTENTS_USER5			=(1<<20);	//current_0
		public const UInt32 BSP_CONTENTS_USER6			=(1<<21);	//current_90
		public const UInt32 BSP_CONTENTS_USER7			=(1<<22);	//current_180
		public const UInt32 BSP_CONTENTS_USER8			=(1<<23);	//current_270
		public const UInt32 BSP_CONTENTS_USER9			=(1<<24);	//current_UP
		public const UInt32 BSP_CONTENTS_USER10			=(1<<25);	//current_DOWN
		public const UInt32 BSP_CONTENTS_USER11			=(1<<26);	//ladder
		public const UInt32 BSP_CONTENTS_USER12			=(1<<27);	//trigger
		public const UInt32 BSP_CONTENTS_USER13			=(1<<28);	//nodrop
		public const UInt32 BSP_CONTENTS_USER14			=(1<<29);
		public const UInt32 BSP_CONTENTS_USER15			=(1<<30);
		public const UInt32 BSP_CONTENTS_USER16			=(0x80000000);
		
		//These contents are all solid types
		public const UInt32 BSP_CONTENTS_SOLID_CLIP		=(BSP_CONTENTS_SOLID2 | BSP_CONTENTS_WINDOW2 | BSP_CONTENTS_CLIP2);
		
		//These contents are all visible types
		public const UInt32 BSP_VISIBLE_CONTENTS		=(BSP_CONTENTS_SOLID2 | BSP_CONTENTS_EMPTY2 | BSP_CONTENTS_WINDOW2 | BSP_CONTENTS_SHEET | BSP_CONTENTS_WAVY2);
		
		//These contents define where faces are NOT allowed to merge across
		public const UInt32 BSP_MERGE_SEP_CONTENTS		=(BSP_CONTENTS_WAVY2 | BSP_CONTENTS_HINT2 | BSP_CONTENTS_AREA2);


		public GBSPBrush() { }
		public GBSPBrush(GBSPBrush copyMe)
		{
			mBounds		=new Bounds(copyMe.mBounds);
			mOriginal	=copyMe.mOriginal;
			mSide		=copyMe.mSide;
			mTestSide	=copyMe.mTestSide;

			mSides.Clear();

			foreach(GBSPSide side in copyMe.mSides)
			{
				mSides.Add(new GBSPSide(side));
			}
		}


		public GBSPBrush(MapBrush mb)
		{
			Int32	Vis	=0;

			for(int j=0;j < mb.mOriginalSides.Count;j++)
			{
				if(mb.mOriginalSides[j].mPoly.mVerts.Count > 2)
				{
					Vis++;
				}
			}

			if(Vis == 0)
			{
				return;
			}

			mOriginal	=mb;

			for(int j=0;j < mb.mOriginalSides.Count;j++)
			{
				mSides.Add(new GBSPSide(mb.mOriginalSides[j]));
				if((mb.mOriginalSides[j].mFlags & GBSPSide.SIDE_HINT) != 0)
				{
					mSides[j].mFlags	|=GBSPSide.SIDE_VISIBLE;
				}
			}

			mBounds	=mb.mBounds;

			BoundBrush();

			if(!CheckBrush())
			{
				Map.Print("MakeBSPBrushes:  Bad brush.\n");
				return;
			}
		}


		bool CheckBrush()
		{
			if(mSides.Count < 3)
			{
				return	false;
			}

			if(mBounds.IsMaxExtents())
			{
				return	false;
			}
			return	true;
		}


		void BoundBrush()
		{
			mBounds.Clear();

			for(int i=0;i < mSides.Count;i++)
			{
				mSides[i].AddToBounds(mBounds);
			}
		}


		bool Overlaps(GBSPBrush otherBrush)
		{
			//check bounds first
			if(!mBounds.Overlaps(otherBrush.mBounds))
			{
				return	false;
			}

			for(int i=0;i < mSides.Count;i++)
			{
				for(int j=0;j < otherBrush.mSides.Count;j++)
				{
					if(mSides[i].mPlaneNum == otherBrush.mSides[j].mPlaneNum &&
						mSides[i].mPlaneSide != otherBrush.mSides[j].mPlaneSide)
					{
						return	false;
					}
				}
			}
			return	true;
		}


		bool BrushCanBite(GBSPBrush otherBrush)
		{
			UInt32	c1, c2;

			c1	=mOriginal.mContents;
			c2	=otherBrush.mOriginal.mContents;

			if(((c1 & BSP_CONTENTS_DETAIL2) != 0) &&
				!((c2 & BSP_CONTENTS_DETAIL2) != 0))
			{
				return	false;
			}

			if(((c1|c2) & BSP_CONTENTS_FLOCKING) != 0)
			{
				return	false;
			}

			if((c1 & BSP_CONTENTS_SOLID2) != 0)
			{
				return	true;
			}

			return	false;
		}


		UInt32 MostlyOnSide(GBSPPlane plane)
		{
			UInt32	side	=GBSPPlane.PSIDE_FRONT;
			float	max		=0.0f;

			for(int i=0;i < mSides.Count;i++)
			{
				if(mSides[i].mPoly.mVerts.Count < 3)
				{
					continue;
				}

				for(int j=0;j < mSides[i].mPoly.mVerts.Count;j++)
				{
					float	d	=Vector3.Dot(mSides[i].mPoly.mVerts[j], plane.mNormal) - plane.mDist;
					if(d > max)
					{
						max		=d;
						side	=GBSPPlane.PSIDE_FRONT;
					}
					if(-d > max)
					{
						max		=-d;
						side	=GBSPPlane.PSIDE_BACK;
					}
				}
			}

			return	side;
		}


		static void Split(GBSPBrush Brush, Int32 PNum, sbyte PSide, byte MidFlags, bool Visible, PlanePool pool, out GBSPBrush Front, out GBSPBrush Back)
		{
			Int32		i, j;
			GBSPPoly	p, MidPoly;
			GBSPPlane	Plane, Plane2;
			float		d, FrontD, BackD;
			GBSPPlane	pPlane1;
			GBSPBrush	[]Brushes	=new GBSPBrush[2];

			pPlane1	=pool.mPlanes[PNum];

			Plane		=pPlane1;
			Plane.mType	=GBSPPlane.PLANE_ANY;

			if(PSide != 0)
			{
				Plane.Inverse();
			}

			Front	=Back	=null;

			// Check all points
			FrontD = BackD = 0.0f;

			for(i=0;i < Brush.mSides.Count;i++)
			{
				p	=Brush.mSides[i].mPoly;

				if(p == null)
				{
					continue;
				}

				foreach(Vector3 vert in Brush.mSides[i].mPoly.mVerts)
				{
					d	=pPlane1.DistanceFast(vert);

					if(PSide != 0)
					{
						d	=-d;
					}

					if(d > FrontD)
					{
						FrontD	=d;
					}
					else if(d < BackD)
					{
						BackD	=d;
					}
				}
			}
			
			if(FrontD < 0.1f)
			{
				Back	=new GBSPBrush(Brush);
				return;
			}

			if(BackD > -0.1f)
			{
				Front	=new GBSPBrush(Brush);
				return;
			}

			//create a new poly from the split plane
			p	=new GBSPPoly(Plane);
			if(p == null)
			{
				Map.Print("Could not create poly.\n");
			}
			
			//Clip the poly by all the planes of the brush being split
			for(i=0;i < Brush.mSides.Count && !p.IsTiny();i++)
			{
				Plane2	=pool.mPlanes[Brush.mSides[i].mPlaneNum];
				
				p.ClipPolyEpsilon(0.0f, Plane2, Brush.mSides[i].mPlaneSide == 0);
			}

			if(p.IsTiny())
			{	
				UInt32	Side	=Brush.MostlyOnSide(Plane);
				
				if(Side == GBSPPlane.PSIDE_FRONT)
				{
					Front	=new GBSPBrush(Brush);
				}
				if(Side == GBSPPlane.PSIDE_BACK)
				{
					Back	=new GBSPBrush(Brush);
				}
				return;
			}

			//Store the mid poly
			MidPoly	=p;					

			//Create 2 brushes
			for(i=0;i < 2;i++)
			{
				Brushes[i]	=new GBSPBrush();
				
				if(Brushes[i] == null)
				{
					Map.Print("SplitBrush:  Out of memory for brush.\n");
				}
				
				Brushes[i].mOriginal	=Brush.mOriginal;
			}

			//Split all the current polys of the brush being split, and distribute it to the other 2 brushes
			foreach(GBSPSide pSide in Brush.mSides)
			{
				GBSPPoly	[]Poly	=new GBSPPoly[2];
				
				if(pSide.mPoly == null)
				{
					continue;
				}

				p	=new GBSPPoly(pSide.mPoly);
				if(!p.SplitEpsilon(0.0f, Plane, out Poly[0], out Poly[1], false))
				{
					Map.Print("Error splitting poly...\n");
				}

				for(j=0;j < 2;j++)
				{
					GBSPSide	pDestSide;

					if(Poly[j] == null)
					{
						continue;
					}

					pDestSide	=new GBSPSide(pSide);

					Brushes[j].mSides.Add(pDestSide);
					
					pDestSide.mPoly		= Poly[j];
					pDestSide.mFlags	&=~GBSPSide.SIDE_TESTED;
				}
			}

			for(i=0;i < 2;i++)
			{
				Brushes[i].BoundBrush();

				if(!Brushes[i].CheckBrush())
				{
					Brushes[i]	=null;
				}			
			}

			if(Brushes[0] == null || Brushes[1] == null)
			{				
				if(Brushes[0] == null && Brushes[1] == null)
				{
					Map.Print("Split removed brush\n");
				}
				else
				{
					Map.Print("Split not on both sides\n");
				}
				
				if(Brushes[0] != null)
				{
					Front	=new GBSPBrush(Brush);
				}
				if(Brushes[1] != null)
				{
					Back	=new GBSPBrush(Brush);
				}
				return;
			}

			for(i=0;i < 2;i++)
			{
				GBSPSide	pSide	=new GBSPSide();

				Brushes[i].mSides.Add(pSide);

				pSide.mPlaneNum		=PNum;
				pSide.mPlaneSide	=(sbyte)PSide;

				if(Visible)
				{
					pSide.mFlags	|=GBSPSide.SIDE_VISIBLE;
				}

				pSide.mFlags	&=~GBSPSide.SIDE_TESTED;
				pSide.mFlags	|=MidFlags;
			
				if(i == 0)
				{
					pSide.mPlaneSide	=(pSide.mPlaneSide == 0)? (sbyte)1 : (sbyte)0;

					pSide.mPoly	=new GBSPPoly(MidPoly);
					pSide.mPoly.Reverse();
				}
				else
				{
					//might not need to copy this
					pSide.mPoly	=new GBSPPoly(MidPoly);
				}
			}

			{
				float	v1;
				for(int z=0;z < 2;z++)
				{
					v1	=Brushes[z].Volume(pool);
					if(v1 < 1.0f)
					{
						Brushes[z]	=null;
						//GHook.Printf("Tiny volume after clip\n");
					}
				}
			}

			if(Brushes[0] == null || Brushes[1] == null)
			{
				Map.Print("SplitBrush:  Brush was not split.\n");
			}
			
			Front	=Brushes[0];
			Back	=Brushes[1];
		}


		internal float Volume(PlanePool pool)
		{
			GBSPPoly	p	=null;
			int			i	=0;

			for(i=0;i < mSides.Count;i++)
			{
				if(mSides[i].mPoly.mVerts.Count > 2)
				{
					p	=mSides[i].mPoly;
					break;
				}
			}
			if(p == null)
			{
				return	0.0f;
			}

			Vector3	corner	=p.mVerts[0];

			float	volume	=0.0f;
			for(;i < mSides.Count;i++)
			{
				p	=mSides[i].mPoly;
				if(p == null)
				{
					continue;
				}

				GBSPPlane	plane	=pool.mPlanes[mSides[i].mPlaneNum];

				if(mSides[i].mPlaneSide != 0)
				{
					plane.Inverse();
				}

				float	d		=-(Vector3.Dot(corner, plane.mNormal) - plane.mDist);
				float	area	=p.Area();

				volume	+=d * area;
			}

			volume	/=3.0f;
			return	volume;
		}


		static GBSPBrush Subtract(GBSPBrush a, GBSPBrush b, PlanePool pool)
		{
			GBSPBrush	Outside, Inside;
			GBSPBrush	Front, Back;
			Int32		i;

			Inside	=a;	// Default a being inside b
			Outside	=null;

			//Splitting the inside list against each plane of brush b, only keeping peices that fall on the
			//outside
			for(i=0;i < b.mSides.Count && Inside != null;i++)
			{
				Split(Inside, b.mSides[i].mPlaneNum, b.mSides[i].mPlaneSide, (byte)GBSPSide.SIDE_NODE, false, pool, out Front, out Back);

				//Make sure we don't free a, but free all other fragments
				if(Inside != a)
				{
					Inside	=null;
				}

				//Keep all front sides, and put them in the Outside list
				if(Front != null)
				{	
					Front.mNext	=Outside;
					Outside		=Front;
				}

				Inside	=Back;
			}

			if(Inside == null)
			{
				FreeBrushList(Outside);		
				return	a;	//Nothing on inside list, so cancel all cuts, and return original
			}
			
			Inside	=null;	//Free all inside fragments

			return	Outside;	//Return what was on the outside
		}


		internal static GBSPBrush CSGBrushes(GBSPGlobals gbs, GBSPBrush Head, PlanePool pool)
		{
			GBSPBrush	b1, b2, Next;
			GBSPBrush	Tail;
			GBSPBrush	Keep;
			GBSPBrush	Sub, Sub2;
			Int32		c1, c2;

			if(gbs.Verbose)
			{
				Map.Print("---- CSGBrushes ----\n");
				Map.Print("Num brushes before CSG : " + CountBrushList(Head) + "\n");
			}

			Keep	=null;

		NewList:

			if(Head == null)
			{
				return null;
			}

			for(Tail=Head;Tail.mNext != null;Tail=Tail.mNext);

			for(b1=Head;b1 != null;b1=Next)
			{
				Next = b1.mNext;
				
				for(b2=b1.mNext;b2 != null;b2 = b2.mNext)
				{
					if(!b1.Overlaps(b2))
					{
						continue;
					}

					Sub		=null;
					Sub2	=null;
					c1		=999999;
					c2		=999999;

					if(b2.BrushCanBite(b1))
					{
						Sub	=Subtract(b1, b2, pool);

						if(Sub == b1)
						{
							continue;
						}

						if(Sub == null)
						{
							Head = RemoveBrushList(b1, b1);
							goto NewList;
						}
						c1 = CountBrushList (Sub);
					}

					if(b1.BrushCanBite(b2))
					{
						Sub2	=Subtract(b2, b1, pool);

						if(Sub2 == b2)
						{
							continue;
						}

						if(Sub2 == null)
						{	
							FreeBrushList(Sub);
							Head	=RemoveBrushList(b1, b2);
							goto NewList;
						}
						c2	=CountBrushList(Sub2);
					}

					if(Sub == null && Sub2 == null)
					{
						continue;
					}

					if(c1 > 4 && c2 > 4)
					{
						if(Sub2 != null)
						{
							FreeBrushList(Sub2);
						}
						if(Sub != null)
						{
							FreeBrushList(Sub);
						}
						continue;
					}					

					if(c1 < c2)
					{
						if(Sub2 != null)
						{
							FreeBrushList(Sub2);
						}
						Tail	=AddBrushListToTail(Sub, Tail);
						Head	=RemoveBrushList(b1, b1);
						goto NewList;
					}
					else
					{
						if(Sub != null)
						{
							FreeBrushList(Sub);
						}
						Tail	=AddBrushListToTail(Sub2, Tail);
						Head	=RemoveBrushList(b1, b2);
						goto NewList;
					}
				}

				if(b2 == null)
				{	
					b1.mNext	=Keep;
					Keep		=b1;
				}
			}

			if(gbs.Verbose)
			{
				Map.Print("Num brushes after CSG  : " + CountBrushList(Keep) + "\n");
			}

			return	Keep;
		}


		static GBSPBrush AddBrushListToTail(GBSPBrush List, GBSPBrush Tail)
		{
			GBSPBrush	Walk, Next;

			for (Walk=List;Walk != null;Walk=Next)
			{	// add to end of list
				Next		=Walk.mNext;
				Walk.mNext	=null;
				Tail.mNext	=Walk;
				Tail		=Walk;
			}
			return	Tail;
		}


		static Int32 CountBrushList(GBSPBrush Brushes)
		{
			Int32	c	=0;
			for(;Brushes != null;Brushes=Brushes.mNext)
			{
				c++;
			}
			return	c;
		}


		static GBSPBrush RemoveBrushList(GBSPBrush List, GBSPBrush Remove)
		{
			GBSPBrush	NewList;
			GBSPBrush	Next;

			NewList	=null;

			for(;List != null;List = Next)
			{
				Next	=List.mNext;

				if(List == Remove)
				{
					List	=null;
					continue;
				}

				List.mNext	=NewList;
				NewList		=List;
			}
			return	NewList;
		}


		internal static GBSPSide SelectSplitSide(GBSPGlobals gbs, GBSPBrush Brushes,
			GBSPNode Node, PlanePool pool)
		{
			Int32		Value, BestValue;
			GBSPBrush	Brush, Test;
			GBSPSide	Side, BestSide;
			Int32		i, j, Pass, NumPasses;
			Int32		PNum, PSide;
			UInt32		s;
			Int32		Front, Back, Both, Facing, Splits;
			Int32		BSplits;
			Int32		BestSplits;
			Int32		EpsilonBrush;
			bool		HintSplit	=false;

			BestSide	=null;
			BestValue	=-999999;
			BestSplits	=0;
			NumPasses	=4;
			for(Pass = 0;Pass < NumPasses;Pass++)
			{
				for(Brush = Brushes;Brush != null;Brush=Brush.mNext)
				{
					if(((Pass & 1) != 0)
						&& ((Brush.mOriginal.mContents & BSP_CONTENTS_DETAIL2) == 0))
					{
						continue;
					}
					if(((Pass & 1) == 0)
						&& ((Brush.mOriginal.mContents & BSP_CONTENTS_DETAIL2) != 0))
					{
						continue;
					}
					
					for(i=0;i < Brush.mSides.Count;i++)
					{
						Side	=Brush.mSides[i];

						if(Side.mPoly == null)
						{
							continue;
						}
						if((Side.mFlags & (GBSPSide.SIDE_TESTED | GBSPSide.SIDE_NODE)) != 0)
						{
							continue;
						}
 						if(((Side.mFlags & GBSPSide.SIDE_VISIBLE) == 0) && Pass < 2)
						{
							continue;
						}

						PNum	=Side.mPlaneNum;
						PSide	=Side.mPlaneSide;
						
						Debug.Assert(Node.CheckPlaneAgainstParents(PNum) == true);
												
						Front			=0;
						Back			=0;
						Both			=0;
						Facing			=0;
						Splits			=0;
						EpsilonBrush	=0;

						for(Test=Brushes;Test != null;Test=Test.mNext)
						{
							s	=Test.TestBrushToPlane(PNum, PSide, pool, out BSplits, out HintSplit, ref EpsilonBrush);

							Splits	+=BSplits;

							if(BSplits != 0 && ((s & GBSPPlane.PSIDE_FACING) != 0))
							{
								Map.Print("PSIDE_FACING with splits\n");
							}

							Test.mTestSide	=s;

							if((s & GBSPPlane.PSIDE_FACING) != 0)
							{
								Facing++;
								for(j=0;j < Test.mSides.Count;j++)
								{
									if(Test.mSides[j].mPlaneNum == PNum)
									{
										Test.mSides[j].mFlags	|=GBSPSide.SIDE_TESTED;
									}
								}
							}
							if((s & GBSPPlane.PSIDE_FRONT) != 0)
							{
								Front++;
							}
							if((s & GBSPPlane.PSIDE_BACK) != 0)
							{
								Back++;
							}
							if (s == GBSPPlane.PSIDE_BOTH)
							{
								Both++;
							}
						}

						Value	=5 * Facing - 5 * Splits - Math.Abs(Front - Back);
						
						if(pool.mPlanes[PNum].mType < 3)
						{
							Value	+=5;
						}
						
						Value	-=EpsilonBrush * 1000;	

						if(HintSplit && ((Side.mFlags & GBSPSide.SIDE_HINT) == 0))
						{
							Value	=-999999;
						}

						if(Value > BestValue)
						{
							BestValue	=Value;
							BestSide	=Side;
							BestSplits	=Splits;
							for(Test=Brushes;Test != null;Test=Test.mNext)
							{
								Test.mSide	=Test.mTestSide;
							}
						}
					}
				}

				if(BestSide != null)
				{
					if(Pass > 1)
					{
						gbs.NumNonVisNodes++;
					}
					
					if(Pass > 0)
					{
						Node.mDetail	=true;	//Not needed for vis
						if((BestSide.mFlags & GBSPSide.SIDE_HINT) != 0)
						{
							Map.Print("*** Hint as Detail!!! ***\n");
						}
					}					
					break;
				}
			}

			for(Brush = Brushes;Brush != null;Brush=Brush.mNext)
			{
				for(i=0;i < Brush.mSides.Count;i++)
				{
					Brush.mSides[i].mFlags	&=~GBSPSide.SIDE_TESTED;
				}
			}

			return	BestSide;
		}


		UInt32 TestBrushToPlane(int PlaneNum, int PSide, PlanePool pool, out int NumSplits, out bool HintSplit, ref int EpsilonBrush)
		{
			GBSPPlane	Plane;
			UInt32		s;
			float		d, FrontD, BackD;
			Int32		Front, Back;

			NumSplits	=0;
			HintSplit	=false;

			for(int i=0;i < mSides.Count;i++)
			{
				int	Num	=mSides[i].mPlaneNum;
				
				if(Num == PlaneNum && mSides[i].mPlaneSide == 0)
				{
					return	GBSPPlane.PSIDE_BACK | GBSPPlane.PSIDE_FACING;
				}

				if(Num == PlaneNum && mSides[i].mPlaneSide != 0)
				{
					return	GBSPPlane.PSIDE_FRONT | GBSPPlane.PSIDE_FACING;
				}
			}
			
			//See if it's totally on one side or the other
			Plane	=pool.mPlanes[PlaneNum];

			s	=mBounds.BoxOnPlaneSide(Plane);

			if(s != GBSPPlane.PSIDE_BOTH)
			{
				return	s;
			}
			
			//The brush is split, count the number of splits 
			FrontD	=BackD	=0.0f;

			foreach(GBSPSide pSide in mSides)
			{
				if((pSide.mFlags & GBSPSide.SIDE_NODE) != 0)
				{
					continue;
				}
				if((pSide.mFlags & GBSPSide.SIDE_VISIBLE) == 0)
				{
					continue;
				}
				if(pSide.mPoly.mVerts.Count < 3)
				{
					continue;
				}

				PSide	=pSide.mPlaneSide;
				Front	=Back	=0;

				foreach(Vector3 vert in pSide.mPoly.mVerts)
				{
					d	=Plane.DistanceFast(vert);

					if(d > FrontD)
					{
						FrontD	=d;
					}
					else if(d < BackD)
					{
						BackD	=d;
					}

					if(d > 0.1f)
					{
						Front	=1;
					}
					else if(d < -0.1)
					{
						Back	=1;
					}
				}

				if(Front != 0 && Back != 0)
				{
					NumSplits++;
					if((pSide.mFlags & GBSPSide.SIDE_HINT) != 0)
					{
						HintSplit	=true;
					}
				}
			}

			//Check to see if this split would produce a tiny brush (would result in tiny leafs, bad for vising)
			if((FrontD > 0.0f && FrontD < 1.0f) || (BackD < 0.0f && BackD > -1.0f))
			{
				EpsilonBrush++;
			}

			return	s;
		}


		internal void GetTriangles(List<Vector3> verts, List<uint> indexes, bool bCheckFlags)
		{
			foreach(GBSPSide s in mSides)
			{
				s.GetTriangles(verts, indexes, bCheckFlags);
			}
		}


		internal static void FreeBrushList(GBSPBrush brushes)
		{
			GBSPBrush	next;

			for(;brushes != null;brushes = next)
			{
				next	=brushes.mNext;

				brushes.mSides.Clear();
				brushes.mBounds	=null;
				brushes			=null;
			}
		}


		internal static void SplitBrushList(GBSPBrush Brushes, GBSPNode Node, PlanePool pool,
			out GBSPBrush Front, out GBSPBrush Back)
		{
			GBSPBrush	Brush, NewBrush, NewBrush2, Next;
			GBSPSide	Side;
			UInt32		Sides;
			Int32		i;

			Front = Back = null;

			for(Brush = Brushes;Brush != null;Brush = Next)
			{
				Next	=Brush.mNext;
				Sides	=Brush.mSide;

				if(Sides == GBSPPlane.PSIDE_BOTH)
				{
					Split(Brush, Node.mPlaneNum, 0, (byte)GBSPSide.SIDE_NODE, false, pool, out NewBrush, out NewBrush2);
					if(NewBrush != null)
					{
						NewBrush.mNext	=Front;
						Front			=NewBrush;
					}
					if(NewBrush2 != null)
					{
						NewBrush2.mNext	=Back;
						Back			=NewBrush2;
					}
					continue;
				}

				NewBrush	=new GBSPBrush(Brush);

				if((Sides & GBSPPlane.PSIDE_FACING) != 0)
				{
					for(i=0;i < NewBrush.mSides.Count;i++)
					{
						Side	=NewBrush.mSides[i];
						if(Side.mPlaneNum == Node.mPlaneNum)
						{
							Side.mFlags	|=GBSPSide.SIDE_NODE;
						}
					}
				}

				if((Sides & GBSPPlane.PSIDE_FRONT) != 0)
				{
					NewBrush.mNext	=Front;
					Front			=NewBrush;
					continue;
				}
				if((Sides & GBSPPlane.PSIDE_BACK) != 0)
				{
					NewBrush.mNext	=Back;
					Back			=NewBrush;
					continue;
				}
			}
		}
	}
}