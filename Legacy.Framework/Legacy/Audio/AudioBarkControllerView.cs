using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.EffectEngine;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Audio
{
	[RequireComponent(typeof(PlayerEntityView))]
	public class AudioBarkControllerView : BaseView
	{
		private static WaitForSeconds WAIT = new WaitForSeconds(2f);

		private List<BarkEventArgs> m_Barklist = new List<BarkEventArgs>();

		private Boolean m_GetPriority;

		private Boolean m_GotID;

		private Single m_CallTime;

		private String[] m_BarkBefore = new String[4];

		private String[] m_OverlapBarkList = new String[4];

		private Int32[] m_OverlappingCharIndex = new Int32[4];

		private String[] m_OverlappingCategory = new String[4];

		private Boolean m_GotRepeat;

		private String m_RepeatedBark;

		private Int32 m_NumOfRepeats;

		private Int32 m_NumOfChecks;

		private PlayerEntityView m_PlayerView;

		private Boolean m_LastConfigTriggerBark;

		protected override void Awake()
		{
			base.Awake();
			m_PlayerView = GetComponent<PlayerEntityView>();
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_ATTACKS, new EventHandler(AttackEvent));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_ATTACKS_RANGED, new EventHandler(AttackEvent));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_CAST_SPELL, new EventHandler(SpellEvent));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.TRAP_TRIGGERED, new EventHandler(TrapEvent));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_BARK, new EventHandler(BarkEvent));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FIXED_DELAY, EEventType.CHARACTER_BARK, new EventHandler(BarkEventDelayed));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(PartyMoved));
			}
			if (MyController != null)
			{
				DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_ATTACKS, new EventHandler(AttackEvent));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_ATTACKS_RANGED, new EventHandler(AttackEvent));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_CAST_SPELL, new EventHandler(SpellEvent));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.TRAP_TRIGGERED, new EventHandler(TrapEvent));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_BARK, new EventHandler(BarkEvent));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FIXED_DELAY, EEventType.CHARACTER_BARK, new EventHandler(BarkEventDelayed));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(PartyMoved));
				m_LastConfigTriggerBark = ConfigManager.Instance.Options.TriggerBarks;
				LoadBarkSounds(m_LastConfigTriggerBark);
			}
		}

		private void AttackEvent(Object p_sender, EventArgs p_args)
		{
			m_GetPriority = false;
			m_GotID = false;
			AttacksEventArgs attacksEventArgs = (AttacksEventArgs)p_args;
			foreach (AttacksEventArgs.AttackedTarget attackedTarget in attacksEventArgs.Attacks)
			{
				if (attackedTarget.BarkEventArgs != null)
				{
					for (Int32 i = 0; i < attackedTarget.BarkEventArgs.Length; i++)
					{
						BarkEventArgs barkEventArgs = attackedTarget.BarkEventArgs[i];
						if (barkEventArgs != null)
						{
							if (barkEventArgs.priority == 100 || (barkEventArgs.priority == 0 && !m_GotID))
							{
								String text;
								String text2;
								GenerateAudioID(barkEventArgs.character, barkEventArgs.barkclip, out text, out text2);
								if (!AudioController.IsPlaying(text2))
								{
									m_GotID = true;
									PlayBark(barkEventArgs.character.Index, text2);
								}
							}
							else if (!m_GetPriority && !m_GotID)
							{
								m_Barklist.Add(barkEventArgs);
								m_GetPriority = true;
							}
						}
						if (i == attackedTarget.BarkEventArgs.Length - 1)
						{
							m_GetPriority = true;
						}
					}
				}
			}
		}

		private Boolean IsBarkEventDelayed(BarkEventArgs p_args)
		{
			return p_args.barkclip == "Poison" || p_args.barkclip == "Cursed" || p_args.barkclip == "Immobile" || p_args.barkclip == "Feeble" || p_args.barkclip == "Weak" || p_args.barkclip == "LevelUp";
		}

		private void BarkEvent(Object p_sender, EventArgs p_args)
		{
			BarkEventArgs p_args2 = (BarkEventArgs)p_args;
			if (!IsBarkEventDelayed(p_args2))
			{
				BarkEventGeneric(p_sender, p_args);
			}
		}

		private void BarkEventDelayed(Object p_sender, EventArgs p_args)
		{
			BarkEventArgs p_args2 = (BarkEventArgs)p_args;
			if (IsBarkEventDelayed(p_args2))
			{
				BarkEventGeneric(p_sender, p_args);
			}
		}

		private void BarkEventGeneric(Object p_sender, EventArgs p_args)
		{
			m_GetPriority = false;
			m_GotID = false;
			BarkEventArgs barkEventArgs = (BarkEventArgs)p_args;
			if (barkEventArgs.priority == 100 || (barkEventArgs.priority == 0 && !m_GotID))
			{
				m_GotID = true;
				String text;
				String text2;
				GenerateAudioID(barkEventArgs.character, barkEventArgs.barkclip, out text, out text2);
				if (!AudioController.IsPlaying(text2))
				{
					if (barkEventArgs.barkclip != "Quest" && barkEventArgs.barkclip != "QuestComplete" && barkEventArgs.barkclip != "Success" && barkEventArgs.barkclip != "Story1" && barkEventArgs.barkclip != "Story2" && barkEventArgs.barkclip != "Story3" && barkEventArgs.barkclip != "Story4" && barkEventArgs.barkclip != "Story5" && barkEventArgs.barkclip != "Story6" && barkEventArgs.barkclip != "Story7" && barkEventArgs.barkclip != "Story8" && barkEventArgs.barkclip != "Story9" && barkEventArgs.barkclip != "Story10" && barkEventArgs.barkclip != "Loot")
					{
						PlayBark(barkEventArgs.character.Index, text2);
					}
					else if (ConfigManager.Instance.Options.TriggerBarks && !ConfigManager.Instance.Game.ChineseVersion)
					{
						StartCoroutine(PlayLateBark(barkEventArgs.character.Index, text2));
					}
				}
			}
			else if (!m_GetPriority && !m_GotID)
			{
				m_Barklist.Add(barkEventArgs);
				m_GetPriority = true;
			}
		}

		private void PartyMoved(Object p_sender, EventArgs p_args)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (p_sender == party)
			{
				GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(party.Position);
				LegacyLogic.Instance.CharacterBarkHandler.UpdateCurrenExplorationData(slot);
			}
		}

		private void SpellEvent(Object p_sender, EventArgs p_args)
		{
			m_GetPriority = false;
			m_GotID = false;
			SpellEventArgs spellEventArgs = (SpellEventArgs)p_args;
			if (spellEventArgs.BarkEventArgs == null)
			{
				return;
			}
			for (Int32 i = 0; i < spellEventArgs.BarkEventArgs.Length; i++)
			{
				BarkEventArgs barkEventArgs = spellEventArgs.BarkEventArgs[i];
				if (barkEventArgs != null)
				{
					if (barkEventArgs.priority == 100 || (barkEventArgs.priority == 0 && !m_GotID))
					{
						m_GotID = true;
						String text;
						String text2;
						GenerateAudioID(barkEventArgs.character, barkEventArgs.barkclip, out text, out text2);
						if (!AudioController.IsPlaying(text2))
						{
							PlayBark(barkEventArgs.character.Index, text2);
						}
					}
					else if (!m_GetPriority && !m_GotID)
					{
						m_Barklist.Add(barkEventArgs);
					}
				}
				if (i == spellEventArgs.BarkEventArgs.Length - 1)
				{
					m_GetPriority = true;
				}
			}
		}

		private void TrapEvent(Object p_sender, EventArgs p_args)
		{
			m_GetPriority = false;
			m_GotID = false;
			TrapEventArgs trapEventArgs = (TrapEventArgs)p_args;
			if (trapEventArgs.BarkEventArgs == null)
			{
				return;
			}
			for (Int32 i = 0; i < trapEventArgs.BarkEventArgs.Length; i++)
			{
				BarkEventArgs barkEventArgs = trapEventArgs.BarkEventArgs[i];
				if (barkEventArgs != null)
				{
					if (barkEventArgs.priority == 100 || (barkEventArgs.priority == 0 && !m_GotID))
					{
						m_GotID = true;
						String text;
						String text2;
						GenerateAudioID(barkEventArgs.character, barkEventArgs.barkclip, out text, out text2);
						if (!AudioController.IsPlaying(text2))
						{
							PlayBark(barkEventArgs.character.Index, text2);
						}
					}
					else if (!m_GetPriority && !m_GotID)
					{
						m_Barklist.Add(barkEventArgs);
					}
				}
				if (i == trapEventArgs.BarkEventArgs.Length - 1)
				{
					m_GetPriority = true;
				}
			}
		}

		private void Update()
		{
			Boolean triggerBarks = ConfigManager.Instance.Options.TriggerBarks;
			if (triggerBarks != m_LastConfigTriggerBark)
			{
				m_LastConfigTriggerBark = triggerBarks;
				LoadBarkSounds(triggerBarks);
			}
			if (m_GetPriority)
			{
				if (m_CallTime < Time.time || m_NumOfChecks == 0)
				{
					m_OverlapBarkList.Clear<String>();
					m_OverlappingCategory.Clear<String>();
					m_OverlappingCharIndex.Clear<Int32>();
				}
				for (Int32 i = 0; i < 4; i++)
				{
					CheckPriorityAndPlay(i);
				}
				m_Barklist.Clear();
				m_GetPriority = false;
			}
			WatchOverlapBarks();
		}

		private void LoadBarkSounds(Boolean enable)
		{
			Party party = (Party)MyController;
			for (Int32 i = 0; i < 4; i++)
			{
				Character member = party.GetMember(i);
				String text;
				String text2;
				GenerateAudioID(member, String.Empty, out text, out text2);
				text = "AudioToolkit/AudioCategories/AudioController_" + text;
				if (enable)
				{
					AudioManager.Instance.Request(text);
				}
				else
				{
					AudioManager.Instance.Unload(text);
				}
			}
		}

		private void CheckPriorityAndPlay(Int32 p_charindex)
		{
			Boolean flag = false;
			Int32 num = Int32.MaxValue;
			BarkEventArgs barkEventArgs = null;
			foreach (BarkEventArgs barkEventArgs2 in m_Barklist)
			{
				if (barkEventArgs2.priority < num && barkEventArgs2.character.Index == p_charindex)
				{
					flag = barkEventArgs2.onrecieve;
					num = barkEventArgs2.priority;
					barkEventArgs = barkEventArgs2;
				}
			}
			if (barkEventArgs == null)
			{
				return;
			}
			if (!flag && !m_GotID)
			{
				String p_Category;
				String text;
				GenerateAudioID(barkEventArgs.character, barkEventArgs.barkclip, out p_Category, out text);
				PlayBark(barkEventArgs.character.Index, text);
			}
			else if (m_BarkBefore[p_charindex] != barkEventArgs.barkclip && !m_GotID)
			{
				String p_Category;
				String text;
				GenerateAudioID(barkEventArgs.character, barkEventArgs.barkclip, out p_Category, out text);
				CheckForOverlapping(barkEventArgs.barkclip, text, barkEventArgs.character.Index, p_Category);
			}
			m_BarkBefore[p_charindex] = barkEventArgs.barkclip;
		}

		private void CheckForOverlapping(String p_Clip, String p_AudioID, Int32 p_Char, String p_Category)
		{
			if (m_CallTime > Time.time || m_NumOfChecks == 0)
			{
				if (m_NumOfChecks == 0)
				{
					m_CallTime = Time.time + 0.1f;
					m_OverlapBarkList[m_NumOfRepeats] = p_AudioID;
					m_OverlappingCharIndex[m_NumOfRepeats] = p_Char;
					m_OverlappingCategory[m_NumOfRepeats] = p_Category;
					m_RepeatedBark = p_Clip;
					m_GotRepeat = true;
					m_NumOfRepeats++;
				}
				if (m_NumOfChecks != 0)
				{
					if (p_Clip == m_RepeatedBark)
					{
						m_OverlapBarkList[m_NumOfRepeats] = p_AudioID;
						m_OverlappingCharIndex[m_NumOfRepeats] = p_Char;
						m_OverlappingCategory[m_NumOfRepeats] = p_Category;
						m_RepeatedBark = p_Clip;
						m_GotRepeat = true;
						m_NumOfRepeats++;
					}
					if (p_Clip != m_RepeatedBark)
					{
						PlayBark(p_Char, p_AudioID);
					}
				}
				m_NumOfChecks++;
				m_GotID = false;
			}
		}

		private void WatchOverlapBarks()
		{
			if ((m_CallTime < Time.time || m_NumOfChecks >= 3) && m_GotRepeat)
			{
				Int32 num = Random.Range(0, m_NumOfRepeats);
				for (Int32 i = 0; i <= m_NumOfRepeats; i++)
				{
					if (i == num)
					{
						PlayBark(m_OverlappingCharIndex[i], m_OverlapBarkList[i]);
					}
				}
				m_RepeatedBark = " ";
				m_NumOfRepeats = 0;
				m_NumOfChecks = 0;
				m_GotRepeat = false;
			}
		}

		private void PlayBark(Int32 characterIndex, String p_barkID)
		{
			if (!ConfigManager.Instance.Options.TriggerBarks || ConfigManager.Instance.Game.ChineseVersion)
			{
				return;
			}
			if (AudioManager.Instance.IsValidAudioID(p_barkID))
			{
				if (!LegacyLogic.Instance.ConversationManager.IsOpen || String.IsNullOrEmpty(LegacyLogic.Instance.ConversationManager.IndoorScene))
				{
					AudioManager.Instance.RequestPlayAudioID(p_barkID, 100, -1f, m_PlayerView.GetMemberGameObject(characterIndex).transform, 1f, 0f, 0f, null);
				}
				else
				{
					AudioManager.Instance.RequestPlayAudioID(p_barkID, 100);
				}
			}
			else
			{
				Debug.LogError("AudioID: '" + p_barkID + "' not found!\n");
			}
		}

		private IEnumerator PlayLateBark(Int32 characterIndex, String p_barkID)
		{
			yield return WAIT;
			if (AudioManager.Instance.IsValidAudioID(p_barkID))
			{
				if (!LegacyLogic.Instance.ConversationManager.IsOpen || String.IsNullOrEmpty(LegacyLogic.Instance.ConversationManager.IndoorScene))
				{
					AudioManager.Instance.RequestPlayAudioID(p_barkID, 100, -1f, m_PlayerView.GetMemberGameObject(characterIndex).transform, 1f, 0f, 0f, null);
				}
				else
				{
					AudioManager.Instance.RequestPlayAudioID(p_barkID, 100);
				}
			}
			else
			{
				Debug.LogError("AudioID: '" + p_barkID + "' not found!\n");
			}
			yield break;
		}

		private static void GenerateAudioID(Character character, String barkClipname, out String category, out String audioID)
		{
			String text = character.Class.Race.ToString().ToLowerInvariant();
			text = Char.ToUpperInvariant(text[0]) + text.Remove(0, 1);
			String text2 = character.Gender.ToString().ToLowerInvariant();
			text2 = Char.ToUpperInvariant(text2[0]) + text2.Remove(0, 1);
			String text3 = character.VoiceSetting.ToString().ToLowerInvariant();
			text3 = Char.ToUpperInvariant(text3[0]) + text3.Remove(0, 1);
			category = "Bark" + text + text2 + text3;
			audioID = String.Concat(new String[]
			{
				text,
				"_",
				text2,
				"_",
				text3,
				"_",
				barkClipname
			});
		}
	}
}
