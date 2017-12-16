using System;
using UnityEngine;

namespace Legacy
{
	[Serializable]
	public sealed class AssetWeakReference
	{
		[SerializeField]
		private String m_AssetGUID;

		public AssetWeakReference()
		{
		}

		public AssetWeakReference(UnityEngine.Object target)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			Target = target;
		}

		public Boolean IsAlive => false;

	    public UnityEngine.Object Target
		{
			get => null;
	        set => Debug.LogError("AssetWeakReference: UNITY_EDITOR compiler symbol not defined!");
	    }
	}
}
