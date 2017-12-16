using System;
using System.Collections;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Entities;
using UnityEngine;

namespace Legacy.EffectEngine
{
	public class CrystalCageFXQue : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_Cage_Destroyed_FX;

		[SerializeField]
		private GameObject m_Cage_Explode_FX;

		private Boolean m_Done;

		private Boolean m_MonsterSelected;

		private Monster m_CageMonster;

		private void GetCageMonster()
		{
			m_CageMonster = LegacyLogic.Instance.WorldManager.Party.SelectedMonster;
			if (m_CageMonster.StaticData.Type == EMonsterType.CAGE)
			{
				m_MonsterSelected = true;
			}
		}

		private void Update()
		{
			if (!m_MonsterSelected)
			{
				GetCageMonster();
			}
			Int32 currentHealth = m_CageMonster.CurrentHealth;
			Boolean flag = m_CageMonster.BuffHandler.HasBuff(EMonsterBuffType.OSCILLATION);
			if (flag && !m_Done)
			{
				MonsterBuff buff = m_CageMonster.BuffHandler.GetBuff(EMonsterBuffType.OSCILLATION);
				Int32 duration = buff.Duration;
				if (currentHealth <= 0 && duration > 0 && !m_Done)
				{
					GameObject gameObject = Helper.Instantiate<GameObject>(m_Cage_Destroyed_FX);
					gameObject.transform.position = transform.position;
					Destroy(this.gameObject, 0f);
					m_Done = true;
				}
				if (duration == 0 && !m_Done)
				{
					GameObject gameObject2 = Helper.Instantiate<GameObject>(m_Cage_Explode_FX);
					gameObject2.transform.position = transform.position;
					Destroy(gameObject, 2f);
					m_Done = true;
					StartCoroutine(PlayLateShakeFX());
				}
			}
			if (!flag && !m_Done && currentHealth <= 0)
			{
				GameObject gameObject3 = Helper.Instantiate<GameObject>(m_Cage_Destroyed_FX);
				gameObject3.transform.position = transform.position;
				Destroy(gameObject, 0f);
				m_Done = true;
			}
		}

		private static IEnumerator PlayLateShakeFX()
		{
			yield return new WaitForSeconds(1.9f);
			Vector3 vec = new Vector3(0f, 0.2f, 0f);
			FXMainCamera.Instance.PlayShakeFX(0.75f, vec);
			yield break;
		}
	}
}
