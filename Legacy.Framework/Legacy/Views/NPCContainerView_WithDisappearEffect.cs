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
	[AddComponentMenu("MM Legacy/Views/NPCContainerView_WithDisappearEffect")]
	internal class NPCContainerView_WithDisappearEffect : BaseView
	{
		[SerializeField]
		private GameObject[] m_npcObjects;

		public GameObject DisappearEffect;

		public Single DisappearDelay = 2f;

		public Single EffectDelay = 2f;

		public GameObject[] NPCObjects
		{
			get => m_npcObjects;
		    set => m_npcObjects = value;
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
				transform.localPosition = a + new Vector3(interactiveObject.OffsetPosition.X, interactiveObject.OffsetPosition.Y, interactiveObject.OffsetPosition.Z);
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
					foreach (GameObject gameObject in m_npcObjects)
					{
						if (gameObject != null)
						{
							gameObject.SetActive(true);
						}
					}
				}
				else
				{
					foreach (GameObject x in m_npcObjects)
					{
						if (x != null)
						{
							foreach (GameObject p_obj in m_npcObjects)
							{
								StartCoroutine(TriggerAnim(p_obj));
							}
						}
					}
				}
			}
		}

		private void HideObjects()
		{
			if (m_npcObjects != null)
			{
				foreach (GameObject gameObject in m_npcObjects)
				{
					if (gameObject != null)
					{
						gameObject.SetActive(false);
					}
				}
			}
		}

		private IEnumerator TriggerAnim(GameObject p_obj)
		{
			Animator anim = p_obj.GetComponent<Animator>();
			if (anim != null)
			{
				anim.SetInteger("MagicAttackType", 4);
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				anim.SetInteger("MagicAttackType", 0);
			}
			yield return new WaitForSeconds(EffectDelay);
			if (DisappearEffect != null)
			{
				GameObject newEffect = (GameObject)Instantiate(DisappearEffect);
				newEffect.transform.position = p_obj.transform.position;
			}
			yield return new WaitForSeconds(DisappearDelay);
			HideObjects();
			yield break;
		}
	}
}
