using System;
using System.Collections.Generic;
using Legacy.Core.Entities;
using Legacy.Core.Map;

namespace Legacy.Core.Pathfinding
{
	public static class AStarHelper<T> where T : IPathNode<T>
	{
		private static List<T> m_Result = new List<T>(16);

		private static HashSet<T> m_ClosedSet = new HashSet<T>();

		private static HashSet<T> m_OpenSet = new HashSet<T>();

		private static Dictionary<T, T> m_CameFrom = new Dictionary<T, T>(16);

		private static Dictionary<T, NodeScore> m_NoteScores = new Dictionary<T, NodeScore>(16);

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
			{
				throw new ArgumentNullException("start");
			}
			if (goal == null)
			{
				throw new ArgumentNullException("goal");
			}
			if (maxSteps <= 0)
			{
				throw new ArgumentOutOfRangeException("maxSteps", "0 is not supported");
			}
			Int32 num = maxSteps * maxSteps;
			Int32 num2 = (Int32)Position.DistanceSquared(start.Position, goal.Position);
			if (num2 >= num)
			{
				return 0;
			}
			try
			{
				m_OpenSet.Add(start);
				NodeScore value;
				value.G = 0;
				value.H = HeuristicCostEstimate(start, goal);
				value.F = value.H;
				m_NoteScores[start] = value;
				while (m_OpenSet.Count != 0)
				{
					T t = LowestFScore(m_OpenSet, m_NoteScores);
					if (t.Equals(goal))
					{
						num2 = 0;
						if (resultPath != null)
						{
							num2 = ReconstructPath(m_CameFrom, t, maxSteps, m_Result);
							if (num2 != 0)
							{
								m_Result.Reverse();
								foreach (T item in m_Result)
								{
									resultPath.Add(item);
								}
							}
						}
						else
						{
							num2 = ReconstructPath(m_CameFrom, t, maxSteps);
						}
						return num2;
					}
					m_OpenSet.Remove(t);
					m_ClosedSet.Add(t);
					List<T> connections = t.GetConnections(p_throughClosedDoors);
					Int32 i = 0;
					Int32 count = connections.Count;
					while (i < count)
					{
						T t2 = connections[i];
						num2 = (Int32)Position.DistanceSquared(start.Position, t2.Position);
						if (num2 < num)
						{
							num2 = (Int32)Position.DistanceSquared(t2.Position, goal.Position);
							if (!Invalid(t2) && num2 < maxSteps * maxSteps && (p_entity == null || t2.IsPassableForEntity(p_entity, p_isForDistanceCalc, p_checkForSummons)) && !m_ClosedSet.Contains(t2))
							{
								m_NoteScores.TryGetValue(t, out value);
								NodeScore value2;
								m_NoteScores.TryGetValue(t2, out value2);
								Int16 num3 = (Int16)(value.G + Distance(t, t2));
								Boolean flag = false;
								if (!m_OpenSet.Contains(t2))
								{
									m_OpenSet.Add(t2);
									flag = true;
								}
								else if (num3 < value2.G)
								{
									flag = true;
								}
								if (flag)
								{
									m_CameFrom[t2] = t;
									value2.G = num3;
									value2.H = HeuristicCostEstimate(t2, goal);
									value2.F = (Int16)(value2.G + value2.H);
									m_NoteScores[t2] = value2;
								}
							}
						}
						i++;
					}
				}
			}
			finally
			{
				m_ClosedSet.Clear();
				m_OpenSet.Clear();
				m_CameFrom.Clear();
				m_NoteScores.Clear();
				m_Result.Clear();
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
			Single num = Single.MaxValue;
			foreach (T t in openset)
			{
				Single num2 = scores[t].F;
				if (num2 < num)
				{
					num = num2;
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
