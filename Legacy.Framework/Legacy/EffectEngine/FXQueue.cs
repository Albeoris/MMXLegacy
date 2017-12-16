using System;
using System.Threading;
using Legacy.EffectEngine.Effects;
using UnityEngine;

namespace Legacy.EffectEngine
{
	public class FXQueue : MonoBehaviour
	{
		[SerializeField]
		internal Entry[] m_Effects = Arrays<Entry>.Empty;

		[SerializeField]
		internal Int32 m_FinishedCallbackIndex = -1;

		private Int32 m_CurrentIndex = -1;

		private Boolean m_IsExecuting;

		private FXArgs m_EffectArgs;

	    public event EventHandler Finished;

		public Boolean IsExecuting => m_IsExecuting;

	    public FXArgs EffectArguments
		{
			get => m_EffectArgs;
	        set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (m_IsExecuting)
				{
					throw new InvalidOperationException();
				}
				m_EffectArgs = value;
			}
		}

		public void Execute()
		{
			if (m_EffectArgs == null)
			{
				throw new InvalidOperationException("EffectArguments not defined!");
			}
			Execute(m_EffectArgs);
		}

		public void Execute(FXArgs args)
		{
			if (m_IsExecuting)
			{
				throw new InvalidOperationException();
			}
			EffectArguments = args;
			m_EffectArgs = args;
			m_IsExecuting = true;
			enabled = true;
		}

		public void SetData(Entry[] p_Effects, Int32 p_FinishedCallbackIndex)
		{
			m_Effects = p_Effects;
			m_FinishedCallbackIndex = p_FinishedCallbackIndex;
		}

		private void Awake()
		{
			enabled = false;
			for (Int32 i = 0; i < m_Effects.Length; i++)
			{
				if (m_Effects[i].m_Effect == null)
				{
					Debug.LogError("Not defined effect at index " + i);
					m_Effects[i] = null;
				}
				else
				{
					m_Effects[i].m_Effect = Helper.Instantiate<FXBase>(m_Effects[i].m_Effect);
				}
			}
		}

		private void OnDestroy()
		{
			for (Int32 i = 0; i < m_Effects.Length; i++)
			{
				if (m_Effects[i] != null)
				{
					m_Effects[i].DestroyEffect();
				}
			}
			ExecuteFinishCallback();
		}

		private void Update()
		{
			if (!m_IsExecuting)
			{
				enabled = false;
				return;
			}
			Single deltaTime = Time.deltaTime;
			Int32 num = 0;
			for (Int32 i = 0; i < m_Effects.Length; i++)
			{
				Entry entry = m_Effects[i];
				try
				{
					if (entry == null)
					{
						num++;
					}
					else
					{
						if (m_CurrentIndex < i)
						{
							m_CurrentIndex = i;
							entry.m_Effect.Begin(entry.m_Lifetime, m_EffectArgs);
						}
						if (entry.m_Effect != null)
						{
							if (!entry.m_Effect.IsFinished)
							{
								entry.m_Effect.Update();
							}
							else
							{
								entry.DestroyEffect();
							}
						}
						else if (entry.m_BlockTime <= 0f)
						{
							m_Effects[i] = null;
							num++;
							if (Finished != null && m_FinishedCallbackIndex == i)
							{
								ExecuteFinishCallback();
							}
							goto IL_13A;
						}
						if (entry.m_BlockTime == -1f)
						{
							break;
						}
						if (entry.m_BlockTime > 0f)
						{
							entry.m_BlockTime -= deltaTime;
							break;
						}
					}
				}
				catch (Exception arg)
				{
					Debug.LogError(this + " " + arg, this);
					num++;
				}
				IL_13A:;
			}
			if (num == m_Effects.Length)
			{
				Destroy(gameObject);
			}
		}

		private void ExecuteFinishCallback()
		{
			if (Finished != null)
			{
				try
				{
					Finished(this, EventArgs.Empty);
				}
				catch (Exception message)
				{
					Debug.LogError(message, this);
				}
				Finished = null;
			}
		}

		[Serializable]
		public class Entry
		{
			public FXBase m_Effect;

			public Single m_Lifetime;

			public Single m_BlockTime;

			public Entry()
			{
			}

			public Entry(FXBase p_Effect, Single p_Lifetime, Single p_BlockTime)
			{
				m_Effect = p_Effect;
				m_Lifetime = p_Lifetime;
				m_BlockTime = p_BlockTime;
			}

			public void DestroyEffect()
			{
				if (m_Effect != null)
				{
					m_Effect.Destroy();
					Destroy(m_Effect);
					m_Effect = null;
				}
			}
		}
	}
}
