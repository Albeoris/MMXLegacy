using System;
using System.Collections;
using Legacy.Core.Map;
using UnityEngine;

namespace Legacy.Views.Cutscenes
{
	internal class CutscenePaoKaiEntrance : MonoBehaviour
	{
		[SerializeField]
		private Animation m_PaoKaiAnimation;

		[SerializeField]
		private GameObject m_CameraAnchor;

		[SerializeField]
		private Int32 m_PaoKaiSpawnerID;

		[SerializeField]
		private Int32 m_PaoKaiPosX;

		[SerializeField]
		private Int32 m_PaoKaiPosY;

		[SerializeField]
		private EDirection m_PaoKaiDir;

		private CutsceneView m_CutsceneView;

		private void Awake()
		{
			StartCoroutine(PlayAnim());
		}

		private IEnumerator PlayAnim()
		{
			yield return new WaitForSeconds(3f);
			m_PaoKaiAnimation.Play("paokai_l05_altar_entry");
			Single length = m_PaoKaiAnimation["paokai_l05_altar_entry"].length - 1.5f;
			yield return new WaitForSeconds(length);
			yield break;
		}
	}
}
