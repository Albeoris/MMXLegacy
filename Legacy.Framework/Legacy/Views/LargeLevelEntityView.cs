using System;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	public class LargeLevelEntityView : LevelEntityView
	{
		[SerializeField]
		private Vector3 m_PositionOffset;

		public Vector3 PositionOffset => m_PositionOffset;

	    public override void SetEntityPosition()
		{
			base.SetEntityPosition();
			Vector3 b = m_animatorControl.TargetRotation * m_PositionOffset;
			transform.localPosition += b;
			m_animatorControl.TargetPosition += b;
		}

		public override void PushEntityToPosition()
		{
			SetEntityPosition();
		}

		protected override void OnMoveEntity(Object p_sender, EventArgs p_args)
		{
			base.OnMoveEntity(p_sender, p_args);
			MoveEntityEventArgs moveEntityEventArgs = p_args as MoveEntityEventArgs;
			if (moveEntityEventArgs != null && p_sender == MyController)
			{
				MovingEntity myController = MyController;
				Vector3 vector = GetVectorPosition(myController.Position, myController.Size, myController.Height);
				Quaternion quaternion = Helper.GridDirectionToQuaternion(myController.Direction);
				vector = ViewManager.Instance.GridOrigin.TransformPoint(vector);
				vector += quaternion * m_PositionOffset;
				m_animatorControl.MoveTo(vector, quaternion);
			}
		}
	}
}
