using System;
using UnityEngine;

namespace Legacy.MMGUI
{
	[RequireComponent(typeof(Camera))]
	public class GUIMainCamera : MonoBehaviour
	{
		public static GUIMainCamera Instance { get; private set; }

		public new Camera camera { get; private set; }

		private void Awake()
		{
			Instance = this;
			camera = base.camera;
		}
	}
}
