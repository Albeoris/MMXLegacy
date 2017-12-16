using System;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.Pathfinding;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.Minimap
{
	[AddComponentMenu("MM Legacy/MMGUI/Minimap/Monster symbol view")]
	public class MonsterSymbolView : MoveableSymbolView
	{
		[SerializeField]
		private String m_TooltipLocaKeyNormal;

		[SerializeField]
		private String m_SpriteNormal;

		[SerializeField]
		private String m_TooltipLocaKeyBoss;

		[SerializeField]
		private String m_SpriteBoss;

		public override void CheckVisibility(Boolean skipAnimation)
		{
			base.CheckVisibility(skipAnimation);
			Single num = 0f;
			if (MyController != null)
			{
				Monster monster = (Monster)MyController;
				Party party = LegacyLogic.Instance.WorldManager.Party;
				if (party != null && party.HasDangerSense())
				{
					Int32 monsterVisibilityRangeWithDangerSense = ConfigManager.Instance.Game.MonsterVisibilityRangeWithDangerSense;
					Grid grid = LegacyLogic.Instance.MapLoader.Grid;
					GridSlot slot = grid.GetSlot(monster.Position);
					GridSlot slot2 = grid.GetSlot(party.Position);
					Int32 num2 = AStarHelper<GridSlot>.Calculate(slot, slot2, monsterVisibilityRangeWithDangerSense, null, false, null);
					if (num2 > 0)
					{
						num = 1f;
					}
				}
				else if (party != null && monster.IsAggro && 1f >= Position.DistanceSquared(monster.Position, party.Position))
				{
					num = 1f;
				}
			}
			if (num == 0f)
			{
				TooltipManager.Instance.Hide(this);
			}
			collider.enabled = (num > 0f);
			TweenAlpha.Begin(gameObject, (!skipAnimation) ? 1 : 0, num);
		}

		protected override void Awake()
		{
			base.Awake();
			enabled = true;
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnCharacterCastSpell));
			}
			if (MyController != null)
			{
				SetupSymbol();
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnCharacterCastSpell));
			}
		}

		protected virtual void OnCharacterCastSpell(Object p_sender, EventArgs p_args)
		{
			CheckVisibility(false);
		}

		protected override void OnMoveEntity(Object p_sender, EventArgs p_args)
		{
			base.OnMoveEntity(p_sender, p_args);
			if (p_sender is Party)
			{
				CheckVisibility(false);
			}
		}

		protected override void OnSetEntityPosition(Object p_sender, EventArgs p_args)
		{
			base.OnSetEntityPosition(p_sender, p_args);
			if (p_sender is Party)
			{
				CheckVisibility(false);
			}
		}

		protected override void Update()
		{
			base.Update();
			enabled = true;
		}

		private void SetupSymbol()
		{
			Monster monster = (Monster)MyController;
			UISprite uisprite = (UISprite)MyUIWidget;
			if (monster.StaticData.Grade >= EMonsterGrade.CHAMPION)
			{
				uisprite.spriteName = m_SpriteBoss;
				m_TooltipLocaKey = m_TooltipLocaKeyBoss;
			}
			else
			{
				uisprite.spriteName = m_SpriteNormal;
				m_TooltipLocaKey = m_TooltipLocaKeyNormal;
			}
			uisprite.MakePixelPerfect();
			NGUITools.AddWidgetCollider(gameObject);
		}
	}
}
