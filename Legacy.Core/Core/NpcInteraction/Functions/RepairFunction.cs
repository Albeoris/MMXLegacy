using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.EventManagement;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class RepairFunction : DialogFunction
	{
		private ConversationManager m_manager;

		private Int32 m_dialogID;

		private ERepairType m_repairType;

		private Int32 m_price;

		public RepairFunction()
		{
		}

		public RepairFunction(ERepairType p_repairType, Int32 p_price, Int32 p_dialogID)
		{
			m_repairType = p_repairType;
			m_price = p_price;
			m_dialogID = p_dialogID;
		}

		[XmlAttribute("dialogID")]
		public Int32 DialogID
		{
			get => m_dialogID;
		    set => m_dialogID = value;
		}

		[XmlAttribute("repairType")]
		public ERepairType RepairType
		{
			get => m_repairType;
		    set => m_repairType = value;
		}

		[XmlAttribute("price")]
		public Int32 Price
		{
			get => m_price;
		    set => m_price = value;
		}

		public override Boolean RequireGold => true;

	    public override void OnShow(Func<string, string> localisation)
	    {
	        RaiseEventShow(localisation, m_repairType);
	    }

	    public static void RaiseEventShow(Func<String, String> localisation, ERepairType repairType)
	    {
	        LegacyLogic.Instance.EventManager.Get<InitServiceDialogArgs>().TryInvoke(() =>
	        {
	            String caption = localisation("DIALOG_OPTION_SERVICES") + ":"; // Services:
	            String title;

	            switch (repairType)
	            {
	                case ERepairType.ALL:
	                    title = localisation("DIALOG_OPTION_REPAIR_ALL"); // Repair: All Items
	                    break;
	                case ERepairType.WEAPONS:
	                    title = localisation("DIALOG_OPTION_REPAIR_WEAPONS"); // Repair: Weapons
	                    break;
	                case ERepairType.ARMOR_AND_SHIELD:
	                    title = localisation("DIALOG_OPTION_REPAIR_ARMOR"); // Repair: Armour and Shields
	                    break;
	                default:
	                    title = localisation("DIALOG_SHORTCUT_REPAIR"); // Repair
	                    break;
	            }

	            return new InitServiceDialogArgs(caption, title);
	        });
	    }

	    public override void Trigger(ConversationManager p_manager)
		{
			Int32 num = m_price;
			if (num == -1)
			{
				num = ConfigManager.Instance.Game.CostRepair;
			}
			if (LegacyLogic.Instance.WorldManager.Difficulty == EDifficulty.HARD)
			{
				num = (Int32)Math.Ceiling(num * ConfigManager.Instance.Game.NpcItemRepairFactor);
			}
			m_manager = p_manager;
			m_manager.CurrentNpc.RepairController.Init(m_repairType);
			m_manager.CurrentNpc.RepairController.RepairPrice = num;
			m_manager.CurrentNpc.RepairController.UpdateDialog += OnEndRepair;
			m_manager.CurrentNpc.RepairController.StartRepair();
		}

		private void OnEndRepair(Object p_sender, EventArgs p_args)
		{
			m_manager.CurrentNpc.IdentifyController.UpdateDialog -= OnEndRepair;
			m_manager._ChangeDialog(m_manager.CurrentNpc.StaticID, m_dialogID);
		}
	}
}
