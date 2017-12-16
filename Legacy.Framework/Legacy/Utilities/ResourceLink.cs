using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/Resource Link")]
	public class ResourceLink : ScriptableObject
	{
		[SerializeField]
		private UnityEngine.Object m_linkTarget;

		public UnityEngine.Object Link => m_linkTarget;
	}
}
