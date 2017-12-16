using System;
using Legacy.EffectEngine;
using Legacy.Game.IngameManagement;
using UnityEngine;

namespace Legacy.Game.Cheats
{
	[AddComponentMenu("MM Legacy/Cheats/CheatsCamera")]
	public class CheatsCamera : MonoBehaviour
	{
		private Vector3 oldPosition;

		private Quaternion oldRotation;

		private void OnClickFreeCamera()
		{
			GameObject currentCamera = FXMainCamera.Instance.CurrentCamera;
			MonoBehaviour monoBehaviour = (MonoBehaviour)currentCamera.GetComponent("TestCamera");
			if (monoBehaviour == null)
			{
				monoBehaviour = (MonoBehaviour)currentCamera.AddComponent("TestCamera");
				monoBehaviour.enabled = false;
			}
			if (!monoBehaviour.enabled)
			{
				monoBehaviour.enabled = true;
				oldPosition = currentCamera.transform.localPosition;
				oldRotation = currentCamera.transform.localRotation;
				IngameController.Instance.IngameInput.MovementActive = false;
			}
			else
			{
				monoBehaviour.enabled = false;
				currentCamera.transform.localPosition = oldPosition;
				currentCamera.transform.localRotation = oldRotation;
				IngameController.Instance.IngameInput.MovementActive = true;
			}
			FreeRotationCamera component = currentCamera.transform.parent.GetComponent<FreeRotationCamera>();
			component.enabled = !component.enabled;
		}
	}
}
