using System;
using UnityEngine;

namespace Legacy.Game.IngameManagement
{
	[AddComponentMenu("MM Legacy/IngameManagement/OverlayManager")]
	public class OverlayManager : MonoBehaviour
	{
		[SerializeField]
		private UITexture m_frontOverlay;

		[SerializeField]
		private UITexture m_backOverlay;

		private TweenAlpha m_frontTween;

		private TweenAlpha m_backTween;

		private EventHandler m_fadeFrontFinishCallback;

		private EventHandler m_fadeBackFinishCallback;

		public static OverlayManager Instance { get; private set; }

		public Single FrontAlpha
		{
			get => m_frontOverlay.alpha;
		    set => m_frontOverlay.alpha = Mathf.Clamp01(value);
		}

		public Single BackAlpha
		{
			get => m_backOverlay.alpha;
		    set => m_backOverlay.alpha = Mathf.Clamp01(value);
		}

		public void FadeFrontTo(Single alpha)
		{
			FadeFrontTo(alpha, 1f, null);
		}

		public void FadeFrontTo(Single alpha, Single blendTime, EventHandler callback)
		{
			alpha = Mathf.Clamp01(alpha);
			m_frontOverlay.gameObject.SetActive(true);
			m_frontTween = TweenAlpha.Begin(m_frontOverlay.gameObject, blendTime, alpha);
			m_frontTween.onFinished = new UITweener.OnFinished(FinishTween);
			InvokeCallback(ref m_fadeFrontFinishCallback);
			m_fadeFrontFinishCallback = callback;
			OnResolutionChange();
		}

		public void FadeBackTo(Single alpha)
		{
			FadeBackTo(alpha, 1f, null);
		}

		public void FadeBackTo(Single alpha, Single blendTime, EventHandler callback)
		{
			alpha = Mathf.Clamp01(alpha);
			m_backOverlay.gameObject.SetActive(true);
			m_backTween = TweenAlpha.Begin(m_backOverlay.gameObject, blendTime, alpha);
			m_backTween.onFinished = new UITweener.OnFinished(FinishTween);
			InvokeCallback(ref m_fadeBackFinishCallback);
			m_fadeBackFinishCallback = callback;
			OnResolutionChange();
		}

		private void Awake()
		{
			Instance = this;
			FrontAlpha = 0f;
			BackAlpha = 0f;
			OnResolutionChange();
			m_frontOverlay.gameObject.SetActive(false);
			m_backOverlay.gameObject.SetActive(false);
		}

		private void OnDestroy()
		{
			if (m_frontTween != null)
			{
				m_frontTween.onFinished = null;
			}
			if (m_backTween != null)
			{
				m_backTween.onFinished = null;
			}
		}

		private void OnResolutionChange()
		{
			Single num = 1536f / Screen.height;
			m_frontOverlay.transform.localScale = new Vector3((Int32)(Screen.width * num + 5f), 1541f, 1f);
			m_backOverlay.transform.localScale = new Vector3((Int32)(Screen.width * num + 5f), 1541f, 1f);
		}

		private void FinishTween(UITweener sender)
		{
			if (sender == m_frontTween)
			{
				InvokeCallback(ref m_fadeFrontFinishCallback);
				if (m_frontTween.alpha == 0f)
				{
					m_frontTween.gameObject.SetActive(false);
				}
			}
			else if (sender == m_backTween)
			{
				InvokeCallback(ref m_fadeBackFinishCallback);
				if (m_backTween.alpha == 0f)
				{
					m_backTween.gameObject.SetActive(false);
				}
			}
		}

		private void InvokeCallback(ref EventHandler callback)
		{
			EventHandler eventHandler = callback;
			callback = null;
			if (eventHandler != null)
			{
				try
				{
					eventHandler(this, EventArgs.Empty);
				}
				catch (Exception message)
				{
					Debug.LogError(message);
				}
			}
		}
	}
}
