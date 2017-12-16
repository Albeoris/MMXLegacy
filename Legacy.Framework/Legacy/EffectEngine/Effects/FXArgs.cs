using System;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class FXArgs : EventArgs
	{
		public FXArgs(GameObject p_origin, GameObject p_target, GameObject p_beginPoint, GameObject p_endPoint, Vector3 p_slotOriginPosition, Vector3 p_slotForward, Vector3 p_slotLeft, Vector3 p_slotTargetPosition) : this(p_origin, p_target, p_beginPoint, p_endPoint, p_slotOriginPosition, p_slotForward, p_slotLeft, p_slotTargetPosition, null)
		{
		}

		public FXArgs(GameObject p_origin, GameObject p_target, GameObject p_beginPoint, GameObject p_endPoint, Vector3 p_slotOriginPosition, Vector3 p_slotForward, Vector3 p_slotLeft, Vector3 p_slotTargetPosition, List<GameObject> p_targets)
		{
			if (p_origin == null)
			{
				throw new ArgumentNullException("p_origin");
			}
			if (p_target == null)
			{
				throw new ArgumentNullException("p_target");
			}
			if (p_beginPoint == null)
			{
				throw new ArgumentNullException("p_beginPoint");
			}
			if (p_endPoint == null)
			{
				throw new ArgumentNullException("p_endPoint");
			}
			Origin = p_origin;
			Target = p_target;
			BeginPoint = p_beginPoint;
			EndPoint = p_endPoint;
			SlotOriginPosition = p_slotOriginPosition;
			SlotForward = p_slotForward;
			SlotLeft = p_slotLeft;
			SlotTargetPosition = p_slotTargetPosition;
			OriginTransform = p_origin.transform;
			TargetTransform = p_target.transform;
			BeginPointTransform = p_beginPoint.transform;
			EndPointTransform = p_endPoint.transform;
			Targets = p_targets;
		}

		public GameObject Origin { get; private set; }

		public GameObject Target { get; private set; }

		public List<GameObject> Targets { get; private set; }

		public GameObject BeginPoint { get; private set; }

		public GameObject EndPoint { get; private set; }

		public Vector3 SlotOriginPosition { get; private set; }

		public Vector3 SlotForward { get; private set; }

		public Vector3 SlotLeft { get; private set; }

		public Vector3 SlotTargetPosition { get; private set; }

		public Transform OriginTransform { get; private set; }

		public Transform TargetTransform { get; private set; }

		public Transform BeginPointTransform { get; private set; }

		public Transform EndPointTransform { get; private set; }
	}
}
