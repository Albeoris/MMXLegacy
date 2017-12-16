using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.UpdateLogic.Interactions;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/FloatingObjectVerticalView")]
	public class FloatingObjectVerticalView : BaseView
	{
		private Vector3 oldPos;

		private Vector3 newPos;

		private Single m_timer;

		[SerializeField]
		private Single m_speed = 1f;

		[SerializeField]
		private Single m_allowedHightDelta = 2f;

		[SerializeField]
		private Renderer m_PlateRenderer;

		[SerializeField]
		private ParticleSystem m_changeParticles;

		protected override void Awake()
		{
			base.Awake();
			enabled = false;
			m_timer = 1f;
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SLOT_HEIGHT_CHANGED, new EventHandler(OnHeightChanged));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.REEVALUATE_PASSABLE, new EventHandler(OnEvaluatePassable));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.GRID_HEIGHT_SET, new EventHandler(OnHeightSet));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.GRID_HEIGHT_RESET, new EventHandler(OnHeightReset));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SLOT_HEIGHT_CHANGED, new EventHandler(OnHeightChanged));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.REEVALUATE_PASSABLE, new EventHandler(OnEvaluatePassable));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.GRID_HEIGHT_SET, new EventHandler(OnHeightSet));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.GRID_HEIGHT_RESET, new EventHandler(OnHeightReset));
				OnEvaluatePassable(null, EventArgs.Empty);
			}
		}

		private void OnHeightChanged(Object p_sender, EventArgs p_args)
		{
			if (((InteractiveObject)MyController).Position == ((GridSlot)p_sender).Position)
			{
				oldPos = transform.position;
				Vector3 vector = Helper.SlotLocalPosition(((InteractiveObject)MyController).Position, ((GridSlot)p_sender).Height) + LegacyLogic.Instance.MapLoader.Grid.GetOffset();
				newPos = vector;
				m_timer = 0f;
				enabled = true;
				if (m_changeParticles != null)
				{
					m_changeParticles.Play(true);
				}
			}
		}

		private void OnHeightSet(Object p_sender, EventArgs p_args)
		{
			oldPos = transform.position;
			Vector3 vector = Helper.SlotLocalPosition(((InteractiveObject)MyController).Position, ((SetGridHeightInteraction)p_sender).NewHeight) + LegacyLogic.Instance.MapLoader.Grid.GetOffset();
			newPos = vector;
			m_timer = 0f;
			enabled = true;
			((SetGridHeightInteraction)p_sender).NotifyHeightChanged(((InteractiveObject)MyController).Position, ((SetGridHeightInteraction)p_sender).NewHeight);
			if (m_changeParticles != null)
			{
				m_changeParticles.Play(true);
			}
		}

		private void OnHeightReset(Object p_sender, EventArgs p_args)
		{
			oldPos = transform.position;
			Single originalHeight = ((InteractiveObject)MyController).OriginalHeight;
			Vector3 vector = Helper.SlotLocalPosition(((InteractiveObject)MyController).Position, originalHeight) + LegacyLogic.Instance.MapLoader.Grid.GetOffset();
			newPos = vector;
			m_timer = 0f;
			enabled = true;
			((ResetGridHeightInteraction)p_sender).NotifyHeightChanged(((InteractiveObject)MyController).Position, originalHeight);
			if (m_changeParticles != null)
			{
				m_changeParticles.Stop(true);
			}
		}

		private void OnEvaluatePassable(Object p_sender, EventArgs p_args)
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			Position position = ((InteractiveObject)MyController).Position;
			Boolean flag = false;
			GridSlot slot = grid.GetSlot(position);
			for (EDirection edirection = EDirection.NORTH; edirection <= EDirection.WEST; edirection++)
			{
				GridSlot slot2 = grid.GetSlot(position + edirection);
				if (slot2 != null)
				{
					Single num = Math.Abs(slot2.Height - slot.Height);
					flag |= (num <= m_allowedHightDelta);
					if (num <= m_allowedHightDelta)
					{
						slot.GetTransition(edirection).Open();
					}
					else
					{
						slot.GetTransition(edirection).Close();
					}
					if (edirection == EDirection.NORTH && num <= m_allowedHightDelta)
					{
						flag = true;
					}
					if (edirection == EDirection.EAST && num <= m_allowedHightDelta)
					{
						flag = true;
					}
					if (edirection == EDirection.SOUTH && num <= m_allowedHightDelta)
					{
						flag = true;
					}
					if (edirection == EDirection.WEST && num <= m_allowedHightDelta)
					{
						flag = true;
					}
				}
			}
			if (m_PlateRenderer != null && m_PlateRenderer.material != null)
			{
				if (flag)
				{
					m_PlateRenderer.material.SetFloat("_GlowMin", 0.5f);
					m_PlateRenderer.material.SetFloat("_GlowScale", 2f);
				}
				else
				{
					m_PlateRenderer.material.SetFloat("_GlowMin", 0f);
					m_PlateRenderer.material.SetFloat("_GlowScale", 0f);
				}
			}
		}

		private void Update()
		{
			if (m_timer <= 1f)
			{
				m_timer += Time.deltaTime * m_speed;
				transform.position = Vector3.Lerp(oldPos, newPos, m_timer);
			}
			else
			{
				OnEvaluatePassable(null, null);
				enabled = false;
				if (m_changeParticles != null)
				{
					m_changeParticles.Stop(true);
				}
			}
		}
	}
}
