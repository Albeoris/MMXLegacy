using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public abstract class FXBase : ScriptableObject
	{
		private Single m_FinishTime;

		protected Single m_Lifetime;

		public virtual Boolean IsFinished => m_FinishTime == 0f || Time.time > m_FinishTime;

	    public Single BeginTime => m_FinishTime - m_Lifetime;

	    public Single FinishTime => m_FinishTime;

	    public FXArgs EffectArguments { get; private set; }

		public virtual void Begin(Single p_lifetime, FXArgs p_args)
		{
			if (p_args == null)
			{
				throw new ArgumentNullException("args");
			}
			m_Lifetime = p_lifetime;
			m_FinishTime = Time.time + p_lifetime;
			EffectArguments = p_args;
		}

		public virtual void Update()
		{
		}

		public virtual void Destroy()
		{
		}

		protected void ForceFinish()
		{
			m_FinishTime = 0f;
		}

		protected virtual void OnDestroy()
		{
			EffectArguments = null;
		}

		protected void SendBroadcastBeginEffect<T>(GameObject p_target, T p_args) where T : UnityEventArgs
		{
			p_target.SendBroadcastEvent("OnBeginEffect", p_args);
		}

		protected void SendBroadcastEndEffect<T>(GameObject p_target, T p_args) where T : UnityEventArgs
		{
			p_target.SendBroadcastEvent("OnEndEffect", p_args);
		}
	}
}
