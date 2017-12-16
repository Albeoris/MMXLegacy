using System;
using Legacy.Animations;
using UnityEngine;

namespace Legacy
{
	public class AnimatorControlTool : AnimatorControl
	{
		[SerializeField]
		private Animator m_animator;

		[SerializeField]
		private String[] AnimationProperty;

		[SerializeField]
		private String[] AnimatorTag;

		[SerializeField]
		private Int32[] AnimationNumber;

		[SerializeField]
		private Single[] PlayChance;

		[SerializeField]
		private Single[] Delay;

		[SerializeField]
		private Boolean LoopSequenz;

		[SerializeField]
		private Boolean RandomShuffle;

		private Boolean m_playedClip;

		private Boolean m_requested;

		private Boolean m_started;

		private Int32 m_num;

		private Single m_targetTime;

		private Boolean m_delayed;

		protected override void Update()
		{
			if (m_delayed)
			{
				if (m_targetTime <= Time.realtimeSinceStartup)
				{
					m_delayed = false;
					PlayAnimation();
				}
			}
			else if (!m_playedClip)
			{
				if (!m_requested)
				{
					StartAnimation();
					m_requested = true;
				}
				else if (!m_started)
				{
					if (m_animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimatorTag[m_num]))
					{
						m_started = true;
					}
				}
				else if (m_animator.GetCurrentAnimatorStateInfo(0).IsTag("IDLE"))
				{
					if (m_num < AnimationProperty.Length - 1)
					{
						if (!RandomShuffle)
						{
							m_num++;
							m_started = false;
							StartAnimation();
						}
						else
						{
							m_num = GetRandomNum(m_num);
							m_started = false;
							StartAnimation();
						}
					}
					else if (LoopSequenz)
					{
						m_num = 0;
					}
					else
					{
						m_playedClip = true;
						enabled = false;
					}
				}
			}
		}

		private void StartAnimation()
		{
			if (Delay[m_num] > 0f)
			{
				m_targetTime = Time.realtimeSinceStartup + Delay[m_num];
				m_delayed = true;
			}
			else
			{
				PlayAnimation();
			}
		}

		private void PlayAnimation()
		{
			String text = AnimationProperty[m_num];
			switch (text)
			{
			case "IdleType":
				IdleSpecial(AnimationNumber[m_num]);
				break;
			case "AttackType":
				AttackSpecial(AnimationNumber[m_num]);
				break;
			case "AttackCriticalType":
				AttackCriticalSpecial(AnimationNumber[m_num]);
				break;
			case "DieType":
				Die();
				break;
			case "EvadeState":
				EvadeSpecial(AnimationNumber[m_num]);
				break;
			case "HitType":
				HitSpecial(AnimationNumber[m_num]);
				break;
			case "BlockType":
				BlockSpecial(AnimationNumber[m_num]);
				break;
			case "EventType":
				NPCEventSpecial(AnimationNumber[m_num]);
				break;
			}
		}

		private Int32 GetRandomNum(Int32 p_num)
		{
			Single num = 0f;
			Single num2 = 0f;
			for (Int32 i = 0; i < PlayChance.Length; i++)
			{
				num2 += PlayChance[i];
			}
			for (Int32 j = 0; j < AnimationProperty.Length; j++)
			{
				Single num3 = Random.Range(0f, num2);
				num += PlayChance[j];
				if (num3 < num)
				{
					p_num = j;
					break;
				}
			}
			return p_num;
		}

		private void AttackSpecial(Int32 AniNum)
		{
			if (m_IsDead)
			{
				return;
			}
			m_AttackTrigger.Trigger(0, AniNum, 0);
		}

		public void AttackCriticalSpecial(Int32 AniNum)
		{
			if (m_IsDead)
			{
				return;
			}
			m_AttackCriticalTrigger.Trigger(0, AniNum, 0);
		}

		public void AttackRangeSpecial(Int32 AniNum)
		{
			if (m_IsDead)
			{
				return;
			}
			m_AttackRangeTrigger.Trigger(0, AniNum, 0);
		}

		public void EvadeSpecial(Int32 AniNum)
		{
			if (m_IsDead)
			{
				return;
			}
			m_EvadeTrigger.Trigger(0, AniNum, 0);
		}

		public void NPCEventSpecial(Int32 AniNum)
		{
			if (m_IsDead)
			{
				return;
			}
			m_EventTrigger.Trigger(0, AniNum, 0);
		}

		public void HitSpecial(Int32 AniNum)
		{
			if (m_IsDead)
			{
				return;
			}
			m_HitTrigger.Trigger(0, AniNum, 0);
		}

		public void BlockSpecial(Int32 AniNum)
		{
			if (m_IsDead)
			{
				return;
			}
			m_BlockTrigger.Trigger(0, AniNum, 0);
		}
	}
}
