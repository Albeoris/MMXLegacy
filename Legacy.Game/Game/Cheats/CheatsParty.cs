using System;
using System.Collections.Generic;
using System.IO;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;
using UnityEngine;

namespace Legacy.Game.Cheats
{
	[AddComponentMenu("MM Legacy/Cheats/CheatsParty")]
	public class CheatsParty : MonoBehaviour
	{
		[SerializeField]
		private UIPopupList m_mapList;

		[SerializeField]
		private UIInput m_inputPosX;

		[SerializeField]
		private UIInput m_inputPosY;

		[SerializeField]
		private UIInput m_inputGold;

		[SerializeField]
		private UIInput m_inputFood;

		[SerializeField]
		private UIPopupList m_tokenList;

		[SerializeField]
		private UIInput m_tokenID;

		[SerializeField]
		private UIInput m_partySpawnerID;

		[SerializeField]
		private UILabel m_selectedTokenLabel;

		[SerializeField]
		private UILabel m_spawnerIDsLabel;

		private List<Int32> m_tokenIdList;

		private void OnEnable()
		{
			if (m_mapList != null)
			{
				m_mapList.items.Clear();
				String searchPattern = "*.xml";
				String path = Application.streamingAssetsPath + "/Maps/";
				String[] files = Directory.GetFiles(path, searchPattern);
				foreach (String path2 in files)
				{
					String fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path2);
					m_mapList.items.Add(fileNameWithoutExtension);
				}
				if (m_mapList.items.Count > 0)
				{
					m_mapList.selection = m_mapList.items[0];
				}
				else
				{
					m_mapList.selection = String.Empty;
				}
			}
			if (m_tokenList != null)
			{
				m_tokenIdList = new List<Int32>();
				IEnumerable<TokenStaticData> iterator = StaticDataHandler.GetIterator<TokenStaticData>(EDataType.TOKEN);
				m_tokenList.items.Clear();
				foreach (TokenStaticData tokenStaticData in iterator)
				{
					m_tokenList.items.Add(tokenStaticData.Name);
					m_tokenIdList.Add(tokenStaticData.StaticID);
				}
				if (m_tokenList.items.Count > 0)
				{
					m_tokenList.selection = m_tokenList.items[0];
				}
				else
				{
					m_tokenList.selection = String.Empty;
				}
			}
			if (m_inputPosX != null && m_inputPosY != null)
			{
				Party party = LegacyLogic.Instance.WorldManager.Party;
				m_inputPosX.text = party.Position.X.ToString();
				m_inputPosY.text = party.Position.Y.ToString();
			}
			m_partySpawnerID.text = (-1).ToString();
		}

		public void OnChangeMapButtonClick()
		{
			if (m_mapList != null && m_mapList.items.Count > 0)
			{
				String p_mapFile = m_mapList.selection + ".xml";
				Int32 startSpawnerID = Int32.Parse(m_partySpawnerID.text);
				LegacyLogic.Instance.WorldManager.Party.StartSpawnerID = startSpawnerID;
				LegacyLogic.Instance.WorldManager.Save("CheatSave.lsg", null);
				LegacyLogic.Instance.WorldManager.CheckPassableOnMovement = false;
				LegacyLogic.Instance.WorldManager.CheatToLevel(p_mapFile);
			}
		}

		public void OnTeleportButtonClick()
		{
			if (m_inputPosX != null && m_inputPosY != null)
			{
				Position position = new Position(Int32.Parse(m_inputPosX.text), Int32.Parse(m_inputPosY.text));
				Grid grid = LegacyLogic.Instance.MapLoader.Grid;
				Party party = LegacyLogic.Instance.WorldManager.Party;
				Position position2 = party.Position;
				if (grid.AddMovingEntity(position, party))
				{
					grid.GetSlot(position2).RemoveEntity(party);
					party.Position = position;
					BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(party, position);
					LegacyLogic.Instance.EventManager.InvokeEvent(party, EEventType.SET_ENTITY_POSITION, p_eventArgs);
				}
			}
		}

		public void OnAddResourcesButtonClick()
		{
			if (m_inputGold != null && m_inputFood != null)
			{
				Party party = LegacyLogic.Instance.WorldManager.Party;
				Int32 p_delta = 0;
				Int32 supplies = 0;
				Int32.TryParse(m_inputGold.text, out p_delta);
				Int32.TryParse(m_inputFood.text, out supplies);
				party.ChangeGold(p_delta);
				party.SetSupplies(supplies);
			}
		}

		public void OnAddTokenButtonClick()
		{
			Int32 index = m_tokenList.items.IndexOf(m_tokenList.selection);
			Int32 p_id = m_tokenIdList[index];
			Party party = LegacyLogic.Instance.WorldManager.Party;
			party.TokenHandler.AddToken(p_id);
		}

		public void OnAddTokenIDButtonClick()
		{
			Int32 p_id;
			Int32.TryParse(m_tokenID.text, out p_id);
			m_tokenID.text = p_id.ToString();
			Party party = LegacyLogic.Instance.WorldManager.Party;
			party.TokenHandler.AddToken(p_id);
		}

		public void OnNextMapButtonClick()
		{
			Int32 num = m_mapList.items.IndexOf(m_mapList.selection);
			if (num + 1 < m_mapList.items.Count)
			{
				m_mapList.selection = m_mapList.items[num + 1];
			}
		}

		public void OnPrevMapButtonClick()
		{
			Int32 num = m_mapList.items.IndexOf(m_mapList.selection);
			if (num - 1 > 0)
			{
				m_mapList.selection = m_mapList.items[num - 1];
			}
		}

		public void UpdateTokenName()
		{
			Int32 num;
			Int32.TryParse(m_tokenID.text, out num);
			m_tokenID.text = num.ToString();
			if (num > 0 && num < m_tokenList.items.Count)
			{
				m_selectedTokenLabel.text = m_tokenList.items[num];
				m_selectedTokenLabel.Update();
			}
		}

		public void UpdateSpawnerIDs()
		{
			m_spawnerIDsLabel.text = String.Empty;
			String path = Application.streamingAssetsPath + "/Maps/";
			String path2 = m_mapList.selection + ".xml";
			String p_fileName = Path.Combine(path, path2);
			Grid grid = GridLoader.Load(p_fileName);
			foreach (GridSlot gridSlot in grid.SlotIterator())
			{
				foreach (Spawn spawn in gridSlot.SpawnObjects)
				{
					if (spawn.ObjectType == EObjectType.PARTY)
					{
						UILabel spawnerIDsLabel = m_spawnerIDsLabel;
						spawnerIDsLabel.text = spawnerIDsLabel.text + spawn.ID.ToString() + ";";
					}
				}
			}
			m_spawnerIDsLabel.Update();
		}
	}
}
