using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.Minimap
{
	[AddComponentMenu("MM Legacy/MMGUI/Minimap/AreaCameraView")]
	public class AreaCameraView : MoveableSymbolView
	{
		private Camera m_camera;

		public Camera Camera => m_camera;

	    protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.FINISH_SCENE_LOAD, new EventHandler(OnFinishSceneLoad));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.FINISH_SCENE_LOAD, new EventHandler(OnFinishSceneLoad));
			}
		}

		private void OnFinishSceneLoad(Object p_sender, EventArgs p_args)
		{
			OnSetEntityPosition(MyController, EventArgs.Empty);
		}

		protected override void OnSetEntityPosition(Object p_sender, EventArgs p_args)
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			if (grid.Width > 32 || grid.Height > 32)
			{
				if (p_sender == MyController)
				{
					transform.localPosition = MyControllerPosition;
					enabled = false;
				}
			}
			else
			{
				transform.localPosition = new Vector3(grid.Width * 12, grid.Height * 12, 0f);
				enabled = false;
			}
			transform.localRotation = s_InverseRotation;
		}

		protected override void Awake()
		{
			base.Awake();
			m_camera = camera;
			m_camera.enabled = false;
		}

		public override void MakePixelPerfect()
		{
		}

		protected override void Update()
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			if (grid.Width > 32 || grid.Height > 32)
			{
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, MyControllerPosition, 40f * Time.deltaTime);
			}
			else
			{
				transform.localPosition = new Vector3(grid.Width * 12, grid.Height * 12, 0f);
			}
			transform.localRotation = s_InverseRotation;
		}
	}
}
