using System;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	public struct OrientedBoundingBox
	{
		public Vector3 Extents;

		public Matrix4x4 Transformation;

		public OrientedBoundingBox(Vector3 extents, Matrix4x4 transformation)
		{
			Extents = extents;
			Transformation = transformation;
		}

		public Vector3 Size => Extents * 2f;

	    public Vector3 Center => new Vector3(Transformation.m03, Transformation.m13, Transformation.m23);

	    public Boolean Contains(ref Vector3 point)
		{
			Vector3 vector = Transformation.inverse.MultiplyPoint(point);
			vector.x = Math.Abs(vector.x);
			vector.y = Math.Abs(vector.y);
			vector.z = Math.Abs(vector.z);
			return (Math.Abs(vector.x - Extents.x) < 1E-06f && Math.Abs(vector.y - Extents.y) < 1E-06f && Math.Abs(vector.z - Extents.z) < 1E-06f) || (vector.x < Extents.x && vector.y < Extents.y && vector.z < Extents.z);
		}

		public Boolean Contains(Vector3 point)
		{
			return Contains(ref point);
		}

		public Boolean Equals(OrientedBoundingBox value)
		{
			return Extents == value.Extents && Transformation == value.Transformation;
		}

		public override Boolean Equals(Object value)
		{
			return value is OrientedBoundingBox && Equals((OrientedBoundingBox)value);
		}

		public override Int32 GetHashCode()
		{
			return Extents.GetHashCode() + Transformation.GetHashCode();
		}

		public static Boolean operator ==(OrientedBoundingBox left, OrientedBoundingBox right)
		{
			return left.Equals(right);
		}

		public static Boolean operator !=(OrientedBoundingBox left, OrientedBoundingBox right)
		{
			return !left.Equals(right);
		}
	}
}
