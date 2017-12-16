using System;
using Legacy.Animations;
using UnityEngine;

public class TestAnimatorControl : MonoBehaviour
{
	public AnimatorControl m_Anim;

	public Single timescale = 1f;

	public Int32 targetframerate = 30;

	private Quaternion m_MonsterRotation;

	private GameObject m_ActiveMonster;

	private Single m_Degree;

	private Rect pos = new Rect(0f, 0f, 400f, 700f);

	private void OnGUI()
	{
		pos = GUI.Window(112312333, pos, new GUI.WindowFunction(DrawWindow), "Animator Control");
	}

	private void DrawWindow(Int32 id)
	{
		Time.timeScale = timescale;
		Application.targetFrameRate = targetframerate;
		if (m_Anim != null)
		{
			Int32 attackMagicMaxValue = m_Anim.AttackMagicMaxValue;
			Int32 attackMeleeMaxValue = m_Anim.AttackMeleeMaxValue;
			Int32 attackRangedMaxValue = m_Anim.AttackRangedMaxValue;
			Int32 attackCriticalMeleeMaxValue = m_Anim.AttackCriticalMeleeMaxValue;
			Int32 idleMaxValue = m_Anim.IdleMaxValue;
			Int32 evadeMaxValue = m_Anim.EvadeMaxValue;
			Int32 blockMaxValue = m_Anim.BlockMaxValue;
			Int32 hitMaxValue = m_Anim.HitMaxValue;
			Int32 dieMaxValue = m_Anim.DieMaxValue;
			Int32 eventMaxValue = m_Anim.EventMaxValue;
			m_Anim.InCombat = GUILayout.Toggle(m_Anim.InCombat, "InCombat", new GUILayoutOption[0]);
			GUILayout.Label("Attack Animations:", new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			for (Int32 i = 1; i <= attackMeleeMaxValue; i++)
			{
				if (GUILayout.Button("#" + i, new GUILayoutOption[0]))
				{
					m_Anim.Attack(i);
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Label("AttackCritical Animations:", new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			for (Int32 j = 1; j <= attackCriticalMeleeMaxValue; j++)
			{
				if (GUILayout.Button("#" + j, new GUILayoutOption[0]))
				{
					m_Anim.AttackCritical(j);
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Label("RangedAttack Animations:", new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			for (Int32 k = 1; k <= attackRangedMaxValue; k++)
			{
				if (GUILayout.Button("#" + k, new GUILayoutOption[0]))
				{
					m_Anim.AttackRange(k);
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Label("Idle Animations:", new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			for (Int32 l = 1; l <= idleMaxValue; l++)
			{
				if (GUILayout.Button("#" + l, new GUILayoutOption[0]))
				{
					m_Anim.IdleSpecial(l);
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Label("Evade Animations:", new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			for (Int32 m = 1; m <= evadeMaxValue; m++)
			{
				if (GUILayout.Button("#" + m, new GUILayoutOption[0]))
				{
					m_Anim.Evade(m);
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Label("Hit Animations:", new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			for (Int32 n = 1; n <= hitMaxValue; n++)
			{
				if (GUILayout.Button("#" + n, new GUILayoutOption[0]))
				{
					m_Anim.Hit(n);
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Label("Event Animations:", new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			for (Int32 num = 2; num <= eventMaxValue; num++)
			{
				if (GUILayout.Button("#" + (num - 1), new GUILayoutOption[0]))
				{
					m_Anim.EventSummon(num);
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Label("Block Animations:", new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			for (Int32 num2 = 1; num2 <= blockMaxValue; num2++)
			{
				if (GUILayout.Button("#" + num2, new GUILayoutOption[0]))
				{
					m_Anim.Block(num2);
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Label("MagicAttackAnimations:", new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			for (Int32 num3 = 1; num3 <= attackMagicMaxValue; num3++)
			{
				if (GUILayout.Button("#" + num3, new GUILayoutOption[0]))
				{
					m_Anim.AttackMagic(num3);
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Label("Die Animations:", new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			for (Int32 num4 = 1; num4 <= dieMaxValue; num4++)
			{
				if (GUILayout.Button("#" + num4, new GUILayoutOption[0]))
				{
					m_Anim.Die(num4);
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Label("Movement/Turn Animations:", new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("StartPosition", new GUILayoutOption[0]))
			{
				Quaternion lookDirection = Quaternion.Euler(0f, 0f, 0f);
				Vector3 position = new Vector3(0f, 0f, 0f);
				m_Anim.MoveTo(position, lookDirection);
			}
			if (GUILayout.Button("LeftPosition", new GUILayoutOption[0]))
			{
				Vector3 position2 = new Vector3(20f, 0f, 0f);
				m_Anim.MoveTo(position2, transform.rotation);
			}
			if (GUILayout.Button("RightPosition", new GUILayoutOption[0]))
			{
				Vector3 position3 = new Vector3(-10f, 0f, 0f);
				m_Anim.MoveTo(position3, transform.rotation);
			}
			if (GUILayout.Button("BackPosition", new GUILayoutOption[0]))
			{
				Vector3 position4 = new Vector3(0f, 0f, -10f);
				m_Anim.MoveTo(position4, transform.rotation);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("Rotate Left", new GUILayoutOption[0]))
			{
				m_MonsterRotation = transform.rotation;
				m_Degree = RotationValueHandler(-90f);
				m_MonsterRotation = Quaternion.Euler(0f, m_Degree, 0f);
				m_Anim.RotateTo(m_MonsterRotation);
			}
			if (GUILayout.Button("Rotate Right", new GUILayoutOption[0]))
			{
				m_MonsterRotation = transform.rotation;
				m_Degree = RotationValueHandler(90f);
				m_MonsterRotation = Quaternion.Euler(0f, m_Degree, 0f);
				m_Anim.RotateTo(m_MonsterRotation);
			}
			GUILayout.EndHorizontal();
			GUILayout.Label("IsMoving: " + m_Anim.IsMoving, new GUILayoutOption[0]);
			GUILayout.Label("IsRotating: " + m_Anim.IsRotating, new GUILayoutOption[0]);
			GUILayout.Label("InMovement: " + m_Anim.InMovement, new GUILayoutOption[0]);
			GUILayout.Label("IsDead: " + m_Anim.IsDead, new GUILayoutOption[0]);
			GUILayout.Label("InCombat: " + m_Anim.InCombat, new GUILayoutOption[0]);
		}
	}

	private Single RotationValueHandler(Single p_Deg)
	{
		Single num = m_Degree;
		num += p_Deg;
		if (num == 360f)
		{
			num = 0f;
		}
		return num;
	}
}
