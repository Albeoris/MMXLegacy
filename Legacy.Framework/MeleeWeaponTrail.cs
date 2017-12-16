using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("MM Legacy/Effects/Weapon Trails")]
public class MeleeWeaponTrail : MonoBehaviour
{
	[SerializeField]
	private Boolean _emit;

	private Boolean _use = true;

	[SerializeField]
	private Single _emitTime;

	[SerializeField]
	private Material _material;

	[SerializeField]
	private Single _lifeTime = 0.25f;

	[SerializeField]
	private Color[] _colors;

	[SerializeField]
	private Single[] _sizes;

	[SerializeField]
	private Single _minVertexDistance = 0.1f;

	[SerializeField]
	private Single _maxVertexDistance = 10f;

	private Single _minVertexDistanceSqr;

	private Single _maxVertexDistanceSqr;

	[SerializeField]
	private Single _maxAngle = 3f;

	[SerializeField]
	private Boolean _autoDestruct;

	[SerializeField]
	private Transform _base;

	[SerializeField]
	private Transform _tip;

	private List<Point> _points = new List<Point>();

	private GameObject _trailObject;

	private Mesh _trailMesh;

	private Vector3 _lastPosition;

	private static GameObject _sceneTrails;

	public Boolean Emit
	{
		set => _emit = value;
	}

	public Boolean Use
	{
		set => _use = value;
	}

	public static GameObject SceneTrails
	{
		get
		{
			if (_sceneTrails == null)
			{
				_sceneTrails = new GameObject("_SceneTrails");
			}
			return _sceneTrails;
		}
	}

	private void Start()
	{
		_lastPosition = _base.transform.position;
		_trailObject = new GameObject("Trail");
		_trailObject.transform.parent = SceneTrails.transform;
		_trailObject.AddComponent<MeshRenderer>().material = _material;
		_trailMesh = new Mesh();
		_trailMesh.name = name + "TrailMesh";
		_trailObject.AddComponent<MeshFilter>().mesh = _trailMesh;
		_minVertexDistanceSqr = _minVertexDistance * _minVertexDistance;
		_maxVertexDistanceSqr = _maxVertexDistance * _maxVertexDistance;
	}

	private void OnDestroy()
	{
		Destroy(_trailMesh);
		Destroy(_trailObject);
	}

	private void LateUpdate()
	{
		if (!_use)
		{
			return;
		}
		if (_emit && _emitTime != 0f)
		{
			_emitTime -= Time.deltaTime;
			if (_emitTime == 0f)
			{
				_emitTime = -1f;
			}
			if (_emitTime < 0f)
			{
				_emit = false;
			}
		}
		if (!_emit && _points.Count == 0 && _autoDestruct)
		{
			Destroy(gameObject);
		}
		if (!Camera.main)
		{
			return;
		}
		Single sqrMagnitude = (_lastPosition - _base.transform.position).sqrMagnitude;
		if (_emit)
		{
			if (sqrMagnitude > _minVertexDistanceSqr)
			{
				Boolean flag = false;
				if (_points.Count < 3)
				{
					flag = true;
				}
				else
				{
					Vector3 from = _points[_points.Count - 2].tipPosition - _points[_points.Count - 3].tipPosition;
					Vector3 to = _points[_points.Count - 1].tipPosition - _points[_points.Count - 2].tipPosition;
					if (Vector3.Angle(from, to) > _maxAngle || sqrMagnitude > _maxVertexDistanceSqr)
					{
						flag = true;
					}
				}
				if (flag)
				{
					Point point = new Point();
					point.basePosition = _base.position;
					point.tipPosition = _tip.position;
					point.timeCreated = Time.time;
					_points.Add(point);
					_lastPosition = _base.transform.position;
				}
				else
				{
					_points[_points.Count - 1].basePosition = _base.position;
					_points[_points.Count - 1].tipPosition = _tip.position;
				}
			}
			else if (_points.Count > 0)
			{
				_points[_points.Count - 1].basePosition = _base.position;
				_points[_points.Count - 1].tipPosition = _tip.position;
			}
		}
		RemoveOldPoints(_points);
		if (_points.Count == 0)
		{
			_trailMesh.Clear();
		}
		List<Point> points = _points;
		if (points.Count > 1)
		{
			Vector3[] array = new Vector3[points.Count * 2];
			Vector2[] array2 = new Vector2[points.Count * 2];
			Int32[] array3 = new Int32[(points.Count - 1) * 6];
			Color[] array4 = new Color[points.Count * 2];
			for (Int32 i = 0; i < points.Count; i++)
			{
				Point point2 = points[i];
				Single num = (Time.time - point2.timeCreated) / _lifeTime;
				Color color = Color.Lerp(Color.white, Color.clear, num);
				if (_colors != null && _colors.Length > 0)
				{
					Single num2 = num * (_colors.Length - 1);
					Single num3 = Mathf.Floor(num2);
					Single num4 = Mathf.Clamp(Mathf.Ceil(num2), 1f, _colors.Length - 1);
					Single t = Mathf.InverseLerp(num3, num4, num2);
					if (num3 >= _colors.Length)
					{
						num3 = _colors.Length - 1;
					}
					if (num3 < 0f)
					{
						num3 = 0f;
					}
					if (num4 >= _colors.Length)
					{
						num4 = _colors.Length - 1;
					}
					if (num4 < 0f)
					{
						num4 = 0f;
					}
					color = Color.Lerp(_colors[(Int32)num3], _colors[(Int32)num4], t);
				}
				Single num5 = 0f;
				if (_sizes != null && _sizes.Length > 0)
				{
					Single num6 = num * (_sizes.Length - 1);
					Single num7 = Mathf.Floor(num6);
					Single num8 = Mathf.Clamp(Mathf.Ceil(num6), 1f, _sizes.Length - 1);
					Single t2 = Mathf.InverseLerp(num7, num8, num6);
					if (num7 >= _sizes.Length)
					{
						num7 = _sizes.Length - 1;
					}
					if (num7 < 0f)
					{
						num7 = 0f;
					}
					if (num8 >= _sizes.Length)
					{
						num8 = _sizes.Length - 1;
					}
					if (num8 < 0f)
					{
						num8 = 0f;
					}
					num5 = Mathf.Lerp(_sizes[(Int32)num7], _sizes[(Int32)num8], t2);
				}
				Vector3 a = point2.tipPosition - point2.basePosition;
				array[i * 2] = point2.basePosition - a * (num5 * 0.5f);
				array[i * 2 + 1] = point2.tipPosition + a * (num5 * 0.5f);
				array4[i * 2] = (array4[i * 2 + 1] = color);
				Single x = i / (Single)points.Count;
				array2[i * 2] = new Vector2(x, 0f);
				array2[i * 2 + 1] = new Vector2(x, 1f);
				if (i > 0)
				{
					array3[(i - 1) * 6] = i * 2 - 2;
					array3[(i - 1) * 6 + 1] = i * 2 - 1;
					array3[(i - 1) * 6 + 2] = i * 2;
					array3[(i - 1) * 6 + 3] = i * 2 + 1;
					array3[(i - 1) * 6 + 4] = i * 2;
					array3[(i - 1) * 6 + 5] = i * 2 - 1;
				}
			}
			_trailMesh.Clear();
			_trailMesh.vertices = array;
			_trailMesh.colors = array4;
			_trailMesh.uv = array2;
			_trailMesh.triangles = array3;
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (_base == null || _tip == null)
		{
			return;
		}
		Gizmos.DrawLine(_base.position, _tip.position);
	}

	private void RemoveOldPoints(List<Point> pointList)
	{
		List<Point> list = new List<Point>();
		foreach (Point point in pointList)
		{
			if (Time.time - point.timeCreated > _lifeTime)
			{
				list.Add(point);
			}
		}
		foreach (Point item in list)
		{
			pointList.Remove(item);
		}
	}

	[Serializable]
	private class Point
	{
		public Single timeCreated;

		public Vector3 basePosition;

		public Vector3 tipPosition;
	}
}
