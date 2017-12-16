using System;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Map;
using UnityEngine;

namespace Legacy.Game.MMGUI.Minimap
{
	public abstract class SymbolView : MonoBehaviour, IDisposable
	{
		private static Quaternion s_NorthDir = Quaternion.AngleAxis(0f, Vector3.forward);

		private static Quaternion s_EastDir = Quaternion.AngleAxis(-90f, Vector3.forward);

		private static Quaternion s_SouthDir = Quaternion.AngleAxis(-180f, Vector3.forward);

		private static Quaternion s_WestDir = Quaternion.AngleAxis(-270f, Vector3.forward);

		[SerializeField]
		private UIWidget m_MyUIWidget;

		public static Quaternion s_InverseRotation = Quaternion.identity;

		private Transform m_CachedTransform;

		public Boolean IsDisposed { get; private set; }

		public BaseObject MyController { get; private set; }

		public UIWidget MyUIWidget
		{
			get
			{
				if (m_MyUIWidget == null)
				{
					m_MyUIWidget = GetComponentInChildren<UIWidget>();
				}
				return m_MyUIWidget;
			}
		}

		public virtual Position MyControllerGridPosition
		{
			get
			{
				Position position = new Position(-1, -1);
				if (MyController is MovingEntity)
				{
					position = ((MovingEntity)MyController).Position;
				}
				else if (MyController is InteractiveObject)
				{
					position = ((InteractiveObject)MyController).Position;
				}
				return position;
			}
		}

		public virtual EDirection MyControllerGridDirection
		{
			get
			{
				EDirection result = EDirection.NORTH;
				if (MyController is MovingEntity)
				{
					result = ((MovingEntity)MyController).Direction;
				}
				else if (MyController is InteractiveObject)
				{
					result = ((InteractiveObject)MyController).Location;
				}
				return result;
			}
		}

		public Vector3 MyControllerPosition
		{
			get
			{
				Position myControllerGridPosition = MyControllerGridPosition;
				return new Vector3(myControllerGridPosition.X * 24, myControllerGridPosition.Y * 24, 0f);
			}
		}

		public Quaternion MyControllerRotation
		{
			get
			{
				switch (MyControllerGridDirection)
				{
				case EDirection.NORTH:
					break;
				case EDirection.EAST:
					return s_EastDir;
				case EDirection.SOUTH:
					return s_SouthDir;
				case EDirection.WEST:
					return s_WestDir;
				default:
					Debug.LogError("GridDirectionToQuaternion: Wrong direction: " + MyControllerGridDirection);
					break;
				}
				return s_NorthDir;
			}
		}

		public new Transform transform
		{
			get => (!(m_CachedTransform == null)) ? m_CachedTransform : (m_CachedTransform = base.transform);
		    private set => m_CachedTransform = value;
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

		public virtual void MakePixelPerfect()
		{
			MyUIWidget.MakePixelPerfect();
		}

		public virtual void CheckVisibility(Boolean skipAnimation)
		{
		}

		protected virtual void Awake()
		{
			transform = base.transform;
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
