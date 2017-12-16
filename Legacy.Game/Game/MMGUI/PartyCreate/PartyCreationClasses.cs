using System;
using System.Collections.Generic;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.PartyCreate
{
	[AddComponentMenu("MM Legacy/MMGUI/PartyCreate/PartyCreationClasses")]
	public class PartyCreationClasses : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_classDesc;

		[SerializeField]
		private UILabel m_promotionCaption;

		[SerializeField]
		private UILabel m_promotionClass;

		[SerializeField]
		private UISprite m_promotionIcon;

		[SerializeField]
		private ClassSelectButton m_btnMight;

		[SerializeField]
		private ClassSelectButton m_btnHybrid;

		[SerializeField]
		private ClassSelectButton m_btnMagic;

		[SerializeField]
		private UITexture m_malePosing;

		[SerializeField]
		private UITexture m_femalePosing;

		[SerializeField]
		private UILabel[] m_abilityHeader;

		[SerializeField]
		private UILabel[] m_abilityDesc;

		[SerializeField]
		private UISprite[] m_abilityBorder;

		[SerializeField]
		private UISprite[] m_abilityIcon;

		[SerializeField]
		private UISprite m_textBG;

		[SerializeField]
		private UISprite m_textBorder;

		[SerializeField]
		private GameObject m_textRoot;

		private PartyCreator m_partyCreator;

		public void Init(PartyCreator p_partyCreator)
		{
			m_partyCreator = p_partyCreator;
			m_btnMight.OnClassSelected += OnClassSelected;
			m_btnHybrid.OnClassSelected += OnClassSelected;
			m_btnMagic.OnClassSelected += OnClassSelected;
		}

		public void OnAfterActivate()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			if (selectedDummyCharacter.Class == EClass.NONE && selectedDummyCharacter.Race != ERace.NONE)
			{
				selectedDummyCharacter.Class = m_partyCreator.GetUnusedClass();
			}
			UpdatePromotion();
			UpdateButtons();
		}

		public void UndoSelection()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			selectedDummyCharacter.Class = EClass.NONE;
		}

		public void Cleanup()
		{
			m_btnMight.OnClassSelected -= OnClassSelected;
			m_btnHybrid.OnClassSelected -= OnClassSelected;
			m_btnMagic.OnClassSelected -= OnClassSelected;
		}

		private void OnClassSelected(Object p_sender, EventArgs p_args)
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			EClass selectedClass = (p_sender as ClassSelectButton).SelectedClass;
			selectedDummyCharacter.Class = selectedClass;
			UpdatePromotion();
			UpdateButtons();
			StatusChangedEventArgs p_eventArgs = new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.CONDITIONS);
			LegacyLogic.Instance.EventManager.InvokeEvent(selectedDummyCharacter, EEventType.DUMMY_CHARACTER_STATUS_CHANGED, p_eventArgs);
		}

		private void UpdatePromotion()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			if (selectedDummyCharacter.Class != EClass.NONE)
			{
				CharacterClassStaticData staticData = StaticDataHandler.GetStaticData<CharacterClassStaticData>(EDataType.CHARACTER_CLASS, (Int32)selectedDummyCharacter.Class);
				m_classDesc.text = LocaManager.GetText(staticData.NameKey + "_DESC");
				m_promotionClass.text = GetAdvancedClassName(selectedDummyCharacter.Class);
				m_promotionIcon.spriteName = GetAdvancedClassSprite(selectedDummyCharacter.Class);
				Int32 num = UpdateAbilities(selectedDummyCharacter.Class);
				Single num2 = m_classDesc.relativeSize.y * m_classDesc.transform.localScale.y;
				Vector3 localPosition = m_classDesc.transform.localPosition;
				m_promotionCaption.transform.localPosition = new Vector3(m_promotionCaption.transform.localPosition.x, localPosition.y - num2 - 35f, m_promotionCaption.transform.localPosition.z);
				m_promotionClass.transform.localPosition = new Vector3(m_promotionClass.transform.localPosition.x, localPosition.y - m_promotionIcon.cachedTransform.localScale.y / 2f - num2 - 95f, m_promotionClass.transform.localPosition.z);
				m_promotionIcon.transform.localPosition = new Vector3(m_promotionIcon.transform.localPosition.x, localPosition.y - num2 - 95f, m_promotionIcon.transform.localPosition.z);
				Single num3 = m_promotionIcon.transform.localPosition.y - m_promotionIcon.transform.localScale.y - 30f;
				Single y = m_abilityHeader[0].cachedTransform.localScale.y;
				for (Int32 i = 0; i < num; i++)
				{
					m_abilityHeader[i].transform.localPosition = new Vector3(m_abilityHeader[i].transform.localPosition.x, num3 + 10f, m_abilityHeader[i].transform.localPosition.z);
					m_abilityDesc[i].transform.localPosition = new Vector3(m_abilityDesc[i].transform.localPosition.x, m_abilityHeader[i].transform.localPosition.y - y, m_abilityDesc[i].transform.localPosition.z);
					m_abilityIcon[i].transform.localPosition = new Vector3(m_abilityIcon[i].transform.localPosition.x, num3, m_abilityIcon[i].transform.localPosition.z);
					m_abilityBorder[i].transform.localPosition = new Vector3(m_abilityIcon[i].transform.localPosition.x - 8f, num3 + 8f, m_abilityBorder[i].transform.localPosition.z);
					Single val = m_abilityDesc[i].relativeSize.y * m_abilityDesc[i].transform.localScale.y;
					Single y2 = m_abilityBorder[i].transform.localScale.y;
					Single num4 = Math.Max(val, y2) + 30f;
					num3 -= num4;
				}
				Single val2 = m_abilityDesc[num - 1].transform.localScale.y * m_abilityDesc[num - 1].relativeSize.y;
				Single y3 = m_abilityBorder[num - 1].transform.localScale.y;
				Single num5 = Math.Max(val2, y3);
				Single num6 = Math.Abs(m_abilityDesc[num - 1].transform.localPosition.y) + num5 + 30f;
				m_textBG.transform.localScale = new Vector3(m_textBG.transform.localScale.x, num6, m_textBG.transform.localScale.z);
				m_textBorder.transform.localScale = new Vector3(m_textBorder.transform.localScale.x, num6, m_textBorder.transform.localScale.z);
				m_textRoot.transform.localPosition = new Vector3(m_textRoot.transform.localPosition.x, 434f - (760f - num6) / 2f, m_textRoot.transform.localPosition.z);
			}
		}

		private String GetAdvancedClassName(EClass p_class)
		{
			if (p_class == EClass.MERCENARY)
			{
				return LocaManager.GetText("ADVANCED_CLASS_NAME_WINDSWORD_M");
			}
			if (p_class == EClass.CRUSADER)
			{
				return LocaManager.GetText("ADVANCED_CLASS_NAME_PALADIN_M");
			}
			if (p_class == EClass.FREEMAGE)
			{
				return LocaManager.GetText("ADVANCED_CLASS_NAME_ARCHMAGE_M");
			}
			if (p_class == EClass.BLADEDANCER)
			{
				return LocaManager.GetText("ADVANCED_CLASS_NAME_BLADEMASTER_M");
			}
			if (p_class == EClass.RANGER)
			{
				return LocaManager.GetText("ADVANCED_CLASS_NAME_WARDEN_M");
			}
			if (p_class == EClass.DRUID)
			{
				return LocaManager.GetText("ADVANCED_CLASS_NAME_DRUID_ELDER_M");
			}
			if (p_class == EClass.DEFENDER)
			{
				return LocaManager.GetText("ADVANCED_CLASS_NAME_SHIELD_GUARD_M");
			}
			if (p_class == EClass.SCOUT)
			{
				return LocaManager.GetText("ADVANCED_CLASS_NAME_PATHFINDER_M");
			}
			if (p_class == EClass.RUNEPRIEST)
			{
				return LocaManager.GetText("ADVANCED_CLASS_NAME_RUNELORD_M");
			}
			if (p_class == EClass.BARBARIAN)
			{
				return LocaManager.GetText("ADVANCED_CLASS_NAME_WARMONGER_M");
			}
			if (p_class == EClass.HUNTER)
			{
				return LocaManager.GetText("ADVANCED_CLASS_NAME_MARAUDER_M");
			}
			if (p_class == EClass.SHAMAN)
			{
				return LocaManager.GetText("ADVANCED_CLASS_NAME_BLOODCALLER_M");
			}
			return String.Empty;
		}

		private String GetAdvancedClassSprite(EClass p_class)
		{
			if (p_class == EClass.MERCENARY)
			{
				return "ICO_award_promotion_mercenary_upgrade";
			}
			if (p_class == EClass.CRUSADER)
			{
				return "ICO_award_promotion_crusader_upgrade";
			}
			if (p_class == EClass.FREEMAGE)
			{
				return "ICO_award_promotion_freemage_upgrade";
			}
			if (p_class == EClass.BLADEDANCER)
			{
				return "ICO_award_promotion_bladedancer_upgrade";
			}
			if (p_class == EClass.RANGER)
			{
				return "ICO_award_promotion_ranger_upgrade";
			}
			if (p_class == EClass.DRUID)
			{
				return "ICO_award_promotion_druid_upgrade";
			}
			if (p_class == EClass.DEFENDER)
			{
				return "ICO_award_promotion_defender_upgrade";
			}
			if (p_class == EClass.SCOUT)
			{
				return "ICO_award_promotion_scout_upgrade";
			}
			if (p_class == EClass.RUNEPRIEST)
			{
				return "ICO_award_promotion_runepriest_upgrade";
			}
			if (p_class == EClass.BARBARIAN)
			{
				return "ICO_award_promotion_barbarian_upgrade";
			}
			if (p_class == EClass.HUNTER)
			{
				return "ICO_award_promotion_hunter_upgrade";
			}
			if (p_class == EClass.SHAMAN)
			{
				return "ICO_award_promotion_shaman_upgrade";
			}
			return String.Empty;
		}

		private void UpdateButtons()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			if (selectedDummyCharacter.Race == ERace.HUMAN)
			{
				UpdateButton(m_btnMight, EClass.MERCENARY);
				UpdateButton(m_btnHybrid, EClass.CRUSADER);
				UpdateButton(m_btnMagic, EClass.FREEMAGE);
			}
			else if (selectedDummyCharacter.Race == ERace.ELF)
			{
				UpdateButton(m_btnMight, EClass.BLADEDANCER);
				UpdateButton(m_btnHybrid, EClass.RANGER);
				UpdateButton(m_btnMagic, EClass.DRUID);
			}
			else if (selectedDummyCharacter.Race == ERace.DWARF)
			{
				UpdateButton(m_btnMight, EClass.DEFENDER);
				UpdateButton(m_btnHybrid, EClass.SCOUT);
				UpdateButton(m_btnMagic, EClass.RUNEPRIEST);
			}
			else
			{
				UpdateButton(m_btnMight, EClass.BARBARIAN);
				UpdateButton(m_btnHybrid, EClass.HUNTER);
				UpdateButton(m_btnMagic, EClass.SHAMAN);
			}
			m_btnMight.SetSelected(selectedDummyCharacter.Class);
			m_btnHybrid.SetSelected(selectedDummyCharacter.Class);
			m_btnMagic.SetSelected(selectedDummyCharacter.Class);
			if (selectedDummyCharacter.Class != EClass.NONE)
			{
				CharacterClassStaticData staticData = StaticDataHandler.GetStaticData<CharacterClassStaticData>(EDataType.CHARACTER_CLASS, (Int32)selectedDummyCharacter.Class);
				String str = "CharacterPosings/" + staticData.PosingBase;
				Texture mainTexture = m_malePosing.mainTexture;
				mainTexture.UnloadAsset();
				Texture mainTexture2 = Helper.ResourcesLoad<Texture2D>(str + "_male_1", false);
				m_malePosing.mainTexture = mainTexture2;
				Texture mainTexture3 = m_femalePosing.mainTexture;
				mainTexture3.UnloadAsset();
				Texture mainTexture4 = Helper.ResourcesLoad<Texture2D>(str + "_female_1", false);
				m_femalePosing.mainTexture = mainTexture4;
			}
		}

		private void UpdateButton(ClassSelectButton m_btn, EClass p_class)
		{
			CharacterClassStaticData staticData = StaticDataHandler.GetStaticData<CharacterClassStaticData>(EDataType.CHARACTER_CLASS, (Int32)p_class);
			if (staticData != null)
			{
				m_btn.Init(p_class, staticData);
			}
			else
			{
				m_btn.Init(EClass.NONE, null);
			}
		}

		private Int32 UpdateAbilities(EClass p_class)
		{
			for (Int32 i = 0; i < 2; i++)
			{
				m_abilityDesc[i].text = String.Empty;
				m_abilityHeader[i].text = String.Empty;
				NGUITools.SetActive(m_abilityIcon[i].gameObject, false);
				NGUITools.SetActive(m_abilityBorder[i].gameObject, false);
			}
			Int32 num = 0;
			List<ParagonAbilitiesStaticData> list = new List<ParagonAbilitiesStaticData>(StaticDataHandler.GetIterator<ParagonAbilitiesStaticData>(EDataType.PARAGON_ABILITES));
			foreach (ParagonAbilitiesStaticData paragonAbilitiesStaticData in list)
			{
				if (paragonAbilitiesStaticData.Class == p_class)
				{
					m_abilityDesc[num].text = paragonAbilitiesStaticData.GetDescription();
					m_abilityHeader[num].text = LocaManager.GetText(paragonAbilitiesStaticData.NameKey);
					m_abilityIcon[num].spriteName = paragonAbilitiesStaticData.Icon;
					NGUITools.SetActive(m_abilityIcon[num].gameObject, true);
					NGUITools.SetActive(m_abilityBorder[num].gameObject, true);
					num++;
				}
			}
			return num;
		}
	}
}
