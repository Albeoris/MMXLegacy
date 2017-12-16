using System;
using System.Collections;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Views/Scene/SceneEventView_ColorTrigger")]
	public class SceneEventView_ColorTrigger : BaseView
	{
		[SerializeField]
		private String m_viewListenCommandName;

		[SerializeField]
		private Single m_BlendTime = 1.5f;

		[SerializeField]
		private Renderer[] m_Renderers;

		[SerializeField]
		private ParticleAnimator[] m_ParticleAnimator;

		[SerializeField]
		private Color[] m_Blue;

		[SerializeField]
		private Color[] m_Green;

		[SerializeField]
		private Color[] m_Purple;

		[SerializeField]
		private Color[] m_Red;

		[SerializeField]
		private Color[] m_Yellow;

		private void Start()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PREFAB_CONTAINER_TRIGGER_ANIM, new EventHandler(OnAnimTriggered));
		}

		private void OnAnimTriggered(Object p_sender, EventArgs p_args)
		{
			StringEventArgs stringEventArgs = p_args as StringEventArgs;
			if (stringEventArgs != null && p_sender == null)
			{
				String[] array = stringEventArgs.text.Split(new Char[]
				{
					'_'
				});
				if (array.Length > 1 && array[0] == m_viewListenCommandName)
				{
					StopAllCoroutines();
					String text = array[1];
					switch (text)
					{
					case "Blue":
						StartCoroutine(ColorTween(m_Blue));
						return;
					case "Green":
						StartCoroutine(ColorTween(m_Green));
						return;
					case "Purple":
						StartCoroutine(ColorTween(m_Purple));
						return;
					case "Red":
						StartCoroutine(ColorTween(m_Red));
						return;
					case "Yellow":
						StartCoroutine(ColorTween(m_Yellow));
						return;
					}
					Debug.LogError("Fail parse color value " + array[1]);
				}
			}
		}

		private IEnumerator ColorTween(Color[] targetColors)
		{
			WaitForEndOfFrame wait = new WaitForEndOfFrame();
			Single timeEnd = Time.time + m_BlendTime;
			Color[] materialBaseColors = new Color[m_Renderers.Length];
			Color[] materialFlowColors = new Color[m_Renderers.Length];
			Color[,] ParticleColors = new Color[m_ParticleAnimator.Length, 5];
			for (Int32 i = 0; i < m_Renderers.Length; i++)
			{
				materialBaseColors[i] = m_Renderers[i].sharedMaterial.GetColor("_BaseColor");
				materialFlowColors[i] = m_Renderers[i].sharedMaterial.GetColor("_FlowColor");
			}
			for (Int32 j = 0; j < m_ParticleAnimator.Length; j++)
			{
				ParticleColors[j, 0] = m_ParticleAnimator[j].colorAnimation[0];
				ParticleColors[j, 1] = m_ParticleAnimator[j].colorAnimation[1];
				ParticleColors[j, 2] = m_ParticleAnimator[j].colorAnimation[2];
				ParticleColors[j, 3] = m_ParticleAnimator[j].colorAnimation[3];
				ParticleColors[j, 4] = m_ParticleAnimator[j].colorAnimation[4];
			}
			for (;;)
			{
				Single blend = (m_BlendTime > 0f) ? Mathf.Clamp01(1f - (timeEnd - Time.time) / m_BlendTime) : 1f;
				for (Int32 k = 0; k < m_Renderers.Length; k++)
				{
					m_Renderers[k].sharedMaterial.SetColor("_BaseColor", Color.Lerp(materialBaseColors[k], targetColors[0], blend));
					m_Renderers[k].sharedMaterial.SetColor("_FlowColor", Color.Lerp(materialFlowColors[k], targetColors[1], blend));
				}
				for (Int32 l = 0; l < m_ParticleAnimator.Length; l++)
				{
					Color _A = Color.Lerp(ParticleColors[l, 0], targetColors[0], blend);
					Color _B = Color.Lerp(ParticleColors[l, 1], targetColors[1], blend);
					Color _C = Color.Lerp(ParticleColors[l, 2], targetColors[2], blend);
					Color _D = Color.Lerp(ParticleColors[l, 3], targetColors[3], blend);
					Color _E = Color.Lerp(ParticleColors[l, 4], targetColors[4], blend);
					m_ParticleAnimator[l].colorAnimation = new Color[]
					{
						_A,
						_B,
						_C,
						_D,
						_E
					};
				}
				if (blend == 1f)
				{
					break;
				}
				yield return wait;
			}
			yield break;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PREFAB_CONTAINER_TRIGGER_ANIM, new EventHandler(OnAnimTriggered));
		}

		[ContextMenu("Change Color RED")]
		public void ActivateColorRed()
		{
			OnAnimTriggered(null, new StringEventArgs(m_viewListenCommandName + "_Red"));
		}

		[ContextMenu("Change Color BLUE")]
		public void ActivateColorBlue()
		{
			OnAnimTriggered(null, new StringEventArgs(m_viewListenCommandName + "_Blue"));
		}

		[ContextMenu("Change Color GREEN")]
		public void ActivateColorGreen()
		{
			OnAnimTriggered(null, new StringEventArgs(m_viewListenCommandName + "_Green"));
		}

		[ContextMenu("Change Color YELLOW")]
		public void ActivateColorYellow()
		{
			OnAnimTriggered(null, new StringEventArgs(m_viewListenCommandName + "_Yellow"));
		}

		[ContextMenu("Change Color PURPLE")]
		public void ActivateColorPurple()
		{
			OnAnimTriggered(null, new StringEventArgs(m_viewListenCommandName + "_Purple"));
		}
	}
}
