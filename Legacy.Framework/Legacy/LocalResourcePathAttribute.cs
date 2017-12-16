using System;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public sealed class LocalResourcePathAttribute : TooltipAttribute
	{
		public LocalResourcePathAttribute()
		{
		}

		public LocalResourcePathAttribute(String tooltip) : base(tooltip)
		{
		}

		public static T LoadAsset<T>(String path) where T : UnityEngine.Object
		{
			if (path != null && path.Length > 32)
			{
				return Resources.Load(path.Substring(32), typeof(T)) as T;
			}
			return null;
		}
	}
}
