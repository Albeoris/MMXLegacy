using System;
using UnityEngine;

public class CrushingWeight : MonoBehaviour
{
	public Single AppearAt = 1f;

	public Single BreakAt = 2f;

	public Single DisappearAt = 3f;

	private Single startTime;

	private Boolean appeared;

	private Boolean broken;

	private GameObject[] Stones;

	private GameObject MainStone;

	private ParticleSystem MainStoneExplode;

	private Transform[] allChildren;

	private void Start()
	{
		startTime = Time.time;
		MainStone = transform.FindChild("MainStone").gameObject;
		MainStoneExplode = transform.FindChild("ExplodeTheStone").particleSystem;
		allChildren = GetComponentsInChildren<Transform>();
		transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		AudioController.Play("EarthCrushingWeightStoneGrindLong", transform);
	}

	private void Update()
	{
		if (!appeared && startTime + AppearAt <= Time.time)
		{
			foreach (Transform transform in allChildren)
			{
				if (transform.name == "crushingwheigthstone")
				{
					AudioController.Play("EarthCrushingWeightStoneGrind", transform);
					transform.renderer.enabled = true;
				}
			}
			MainStone.renderer.enabled = true;
			appeared = true;
		}
		if (appeared)
		{
			transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1f, 1f, 1f), 0.1f);
		}
		if (appeared && gameObject.transform.localScale.x > 0.97f)
		{
			transform.localScale = new Vector3(1f, 1f, 1f);
		}
		if (!broken && startTime + BreakAt <= Time.time)
		{
			AudioController.Play("EarthCrushingWeightExplosion", MainStone.transform);
			MainStoneExplode.Play();
			foreach (Transform transform2 in allChildren)
			{
				if (transform2.name == "crushingwheigthstone")
				{
					transform2.collider.enabled = true;
					transform2.rigidbody.AddForce(UnityEngine.Random.insideUnitSphere * UnityEngine.Random.value * 8f);
					transform2.rigidbody.AddForce(Vector3.up * (UnityEngine.Random.value - 0.85f) * 500f);
					transform2.rigidbody.useGravity = true;
				}
			}
			MainStone.renderer.enabled = false;
			MainStone.collider.enabled = true;
			broken = true;
		}
		if (startTime + DisappearAt <= Time.time)
		{
			transform.position -= new Vector3(0f, 0.05f, 0f);
		}
		if (startTime + DisappearAt + 1f <= Time.time)
		{
			Destroy(transform.parent.gameObject);
		}
	}
}
