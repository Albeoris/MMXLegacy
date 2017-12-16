using System;
using System.Xml.Serialization;
using Legacy.Core.Entities.Items;
using Legacy.Core.Entities.Skills;
using Legacy.Core.NpcInteraction;
using Legacy.Core.NpcInteraction.Conditions;
using Legacy.Core.NpcInteraction.Functions;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells.CharacterSpells;

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
	public class NpcConversationStaticData
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

		public struct Dialog
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
	}
}
