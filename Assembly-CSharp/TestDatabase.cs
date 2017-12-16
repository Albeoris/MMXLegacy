using System;
using System.Collections.Generic;
using UnityEngine;

public class TestDatabase : MonoBehaviour
{
	[SerializeField]
	private TestMonster[] m_Monsters;

	[SerializeField]
	private List<TestSpell> m_PartySpellList = new List<TestSpell>();

	[SerializeField]
	public String m_PartySpellCSVPath;

	[SerializeField]
	public String m_PartyBuffCSVPath;

	[SerializeField]
	public String m_MonsterBuffCSVPath;

	public TestMonster[] Monsters => m_Monsters;

    public List<TestSpell> PartySpells => m_PartySpellList;
}
