using System;
using UnityEngine;

[AddComponentMenu("Camera-Control/Smooth Mouse Orbit - Unluck Software")]
public class SmoothCameraOrbit : MonoBehaviour
{
	public Transform target;

	public Vector3 targetOffset;

	public Single distance = 5f;

	public Single maxDistance = 20f;

	public Single minDistance = 0.6f;

	public Single xSpeed = 200f;

	public Single ySpeed = 200f;

	public Int32 yMinLimit = -80;

	public Int32 yMaxLimit = 80;

	public Int32 zoomRate = 40;

	public Single panSpeed = 0.3f;

	public Single zoomDampening = 5f;

	public Single autoRotate = 1f;

	private Single xDeg;

	private Single yDeg;

	private Single currentDistance;

	private Single desiredDistance;

	private Quaternion currentRotation;

	private Quaternion desiredRotation;

	private Quaternion rotation;

	private Vector3 position;

	private Single idleTimer;

	private Single idleSmooth;

	private void Start()
	{
		Init();
	}

	private void OnEnable()
	{
		Init();
	}

	public void Init()
	{
		if (!target)
		{
			target = new GameObject("Cam Target")
			{
				transform = 
				{
					position = transform.position + transform.forward * distance
				}
			}.transform;
		}
		currentDistance = distance;
		desiredDistance = distance;
		position = transform.position;
		rotation = transform.rotation;
		currentRotation = transform.rotation;
		desiredRotation = transform.rotation;
		xDeg = Vector3.Angle(Vector3.right, transform.right);
		yDeg = Vector3.Angle(Vector3.up, transform.up);
		position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
	}

	private void LateUpdate()
	{
		if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl))
		{
			desiredDistance -= Input.GetAxis("Mouse Y") * 0.02f * zoomRate * 0.125f * Mathf.Abs(desiredDistance);
		}
		else if (Input.GetMouseButton(0))
		{
			xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
			yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
			yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
			desiredRotation = Quaternion.Euler(yDeg, xDeg, 0f);
			currentRotation = transform.rotation;
			rotation = Quaternion.Lerp(currentRotation, desiredRotation, 0.02f * zoomDampening);
			transform.rotation = rotation;
			idleTimer = 0f;
			idleSmooth = 0f;
		}
		else
		{
			idleTimer += 0.02f;
			if (idleTimer > autoRotate && autoRotate > 0f)
			{
				idleSmooth += (0.02f + idleSmooth) * 0.005f;
				idleSmooth = Mathf.Clamp(idleSmooth, 0f, 1f);
				xDeg += xSpeed * 0.001f * idleSmooth;
			}
			yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
			desiredRotation = Quaternion.Euler(yDeg, xDeg, 0f);
			currentRotation = transform.rotation;
			rotation = Quaternion.Lerp(currentRotation, desiredRotation, 0.02f * zoomDampening * 2f);
			transform.rotation = rotation;
		}
		desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * 0.02f * zoomRate * Mathf.Abs(desiredDistance);
		desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
		currentDistance = Mathf.Lerp(currentDistance, desiredDistance, 0.02f * zoomDampening);
		position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
		transform.position = position;
	}

	private static Single ClampAngle(Single angle, Single min, Single max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}
}
