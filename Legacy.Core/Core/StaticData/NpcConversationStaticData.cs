using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Legacy.Core.Entities.Items;
using Legacy.Core.Entities.Skills;
using Legacy.Core.NpcInteraction;
using Legacy.Core.NpcInteraction.Conditions;
using Legacy.Core.NpcInteraction.Functions;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.Utilities;

namespace Legacy.Core.StaticData
{
	[XmlInclude(typeof(GiveTokenFunction))]
	[XmlInclude(typeof(GoToFunction))]
	[XmlInclude(typeof(SkillLevelReachedCondition))]
	[XmlInclude(typeof(IdentifyFunction))]
	[XmlInclude(typeof(UnidentifiedItemsCondition))]
	[XmlInclude(typeof(HirelingNoFreeSlotCondition))]
	[XmlInclude(typeof(HirelingFreeSlotCondition))]
	[XmlInclude(typeof(HirelingPeriodicityCondition))]
	[XmlInclude(typeof(HirelingFunction))]
	[XmlInclude(typeof(StartDLCFunction))]
	[XmlInclude(typeof(QuestInactiveCondition))]
	[XmlInclude(typeof(QuestActiveCondition))]
	[XmlInclude(typeof(PartyNotHasRaceCondition))]
	[XmlInclude(typeof(PartyNotHasClassCondition))]
	[XmlInclude(typeof(PartyHasRaceGenderCondition))]
	[XmlInclude(typeof(QuestFinishedCondition))]
	[XmlInclude(typeof(QuestNotInactiveCondition))]
	[XmlInclude(typeof(PartyHasRaceCondition))]
	[XmlInclude(typeof(QuestNotActiveCondition))]
	[XmlInclude(typeof(BrokenItemsCondition))]
	[XmlInclude(typeof(QuestNotFinishedCondition))]
	[XmlInclude(typeof(CharacterCanDoSomethingCondition))]
	[XmlInclude(typeof(PartyHasClassCondition))]
	[XmlInclude(typeof(ItemTradingFunction))]
	[XmlInclude(typeof(QuestFunction))]
	[XmlInclude(typeof(QuitFunction))]
	[XmlInclude(typeof(RestedCondition))]
	[XmlInclude(typeof(MaxSuppliesCondition))]
	[XmlInclude(typeof(ObjectiveSolvedCondition))]
	[XmlInclude(typeof(ObjectiveNotSolvedCondition))]
	[XmlInclude(typeof(TokenAcquiredCondition))]
	[XmlInclude(typeof(TokenNotAcquiredCondition))]
	[XmlInclude(typeof(MuleInventoryEmptyCondition))]
	[XmlInclude(typeof(MuleInventoryNotEmptyCondition))]
	[XmlInclude(typeof(DayTimeEqualsCondition))]
	[XmlInclude(typeof(DayTimeNotEqualsCondition))]
	[XmlInclude(typeof(RewardUnlockedCondition))]
	[XmlInclude(typeof(PrivilegeUnlockedCondition))]
	[XmlInclude(typeof(HirelingHiredCondition))]
	[XmlInclude(typeof(HirelingNotHiredCondition))]
	[XmlInclude(typeof(GoldCondition))]
	[XmlInclude(typeof(CheckOnMapCondition))]
	[XmlInclude(typeof(CheckNotOnMapCondition))]
	[XmlInclude(typeof(UnrestoredCharactersCondition))]
	[XmlInclude(typeof(DeadCharacterCondition))]
	[XmlInclude(typeof(RemoveTokenFunction))]
	[XmlInclude(typeof(CharacterHasCurableCondition))]
	[XmlInclude(typeof(AutoSaveFunction))]
	[XmlInclude(typeof(SolveQuestFunction))]
	[XmlInclude(typeof(RepairFunction))]
	[XmlInclude(typeof(TriggerAggroFunction))]
	[XmlInclude(typeof(MoveInteractiveObjectFunction))]
	[XmlInclude(typeof(ChangeNpcContainerFunction))]
	[XmlInclude(typeof(AddBuffFunction))]
	[XmlInclude(typeof(ActivateLevelTriggerFunction))]
	[XmlInclude(typeof(TravelFunction))]
	[XmlInclude(typeof(DecreaseAttributeFunction))]
	[XmlInclude(typeof(RestFunction))]
	[XmlInclude(typeof(WhoWillFunction))]
	[XmlInclude(typeof(UserInputFunction))]
	[XmlInclude(typeof(CutsceneStateFunction))]
	[XmlInclude(typeof(CutsceneNextStateFunction))]
	[XmlInclude(typeof(CutsceneBackStateFunction))]
	[XmlInclude(typeof(RemoveSetFunction))]
	[XmlInclude(typeof(KillMonsterFunction))]
	[XmlInclude(typeof(RespecFunction))]
	[XmlInclude(typeof(ExecuteTriggerFunction))]
	[XmlInclude(typeof(ObeliskFunction))]
	[XmlInclude(typeof(ForceSolveQuestFunction))]
	[XmlInclude(typeof(GameOverFunction))]
	[XmlInclude(typeof(SkillLearnedCondition))]
	[XmlInclude(typeof(DialogEntryInjection))]
    public class NpcConversationStaticData : IXmlStaticData
    {
		[XmlAttribute("rootDialogID")]
		public Int32 m_rootDialogID;

		[XmlElement("dialog")]
		public Dialog[] m_dialogs;

		[XmlElement("offer")]
		public Offer[] m_offers;

		[XmlElement("spellOffer")]
		public SpellOffer[] m_spellOffers;

		[XmlElement("bookmark")]
		public DialogBookmark[] m_bookmarks;

		public class Dialog
		{
			[XmlAttribute("id")]
			public Int32 m_id;

			[XmlAttribute("randomText")]
			public Boolean m_randomText;

			[XmlElement("text")]
			public DialogText[] m_texts;

			[XmlElement("entry")]
			public DialogEntry[] m_entries;

			[XmlElement("feature")]
			public DialogFeature m_feature;

			[XmlAttribute("fakeNpcID")]
			public Int32 m_fakeNpcID;

			[XmlAttribute("hideBackButton")]
			public Boolean m_hideBackButton;

			[XmlAttribute("hideNpcsAndCloseButton")]
			public Boolean m_hideNpcsAndCloseButton;

			[XmlAttribute("hideNpcAndPortrait")]
			public Boolean m_hideNpcAndPortrait;

			[XmlAttribute("backToDialodId")]
			public Int32 m_backToDialodId;

		    public void Update(Dialog newDialog)
		    {
                if (newDialog.m_entries == null || newDialog.m_entries.Length == 0)
		            return; // Not implemented

                var result = new LinkedList<DialogEntry>(m_entries);
		        var current = new Dictionary<String, LinkedListNode<DialogEntry>>(m_entries.Length);

                if (result.Count > 0)
                {
                    for (var node = result.First; node != null; node = node.Next)
                        current[node.Value.GetKeyFromText()] = node;
                }

                foreach (DialogEntry ent in newDialog.m_entries)
                {
                    DialogEntry entry = ent;
                    if (entry.m_injection == null)
                    {
                        result.AddLast(entry);
                    }
                    else
                    {
                        // Remove injection to avoid memory growing
                        DialogEntryInjection injection = entry.m_injection;
                        entry.m_injection = null;

                        LinkedListNode<DialogEntry> node;
                        if (current.TryGetValue(injection.TextKey, out node))
                        {
                            switch (injection.InjectionType)
                            {
                                case EDialogInjectionType.InsertAfter:
                                    result.AddAfter(node, entry);
                                    break;
                                case EDialogInjectionType.InsertBefore:
                                    result.AddBefore(node, entry);
                                    break;
                                case EDialogInjectionType.Replace:
                                    result.AddAfter(node, entry);
                                    result.Remove(node);
                                    break;
                                default:
                                    throw new NotImplementedException(injection.InjectionType.ToString());
                            }
                        }
                        else
                        {
                            result.AddLast(entry);
                            LegacyLogger.LogError($"Cannot find injection target [{injection.TextKey}] in the dialog.");
                        }
                    }
                }

                m_entries = result.ToArray();
		    }
		}

		public struct DialogFeature
		{
			[XmlAttribute("type")]
			public EDialogFeature m_type;

			[XmlAttribute("skillID")]
			public Int32 m_skillID;

			[XmlAttribute("skillRank")]
			public ETier m_skillRank;

			[XmlAttribute("wellRested")]
			public Boolean m_wellRested;

			[XmlAttribute("optionKey")]
			public String m_optionKey;

			[XmlAttribute("price")]
			public Int32 m_price;

			[XmlAttribute("sharePrice")]
			public Int32 m_sharePrice;

			[XmlAttribute("dialogID")]
			public Int32 m_dialogID;

			[XmlAttribute("npcID")]
			public Int32 m_npcID;

			[XmlAttribute("count")]
			public Int32 m_count;

			[XmlAttribute("repairType")]
			public ERepairType m_repairType;

			[XmlElement("curableCondition")]
			public CurableCondition[] m_curableConditions;

			[XmlArrayItem("spell")]
			[XmlArray("spells")]
			public ECharacterSpell[] m_spells;

			[XmlAttribute("characterAttribute")]
			public EPotionTarget m_characterAttribute;

			[XmlAttribute("minimumValue")]
			public Int32 m_minimumValue;

			[XmlAttribute("successDialogID")]
			public Int32 m_successDialogID;

			[XmlAttribute("failDialogID")]
			public Int32 m_failDialogID;

			[XmlAttribute("tokenID")]
			public Int32 m_tokenID;

			[XmlAttribute("popupText")]
			public String m_popupText;

			[XmlElement("userInputSolution")]
			public UserInputSolution[] m_inputSolutions;

			[XmlAttribute("mapName")]
			public String m_mapName;

			[XmlAttribute("targetSpawnerId")]
			public Int32 m_targetSpawnId;

			[XmlAttribute("questID")]
			public Int32 m_questID;
		}

		public struct UserInputSolution
		{
			[XmlAttribute("key")]
			public String m_answerLocaKey;
		}

		public struct CurableCondition
		{
			[XmlAttribute("type")]
			public ECondition m_type;
		}

		public struct DialogEntry
		{
			[XmlElement("function")]
			public DialogFunction[] m_functions;

			[XmlElement("text")]
			public DialogText[] m_texts;

			[XmlElement("condition")]
			public DialogCondition[] m_conditions;

		    [XmlElement("injection")]
		    public DialogEntryInjection m_injection;

		    public String GetKeyFromText()
		    {
		        if (m_texts == null)
		            return "NULL";
		        if (m_texts.Length == 0)
		            return "EMPTY";
		        if (m_texts.Length == 1)
		            return m_texts[0].m_locaKey;

		        StringBuilder sb = new StringBuilder(m_texts.Length * 16);
		        sb.Append(m_texts[0].m_locaKey);
		        for (Int32 i = 1; i < m_texts.Length; i++)
		        {
		            sb.Append(';');
		            sb.Append(m_texts[i].m_locaKey);
		        }

		        return sb.ToString();
		    }
		}

		public struct DialogText
		{
			[XmlAttribute("locaKey")]
			public String m_locaKey;

			[XmlAttribute("voiceID")]
			public String m_voiceID;

			[XmlAttribute("replacement")]
			public EDialogReplacement m_replacement;

			[XmlElement("condition")]
			public DialogCondition[] m_conditions;
		}

		public struct Offer
		{
			[XmlAttribute("id")]
			public Int32 m_id;

			[XmlElement("condition")]
			public DialogCondition[] m_conditions;
		}

		public struct SpellOffer
		{
			[XmlAttribute("id")]
			public Int32 m_id;

			[XmlElement("condition")]
			public DialogCondition[] m_conditions;
		}

		public struct DialogBookmark
		{
			[XmlAttribute("function")]
			public String m_function;

			[XmlAttribute("dialogID")]
			public Int32 m_dialogID;

			[XmlAttribute("icon")]
			public String m_icon;
		}

        public void Update(IXmlStaticData additionalData)
        {
            NpcConversationStaticData other = (NpcConversationStaticData)additionalData;
            if (other.m_dialogs != null)
            {
                Dictionary<Int32, Dialog> current = m_dialogs.ToDictionary(d => d.m_id);
                Dialog[] updated = other.m_dialogs;

                List<Dialog> result = new List<Dialog>(current.Count + updated.Length);
                result.AddRange(m_dialogs);

                foreach (Dialog newDialog in updated)
                {
                    Dialog oldDialog;
                    if (current.TryGetValue(newDialog.m_id, out oldDialog))
                        oldDialog.Update(newDialog);
                    else
                        result.Add(newDialog);
                }

                if (result.Count > m_dialogs.Length)
                    m_dialogs = result.ToArray();
            }
        }
    }
}
