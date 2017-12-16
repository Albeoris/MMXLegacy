using System;
using Legacy.EffectEngine.Effects;
using UnityEngine;

internal class TestHUD : MonoBehaviour
{
	[SerializeField]
	private TestMain m_Main;

	private void OnGUI()
	{
		if (m_Main.PartyBuffEffects.Count > 0 || m_Main.MonsterBuffEffects.Count > 0)
		{
			GUILayout.BeginArea(new Rect(0f, Screen.height - 30, Screen.width, 30f));
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Stop buff effect", new GUILayoutOption[0]))
			{
				foreach (BuffFX buffFX in m_Main.PartyBuffEffects)
				{
					buffFX.Destroy();
				}
				foreach (BuffFX buffFX2 in m_Main.MonsterBuffEffects)
				{
					buffFX2.Destroy();
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
	}
}
