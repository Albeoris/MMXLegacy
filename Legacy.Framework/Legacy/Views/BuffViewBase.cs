using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Spells;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.Core.Spells.MonsterSpells;
using Legacy.EffectEngine;
using Legacy.EffectEngine.Effects;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	public abstract class BuffViewBase : BaseView
	{
		private List<FXQueue> m_Queues = new List<FXQueue>();

		private List<FXQueue> m_QueuesLight = new List<FXQueue>();

		protected Dictionary<String, String> m_BuffAppliedBySpellMap = new Dictionary<String, String>();

		protected Int32 m_LastAddFrame = -1;

		protected abstract EEventType BuffAddEventType { get; }

		protected abstract EEventType BuffRemoveEventType { get; }

		protected abstract Boolean IsBuffAddEventForMe(Object p_sender, EventArgs p_args);

		protected abstract Boolean IsApplySpellEventForMe(Object p_sender, EventArgs p_args, out String out_buffNameKey, out String out_spellNameKey);

		protected abstract void GetBuffBaseFXPath(Object p_sender, EventArgs p_args, out String out_buffBaseFXPath, out String out_buffNameKey);

		protected abstract String GetBuffDebugName(Object p_sender, EventArgs p_args);

		protected virtual EEventType[] ApplySpellEventTypes => new EEventType[]
		{
		    EEventType.MONSTER_CAST_SPELL,
		    EEventType.CHARACTER_CAST_SPELL
		};

	    protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, BuffAddEventType, new EventHandler(OnBuffEvent));
				foreach (EEventType p_type in ApplySpellEventTypes)
				{
					LegacyLogic.Instance.EventManager.UnregisterEvent(p_type, new EventHandler(OnApplySpell));
				}
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.GAME_LOADED, new EventHandler(OnGameLoaded));
			}
			if (MyController != null)
			{
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, BuffAddEventType, new EventHandler(OnBuffEvent));
				foreach (EEventType p_type2 in ApplySpellEventTypes)
				{
					LegacyLogic.Instance.EventManager.RegisterEvent(p_type2, new EventHandler(OnApplySpell));
				}
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.GAME_LOADED, new EventHandler(OnGameLoaded));
			}
		}

		protected virtual void OnGameLoaded(Object sender, EventArgs e)
		{
		}

		protected virtual Boolean IsBuffRemovedByThisEvent(Object p_AddEventSender, EventArgs p_AddEventArgs, Object p_RemoveEventSender, EventArgs p_RemoveEventArgs)
		{
			return p_AddEventSender == p_RemoveEventSender;
		}

		protected void CancelAllFX()
		{
			foreach (FXQueue fxqueue in m_Queues)
			{
				if (fxqueue != null)
				{
					Destroy(fxqueue.gameObject);
				}
			}
			m_Queues.Clear();
			foreach (FXQueue fxqueue2 in m_QueuesLight)
			{
				if (fxqueue2 != null)
				{
					Destroy(fxqueue2.gameObject);
				}
			}
			m_QueuesLight.Clear();
		}

		protected void CancelAllFX_butNotVisionFX()
		{
			foreach (FXQueue fxqueue in m_Queues)
			{
				if (fxqueue != null)
				{
					Destroy(fxqueue.gameObject);
				}
			}
			m_Queues.Clear();
		}

		protected void OnBuffEvent(Object p_sender, EventArgs p_args)
		{
			if (IsBuffAddEventForMe(p_sender, p_args))
			{
				String text;
				String text2;
				GetBuffBaseFXPath(p_sender, p_args, out text, out text2);
				if (!String.IsNullOrEmpty(text) && !String.IsNullOrEmpty(text2))
				{
					if (text == "SKIP_FX")
					{
						return;
					}
					String text3 = null;
					if (m_BuffAppliedBySpellMap.ContainsKey(text2))
					{
						text3 = m_BuffAppliedBySpellMap[text2];
						if (Helper.ResourcesLoadLinked<BuffFX>(text + text3, false) != null)
						{
							text += text3;
						}
					}
					BuffFX buffFX = Helper.ResourcesLoadLinked<BuffFX>(text, false);
					if (buffFX != null)
					{
						if (m_LastAddFrame == Time.frameCount)
						{
							return;
						}
						m_LastAddFrame = Time.frameCount;
						if (text3 == "SPELL_FIRE_TORCHLIGHT" || text3 == "SPELL_LIGHT_ORB" || text3 == "SPELL_DARK_VISION" || text3 == "SPELL_LIGHT_ORB_LONG" || text3 == "SPELL_FIRE_TORCHLIGHT_LONG")
						{
							CancelAllFX();
						}
						else
						{
							CancelAllFX_butNotVisionFX();
						}
						buffFX = Helper.Instantiate<BuffFX>(buffFX);
						buffFX.SetBuffRemoveCondition(BuffRemoveEventType, (Object p_RemoveEventSender, EventArgs p_RemoveEventArgs) => IsBuffRemovedByThisEvent(p_sender, p_args, p_RemoveEventSender, p_RemoveEventArgs));
						FXQueue fxqueue = new GameObject(name + " " + buffFX.name + " FXQueue").AddComponent<FXQueue>();
						fxqueue.SetData(new FXQueue.Entry[]
						{
							new FXQueue.Entry(buffFX, 0f, 0f)
						}, 0);
						FXArgs args = new FXArgs(gameObject, gameObject, gameObject, gameObject, Vector3.zero, transform.forward, -transform.right, Vector3.zero);
						fxqueue.Execute(args);
						if (text3 == "SPELL_FIRE_TORCHLIGHT" || text3 == "SPELL_LIGHT_ORB" || text3 == "SPELL_DARK_VISION" || text3 == "SPELL_LIGHT_ORB_LONG" || text3 == "SPELL_FIRE_TORCHLIGHT_LONG")
						{
							m_QueuesLight.Add(fxqueue);
						}
						else
						{
							m_Queues.Add(fxqueue);
						}
					}
					else
					{
						Debug.LogError("OnBuffEvent: buff's (" + GetBuffDebugName(p_sender, p_args) + ") given GFX does not exist! " + text);
					}
				}
				else
				{
					Debug.LogWarning(String.Concat(new String[]
					{
						"OnBuffEvent: buff(",
						GetBuffDebugName(p_sender, p_args),
						") GFX or buffKey missing! gfx='",
						text,
						"' buffKey='",
						text2,
						"'"
					}));
				}
			}
		}

		protected String GetSpellNameKey(Spell p_spell)
		{
			if (p_spell is CharacterSpell)
			{
				return ((CharacterSpell)p_spell).SpellType.ToString();
			}
			if (p_spell is MonsterSpell)
			{
				return ((MonsterSpell)p_spell).SpellType.ToString();
			}
			return p_spell.NameKey;
		}

		private void OnApplySpell(Object p_sender, EventArgs p_args)
		{
			String key;
			String value;
			if (IsApplySpellEventForMe(p_sender, p_args, out key, out value))
			{
				m_BuffAppliedBySpellMap[key] = value;
			}
		}
	}
}
