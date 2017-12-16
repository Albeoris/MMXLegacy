using System;
using System.Collections;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/NpcContainerViewAlphaFade")]
	internal class NpcContainerViewAlphaFade : BaseView
	{
		[SerializeField]
		private GameObject[] m_npcObjects;

		[SerializeField]
		private Single m_maxAlpha = 1f;

		[SerializeField]
		private Single m_maxAllPower = 1f;

		private Boolean m_Hide;

		private Boolean m_Disappear;

		private Renderer[] m_Renderer;

		public GameObject[] NPCObjects
		{
			get => m_npcObjects;
		    set => m_npcObjects = value;
		}

		protected override void Awake()
		{
			base.Awake();
			m_Renderer = GetComponentsInChildren<Renderer>();
		}

		public new InteractiveObject MyController => (InteractiveObject)base.MyController;

	    protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_HIRELING_UPDATED, new EventHandler(OnHirelingUpdated));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.OBJECT_ENABLED_CHANGED, new EventHandler(OnEnableStateChanged));
			}
			if (MyController != null)
			{
				UpdateView();
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_HIRELING_UPDATED, new EventHandler(OnHirelingUpdated));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.OBJECT_ENABLED_CHANGED, new EventHandler(OnEnableStateChanged));
			}
		}

		private void OnHirelingUpdated(Object sender, EventArgs args)
		{
			HirelingEventArgs hirelingEventArgs = (HirelingEventArgs)args;
			if (((NpcContainer)MyController).Contains(hirelingEventArgs.Npc))
			{
				UpdateView();
			}
		}

		private void UpdateView()
		{
			if (MyController != null)
			{
				NpcContainer npcContainer = (NpcContainer)MyController;
				SetVisible(npcContainer.IsEnabled && MyController.Enabled);
			}
		}

		private void OnMoveEntity(Object p_sender, EventArgs p_args)
		{
			MoveEntityEventArgs moveEntityEventArgs = (MoveEntityEventArgs)p_args;
			if (moveEntityEventArgs != null && p_sender == MyController)
			{
				Single p_height = 0f;
				GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(moveEntityEventArgs.Position);
				if (slot != null)
				{
					p_height = slot.Height;
				}
				Vector3 a = Helper.SlotLocalPosition(moveEntityEventArgs.Position, p_height);
				InteractiveObject interactiveObject = p_sender as InteractiveObject;
				StartCoroutine(ChangeAlpha(1f, 0f, a + new Vector3(interactiveObject.OffsetPosition.X, interactiveObject.OffsetPosition.Y, interactiveObject.OffsetPosition.Z)));
			}
		}

		private void OnEnableStateChanged(Object p_sender, EventArgs p_args)
		{
			if (p_sender == MyController)
			{
				SetVisible(MyController.Enabled);
			}
		}

		private void SetVisible(Boolean p_visible)
		{
			if (m_npcObjects != null)
			{
				if (p_visible)
				{
					StartCoroutine(ChangeAlpha(0f, 1f, transform.localPosition));
				}
				else
				{
					StartCoroutine(ChangeAlpha(1f, 0f, transform.localPosition));
				}
			}
		}

		private IEnumerator ChangeAlpha(Single p_sourcealpha, Single p_targetAlpha, Vector3 p_targetPosition)
		{
			Single currentAlpha = p_sourcealpha;
			Boolean increase = p_targetAlpha > p_sourcealpha;
			while ((increase && p_targetAlpha - currentAlpha > 0f) || (!increase && p_targetAlpha - currentAlpha < 0f))
			{
				Light light = GetComponentInChildren<Light>();
				if (light != null)
				{
					light.intensity = currentAlpha;
				}
				foreach (Renderer item in m_Renderer)
				{
					for (Int32 i = 0; i < item.materials.Length; i++)
					{
						Material mat = item.materials[i];
						if (item.material.HasProperty("_Color"))
						{
							Color color = item.material.color;
							color.a = currentAlpha * m_maxAlpha;
							mat.color = color;
							item.materials[i] = mat;
						}
						String shadername = item.material.shader.name;
						if (shadername == "Legacy/Special/LighthouseAngelLight")
						{
							item.material.SetFloat("_AllPower", currentAlpha * m_maxAllPower);
						}
					}
				}
				yield return new WaitForEndOfFrame();
				currentAlpha += Time.deltaTime * ((!increase) ? -1 : 1) / 6f;
			}
			if (p_targetPosition != transform.localPosition)
			{
				transform.localPosition = p_targetPosition;
				StartCoroutine(ChangeAlpha(0f, 1f, transform.localPosition));
			}
			yield break;
		}
	}
}
