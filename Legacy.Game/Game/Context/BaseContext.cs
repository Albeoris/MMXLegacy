using System;
using UnityEngine;

namespace Legacy.Game.Context
{
	[AddComponentMenu("MM Legacy/Contexts/BaseContext")]
	public abstract class BaseContext : MonoBehaviour
	{
		[SerializeField]
		private String m_SceneTarget;

		public readonly EContext ID;

		public BaseContext(EContext contextID)
		{
			ID = contextID;
		}

		public virtual void OnEnableContext()
		{
			enabled = true;
			if (!String.IsNullOrEmpty(m_SceneTarget))
			{
				Application.LoadLevel(m_SceneTarget);
			}
		}

		public virtual void OnDisableContext()
		{
			enabled = false;
		}

		protected virtual void Awake()
		{
			enabled = false;
			ContextManager.RegisterContext(this);
		}

		protected virtual void OnDestroy()
		{
			ContextManager.DeregisterContext(ID);
		}

		protected virtual void OnLevelWasLoaded(Int32 levelIndex)
		{
			if (!String.IsNullOrEmpty(m_SceneTarget) && Application.loadedLevelName == m_SceneTarget)
			{
				OnSceneLoaded();
			}
		}

		protected virtual void OnSceneLoaded()
		{
		}
	}
}
