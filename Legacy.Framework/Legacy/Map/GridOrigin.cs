using System;
using UnityEngine;

namespace Legacy.Map
{
	[AddComponentMenu("MM Legacy/Map/GridOrigin")]
	public class GridOrigin : MonoBehaviour
	{
		[SerializeField]
		private AssetWeakReference m_GridDataRef = new AssetWeakReference();

		public UnityEngine.Object GridData
		{
			get => m_GridDataRef.Target;
		    set => m_GridDataRef.Target = value;
		}
	}
}
