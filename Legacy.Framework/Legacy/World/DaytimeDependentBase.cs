using System;
using Legacy.Core.Api;
using UnityEngine;

namespace Legacy.World
{
	public abstract class DaytimeDependentBase : MonoBehaviour
	{
		protected virtual void Awake()
		{
			DaytimeDependentController.Register(this);
		}

		protected virtual void OnDestroy()
		{
			DaytimeDependentController.Unregister(this);
		}

		public abstract void ChangeState(EDayState newState);
	}
}
