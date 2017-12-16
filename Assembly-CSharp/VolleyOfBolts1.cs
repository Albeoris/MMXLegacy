using System;
using UnityEngine;

public class VolleyOfBolts1 : MonoBehaviour
{
	public GameObject[] createThis;

	private Single rndNr;

	public Single delayTime;

	public Int32 thisManyTimes = 3;

	public Single overThisTime = 1f;

	public Single xWidth;

	public Single yWidth;

	public Single zWidth;

	public Single xRotMax;

	public Single yRotMax = 180f;

	public Single zRotMax;

	public Boolean allUseSameRotation;

	private Boolean allRotationDecided;

	public Boolean detachToWorld = true;

	private Single x_cur;

	private Single y_cur;

	private Single z_cur;

	private Single xRotCur;

	private Single yRotCur;

	private Single zRotCur;

	private Single timeCounter;

	private Int32 effectCounter;

	private Single trigger;

	private void Start()
	{
		if (thisManyTimes < 1)
		{
			thisManyTimes = 1;
		}
		trigger = overThisTime / thisManyTimes;
	}

	private void Update()
	{
		if (delayTime > 0f)
		{
			delayTime -= Time.deltaTime;
			return;
		}
		timeCounter += Time.deltaTime;
		if (timeCounter > trigger && effectCounter <= thisManyTimes)
		{
			rndNr = Mathf.Floor(UnityEngine.Random.value * createThis.Length);
			x_cur = transform.position.x + UnityEngine.Random.value * xWidth - xWidth * 0.5f;
			y_cur = transform.position.y + UnityEngine.Random.value * yWidth - yWidth * 0.5f;
			z_cur = transform.position.z + UnityEngine.Random.value * zWidth - zWidth * 0.5f;
			if (!allUseSameRotation || !allRotationDecided)
			{
				xRotCur = transform.rotation.x + UnityEngine.Random.value * xRotMax * 2f - xRotMax;
				yRotCur = transform.rotation.y + UnityEngine.Random.value * yRotMax * 2f - yRotMax;
				zRotCur = transform.rotation.z + UnityEngine.Random.value * zRotMax * 2f - zRotMax;
				allRotationDecided = true;
			}
			GameObject gameObject = (GameObject)Instantiate(createThis[(Int32)rndNr], new Vector3(x_cur, y_cur, z_cur), transform.rotation);
			gameObject.transform.Rotate(xRotCur, yRotCur, zRotCur);
			if (!detachToWorld)
			{
				gameObject.transform.parent = transform;
			}
			timeCounter -= trigger;
			effectCounter++;
		}
		if (effectCounter >= thisManyTimes)
		{
			enabled = false;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(xWidth, yWidth, zWidth));
	}
}
