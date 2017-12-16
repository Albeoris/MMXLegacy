using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.UpdateLogic;
using Legacy.EffectEngine;
using Legacy.Views;
using UnityEngine;

namespace Legacy.Game.IngameManagement
{
	public class MouseInteraction : MonoBehaviour
	{
		private const Single RAYCAST_LENGTH = 50f;

		private void Update()
		{
			if (!enabled)
			{
				return;
			}
			if (!Input.GetMouseButtonUp(0))
			{
				return;
			}
			if (UICamera.hoveredObject != null)
			{
				return;
			}
			if (FXMainCamera.Instance == null)
			{
				return;
			}
			Camera camera = FXMainCamera.Instance.DefaultCamera.camera;
			if (camera == null)
			{
				return;
			}
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, 50f))
			{
				InteractiveObjectHighlight component = raycastHit.collider.GetComponent<InteractiveObjectHighlight>();
				if (component != null && component.IsClickable)
				{
					InteractiveObject myController = component.MyController;
					LegacyLogic.Instance.WorldManager.Party.SelectedInteractiveObject = myController;
					LegacyLogic.Instance.CommandManager.AddCommand(new InteractCommand(myController));
					return;
				}
				if (raycastHit.rigidbody != null)
				{
					BaseView component2 = raycastHit.rigidbody.GetComponent<BaseView>();
					if (component2 != null && component2.MyController is Monster)
					{
						Monster monster = (Monster)component2.MyController;
						Party party = LegacyLogic.Instance.WorldManager.Party;
						Single num = Position.Distance(monster.Position, party.Position);
						if (num == 1f && EDirectionFunctions.GetLineOfSightDirection(party.Position, monster.Position) == party.Direction && monster.CurrentHealth > 0)
						{
							party.SelectedMonster = monster;
						}
						return;
					}
				}
			}
		}
	}
}
