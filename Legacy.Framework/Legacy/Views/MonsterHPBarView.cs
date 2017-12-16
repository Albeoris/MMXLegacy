using System;
using System.Collections.Generic;
using Legacy.Core.Abilities;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Configuration;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.EffectEngine;
using Legacy.EffectEngine.Effects;
using Legacy.HUD;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/Monster HP Bar View")]
	public class MonsterHPBarView : BaseView
	{
		private const Single TIME_SHOW_HPBAR_AFTER_HIT = 3.2f;

		private const Single SPEED_UPDATE_DAMAGE_BAR = 0.5f;

		private const Single TIME_SHOW_FULL_DAMAGE = 0.5f;

		private const Single POSITION_HIGH_TRESHHOLD = 570f;

		private const Single HP_BAR_MINIMUM_SIZE = 64f;

		private const Single HP_BAR_MAXIMUM_SIZE = 768f;

		public EventHandler OnTooltipEvent;

		public static Boolean ShowHpBar = true;

		[SerializeField]
		private Transform m_HPBarAnchor;

		private GameObject m_root;

		private GameObject m_positionRoot;

		private UISlicedSprite m_HPBar;

		private UISlicedSprite m_backgroundHPBar;

		private UISlicedSprite m_recentDamage;

		private UISlicedSprite m_shadowHPBar;

		private Transform m_HPBarRoot;

		private Transform m_BuffAnchor;

		private UILabel m_monsterNameLabel;

		private GameObject m_dummyObject;

		private Rect m_nameCollisionBox;

		private Rect m_barCollisionBox;

		private Rect m_buffsCollisionBox;

		private Int32 m_offsetDir;

		private Single m_collisionOffset;

		private Single m_targetYPos;

		private Single m_lastYPos = 30f;

		private Boolean m_visible;

		private List<MonsterBuffView> m_buffList;

		private Single m_lastHitTime = -10f;

		private Int32 m_lastHitFrame = -10;

		private Boolean m_isDead;

		private Boolean m_isDeadAnimationFinished;

		private Int32 m_initialSize;

		private Single m_centerPoint;

		private Boolean m_movedByBuffs;

		private Int32 m_buffCount;

		private Boolean m_buffLabelEnabled;

		private Single m_targetHPBarScaleX = 1f;

		private Single m_timeStartUpdateDamagebar;

		private Boolean m_isSelected;

		private Color m_selectedColor = Color.white;

		private Color m_onMouseOverColor = new Color(0.4f, 0.4f, 0.4f);

		private Single m_currentlyUsedOutlineSetting = 1f;

		public Rect NameCollisionBox => m_nameCollisionBox;

	    public Rect BarCollisionBox => m_barCollisionBox;

	    public Rect BuffsCollisionBox => m_buffsCollisionBox;

	    public Int32 BuffCount => m_buffCount;

	    public Transform HPBarAnchor => m_HPBarAnchor;

	    protected override void Awake()
		{
			base.Awake();
		}

		private void InitHPBarView()
		{
			HPBarProvider.CreateHPBar(this, m_HPBarAnchor, out m_root, out m_positionRoot, out m_HPBar, out m_recentDamage, out m_backgroundHPBar, out m_shadowHPBar, out m_monsterNameLabel);
			if (m_HPBar != null)
			{
				m_dummyObject = m_HPBar.transform.parent.gameObject;
				m_HPBarRoot = m_dummyObject.transform.parent;
				MonoBehaviour[] components = m_HPBarRoot.GetComponents<MonoBehaviour>();
				for (Int32 i = 0; i < components.Length; i++)
				{
					if (components[i] is IMonsterTooltip)
					{
						((IMonsterTooltip)components[i]).InitializeMonsterTooltip(this);
					}
				}
				InitBuffs();
			}
		}

		private Single ScaleHPBar(Single p_x, Single p_min, Single p_cap, Single p_gradient, Single p_linearEnd)
		{
			Single result;
			if (p_x <= p_linearEnd)
			{
				result = p_x * p_gradient + p_min;
			}
			else
			{
				Single num = p_cap - p_min - p_linearEnd * p_gradient;
				result = p_linearEnd * p_gradient + p_min + num * (1f - Mathf.Exp((p_linearEnd - p_x) * p_gradient / num));
			}
			return result;
		}

		private void SetMonsterName()
		{
			Monster monster = (Monster)MyController;
			if (monster == null)
			{
				if (m_monsterNameLabel != null)
				{
					m_monsterNameLabel.alpha = 0f;
				}
				return;
			}
			InitHPBarView();
			m_monsterNameLabel.text = LocaManager.GetText(monster.StaticData.NameKey);
			m_centerPoint = m_monsterNameLabel.transform.localPosition.x;
			Single num = ScaleHPBar(monster.MaxHealth, 64f, 768f, 1f, 0f);
			m_initialSize = (Int32)num;
			Vector3 localScale = m_HPBar.cachedTransform.localScale;
			localScale.x = num;
			m_HPBar.cachedTransform.localScale = localScale;
			Vector3 localPosition = m_HPBar.transform.localPosition;
			localPosition.x = m_centerPoint - num / 2f;
			m_HPBar.transform.localPosition = localPosition;
			m_shadowHPBar.cachedTransform.localScale = localScale;
			Vector3 localPosition2 = m_shadowHPBar.transform.localPosition;
			localPosition2.x = m_centerPoint - num / 2f;
			m_shadowHPBar.transform.localPosition = localPosition2;
			m_recentDamage.cachedTransform.localScale = localScale;
			Vector3 localPosition3 = m_recentDamage.transform.localPosition;
			localPosition3.x = m_centerPoint - num / 2f;
			m_recentDamage.transform.localPosition = localPosition3;
			Vector3 localScale2 = m_backgroundHPBar.cachedTransform.localScale;
			localScale2.x = num + 4f;
			m_backgroundHPBar.cachedTransform.localScale = localScale2;
			Vector3 localPosition4 = m_backgroundHPBar.transform.localPosition;
			localPosition4.x = m_centerPoint - num / 2f - 1f;
			m_backgroundHPBar.transform.localPosition = localPosition4;
			m_targetHPBarScaleX = localScale.x;
			switch (monster.StaticData.Grade)
			{
			case EMonsterGrade.CORE:
				m_monsterNameLabel.color = Color.yellow;
				break;
			case EMonsterGrade.ELITE:
				m_monsterNameLabel.color = new Color(1f, 0.51f, 0.11f);
				break;
			case EMonsterGrade.CHAMPION:
			case EMonsterGrade.BOSS:
				m_monsterNameLabel.color = Color.red;
				break;
			default:
				m_monsterNameLabel.color = Color.white;
				break;
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			HPBarProvider.DestroyHPBar(this);
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (MyController != null)
			{
				SetMonsterName();
			}
			m_currentlyUsedOutlineSetting = ConfigManager.Instance.Options.EnemyOutlineOpacity;
			if (oldController != MyController)
			{
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.CHARACTER_ATTACKS, new EventHandler(OnCharacterAttacksDelayed));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.CHARACTER_ATTACKS_RANGED, new EventHandler(OnCharacterAttacksDelayed));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnCharacterCastSpellDelayed));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.MONSTER_STATUS_CHANGED, new EventHandler(OnMonsterStatusChanged));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_FINISH, EEventType.MONSTER_DIED, new EventHandler(OnMonsterAnimationDieEnd));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.MONSTER_DIED, new EventHandler(OnMonsterDied));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_ATTACKS, new EventHandler(OnCharacterAttacks));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_ATTACKS_RANGED, new EventHandler(OnCharacterAttacks));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnCharacterCastSpell));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_SELECTED, new EventHandler(OnMonsterSelected));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_ALL_SELECTIONS_REMOVED, new EventHandler(OnMonsterAllSelectionsRemoved));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.MONSTER_BUFF_ADDED, new EventHandler(UpdateBuffs));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.MONSTER_BUFF_REMOVED, new EventHandler(UpdateBuffs));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.MONSTER_BUFF_CHANGED, new EventHandler(UpdateBuffs));
			}
			if (MyController != null)
			{
				DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.CHARACTER_ATTACKS, new EventHandler(OnCharacterAttacksDelayed));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.CHARACTER_ATTACKS_RANGED, new EventHandler(OnCharacterAttacksDelayed));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnCharacterCastSpellDelayed));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.MONSTER_STATUS_CHANGED, new EventHandler(OnMonsterStatusChanged));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FX_FINISH, EEventType.MONSTER_DIED, new EventHandler(OnMonsterAnimationDieEnd));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.MONSTER_DIED, new EventHandler(OnMonsterDied));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_ATTACKS, new EventHandler(OnCharacterAttacks));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_ATTACKS_RANGED, new EventHandler(OnCharacterAttacks));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnCharacterCastSpell));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_SELECTED, new EventHandler(OnMonsterSelected));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_ALL_SELECTIONS_REMOVED, new EventHandler(OnMonsterAllSelectionsRemoved));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.MONSTER_BUFF_ADDED, new EventHandler(UpdateBuffs));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.MONSTER_BUFF_REMOVED, new EventHandler(UpdateBuffs));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.MONSTER_BUFF_CHANGED, new EventHandler(UpdateBuffs));
			}
		}

		private void Update()
		{
			if (MyController != null && m_HPBar != null)
			{
				if (m_isDeadAnimationFinished || m_recentDamage.cachedTransform.localScale.x == 0f)
				{
					SetVisible(true);
					UIWidget[] componentsInChildren = m_HPBar.transform.parent.GetComponentsInChildren<UIWidget>();
					foreach (UIWidget uiwidget in componentsInChildren)
					{
						uiwidget.alpha -= Time.deltaTime;
					}
					if (m_buffList.Count > 0)
					{
						HelperBuffs.DestroyAllBuffViews(m_buffList, OnTooltipEvent);
					}
				}
				SetVisible(IsMonsterInFrontOfParty() || IsMonsterLastHitRecently());
				Vector3 localScale = m_HPBar.cachedTransform.localScale;
				Vector3 localScale2 = m_recentDamage.cachedTransform.localScale;
				if (m_timeStartUpdateDamagebar == 0f && localScale.x != localScale2.x)
				{
					m_timeStartUpdateDamagebar = Time.time + 0.5f;
				}
				else if (Time.time > m_timeStartUpdateDamagebar)
				{
					Single num;
					if (Mathf.Abs(m_targetHPBarScaleX - localScale.x) < 0.001f)
					{
						num = -1f;
					}
					else
					{
						num = 1f;
					}
					Single num2 = Mathf.Sign(localScale.x - localScale2.x);
					localScale2.x += num2 * (m_initialSize * Time.deltaTime) * 0.5f;
					if (Mathf.Sign(localScale.x - localScale2.x) != num2)
					{
						m_timeStartUpdateDamagebar = 0f;
						if (num < 0f)
						{
							m_recentDamage.cachedTransform.localScale = localScale;
						}
						else
						{
							m_HPBar.cachedTransform.localScale = m_recentDamage.cachedTransform.localScale;
						}
					}
					else if (num < 0f)
					{
						m_recentDamage.cachedTransform.localScale = localScale2;
					}
					else
					{
						localScale.x -= num2 * (m_initialSize * Time.deltaTime) * 0.5f;
						m_HPBar.cachedTransform.localScale = localScale;
					}
				}
				if (m_currentlyUsedOutlineSetting != ConfigManager.Instance.Options.EnemyOutlineOpacity)
				{
					m_currentlyUsedOutlineSetting = ConfigManager.Instance.Options.EnemyOutlineOpacity;
					OutlineGlowFX component = GetComponent<OutlineGlowFX>();
					if (component != null && !component.IsDestroyed)
					{
						component.SetGlobalIntensity(m_currentlyUsedOutlineSetting);
						ShowOutline(false, m_selectedColor);
					}
					else if (LegacyLogic.Instance.WorldManager.Party.SelectedMonster == MyController)
					{
						ShowSelection();
					}
				}
				if (LegacyLogic.Instance.ConversationManager.IsOpen)
				{
					SetVisible(false);
				}
			}
		}

		private void LateUpdate()
		{
			if (m_positionRoot != null && m_visible)
			{
				Boolean flag = false;
				Single num = 100f;
				Single num2 = 0f;
				Single num3 = 0f;
				Single num4 = num;
				m_offsetDir = 1;
				Single num5 = 0f;
				Vector3 localPosition = m_positionRoot.transform.localPosition;
				localPosition.y = 30f;
				m_positionRoot.transform.localPosition = localPosition;
				if (m_root.transform.localPosition.y + m_positionRoot.transform.localPosition.y > 570f)
				{
					m_offsetDir = -1;
					Vector3 localPosition2 = m_positionRoot.transform.localPosition;
					localPosition2.y = 570f - m_root.transform.localPosition.y;
					m_positionRoot.transform.localPosition = localPosition2;
				}
				UpdateCollisionRects();
				Int32 num6 = 100;
				while (HPBarProvider.CheckForCollision(this))
				{
					Single num7 = 8f;
					Single num8 = (m_collisionOffset + num7) * m_offsetDir;
					num5 += num8;
					if (m_offsetDir > 0 && m_root.transform.localPosition.y + m_positionRoot.transform.localPosition.y + num5 > 570f)
					{
						flag = true;
						m_offsetDir = -1;
						num8 -= num5;
						num5 = 0f;
					}
					else if (!flag && Mathf.Abs(num5) > num4)
					{
						if (m_offsetDir > 0)
						{
							num2 = num5;
							num8 = num8 - num5 + num3;
							num5 = num3;
						}
						else
						{
							num3 = num5;
							num8 = num8 - num5 + num2;
							num5 = num2;
							num4 += num;
						}
						m_offsetDir *= -1;
					}
					m_barCollisionBox.y = m_barCollisionBox.y + num8;
					m_nameCollisionBox.y = m_nameCollisionBox.y + num8;
					m_buffsCollisionBox.y = m_buffsCollisionBox.y + num8;
					if (num6-- < 0)
					{
						num5 = 0f;
						Debug.LogWarning("To many HP bar collision checks");
						break;
					}
				}
				m_targetYPos = m_positionRoot.transform.localPosition.y + num5;
				if (m_targetYPos != m_lastYPos)
				{
					m_lastYPos += (m_targetYPos - m_lastYPos) * Time.deltaTime * 10f;
				}
				if (m_root.transform.localPosition.y + m_lastYPos > 570f)
				{
					m_lastYPos = 570f - m_root.transform.localPosition.y;
				}
				localPosition = m_positionRoot.transform.localPosition;
				localPosition.y = m_lastYPos;
				m_positionRoot.transform.localPosition = localPosition;
				HPBarProvider.AddForCollision(this);
			}
		}

		private void UpdateCollisionRects()
		{
			Vector3 localScale = m_HPBar.transform.localScale;
			Vector3 vector = m_HPBar.transform.localPosition + m_dummyObject.transform.localPosition + m_root.transform.localPosition + new Vector3(m_HPBar.transform.localScale.x * 0.5f, -m_HPBar.transform.localScale.y * 0.5f, 0f);
			m_barCollisionBox = new Rect(vector.x, vector.y, localScale.x, localScale.y);
			Vector3 vector2 = new Vector3(m_monsterNameLabel.relativeSize.x * m_monsterNameLabel.transform.localScale.x, m_monsterNameLabel.relativeSize.y * m_monsterNameLabel.transform.localScale.y * 0.8f, 0f);
			Vector3 vector3 = m_monsterNameLabel.transform.localPosition + m_dummyObject.transform.localPosition + m_root.transform.localPosition;
			m_nameCollisionBox = new Rect(vector3.x, vector3.y, vector2.x, vector2.y);
			Vector3 vector4 = new Vector3(4f, -2f, 0f) + m_dummyObject.transform.localPosition + m_root.transform.localPosition;
			Vector3 vector5 = new Vector3(m_buffCount * 72f - 8f, 64f, 0f);
			if (m_buffLabelEnabled)
			{
				Single num = 28f;
				vector4.y -= num * 0.5f;
				vector5.y += num;
			}
			m_buffsCollisionBox = new Rect(vector4.x, vector4.y, vector5.x, vector5.y);
		}

		public Boolean CheckCollision(MonsterHPBarView p_other)
		{
			return Intersect(m_barCollisionBox, p_other.BarCollisionBox) || Intersect(m_barCollisionBox, p_other.NameCollisionBox) || (p_other.BuffCount > 0 && Intersect(m_barCollisionBox, p_other.BuffsCollisionBox)) || Intersect(m_nameCollisionBox, p_other.BarCollisionBox) || Intersect(m_nameCollisionBox, p_other.NameCollisionBox) || (p_other.BuffCount > 0 && Intersect(m_nameCollisionBox, p_other.BuffsCollisionBox)) || (m_buffCount > 0 && Intersect(m_buffsCollisionBox, p_other.BarCollisionBox)) || (m_buffCount > 0 && Intersect(m_buffsCollisionBox, p_other.NameCollisionBox)) || (m_buffCount > 0 && p_other.BuffCount > 0 && Intersect(m_buffsCollisionBox, p_other.BuffsCollisionBox));
		}

		private Boolean Intersect(Rect p_rect1, Rect p_rect2)
		{
			if (Mathf.Abs(p_rect1.x - p_rect2.x) + 1f < (p_rect1.width + p_rect2.width) * 0.5f && Mathf.Abs(p_rect1.y - p_rect2.y) + 1f < (p_rect1.height + p_rect2.height) * 0.5f)
			{
				if (Mathf.Sign(p_rect1.y - p_rect2.y) != m_offsetDir)
				{
					m_collisionOffset = (p_rect1.height + p_rect2.height) * 0.5f + Mathf.Abs(p_rect1.y - p_rect2.y);
				}
				else
				{
					m_collisionOffset = (p_rect1.height + p_rect2.height) * 0.5f - Mathf.Abs(p_rect1.y - p_rect2.y);
				}
				return true;
			}
			return false;
		}

		private void SetVisible(Boolean p_visible)
		{
			if (m_HPBarRoot != null)
			{
				GameObject gameObject = m_HPBarRoot.gameObject;
				if (gameObject == null)
				{
					return;
				}
				m_visible = p_visible;
				p_visible &= (ShowHpBar && ConfigManager.Instance.Options.MonsterHPBarsVisible);
				Boolean p_needed = p_visible;
				if (gameObject.activeSelf != p_visible)
				{
					gameObject.SetActive(p_visible);
					p_needed = true;
				}
				UpdateBuffs(p_needed);
			}
		}

		private Boolean IsMonsterLastHitRecently()
		{
			return Time.time - m_lastHitTime < 3.2f;
		}

		private Boolean IsMonsterInFrontOfParty()
		{
			if (MyController != null)
			{
				Monster monster = (Monster)MyController;
				Party party = LegacyLogic.Instance.WorldManager.Party;
				GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(party.Position);
				return Position.Distance(monster.Position, party.Position) <= 1f && EDirectionFunctions.GetLineOfSightDirection(party.Position, monster.Position) == party.Direction && slot != null && slot.GetConnection(party.Direction, false, true) != null;
			}
			return false;
		}

		private void OnCharacterAttacksDelayed(Object p_sender, EventArgs p_args)
		{
			if (MyController != null && m_HPBar != null)
			{
				AttacksEventArgs attacksEventArgs = (AttacksEventArgs)p_args;
				foreach (AttacksEventArgs.AttackedTarget attackedTarget in attacksEventArgs.Attacks)
				{
					if (MyController == attackedTarget.AttackTarget)
					{
						UpdateHealthBar((Monster)MyController);
						UpdateBuffs();
						break;
					}
				}
			}
		}

		private void OnCharacterCastSpellDelayed(Object p_sender, EventArgs p_args)
		{
			if (MyController != null && m_HPBar != null)
			{
				SpellEventArgs spellEventArgs = (SpellEventArgs)p_args;
				foreach (SpellTarget spellTarget in spellEventArgs.SpellTargets)
				{
					if (MyController == spellTarget.Target)
					{
						UpdateHealthBar((Monster)MyController);
						UpdateBuffs();
					}
				}
			}
		}

		private void OnCharacterAttacks(Object p_sender, EventArgs p_args)
		{
			if (MyController != null && m_HPBar != null)
			{
				AttacksEventArgs attacksEventArgs = (AttacksEventArgs)p_args;
				foreach (AttacksEventArgs.AttackedTarget attackedTarget in attacksEventArgs.Attacks)
				{
					if (MyController == attackedTarget.AttackTarget)
					{
						m_lastHitTime = Time.time;
						m_lastHitFrame = Time.frameCount;
						break;
					}
				}
			}
		}

		private void OnCharacterCastSpell(Object p_sender, EventArgs p_args)
		{
			if (MyController != null && m_HPBar != null)
			{
				SpellEventArgs spellEventArgs = (SpellEventArgs)p_args;
				foreach (SpellTarget spellTarget in spellEventArgs.SpellTargets)
				{
					if (MyController == spellTarget.Target)
					{
						m_lastHitTime = Time.time;
						m_lastHitFrame = Time.frameCount;
						break;
					}
				}
			}
		}

		private void OnMonsterSelected(Object p_sender, EventArgs p_args)
		{
			if (!m_isDead && MyController != null && m_HPBar != null)
			{
				BaseObjectEventArgs baseObjectEventArgs = (BaseObjectEventArgs)p_args;
				Vector3 localPosition = m_positionRoot.transform.localPosition;
				if (MyController == baseObjectEventArgs.Object)
				{
					ShowSelection();
					localPosition.z = -1f;
				}
				else
				{
					HideSelection();
					localPosition.z = 0f;
				}
				m_positionRoot.transform.localPosition = localPosition;
			}
		}

		private void OnMonsterAllSelectionsRemoved(Object p_sender, EventArgs p_args)
		{
			if (!m_isDead && MyController != null && m_HPBar != null)
			{
				HideSelection();
			}
		}

		private void ShowOutline(Boolean doHighlight, Color color)
		{
			color.a *= m_currentlyUsedOutlineSetting * m_currentlyUsedOutlineSetting;
			if (color.a > 0.001f)
			{
				OutlineGlowFX outlineGlowFX = GetComponent<OutlineGlowFX>();
				if (outlineGlowFX == null || outlineGlowFX.IsDestroyed)
				{
					outlineGlowFX = gameObject.AddComponent<OutlineGlowFX>();
					outlineGlowFX.SetGlobalIntensity(m_currentlyUsedOutlineSetting);
				}
				outlineGlowFX.ShowOutline(doHighlight, color);
			}
			else
			{
				HideOutline();
			}
		}

		private void ShowSelection()
		{
			m_isSelected = true;
			m_monsterNameLabel.alpha = 1f;
			ShowOutline(true, m_selectedColor);
		}

		private void HideOutline()
		{
			OutlineGlowFX[] components = GetComponents<OutlineGlowFX>();
			foreach (OutlineGlowFX outlineGlowFX in components)
			{
				outlineGlowFX.Destroy();
			}
		}

		private void HideSelection()
		{
			m_isSelected = false;
			m_monsterNameLabel.alpha = 0f;
			HideOutline();
		}

		private void OnMonsterStatusChanged(Object p_sender, EventArgs p_args)
		{
			if (MyController != null && m_HPBar != null && m_lastHitFrame != Time.frameCount)
			{
				StatusChangedEventArgs statusChangedEventArgs = (StatusChangedEventArgs)p_args;
				if (MyController == p_sender && statusChangedEventArgs.Type == StatusChangedEventArgs.EChangeType.HEALTH_POINTS)
				{
					UpdateHealthBar((Monster)MyController);
				}
			}
		}

		private void OnMonsterAnimationDieEnd(Object p_sender, EventArgs p_args)
		{
			if (MyController != null && m_HPBar != null && MyController == p_sender)
			{
				m_isDeadAnimationFinished = true;
			}
		}

		private void OnMonsterDied(Object p_sender, EventArgs p_args)
		{
			if (MyController != null && m_HPBar != null && MyController == p_sender)
			{
				m_isDead = true;
				HideSelection();
			}
		}

		private void UpdateHealthBar(Monster mob)
		{
			Vector3 localScale = m_HPBar.cachedTransform.localScale;
			Vector3 localScale2 = m_recentDamage.cachedTransform.localScale;
			m_targetHPBarScaleX = Mathf.Max(0f, m_initialSize * (mob.CurrentHealth / (Single)mob.MaxHealth));
			if (localScale.x > m_targetHPBarScaleX)
			{
				localScale.x = m_targetHPBarScaleX;
				m_HPBar.cachedTransform.localScale = localScale;
				Color red = Color.red;
				red.a = m_recentDamage.color.a;
				m_recentDamage.color = red;
			}
			else
			{
				localScale2.x = m_targetHPBarScaleX;
				m_recentDamage.cachedTransform.localScale = localScale2;
				Color green = Color.green;
				green.a = m_recentDamage.color.a;
				m_recentDamage.color = green;
			}
			mob.CheckLateDieEffects();
		}

		public void InitBuffs()
		{
			m_buffList = new List<MonsterBuffView>();
			m_BuffAnchor = m_dummyObject.transform.Find("BuffPanel");
		}

		public void UpdateBuffs(Object p_sender, EventArgs p_args)
		{
			MonsterBuffUpdateEventArgs monsterBuffUpdateEventArgs = p_args as MonsterBuffUpdateEventArgs;
			if (monsterBuffUpdateEventArgs != null && monsterBuffUpdateEventArgs.Monster == MyController)
			{
				UpdateBuffs();
			}
		}

		private void UpdateBuffs(Boolean p_needed)
		{
			if (p_needed)
			{
				UpdateBuffs();
			}
		}

		private void UpdateBuffs()
		{
			GameObject p_buffViewPrefab = Helper.ResourcesLoad<GameObject>("GuiPrefabs/MonsterBuffView");
			List<MonsterAbilityBase> passiveAbilities = ((Monster)MyController).AbilityHandler.PassiveAbilities;
			HelperBuffs.AddMonsterAbilities(m_BuffAnchor.gameObject, passiveAbilities, m_buffList, p_buffViewPrefab, OnTooltipEvent);
			List<MonsterBuff> buffList = ((Monster)MyController).BuffHandler.BuffList;
			m_buffCount = buffList.Count + passiveAbilities.Count;
			m_buffLabelEnabled = false;
			if (m_buffCount > 0)
			{
				if (!m_movedByBuffs)
				{
					Vector3 localPosition = m_dummyObject.transform.localPosition;
					localPosition.y += 50f;
					TweenPosition.Begin(m_dummyObject, 0.6f, localPosition);
					m_movedByBuffs = true;
				}
			}
			else if (m_movedByBuffs)
			{
				Vector3 localPosition2 = m_dummyObject.transform.localPosition;
				localPosition2.y -= 50f;
				TweenPosition.Begin(m_dummyObject, 0.6f, localPosition2);
				m_movedByBuffs = false;
			}
			HelperBuffs.UpdateMonsterBuffs(m_BuffAnchor.gameObject, m_HPBarRoot.gameObject.activeSelf, buffList, m_buffList, p_buffViewPrefab, OnTooltipEvent, passiveAbilities.Count);
			foreach (MonsterBuffView monsterBuffView in m_buffList)
			{
				if (monsterBuffView.LabelEnabled)
				{
					m_buffLabelEnabled = true;
					break;
				}
			}
		}

		private void OnMouseOver()
		{
			if (!m_isSelected && !m_isDead && IsMonsterInFrontOfParty() && m_monsterNameLabel != null)
			{
				m_monsterNameLabel.alpha = 0.7f;
				ShowOutline(false, m_onMouseOverColor);
				Vector3 localPosition = m_positionRoot.transform.localPosition;
				localPosition.z = -2f;
				m_positionRoot.transform.localPosition = localPosition;
			}
		}

		private void OnMouseExit()
		{
			if (!m_isSelected && m_monsterNameLabel != null)
			{
				HideOutline();
				m_monsterNameLabel.alpha = 0f;
				Vector3 localPosition = m_positionRoot.transform.localPosition;
				localPosition.z = 0f;
				m_positionRoot.transform.localPosition = localPosition;
			}
		}
	}
}
