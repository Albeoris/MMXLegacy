using System;
using UnityEngine;

public class UIGeometry
{
	public BetterList<Vector3> verts = new BetterList<Vector3>();

	public BetterList<Vector2> uvs = new BetterList<Vector2>();

	public BetterList<Color32> cols = new BetterList<Color32>();

	private BetterList<Vector3> mRtpVerts = new BetterList<Vector3>();

	private Vector3 mRtpNormal;

	private Vector4 mRtpTan;

	public Boolean hasVertices => verts.size > 0;

    public Boolean hasTransformed => mRtpVerts != null && mRtpVerts.size > 0 && mRtpVerts.size == verts.size;

    public void Clear()
	{
		verts.Clear();
		uvs.Clear();
		cols.Clear();
		mRtpVerts.Clear();
	}

	public void ApplyOffset(Vector3 pivotOffset)
	{
		Int32 num = 0;
		while (num < verts.buffer.Length && num < verts.size)
		{
			verts.buffer[num] += pivotOffset;
			num++;
		}
	}

	public void ApplyTransform(Matrix4x4 widgetToPanel, Boolean normals)
	{
		if (verts.size > 0)
		{
			mRtpVerts.Clear();
			mRtpVerts.AddRanged(verts.buffer, 0, verts.size);
			Int32 num = 0;
			while (num < mRtpVerts.buffer.Length && num < mRtpVerts.size)
			{
				mRtpVerts.buffer[num] = widgetToPanel.MultiplyPoint3x4(mRtpVerts.buffer[num]);
				num++;
			}
			mRtpNormal = widgetToPanel.MultiplyVector(Vector3.back).normalized;
			Vector3 normalized = widgetToPanel.MultiplyVector(Vector3.right).normalized;
			mRtpTan = new Vector4(normalized.x, normalized.y, normalized.z, -1f);
		}
		else
		{
			mRtpVerts.Clear();
		}
	}

	public void WriteToBuffers(BetterList<Vector3> v, BetterList<Vector2> u, BetterList<Color32> c, BetterList<Vector3> n, BetterList<Vector4> t)
	{
		if (mRtpVerts != null && mRtpVerts.size > 0)
		{
			if (n == null)
			{
				v.AddRanged(mRtpVerts.buffer, 0, mRtpVerts.size);
				u.AddRanged(uvs.buffer, 0, mRtpVerts.size);
				c.AddRanged(cols.buffer, 0, mRtpVerts.size);
			}
			else
			{
				v.AddRanged(mRtpVerts.buffer, 0, mRtpVerts.size);
				u.AddRanged(uvs.buffer, 0, mRtpVerts.size);
				c.AddRanged(cols.buffer, 0, mRtpVerts.size);
				n.Reserve(mRtpVerts.size);
				t.Reserve(mRtpVerts.size);
				for (Int32 i = 0; i < mRtpVerts.size; i++)
				{
					n.Add(mRtpNormal);
					t.Add(mRtpTan);
				}
			}
		}
	}
}
