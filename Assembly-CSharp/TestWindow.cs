using System;
using System.Collections.Generic;
using Legacy.Animations;
using UnityEngine;

public class TestWindow : MonoBehaviour
{
	private GUIStyle m_leftAlignedBtnStyle;

	private Rect m_WindowRect = new Rect(0f, 160f, 200f, 700f);

	private Int32 m_TabIndex;

	private Single m_VScrollbar;

	private GUIContent[] m_Tabs = new GUIContent[]
	{
		new GUIContent("Party\nSpell"),
		new GUIContent("Monster"),
		new GUIContent("Monster\nAnimation"),
		new GUIContent("Monster\nSpell")
	};

	private GUIContent[] m_PartySelection = new GUIContent[]
	{
		new GUIContent("Party"),
		new GUIContent("Member 1"),
		new GUIContent("Member 2"),
		new GUIContent("Member 3"),
		new GUIContent("Member 4")
	};

	private ETarget m_WizardIndex;

	private ETarget m_TargetIndex;

	private Vector2 m_PartySpellScrollView;

	private Vector2 m_MonsterSpellView;

	private Vector2 m_MonsterScrollView;

	private Quaternion m_MonsterRotation;

	private Single m_Degree;

	private Boolean m_IdleRota;

	[SerializeField]
	private TestMain m_Main;

	[SerializeField]
	private TestDatabase m_Database;

	[SerializeField]
	private AnimatorControl m_Anim;

	private void CheckStyles()
	{
		if (m_leftAlignedBtnStyle == null)
		{
			m_leftAlignedBtnStyle = new GUIStyle(GUI.skin.GetStyle("button"));
			m_leftAlignedBtnStyle.alignment = TextAnchor.MiddleLeft;
		}
	}

	private void OnGUI()
	{
		m_WindowRect = GUILayout.Window(1, m_WindowRect, new GUI.WindowFunction(Window), "Effects", (GUILayoutOption[])null);
		m_WindowRect.x = Mathf.Clamp(m_WindowRect.x, 0f, Screen.width - m_WindowRect.width);
		m_WindowRect.y = Mathf.Clamp(m_WindowRect.y, 0f, Screen.height - m_WindowRect.height);
	}

	private void Window(Int32 id)
	{
		CheckStyles();
		m_TabIndex = GUILayout.SelectionGrid(m_TabIndex, m_Tabs, m_Tabs.Length, (GUILayoutOption[])null);
		switch (m_TabIndex)
		{
		case 0:
			TabPartySpell();
			break;
		case 1:
			TabMonster();
			break;
		case 2:
			m_Anim = m_Main.ActiveMonsterAnimationHandler;
			TabMonsterAnimation();
			break;
		case 3:
			TabMonsterSpell();
			break;
		}
		GUI.DragWindow();
	}

	private void TabPartySpell()
	{
		List<TestSpell> partySpells = m_Database.PartySpells;
		if (partySpells.Count == 0)
		{
			return;
		}
		GUILayout.Label("Wizard", new GUILayoutOption[0]);
		m_WizardIndex = (ETarget)GUILayout.SelectionGrid((Int32)m_WizardIndex, m_PartySelection, m_PartySelection.Length, (GUILayoutOption[])null);
		m_PartySpellScrollView = GUILayout.BeginScrollView(m_PartySpellScrollView, new GUILayoutOption[]
		{
			GUILayout.MaxHeight(700f)
		});
		foreach (TestSpell testSpell in partySpells)
		{
			if (GUILayout.Button(testSpell.Name, m_leftAlignedBtnStyle, new GUILayoutOption[0]))
			{
				m_Main.PartyCastSpell(m_WizardIndex, testSpell);
			}
		}
		GUILayout.EndScrollView();
	}

	private void TabMonster()
	{
		TestMonster[] monsters = m_Database.Monsters;
		if (monsters == null || monsters.Length == 0)
		{
			return;
		}
		m_MonsterScrollView = GUILayout.BeginScrollView(m_MonsterScrollView, new GUILayoutOption[]
		{
			GUILayout.MaxHeight(700f)
		});
		foreach (TestMonster testMonster in monsters)
		{
			if (GUILayout.Button(testMonster.Name, m_leftAlignedBtnStyle, new GUILayoutOption[0]))
			{
				m_Main.LoadMonster(testMonster);
			}
		}
		GUILayout.EndScrollView();
	}

	private void TabMonsterAnimation()
	{
		if (m_Main.ActiveMonsterAnimationHandler != null)
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
			m_IdleRota = GUILayout.Toggle(m_IdleRota, "Switch On/Off Idle Rota", new GUILayoutOption[0]);
			if (!m_IdleRota)
			{
				m_Main.ActiveMonsterView.NumberOfIdleAnimations = 0;
			}
			else
			{
				m_Main.ActiveMonsterView.NumberOfIdleAnimations = idleMaxValue;
			}
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
				m_Anim.MoveTo(transform.position, transform.rotation);
			}
			if (GUILayout.Button("LeftPosition", new GUILayoutOption[0]))
			{
				Vector3 position = new Vector3(10f, 0f, 0f);
				m_Anim.MoveTo(position, transform.rotation);
			}
			if (GUILayout.Button("RightPosition", new GUILayoutOption[0]))
			{
				Vector3 position2 = new Vector3(-10f, 0f, 0f);
				m_Anim.MoveTo(position2, transform.rotation);
			}
			if (GUILayout.Button("BackPosition", new GUILayoutOption[0]))
			{
				Vector3 position3 = new Vector3(0f, 0f, -10f);
				m_Anim.MoveTo(position3, transform.rotation);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("Rotate Left", new GUILayoutOption[0]))
			{
				m_MonsterRotation = m_Main.ActiveMonster.transform.rotation;
				m_Degree = RotationValueHandler(-90f);
				m_MonsterRotation = Quaternion.Euler(0f, m_Degree, 0f);
				m_Anim.RotateTo(m_MonsterRotation);
			}
			if (GUILayout.Button("Rotate Right", new GUILayoutOption[0]))
			{
				m_MonsterRotation = m_Main.ActiveMonster.transform.rotation;
				m_Degree = RotationValueHandler(90f);
				m_MonsterRotation = Quaternion.Euler(0f, m_Degree, 0f);
				m_Anim.RotateTo(m_MonsterRotation);
			}
			GUILayout.EndHorizontal();
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

	private void TabMonsterSpell()
	{
		TestMonster activeMonsterData = m_Main.ActiveMonsterData;
		if (activeMonsterData == null || activeMonsterData.AvailableSpells == null || activeMonsterData.AvailableSpells.Length == 0)
		{
			return;
		}
		GUILayout.Label("Target", new GUILayoutOption[0]);
		m_TargetIndex = (ETarget)GUILayout.SelectionGrid((Int32)m_TargetIndex, m_PartySelection, m_PartySelection.Length, (GUILayoutOption[])null);
		m_MonsterSpellView = GUILayout.BeginScrollView(m_MonsterSpellView, new GUILayoutOption[]
		{
			GUILayout.MaxHeight(200f)
		});
		foreach (TestSpell testSpell in activeMonsterData.AvailableSpells)
		{
			if (GUILayout.Button(testSpell.Name, new GUILayoutOption[0]))
			{
				m_Main.MonsterCastSpell(m_TargetIndex, testSpell);
			}
		}
		GUILayout.EndScrollView();
	}

	private void Update()
	{
		if (Input.GetKey("escape"))
		{
			Application.Quit();
		}
	}
}
