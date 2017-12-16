using System;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.StaticData;

namespace Legacy.Core.Entities.TrapEffects
{
	public static class TrapEffectFactory
	{
		public static BaseTrapEffect CreateTrapEffect(Int32 p_staticID, InteractiveObject p_parent)
		{
			TrapEffectStaticData staticData = StaticDataHandler.GetStaticData<TrapEffectStaticData>(EDataType.TRAP_EFFECT, p_staticID);
			if (staticData != null)
			{
				return CreateTrapEffect(staticData.TrapEffect, p_staticID, p_parent);
			}
			throw new Exception("Data with id not found: " + p_staticID);
		}

		private static BaseTrapEffect CreateTrapEffect(ETrapEffect p_effect, Int32 p_staticID, InteractiveObject p_parent)
		{
			switch (p_effect)
			{
			case ETrapEffect.SHRED:
				return new ShredTrapEffect(p_staticID, p_parent);
			case ETrapEffect.SHRAPNEL:
				return new ShrapnelTrapEffect(p_staticID, p_parent);
			case ETrapEffect.VOLLEY_OF_BOLTS:
				return new VolleyOfBoltsTrapEffect(p_staticID, p_parent);
			case ETrapEffect.POISON_GAS:
				return new PoisonGasTrapEffect(p_staticID, p_parent);
			case ETrapEffect.FROSTCLOUD:
				return new FrostcloudTrapEffect(p_staticID, p_parent);
			case ETrapEffect.VEIL_OF_CONFUSION:
				return new VeilOfConfusionTrapEffect(p_staticID, p_parent);
			case ETrapEffect.VEIL_OF_FATIGUE:
				return new VeilOfFatigueTrapEffect(p_staticID, p_parent);
			case ETrapEffect.VEIL_OF_DOOM:
				return new VeilOfDoomTrapEffect(p_staticID, p_parent);
			case ETrapEffect.STUNNING_SHOCK:
				return new StunningShockTrapEffect(p_staticID, p_parent);
			case ETrapEffect.MANA_VOID:
				return new ManaVoidTrapEffect(p_staticID, p_parent);
			case ETrapEffect.ROT:
				return new RotTrapEffect(p_staticID, p_parent);
			case ETrapEffect.INTRUDER_ALERT:
				return new IntruderAlertTrapEffect(p_staticID, p_parent);
			case ETrapEffect.DISPEL_MAGIC:
				return new DispelMagicTrapEffect(p_staticID, p_parent);
			case ETrapEffect.SCORCHING_FLASH:
				return new ScorchingFlashTrapEffect(p_staticID, p_parent);
			case ETrapEffect.EXPLOSION:
				return new ExplosionTrapEffect(p_staticID, p_parent);
			default:
				throw new NotSupportedException(String.Concat(new Object[]
				{
					"Unknow type! ",
					p_effect,
					" (",
					p_staticID,
					")"
				}));
			}
		}
	}
}
