﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;


namespace PathLib
{
	internal class PathFinder
	{
		AStarNode	mStartNode;
		AStarNode	mEndNode;

		bool	mbDone;

		internal List<Vector3>	mResultPath	=new List<Vector3>();

		List<AStarNode>	mOpen	=new List<AStarNode>();
		List<AStarNode>	mClosed	=new List<AStarNode>();


		public void StartPath(PathNode start, PathNode end)
		{
			mbDone		=false;

			//clear lists
			mResultPath.Clear();
			mOpen.Clear();
			mClosed.Clear();

			AStarNode	asStart	=new AStarNode();
			asStart.mNode		=start;
			asStart.mParent		=null;

			AStarNode	asEnd	=new AStarNode();
			asEnd.mNode			=end;
			asEnd.mParent		=null;

			mStartNode	=asStart;
			mEndNode	=asEnd;
		}


		public bool IsDone()
		{
			return	mbDone;
		}


		public void Go()
		{
			if(mStartNode.mNode == mEndNode.mNode)
			{
				mbDone	=true;
				return;
			}
			mOpen.Add(mStartNode);
			SelectNode(mStartNode);

			while(!mbDone)
			{
				Step();
			}
		}


		AStarNode IsNodeInOpen(PathNode pn)
		{
			foreach(AStarNode asn in mOpen)
			{
				if(asn.mNode == pn)
				{
					return	asn;
				}
			}
			return	null;
		}


		AStarNode IsNodeInClosed(PathNode pn)
		{
			foreach(AStarNode asn in mClosed)
			{
				if(asn.mNode == pn)
				{
					return	asn;
				}
			}
			return	null;
		}


		void FinishPath()
		{
			AStarNode	walk	=mEndNode;
			while(walk != mStartNode)
			{
				Debug.WriteLine("Walking path " + walk.mNode.mPoly.GetCenter());

				Edge	edgeBetween	=walk.mNode.FindEdgeBetween(walk.mParent.mNode);
				if(edgeBetween == null)
				{
					Debug.WriteLine("Null edge between path nodes!");
					return;
				}

				mResultPath.Add(walk.mNode.mPoly.GetCenter());
				mResultPath.Add(edgeBetween.GetCenter());

				walk	=walk.mParent;
			}

			//path is in reverse order, flip it
			mResultPath.Reverse();
		}


		public void SelectNode(AStarNode asn)
		{
			mOpen.Remove(asn);
			mClosed.Add(asn);

			if(asn.mNode == mEndNode.mNode)
			{
				mEndNode	=asn;
				FinishPath();
				mbDone	=true;
				return;
			}
			
			foreach(PathConnection con in asn.mNode.mConnections)
			{
				AStarNode	found	=IsNodeInOpen(con.mConnectedTo);

				if(found != null)
				{
					//check the G score
					float	newGScore	=asn.mGScore + con.mDistanceToCenter;

					//switch parents if this is faster
					if(newGScore < found.mGScore)
					{
						found.mParent	=asn;
						found.mGScore	=newGScore;
					}
				}
				else
				{
					//ensure this node isn't in the closed list
					AStarNode	clnode	=IsNodeInClosed(con.mConnectedTo);
					if(clnode != null)
					{
						continue;
					}

					AStarNode	kid	=new AStarNode();
					kid.mNode		=con.mConnectedTo;
					kid.mParent		=asn;
					kid.mGScore		=kid.mParent.mGScore + con.mDistanceToCenter;
					CalculateHScore(kid);

					mOpen.Add(kid);
				}
			}
		}


		public void CalculateHScore(AStarNode asn)
		{
			//typically this is multiplied by 10, but I am using
			//real world position instead of number of nodes over
			//so this is about the same
			asn.mHScore	=mEndNode.mNode.CenterToCenterDistance(asn.mNode);
		}


		public void Step()
		{
			//pick the node with the smallest f
			float		minScore	=float.MaxValue;
			AStarNode	least		=null;
			foreach(AStarNode asn in mOpen)
			{
				float	fscore	=asn.mGScore + asn.mHScore;
				if(fscore <= minScore)	//take the last one
				{
					minScore	=fscore;
					least		=asn;
				}
			}

			//step to this node
			SelectNode(least);
		}
	}
}
