using System;
using UnityEngine;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/Prefab Anim Based State Switch View")]
	public class PrefabAnimBasedStateSwitchView : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_passiveObject;

		[SerializeField]
		private GameObject m_activeObject;

		private void Start()
		{
			if (enabled)
			{
				SetActiveRecursively(m_passiveObject, true);
				SetActiveRecursively(m_activeObject, false);
			}
		}

		public void ChangeState(Boolean p_enabled)
		{
			SetActiveRecursively(m_passiveObject, !p_enabled);
			SetActiveRecursively(m_activeObject, p_enabled);
		}

		public void ActivateObject()
		{
			ChangeState(true);
		}

		public void DeactivateObject()
		{
			ChangeState(false);
		}

		private void SetActiveRecursively(GameObject obj, Boolean state)
		{
			if (obj != null)
			{
				obj.SetActive(state);
			}
		}

		private void ChestView_Skipped_Animation(Boolean p_state)
		{
			ChangeState(p_state);
		}
	}
}
