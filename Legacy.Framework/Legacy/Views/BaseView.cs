using System;
using Legacy.Core.Entities;
using UnityEngine;

namespace Legacy.Views
{
	public abstract class BaseView : MonoBehaviour, IDisposable
	{
		private Transform m_CachedTransform;

		private GameObject m_CachedGameObject;

		public Boolean IsDisposed { get; private set; }

		public BaseObject MyController { get; private set; }

		public new Transform transform
		{
			get
			{
				if (m_CachedTransform == null)
				{
					m_CachedTransform = base.transform;
				}
				return m_CachedTransform;
			}
			private set => m_CachedTransform = value;
		}

		public new GameObject gameObject
		{
			get
			{
				if (m_CachedGameObject == null)
				{
					m_CachedGameObject = base.gameObject;
				}
				return m_CachedGameObject;
			}
			private set => m_CachedGameObject = value;
		}

		public void InitializeView(BaseObject controller)
		{
			DisposedCheck();
			if (MyController != controller)
			{
				BaseObject myController = MyController;
				MyController = controller;
				OnChangeMyController(myController);
			}
		}

		public void Dispose()
		{
			if (!IsDisposed)
			{
				InitializeView(null);
				IsDisposed = true;
				Destroy(this);
			}
		}

		protected virtual void Awake()
		{
			transform = base.transform;
			gameObject = base.gameObject;
		}

		protected virtual void OnDestroy()
		{
			if (!IsDisposed)
			{
				InitializeView(null);
				IsDisposed = true;
			}
		}

		protected virtual void OnChangeMyController(BaseObject oldController)
		{
			DisposedCheck();
		}

		protected void DisposedCheck()
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException("View already disposed!");
			}
		}
	}
}
