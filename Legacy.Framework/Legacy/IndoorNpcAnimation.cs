using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Utility/IndoorNpcAnimation")]
	public class IndoorNpcAnimation : MonoBehaviour
	{
		private Animation m_Animation;

		private Single m_clipEndTime;

		private Single m_nextRndClipEndTime;

		private List<String> m_clipNames;

		public Single m_randomPlayClipMin = 20f;

		public Single m_randomPlayClipMax = 30f;

		private void Awake()
		{
			m_Animation = animation;
			m_Animation.playAutomatically = true;
			m_clipNames = new List<String>(m_Animation.GetClipCount() - 1);
			IEnumerator enumerator = m_Animation.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Object obj = enumerator.Current;
					AnimationState animationState = (AnimationState)obj;
					if (animationState.clip != m_Animation.clip)
					{
						animationState.wrapMode = WrapMode.Once;
						m_clipNames.Add(animationState.clip.name);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			if (m_clipNames.Count == 0)
			{
				m_nextRndClipEndTime = -1f;
			}
		}

		private void Update()
		{
			if (m_clipEndTime <= Time.time)
			{
				Single length = m_Animation.clip.length;
				m_clipEndTime = Time.time + length - 2f;
				m_Animation.CrossFadeQueued(m_Animation.clip.name, 0.8f, QueueMode.PlayNow);
			}
			if (m_nextRndClipEndTime != -1f && m_nextRndClipEndTime <= Time.time)
			{
				String text = m_clipNames.RandomElement<String>();
				Single length2 = m_Animation[text].clip.length;
				m_nextRndClipEndTime = Time.time + length2 + m_Animation.clip.length + Random.Range(m_randomPlayClipMin, m_randomPlayClipMax);
				m_clipEndTime = Time.time + length2 - 1f;
				m_Animation.CrossFadeQueued(text, 1f, QueueMode.PlayNow);
			}
		}
	}
}
