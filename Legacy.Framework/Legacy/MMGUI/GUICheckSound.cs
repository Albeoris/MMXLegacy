using System;
using System.Collections;
using Legacy.EffectEngine;
using UnityEngine;

namespace Legacy.MMGUI
{
	[AddComponentMenu("NGUI/Interaction/CheckBox Sound")]
	public class GUICheckSound : MonoBehaviour
	{
		public String audioIDCheck = "SOU_ANNO4_CheckBox_Activate";

		public String audioIDUncheck = "SOU_ANNO4_CheckBox_Deactivate";

		private void OnClick()
		{
			if (enabled)
			{
				StartCoroutine(PlayCheckSound());
			}
		}

		private IEnumerator PlayCheckSound()
		{
			yield return new WaitForEndOfFrame();
			UICheckbox checkBox = GetComponent<UICheckbox>();
			if (checkBox != null)
			{
				if (checkBox.isChecked && !String.IsNullOrEmpty(audioIDCheck))
				{
					Play(audioIDCheck);
				}
				if (!checkBox.isChecked && !String.IsNullOrEmpty(audioIDUncheck))
				{
					Play(audioIDUncheck);
				}
			}
			else
			{
				Debug.LogError("GUICheckSound: UICheckbox not found!");
			}
			yield break;
		}

		private void Play(String p_audioID)
		{
			if (FXMainCamera.Instance != null)
			{
				AudioController.Play(p_audioID, FXMainCamera.Instance.transform);
			}
			else
			{
				AudioController.Play(p_audioID);
			}
		}
	}
}
