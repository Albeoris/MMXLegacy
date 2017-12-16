using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class AdvancedTrailRenderer : MonoBehaviour
	{
		public Material material;

		public Material instanceMaterial;

		public Boolean emit = true;

		private Boolean emittingDone;

		public Single lifeTime = 1f;

		private Single lifeTimeRatio = 1f;

		private Single fadeOutRatio;

		public Color[] colors;

		public Single[] widths;

		public Single maxAngle = 2f;

		public Single minVertexDistance = 0.1f;

		public Single maxVertexDistance = 1f;

		public Single optimizeAngleInterval = 0.1f;

		public Single optimizeDistanceInterval = 0.05f;

		public Int32 optimizeCount = 30;

		public GameObject trailObj;

		public Mesh mesh;

		private Point[] points = new Point[100];

		private Int32 pointCnt;

		private void Start()
		{
			trailObj = new GameObject("Trail");
			trailObj.transform.parent = null;
			trailObj.transform.position = Vector3.zero;
			trailObj.transform.rotation = Quaternion.identity;
			trailObj.transform.localScale = Vector3.one;
			MeshFilter meshFilter = (MeshFilter)trailObj.AddComponent(typeof(MeshFilter));
			mesh = meshFilter.mesh;
			trailObj.AddComponent(typeof(MeshRenderer));
			instanceMaterial = new Material(material);
			if (instanceMaterial.HasProperty("_TintColor"))
			{
				fadeOutRatio = 1f / instanceMaterial.GetColor("_TintColor").a;
			}
			else
			{
				fadeOutRatio = 1f;
			}
			trailObj.renderer.material = instanceMaterial;
		}

		private void OnDestroy()
		{
			Destroy(instanceMaterial);
			Destroy(trailObj);
			Destroy(mesh);
		}

		private void Update()
		{
			if (!emit)
			{
				emittingDone = true;
			}
			if (emittingDone)
			{
				emit = false;
			}
			for (Int32 i = pointCnt - 1; i >= 0; i--)
			{
				Point point = points[i];
				if (point != null && point.timeAlive <= lifeTime)
				{
					break;
				}
				points[i] = null;
				pointCnt--;
			}
			if (pointCnt > optimizeCount)
			{
				maxAngle += optimizeAngleInterval;
				maxVertexDistance += optimizeDistanceInterval;
				optimizeCount++;
			}
			if (emit)
			{
				if (pointCnt == 0)
				{
					points[pointCnt++] = new Point(transform);
					points[pointCnt++] = new Point(transform);
				}
				if (pointCnt == 1)
				{
					insertPoint();
				}
				Boolean flag = false;
				Single sqrMagnitude = (points[1].position - transform.position).sqrMagnitude;
				if (sqrMagnitude > minVertexDistance * minVertexDistance)
				{
					if (sqrMagnitude > maxVertexDistance * maxVertexDistance)
					{
						flag = true;
					}
					else if (Quaternion.Angle(transform.rotation, points[1].rotation) > maxAngle)
					{
						flag = true;
					}
				}
				if (flag)
				{
					if (pointCnt == points.Length)
					{
						Array.Resize<Point>(ref points, points.Length + 50);
					}
					insertPoint();
				}
				if (!flag)
				{
					points[0].update(transform);
				}
			}
			if (pointCnt < 2)
			{
				trailObj.renderer.enabled = false;
				return;
			}
			trailObj.renderer.enabled = true;
			lifeTimeRatio = 1f / lifeTime;
			if (emit)
			{
				Vector3[] array = new Vector3[pointCnt * 2];
				Vector2[] array2 = new Vector2[pointCnt * 2];
				Int32[] array3 = new Int32[(pointCnt - 1) * 6];
				Color[] array4 = new Color[pointCnt * 2];
				Single num = 1f / (points[pointCnt - 1].timeAlive - points[0].timeAlive);
				for (Int32 j = 0; j < pointCnt; j++)
				{
					Point point2 = points[j];
					Single num2 = point2.timeAlive * lifeTimeRatio;
					Color color;
					if (colors.Length == 0)
					{
						color = Color.Lerp(Color.white, Color.clear, num2);
					}
					else if (colors.Length == 1)
					{
						color = Color.Lerp(colors[0], Color.clear, num2);
					}
					else if (colors.Length == 2)
					{
						color = Color.Lerp(colors[0], colors[1], num2);
					}
					else
					{
						Single num3 = num2 * (colors.Length - 1);
						Int32 num4 = (Int32)Mathf.Floor(num3);
						Single t = Mathf.InverseLerp(num4, num4 + 1, num3);
						color = Color.Lerp(colors[num4], colors[num4 + 1], t);
					}
					array4[j * 2] = color;
					array4[j * 2 + 1] = color;
					Single num5;
					if (widths.Length == 0)
					{
						num5 = 1f;
					}
					else if (widths.Length == 1)
					{
						num5 = widths[0];
					}
					else if (widths.Length == 2)
					{
						num5 = Mathf.Lerp(widths[0], widths[1], num2);
					}
					else
					{
						Single num6 = num2 * (widths.Length - 1);
						Int32 num7 = (Int32)Mathf.Floor(num6);
						Single t2 = Mathf.InverseLerp(num7, num7 + 1, num6);
						num5 = Mathf.Lerp(widths[num7], widths[num7 + 1], t2);
					}
					trailObj.transform.position = point2.position;
					trailObj.transform.rotation = point2.rotation;
					array[j * 2] = trailObj.transform.TransformPoint(0f, num5 * 0.5f, 0f);
					array[j * 2 + 1] = trailObj.transform.TransformPoint(0f, -num5 * 0.5f, 0f);
					Single x = (point2.timeAlive - points[0].timeAlive) * num;
					array2[j * 2] = new Vector2(x, 0f);
					array2[j * 2 + 1] = new Vector2(x, 1f);
					if (j > 0)
					{
						Int32 num8 = (j - 1) * 6;
						Int32 num9 = j * 2;
						array3[num8] = num9 - 2;
						array3[num8 + 1] = num9 - 1;
						array3[num8 + 2] = num9;
						array3[num8 + 3] = num9 + 1;
						array3[num8 + 4] = num9;
						array3[num8 + 5] = num9 - 1;
					}
				}
				trailObj.transform.position = Vector3.zero;
				trailObj.transform.rotation = Quaternion.identity;
				mesh.Clear();
				mesh.vertices = array;
				mesh.colors = array4;
				mesh.uv = array2;
				mesh.triangles = array3;
				return;
			}
			if (pointCnt == 0)
			{
				return;
			}
			Color color2 = instanceMaterial.GetColor("_TintColor");
			color2.a -= fadeOutRatio * lifeTimeRatio * Time.deltaTime;
			if (color2.a > 0f)
			{
				instanceMaterial.SetColor("_TintColor", color2);
			}
			else
			{
				Destroy(instanceMaterial);
				Destroy(trailObj);
				Destroy(this);
				Destroy(mesh);
			}
		}

		private void insertPoint()
		{
			for (Int32 i = pointCnt; i > 0; i--)
			{
				points[i] = points[i - 1];
			}
			points[0] = new Point(transform);
			pointCnt++;
		}

		private class Point
		{
			public Single timeCreated;

			public Single fadeAlpha;

			public Vector3 position = Vector3.zero;

			public Quaternion rotation = Quaternion.identity;

			public Point(Transform trans)
			{
				position = trans.position;
				rotation = trans.rotation;
				timeCreated = Time.time;
			}

			public Single timeAlive => Time.time - timeCreated;

		    public void update(Transform trans)
			{
				position = trans.position;
				rotation = trans.rotation;
				timeCreated = Time.time;
			}
		}
	}
}
