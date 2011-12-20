﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using UtilityLib;


namespace BSPZone
{
	class RayTrace
	{
		internal Vector3		mOriginalStart, mOriginalEnd;
		internal Vector3		mIntersection;
		internal Int32			mLeaf;
		internal float			mBestDist;
		internal ZonePlane		mBestPlane;
		internal float			mRatio;
		internal bool			mbHitSet, mbLeafHit;
		internal BoundingBox	mRayBox, mMoveBox;

		internal RayTrace()
		{
			mIntersection	=Vector3.Zero;
			mBestPlane		=new ZonePlane();
			mBestDist		=99999.0f;
			mbHitSet		=false;
		}
	}


	public partial class Zone
	{
		//this was written by me back in the genesis days, not well tested
		bool CapsuleIntersect(RayTrace trace, Vector3 start, Vector3 end, float radius, Int32 node)
		{
			if(node < 0)
			{
				Int32	leaf	=-(node + 1);

				trace.mLeaf	=leaf;

				if((mZoneLeafs[leaf].mContents
					& Contents.BSP_CONTENTS_SOLID2) != 0)
				{
					return	true;	//Ray collided with solid space
				}
				else 
				{
					return	false;	//Ray collided with empty space
				}
			}
			ZoneNode	n	=mZoneNodes[node];
			ZonePlane	p	=mZonePlanes[n.mPlaneNum];

			float	frontDist	=p.DistanceFast(start);
			float	backDist	=p.DistanceFast(end);

			if(frontDist >= radius && backDist >= radius)
			{
				return	CapsuleIntersect(trace, start, end, radius, n.mFront);
			}
			if(frontDist < -radius && backDist < -radius)
			{
				return	CapsuleIntersect(trace, start, end, radius, n.mBack);
			}

			//bias the split towards the front
			if(frontDist >= radius || backDist >= radius)
			{
				//split biased to the front
				float	frontFront	=frontDist + radius;
				float	frontBack	=backDist + radius;
				Int32	sideFront	=(frontFront < 0)? 1 : 0;
				float	distFront	=frontFront / (frontFront - frontBack);

				Vector3	frontSplit	=start + distFront * (end - start);

				return	CapsuleIntersect(trace, start, frontSplit, radius,
					(frontFront < 0)? n.mBack : n.mFront);
			}
			else
			{
				//treat as if on the back side
				return	CapsuleIntersect(trace, start, end, radius, n.mBack);
			}
		}


		bool SphereIntersect(Vector3 pnt, float radius, Int32 node,
			ref bool hitLeaf, ref Int32 leafHit, ref Int32 nodeHit)
		{
			if(node < 0)
			{
				Int32	leaf	=-(node + 1);

				leafHit	=leaf;

				if((mZoneLeafs[leaf].mContents & Contents.BSP_CONTENTS_SOLID2) != 0)
				{
					return	true;	//Ray collided with solid space
				}
				else 
				{
					return	false;	//Ray collided with empty space
				}
			}
			ZoneNode	n	=mZoneNodes[node];
			ZonePlane	p	=mZonePlanes[n.mPlaneNum];

			float	dist	=p.DistanceFast(pnt);

			if(dist >= -radius && dist < radius)
			{
				//sphere overlaps plane
				bool	ret	=SphereIntersect(pnt, radius, n.mFront,
								ref hitLeaf, ref leafHit, ref nodeHit);
				if(ret)
				{
					return	true;
				}
				return	SphereIntersect(pnt, radius, n.mBack,
							ref hitLeaf, ref leafHit, ref nodeHit);
			}
			else if(dist >= -radius)
			{
				return(SphereIntersect(pnt, radius, n.mFront,
					ref hitLeaf, ref leafHit, ref nodeHit));
			}
			else if(dist < radius)
			{
				return(SphereIntersect(pnt, radius, n.mBack,
					ref hitLeaf, ref leafHit, ref nodeHit));
			}
			return	false;
		}


		bool RayIntersect(Vector3 start, Vector3 end, Int32 node,
			ref Vector3 intersectionPoint, ref bool hitLeaf,
			ref Int32 leafHit, ref Int32 nodeHit)
		{
			float	Fd, Bd, dist;
			Vector3	I;

			if(node < 0)						
			{
				Int32	leaf	=-(node+1);

				leafHit	=leaf;

				if((mZoneLeafs[leaf].mContents
					& Contents.BSP_CONTENTS_SOLID2) != 0)
				{
					return	true;	//Ray collided with solid space
				}
				else 
				{
					return	false;	//Ray collided with empty space
				}
			}
			ZoneNode	n	=mZoneNodes[node];
			ZonePlane	p	=mZonePlanes[n.mPlaneNum];

			Fd	=p.DistanceFast(start);
			Bd	=p.DistanceFast(end);

			if(Fd >= -1 && Bd >= -1)
			{
				return(RayIntersect(start, end, n.mFront,
					ref intersectionPoint, ref hitLeaf, ref leafHit, ref nodeHit));
			}
			if(Fd < 1 && Bd < 1)
			{
				return(RayIntersect(start, end, n.mBack,
					ref intersectionPoint, ref hitLeaf, ref leafHit, ref nodeHit));
			}

			dist	=Fd / (Fd - Bd);

			I	=start + dist * (end - start);

			//Work our way to the front, from the back side.  As soon as there
			//is no more collisions, we can assume that we have the front portion of the
			//ray that is in empty space.  Once we find this, and see that the back half is in
			//solid space, then we found the front intersection point...
			if(RayIntersect(start, I,
				(Fd < 0)? n.mBack : n.mFront,
				ref intersectionPoint, ref hitLeaf, ref leafHit, ref nodeHit))
			{
				return	true;
			}
			else if(RayIntersect(I, end,
				(Fd < 0)? n.mFront : n.mBack,
				ref intersectionPoint, ref hitLeaf, ref leafHit, ref nodeHit))
			{
				if(!hitLeaf)
				{
					intersectionPoint	=I;
					hitLeaf				=true;
					nodeHit				=node;
				}
				return	true;
			}
			return	false;
		}


		public bool Trace_WorldCollisionBBox(BoundingBox boxBounds,
			Vector3 start, Vector3 end, ref Vector3 I, ref ZonePlane P)
		{
			RayTrace	trace		=new RayTrace();

			//set boxes
			trace.mRayBox	=boxBounds;
			trace.mMoveBox	=Trace_GetMoveBox(boxBounds, start, end);

			ZoneModel	worldModel	=mZoneModels[0];

			if(!trace.mMoveBox.Intersects(worldModel.mBounds))
			{
				return	false;
			}

			trace.mOriginalStart	=start;
			trace.mOriginalEnd		=end;
			FindClosestLeafIntersection_r(trace, worldModel.mRootNode);

			if(trace.mbLeafHit)
			{
				I	=trace.mIntersection;
				P	=trace.mBestPlane;
				return	true;
			}
			return	false;
		}


		public bool Trace_WorldCollisionCapsule(Vector3 start, Vector3 end,
			float radius, ref Vector3 impacto, ref ZonePlane hitPlane)
		{
			RayTrace	trace	=new RayTrace();

			//set boxes
			trace.mRayBox	=new BoundingBox();

			trace.mRayBox.Min	=-Vector3.One * radius;
			trace.mRayBox.Max	=Vector3.One * radius;
			trace.mRayBox.Min.Y	=0.0f;
			trace.mRayBox.Max.Y	*=2.0f;

			trace.mMoveBox	=Trace_GetMoveBox(trace.mRayBox, start, end);

			ZoneModel	worldModel	=mZoneModels[0];

			if(!trace.mMoveBox.Intersects(worldModel.mBounds))
			{
				return	false;
			}

			trace.mOriginalStart	=start;
			trace.mOriginalEnd		=end;
			FindClosestLeafIntersection_r(trace, worldModel.mRootNode);

			if(trace.mbLeafHit)
			{
				impacto		=trace.mIntersection;
				hitPlane	=trace.mBestPlane;
				return	true;
			}
			return	false;
		}


		void FindClosestLeafIntersection_r(RayTrace trace, Int32 node)
		{
			if(node < 0)
			{
				Int32	leaf		=-(node + 1);
				UInt32	contents	=mZoneLeafs[leaf].mContents;

				if((contents & BSPZone.Contents.BSP_CONTENTS_SOLID_CLIP) == 0)
				{
					return;		// Only solid leafs contain side info...
				}

				trace.mbHitSet	=false;
				
				if(mZoneLeafs[leaf].mNumSides == 0)
				{
					return;
				}

				IntersectLeafSides_r(trace, trace.mOriginalStart, trace.mOriginalEnd, leaf, 0, 1);
				return;
			}

			UInt32	side	=Trace_BoxOnPlaneSide(trace.mMoveBox, mZonePlanes[mZoneNodes[node].mPlaneNum]);

			//Go down the sides that the box lands in
			if((side & ZonePlane.PSIDE_FRONT) != 0)
			{
				FindClosestLeafIntersection_r(trace, mZoneNodes[node].mFront);
			}

			if((side & ZonePlane.PSIDE_BACK) != 0)
			{
				FindClosestLeafIntersection_r(trace, mZoneNodes[node].mBack);
			}
		}


		void Trace_ExpandPlaneForBox(ref ZonePlane p, BoundingBox box)
		{
			Vector3	norm	=p.mNormal;
			
			if(norm.X > 0)
			{
				p.mDist	-=norm.X * box.Min.X;
			}
			else
			{
				p.mDist	-=norm.X * box.Max.X;
			}
			
			if(norm.Y > 0)
			{
				p.mDist	-=norm.Y * box.Min.Y;
			}
			else
			{
				p.mDist	-=norm.Y * box.Max.Y;
			}

			if(norm.Z > 0)
			{
				p.mDist	-=norm.Z * box.Min.Z;
			}
			else
			{
				p.mDist	-=norm.Z * box.Max.Z;
			}
		}


		bool IntersectLeafSides_r(RayTrace trace, Vector3 start, Vector3 end,
			Int32 leaf, Int32 side, Int32 pSide)
		{
			if(pSide == 0)
			{
				return	false;
			}

			if(side >= mZoneLeafs[leaf].mNumSides)
			{
				return	true;	//if it lands behind all planes, it is inside
			}

			int	RSide	=mZoneLeafs[leaf].mFirstSide + side;

			ZonePlane	p	=mZonePlanes[mZoneLeafSides[RSide].mPlaneNum];

			p.mType	=ZonePlane.PLANE_ANY;
			
			if(mZoneLeafSides[RSide].mPlaneSide != 0)
			{
				p.Inverse();
			}
			
			//Simulate the point having a box, by pushing the plane out by the box size
			Trace_ExpandPlaneForBox(ref p, trace.mRayBox);

			float	frontDist	=p.DistanceFast(start);
			float	backDist	=p.DistanceFast(end);

			if(frontDist >= 0 && backDist >= 0)
			{
				//Leaf sides are convex hulls, so front side is totally outside
				return	IntersectLeafSides_r(trace, start, end, leaf, side + 1, 0);
			}

			if(frontDist < 0 && backDist < 0)
			{
				return	IntersectLeafSides_r(trace, start, end, leaf, side + 1, 1);
			}

			Int32	splitSide	=(frontDist < 0)? 1 : 0;
			float	splitDist	=0.0f;
			
			if(frontDist < 0)
			{
				splitDist	=(frontDist + UtilityLib.Mathery.ON_EPSILON)
								/ (frontDist - backDist);
			}
			else
			{
				splitDist	=(frontDist - UtilityLib.Mathery.ON_EPSILON)
								/ (frontDist - backDist);
			}

			if(splitDist < 0.0f)
			{
				splitDist	=0.0f;
			}
			
			if(splitDist > 1.0f)
			{
				splitDist	=1.0f;
			}

			Vector3	intersect	=start + splitDist * (end - start);

			//Only go down the back side, since the front side is empty in a convex tree
			if(IntersectLeafSides_r(trace, start, intersect, leaf, side + 1, splitSide))
			{
				trace.mbLeafHit	=true;
				return	true;
			}
			else if(IntersectLeafSides_r(trace, intersect, end, leaf, side + 1, (splitSide == 0)? 1 : 0))
			{
				splitDist	=(intersect - trace.mOriginalStart).Length();

				//Record the intersection closest to the start of ray
				if(splitDist < trace.mBestDist && !trace.mbHitSet)
				{
					trace.mIntersection	=intersect;
					trace.mLeaf			=leaf;
					trace.mBestDist		=splitDist;
					trace.mBestPlane	=p;
					trace.mRatio		=splitDist;
					trace.mbHitSet		=true;
				}
				trace.mbLeafHit	=true;
				return	true;
			}			
			return	false;	
		}


		bool PointInLeafSides(Vector3 pnt, ZoneLeaf leaf, BoundingBox box)
		{
			Int32	f	=leaf.mFirstSide;

			for(int i=0;i < leaf.mNumSides;i++)
			{
				ZonePlane	p	=mZonePlanes[mZoneLeafSides[i + f].mPlaneNum];
				p.mType			=ZonePlane.PLANE_ANY;
			
				if(mZoneLeafSides[i + f].mPlaneSide != 0)
				{
					p.Inverse();
				}

				//Simulate the point having a box, by pushing the plane out by the box size
				Trace_ExpandPlaneForBox(ref p, box);

				if(p.DistanceFast(pnt) >= 0.0f)
				{
					return false;	//Since leafs are convex, it must be outside...
				}
			}
			return	true;
		}


		BoundingBox Trace_GetMoveBox(BoundingBox box, Vector3 start, Vector3 end)
		{
			BoundingBox	ret	=new BoundingBox();

			Mathery.ClearBoundingBox(ref ret);
			Mathery.AddPointToBoundingBox(ref ret, start);
			Mathery.AddPointToBoundingBox(ref ret, end);

			ret.Min	+=box.Min - Vector3.One;
			ret.Max	+=box.Max + Vector3.One;

			return	ret;
		}


		//about 3 times faster than original genesis on xbox
		UInt32 Trace_BoxOnPlaneSide(BoundingBox box, ZonePlane p)
		{
			UInt32	side	=0;
			Vector3	corner0	=Vector3.Zero;
			Vector3	corner1	=Vector3.Zero;
			float	dist1, dist2;

			//Axial planes are easy
			if(p.mType < ZonePlane.PLANE_ANYX)
			{
				if(UtilityLib.Mathery.VecIdx(box.Max, p.mType) >= p.mDist)
				{
					side	|=ZonePlane.PSIDE_FRONT;
				}
				if(UtilityLib.Mathery.VecIdx(box.Min, p.mType) < p.mDist)
				{
					side	|=ZonePlane.PSIDE_BACK;
				}
				return	side;
			}

			//Create the proper leading and trailing verts for the box
			if(p.mNormal.X < 0)
			{
				corner0.X	=box.Min.X;
				corner1.X	=box.Max.X;
			}
			else
			{
				corner1.X	=box.Min.X;
				corner0.X	=box.Max.X;
			}
			if(p.mNormal.Y < 0)
			{
				corner0.Y	=box.Min.Y;
				corner1.Y	=box.Max.Y;
			}
			else
			{
				corner1.Y	=box.Min.Y;
				corner0.Y	=box.Max.Y;
			}
			if(p.mNormal.Z < 0)
			{
				corner0.Z	=box.Min.Z;
				corner1.Z	=box.Max.Z;
			}
			else
			{
				corner1.Z	=box.Min.Z;
				corner0.Z	=box.Max.Z;
			}

			dist1	=Vector3.Dot(p.mNormal, corner0) - p.mDist;
			dist2	=Vector3.Dot(p.mNormal, corner1) - p.mDist;
			
			if(dist1 >= 0)
			{
				side	=ZonePlane.PSIDE_FRONT;
			}
			if(dist2 < 0)
			{
				side	|=ZonePlane.PSIDE_BACK;
			}
			return	side;
		}
	}
}