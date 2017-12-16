using System;
using UnityEngine;

namespace Legacy
{
	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public class TooltipAttribute : PropertyAttribute
	{
		public readonly String Tooltip;

		public TooltipAttribute()
		{
		}

		public TooltipAttribute(String tooltip)
		{
			Tooltip = tooltip;
		}
	}
}
