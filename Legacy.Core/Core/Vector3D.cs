using System;

namespace Legacy.Core
{
	public struct Vector3D
	{
		public Single X;

		public Single Y;

		public Single Z;

		public Vector3D(Single p_x, Single p_y, Single p_z)
		{
			X = p_x;
			Y = p_y;
			Z = p_z;
		}

		public static Vector3D Zero => new Vector3D(0f, 0f, 0f);
	}
}
