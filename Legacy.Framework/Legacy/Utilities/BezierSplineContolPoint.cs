using System;
using UnityEngine;

namespace Legacy.Utilities
{
	public class BezierSplineContolPoint : MonoBehaviour
	{
		public Single weight = 1f;

		public Single speed = 1f;

		public Boolean SharedTangent = true;

		[HideInInspector]
		public Transform LeftSubControlPoint;

		[HideInInspector]
		public Transform RightSubControlPoint;

		public GameObject TargetPosition;

		public Vector3 GlobalPosition
		{
			get
			{
				if (TargetPosition == null)
				{
					return transform.position;
				}
				return TargetPosition.transform.position;
			}
		}

		public Vector3 LeftSubControlGlobalPosition => LeftSubControlPoint.position - GlobalPosition;

	    public Vector3 RightSubControlGlobalPosition => RightSubControlPoint.position - GlobalPosition;

	    public Vector3 LocalPosition
		{
			get
			{
				if (TargetPosition == null)
				{
					return transform.localPosition;
				}
				return TargetPosition.transform.localPosition;
			}
		}
	}
}
