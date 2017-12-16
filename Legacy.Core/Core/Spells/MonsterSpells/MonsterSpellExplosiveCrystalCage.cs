using System;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Entities;
using Legacy.Core.Map;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellExplosiveCrystalCage : MonsterSpellCageBase
	{
		public MonsterSpellExplosiveCrystalCage(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.EXPLOSIVE_CRYSTAL_CAGE, p_effectAnimationClip, p_castProbability)
		{
		}

		protected override void SpawnCagePiece(Position p_position, Boolean p_isVisible)
		{
			Monster monster = new Monster((!p_isVisible) ? 718 : 717, -1);
			monster.Position = p_position;
			monster.Direction = EDirection.EAST;
			monster.SpawnAnim = 1;
			LegacyLogic.Instance.WorldManager.MonsterGroupHandler.GetGroup(10000);
			monster.MonsterGroupID = 10000;
			if (m_grid.AddMovingEntity(monster.Position, monster))
			{
				LegacyLogic.Instance.WorldManager.SpawnObject(monster, monster.Position);
			}
			MonsterBuff monsterBuff = BuffFactory.CreateMonsterBuff(EMonsterBuffType.OSCILLATION, 1f, m_level);
			monsterBuff.ScaleDuration(1f);
			((MonsterBuffOscillation)monsterBuff).ForVisualsOnly = !p_isVisible;
			monster.BuffHandler.AddBuff(monsterBuff);
		}
	}
}
