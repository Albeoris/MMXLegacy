using System;
using UnityEngine;

public class RadiantWeapon : MonoBehaviour
{
	private GameObject obj;

	private Camera mainCam = Camera.main;

	private Vector3 HitPos = Vector3.zero;

	private void Start()
	{
		obj = transform.gameObject;
		GetMonster();
	}

	private void GetMonster()
	{
		if (obj.transform.parent.gameObject != transform.root.gameObject)
		{
			obj = obj.transform.parent.gameObject;
			GetMonster();
		}
		else
		{
			Debug.Log(obj.name);
			GameObject gameObject = obj.transform.FindChild("HP_Anchor").gameObject;
			HitPos = obj.transform.position + new Vector3(0f, gameObject.transform.position.y / 1.8f, 0f);
			transform.position = HitPos - mainCam.transform.forward * 2.9f;
			transform.rotation = mainCam.transform.rotation;
		}
	}
}
