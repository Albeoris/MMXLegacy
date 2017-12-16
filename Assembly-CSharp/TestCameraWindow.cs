using System;
using UnityEngine;

public class TestCameraWindow : MonoBehaviour
{
	private Rect m_WindowRect;

	[SerializeField]
	private Targets[] m_Target;

	[SerializeField]
	private TestCamera m_Camera;

	private void Start()
	{
		if (m_Target.Length > 0)
		{
			ChangeViewTarget(m_Target[0]);
		}
	}

	private void OnGUI()
	{
		m_WindowRect = GUILayout.Window(0, m_WindowRect, new GUI.WindowFunction(Window), "Camera", (GUILayoutOption[])null);
		m_WindowRect.x = Mathf.Clamp(m_WindowRect.x, 0f, Screen.width - m_WindowRect.width);
		m_WindowRect.y = Mathf.Clamp(m_WindowRect.y, 0f, Screen.height - m_WindowRect.height);
	}

	private void Window(Int32 id)
	{
		for (Int32 i = 0; i < m_Target.Length; i++)
		{
			Targets targets = m_Target[i];
			if (GUILayout.Button(targets.Name, new GUILayoutOption[0]))
			{
				ChangeViewTarget(targets);
			}
		}
		GUI.DragWindow();
	}

	private void ChangeViewTarget(Targets target)
	{
		if (target.Target != null)
		{
			m_Camera.transform.position = target.Target.position;
			m_Camera.transform.rotation = target.Target.rotation;
		}
		m_Camera.Reset();
		m_Camera.enabled = !target.Lock;
	}

	[Serializable]
	private class Targets
	{
		public Transform Target;

		public String Name;

		public Boolean Lock;
	}
}
