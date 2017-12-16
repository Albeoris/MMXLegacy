using System;
using System.Threading;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.NpcInteraction;
using Legacy.Core.PartyManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/BilateralScreen")]
	public class BilateralScreen : MonoBehaviour
	{
		private const String ICON_NAME_TRADING = "ICO_screen_trading";

		private const String ICON_NAME_LOOT = "ICO_screen_loot";

		private const String ICON_NAME_INVENTORY = "ICO_screen_inventory";

		private const String RESOURCE_BACKGROUNDS_PATH = "TradingScreen/";

		[SerializeField]
		private CharacterScreen m_characterScreen;

		[SerializeField]
		private PartyScreen m_partyScreen;

		[SerializeField]
		private TradingScreen m_tradingScreen;

		[SerializeField]
		private LootScreen m_lootScreen;

		[SerializeField]
		private UISprite m_icon;

		[SerializeField]
		private UITexture m_background;

		[SerializeField]
		private String m_tradingBackgroundName = "ART_trading_background";

		[SerializeField]
		private String m_normalBackgroundName = "ART_inventory_background";

		private Boolean m_screenSpaceOccupied;

		public event EventHandler OccupyScreenSpaceEvent;

		public event EventHandler ReleaseScreenSpaceEvent;

		public PartyScreen PartyScreen => m_partyScreen;

	    public CharacterScreen CharacterScreen => m_characterScreen;

	    public TradingScreen TradingScreen => m_tradingScreen;

	    public LootScreen LootScreen => m_lootScreen;

	    public Boolean HasOpenScreens => m_partyScreen.IsOpen || m_characterScreen.IsOpen || m_tradingScreen.IsOpen || m_lootScreen.IsOpen;

	    public void Init(Party p_party)
		{
			InitAllScreens(p_party);
			NGUITools.SetActiveSelf(gameObject, false);
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_TRADE_START, new EventHandler(OnTradeStart));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_TRADE_STOP, new EventHandler(OnTradeStop));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartSceneLoad));
		}

		public void CleanUp()
		{
			m_characterScreen.CleanUp();
			m_partyScreen.CleanUp();
			m_tradingScreen.CleanUp();
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_TRADE_START, new EventHandler(OnTradeStart));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_TRADE_STOP, new EventHandler(OnTradeStop));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartSceneLoad));
		}

		private void OnTradeStart(Object sender, EventArgs e)
		{
			String text = "TradingScreen/" + m_tradingBackgroundName;
			Texture texture = Helper.ResourcesLoad<Texture>(text);
			if (texture == null)
			{
				Debug.LogError("Could not load texture from: " + text);
			}
			if (m_background.mainTexture != texture)
			{
				Texture mainTexture = m_background.mainTexture;
				m_background.mainTexture = texture;
				if (mainTexture != null)
				{
					mainTexture.UnloadAsset();
				}
			}
			NPCTradeEventArgs npctradeEventArgs = e as NPCTradeEventArgs;
			if (npctradeEventArgs != null)
			{
				TradingScreen.ItemContainer.SetInventory(npctradeEventArgs.Inventory);
			}
			ToggleTrade();
			PartyScreen.OpenInventoryTab();
		}

		private void OnTradeStop(Object sender, EventArgs e)
		{
			String text = "TradingScreen/" + m_normalBackgroundName;
			Texture texture = Helper.ResourcesLoad<Texture>(text);
			if (texture == null)
			{
				Debug.LogError("Could not load texture from: " + text);
			}
			if (m_background.mainTexture != texture)
			{
				Texture mainTexture = m_background.mainTexture;
				m_background.mainTexture = texture;
				if (mainTexture != null)
				{
					mainTexture.UnloadAsset();
				}
			}
			CloseScreen();
		}

		private void OnStartSceneLoad(Object sender, EventArgs e)
		{
			if (HasOpenScreens)
			{
				CloseAll();
			}
		}

		private void InitAllScreens(Party p_party)
		{
			m_partyScreen.Init(p_party);
			m_characterScreen.Init(p_party);
			m_tradingScreen.Init();
			m_lootScreen.Init();
			InitScreen(m_partyScreen);
			InitScreen(m_characterScreen);
			InitScreen(m_tradingScreen);
			InitScreen(m_lootScreen);
		}

		private void InitScreen(BaseScreen p_screen)
		{
			p_screen.BilateralScreen = this;
			p_screen.Close();
		}

		public void ToggleInventory()
		{
			if (gameObject.activeSelf)
			{
				CloseScreen();
			}
			else
			{
				OpenScreen();
				PartyScreen.ToggleOpenClose();
				CharacterScreen.ToggleOpenClose();
			}
			if (CharacterScreen.IsOpen)
			{
				m_icon.spriteName = "ICO_screen_inventory";
			}
		}

		public void ToggleTrade()
		{
			if (gameObject.activeSelf)
			{
				CloseScreen();
			}
			else
			{
				OpenScreen();
				PartyScreen.ToggleOpenClose();
				TradingScreen.ToggleOpenClose();
			}
			if (TradingScreen.IsOpen)
			{
				m_icon.spriteName = "ICO_screen_trading";
			}
		}

		public void ToggleLoot()
		{
			if (gameObject.activeSelf)
			{
				CloseScreen();
			}
			else
			{
				OpenScreen();
				PartyScreen.ToggleOpenClose();
				LootScreen.ToggleOpenClose();
			}
			if (LootScreen.IsOpen)
			{
				m_icon.spriteName = "ICO_screen_loot";
			}
		}

		public void OpenScreen()
		{
			NGUITools.SetActive(gameObject, true);
			if (!m_screenSpaceOccupied)
			{
				m_screenSpaceOccupied = true;
				if (OccupyScreenSpaceEvent != null)
				{
					OccupyScreenSpaceEvent(this, EventArgs.Empty);
				}
			}
		}

		public void CloseScreen()
		{
			NGUITools.SetActive(gameObject, false);
			if (m_lootScreen.IsOpen)
			{
				m_lootScreen.ItemContainer.Container.CloseContainer();
			}
			CloseAll();
			if (m_screenSpaceOccupied)
			{
				m_screenSpaceOccupied = false;
				if (ReleaseScreenSpaceEvent != null)
				{
					ReleaseScreenSpaceEvent(this, EventArgs.Empty);
				}
			}
		}

		private void CloseAll()
		{
			if (m_partyScreen.IsOpen)
			{
				m_partyScreen.ToggleOpenClose();
			}
			if (m_characterScreen.IsOpen)
			{
				m_characterScreen.ToggleOpenClose();
			}
			if (m_tradingScreen.IsOpen)
			{
				m_tradingScreen.ToggleOpenClose();
				ConversationManager conversationManager = LegacyLogic.Instance.ConversationManager;
				if (conversationManager.CurrentNpc != null && conversationManager.CurrentNpc.TradingInventory.IsTrading)
				{
					conversationManager.CurrentNpc.TradingInventory.StopTrading();
				}
			}
			if (m_lootScreen.IsOpen)
			{
				m_lootScreen.ToggleOpenClose();
			}
		}

		internal void ChangeParty(Party p_party)
		{
			m_characterScreen.ChangeParty(p_party);
			m_partyScreen.ChangeParty(p_party);
		}
	}
}
