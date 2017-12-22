using System;
using System.Collections.Generic;
using Legacy.Core.Entities;
using Legacy.Core.Map;

namespace Legacy.Core.Pathfinding
{
    public static class AStarHelper<T> where T : IPathNode<T>
	{
        private sealed class HeuristicCostComparer : IComparer<T>
        {
            private readonly T m_goal;

            public HeuristicCostComparer(T goal)
            {
                m_goal = goal;
            }

            public Int32 Compare(T x, T y)
            {
                Int16 xCost = HeuristicCostEstimate(x, m_goal);
                Int16 yCost = HeuristicCostEstimate(y, m_goal);
                return xCost.CompareTo(yCost);
            }
        }


        private static List<T> s_Result = new List<T>(16);

	    private static HashSet<T> s_OpenedSet = new HashSet<T>();

        private static HashSet<T> s_ClosedSet = new HashSet<T>();

		private static Dictionary<T, T> s_CameFrom = new Dictionary<T, T>(16);

		private static Dictionary<T, NodeScore> s_NoteScores = new Dictionary<T, NodeScore>(16);

		public static Boolean ForDistanceCalc { get; private set; }

		public static Int32 Calculate(T start, T goal, Int32 maxSteps, MovingEntity p_entity, Boolean p_isForDistanceCalc, ICollection<T> resultPath)
		{
			return Calculate(start, goal, maxSteps, p_entity, p_isForDistanceCalc, false, false, resultPath);
		}

		public static Int32 Calculate(T start, T goal, Int32 maxSteps, MovingEntity p_entity, Boolean p_isForDistanceCalc, Boolean p_checkForSummons, ICollection<T> resultPath)
		{
			return Calculate(start, goal, maxSteps, p_entity, p_isForDistanceCalc, p_checkForSummons, false, resultPath);
		}

	    public static Int32 Calculate(T start, T goal, Int32 maxSteps, MovingEntity p_entity, Boolean p_isForDistanceCalc, Boolean p_checkForSummons, Boolean p_throughClosedDoors, ICollection<T> resultPath)
	    {
	        ForDistanceCalc = p_isForDistanceCalc;
	        p_checkForSummons = false;
	        if (start == null)
	            throw new ArgumentNullException(nameof(start));

            if (goal == null)
	            throw new ArgumentNullException(nameof(goal));

            if (maxSteps <= 0)
	            throw new ArgumentOutOfRangeException(nameof(maxSteps), "0 is not supported");

            Int32 maxSquaredDistance = maxSteps * maxSteps;
	        Int32 squaredDistance = (Int32)Position.DistanceSquared(start.Position, goal.Position);
	        if (squaredDistance >= maxSquaredDistance)
	            return 0;

	        try
	        {
	            s_OpenedSet.Add(start);
	            NodeScore value;
	            value.G = 0;
	            value.H = HeuristicCostEstimate(start, goal);
	            value.F = value.H;
	            s_NoteScores[start] = value;
	            while (s_OpenedSet.Count != 0)
	            {
	                T t = LowestFScore(s_OpenedSet, s_NoteScores);
	                if (t.Equals(goal))
	                {
	                    squaredDistance = 0;
	                    if (resultPath != null)
	                    {
	                        squaredDistance = ReconstructPath(s_CameFrom, t, maxSteps, s_Result);
	                        if (squaredDistance != 0)
	                        {
	                            s_Result.Reverse();
	                            foreach (T item in s_Result)
	                            {
	                                resultPath.Add(item);
	                            }
	                        }
	                    }
	                    else
	                    {
	                        squaredDistance = ReconstructPath(s_CameFrom, t, maxSteps);
	                    }
	                    return squaredDistance;
	                }
	                s_OpenedSet.Remove(t);
	                s_ClosedSet.Add(t);
	                List<T> connections = t.GetConnections(p_throughClosedDoors);
	                Int32 i = 0;
	                Int32 count = connections.Count;
	                while (i < count)
	                {
	                    T t2 = connections[i];
	                    squaredDistance = (Int32)Position.DistanceSquared(start.Position, t2.Position);
	                    if (squaredDistance < maxSquaredDistance)
	                    {
	                        squaredDistance = (Int32)Position.DistanceSquared(t2.Position, goal.Position);
	                        if (!Invalid(t2) && squaredDistance < maxSteps * maxSteps && (p_entity == null || t2.IsPassableForEntity(p_entity, p_isForDistanceCalc, p_checkForSummons)) && !s_ClosedSet.Contains(t2))
	                        {
	                            s_NoteScores.TryGetValue(t, out value);
	                            NodeScore value2;
	                            s_NoteScores.TryGetValue(t2, out value2);
	                            Int16 g = (Int16)(value.G + Distance(t, t2));
	                            Boolean flag = false;
	                            if (!s_OpenedSet.Contains(t2))
	                            {
	                                s_OpenedSet.Add(t2);
	                                flag = true;
	                            }
	                            else if (g < value2.G)
	                            {
	                                flag = true;
	                            }
	                            if (flag)
	                            {
	                                s_CameFrom[t2] = t;
	                                value2.G = g;
	                                value2.H = HeuristicCostEstimate(t2, goal);
	                                value2.F = (Int16)(value2.G + value2.H);
	                                s_NoteScores[t2] = value2;
	                            }
	                        }
	                    }
	                    i++;
	                }
	            }
	        }
	        finally
	        {
	            s_OpenedSet.Clear();
                s_ClosedSet.Clear();
	            s_CameFrom.Clear();
	            s_NoteScores.Clear();
	            s_Result.Clear();
	        }
	        return 0;
	    }

	    private static Boolean Invalid(T inNode)
		{
			return inNode.Invalid;
		}

		private static Int16 Distance(T start, T goal)
		{
			if (Invalid(start) || Invalid(goal))
			{
				return 5000;
			}
			return (Int16)Position.DistanceSquared(start.Position, goal.Position);
		}

		private static Int16 HeuristicCostEstimate(T start, T goal)
		{
			return Distance(start, goal);
		}

		private static T LowestFScore(HashSet<T> openset, Dictionary<T, NodeScore> scores)
		{
			T result = default(T);
			Single minF = Single.MaxValue;
			foreach (T t in openset)
			{
				Single f = scores[t].F;
				if (f < minF)
				{
					minF = f;
					result = t;
				}
			}
			return result;
		}

		private static Int32 ReconstructPath(Dictionary<T, T> cameFrom, T currentNode, Int32 maxSteps, List<T> path)
		{
			Int32 num = 0;
			for (;;)
			{
				path.Add(currentNode);
				if (num >= maxSteps)
				{
					break;
				}
				num++;
				if (!cameFrom.TryGetValue(currentNode, out currentNode))
				{
					return num;
				}
			}
			return 0;
		}

		private static Int32 ReconstructPath(Dictionary<T, T> cameFrom, T currentNode, Int32 maxSteps)
		{
			Int32 i = 0;
			while (i < maxSteps)
			{
				i++;
				if (!cameFrom.TryGetValue(currentNode, out currentNode))
				{
					return i;
				}
			}
			return 0;
		}

		private struct NodeScore
		{
			public Int16 G;

			public Int16 H;

			public Int16 F;

			public NodeScore(Int16 g, Int16 h, Int16 f)
			{
				G = g;
				H = h;
				F = f;
			}
		}
	}
}
