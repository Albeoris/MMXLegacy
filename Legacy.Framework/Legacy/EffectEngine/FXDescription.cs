using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Legacy.Animations;
using Legacy.EffectEngine.Effects;
using UnityEngine;
using Object = System.Object;

namespace Legacy.EffectEngine
{
	public class FXDescription : ScriptableObject, IEnumerable<KeyValuePair<EAnimEventType, FXQueue>>, IEnumerable
	{
		[SerializeField]
		internal FXQueue[] m_FXQueueMap = new FXQueue[8];

	    public event EventHandler Finished;

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public FXQueue this[EAnimEventType p_type] => m_FXQueueMap[(Int32)p_type];

	    public Boolean IsFinish { get; private set; }

		public void Configurate(BaseEventHandler p_handler, FXArgs p_args)
		{
			if (p_handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			if (p_args == null)
			{
				throw new ArgumentNullException("args");
			}
			for (Int32 i = 0; i < m_FXQueueMap.Length; i++)
			{
				if (!(m_FXQueueMap[i] == null))
				{
					m_FXQueueMap[i] = Helper.Instantiate<FXQueue>(m_FXQueueMap[i]);
					m_FXQueueMap[i].EffectArguments = p_args;
					m_FXQueueMap[i].Finished += HandleFinishedFXQueue;
					p_handler.RegisterAnimationCallback((EAnimEventType)i, new Action(m_FXQueueMap[i].Execute));
				}
			}
		}

		private void HandleFinishedFXQueue(Object sender, EventArgs e)
		{
			FXQueue y = (FXQueue)sender;
			Int32 num = 0;
			for (Int32 i = 0; i < m_FXQueueMap.Length; i++)
			{
				if (m_FXQueueMap[i] == null || m_FXQueueMap[i] == y)
				{
					m_FXQueueMap[i] = null;
					num++;
				}
			}
			IsFinish = (num == m_FXQueueMap.Length);
			if (IsFinish && Finished != null)
			{
				Finished(this, EventArgs.Empty);
				Finished = null;
			}
			Destroy(this);
		}

		public IEnumerator<KeyValuePair<EAnimEventType, FXQueue>> GetEnumerator()
		{
			for (Int32 i = 0; i < m_FXQueueMap.Length; i++)
			{
				FXQueue queue = m_FXQueueMap[i];
				if (queue != null)
				{
					yield return new KeyValuePair<EAnimEventType, FXQueue>((EAnimEventType)i, queue);
				}
			}
			yield break;
		}

		private void OnEnable()
		{
			if (m_FXQueueMap == null || m_FXQueueMap.Length != 8)
			{
				Array.Resize<FXQueue>(ref m_FXQueueMap, 8);
			}
		}
	}
}
