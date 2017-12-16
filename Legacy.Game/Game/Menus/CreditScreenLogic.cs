using System;
using System.Collections.Generic;
using System.Text;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.StaticData;
using Legacy.Core.Utilities.StateManagement;
using Legacy.Game.Context;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.Menus
{
	[AddComponentMenu("MM Legacy/Menus/CreditsScreenLogic")]
	public class CreditScreenLogic : MonoBehaviour
	{
		private const Single HEADER_PADDING = 100f;

		private const Single BLOCK_PADDING = 200f;

		private const Single SECTION_PADDING = 60f;

		[SerializeField]
		private GameObject m_creditDataPrefab;

		[SerializeField]
		private GameObject m_moveThing;

		[SerializeField]
		private UILabel m_headerLabelPrefab;

		[SerializeField]
		private UISprite m_logo;

		[SerializeField]
		private Single SPEED = 55f;

		private List<GameObject> m_objects;

		private Single m_currentPosition = -1000f;

		private Single m_tempSize;

		private Boolean m_isSubsection;

		private TimeStateMachine<EState> m_state;

		public void Awake()
		{
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnCancel));
			InputManager.RegisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnCancel));
			m_logo.alpha = 0f;
			m_state = new TimeStateMachine<EState>();
			m_state.AddState(new TimeState<EState>(EState.FADEIN, 2f, new State<EState, Transition<EState>>.StateUpdateMethod(StateFadein)));
			m_state.AddState(new TimeState<EState>(EState.ACTIVE, 0f, new State<EState, Transition<EState>>.StateUpdateMethod(StateActive)));
			m_state.ChangeState(EState.FADEIN);
		}

		public void OnDestroy()
		{
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnCancel));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnCancel));
		}

		private void Start()
		{
			m_objects = new List<GameObject>();
			XmlStaticDataHandler<CreditsData>.RootPath = Application.streamingAssetsPath;
			CreditsData staticData = XmlStaticDataHandler<CreditsData>.GetStaticData("CreditsData");
			InitCredits(staticData);
		}

		private void InitCredits(CreditsData p_data)
		{
			foreach (CreditsData.Credits credits2 in p_data.m_credits)
			{
				UILabel uilabel = Helper.Instantiate<UILabel>(m_headerLabelPrefab);
				uilabel.text = credits2.m_creditHeader;
				Int32 num = credits2.m_creditHeader.Split(new String[]
				{
					"\\n"
				}, StringSplitOptions.None).Length;
				m_moveThing.transform.AddChild(uilabel.transform);
				uilabel.transform.localScale = new Vector3(84f, 84f, 1f);
				uilabel.transform.localPosition = new Vector3(0f, m_currentPosition, -1f);
				m_currentPosition -= 100f * num + 80f;
				m_objects.Add(uilabel.gameObject);
				foreach (CreditsData.CreditSection p_creditBlock in credits2.m_sections)
				{
					GameObject gameObject = CreateCreditData(p_creditBlock);
					if (!(gameObject == null))
					{
						m_moveThing.transform.AddChild(gameObject.transform);
						gameObject.transform.localScale = Vector3.one;
						gameObject.transform.localPosition = new Vector3(0f, m_currentPosition, -1f);
						m_currentPosition -= m_tempSize + 60f;
						m_objects.Add(gameObject);
					}
				}
				m_currentPosition -= 200f;
			}
		}

		private void Update()
		{
			m_state.Update(Time.deltaTime);
		}

		private void StateFadein()
		{
			m_logo.alpha = m_state.CurrentStateTimePer;
			if (m_state.IsStateTimeout)
			{
				m_state.ChangeState(EState.ACTIVE);
			}
		}

		private void StateActive()
		{
			Vector3 localPosition = m_moveThing.transform.localPosition;
			localPosition.y += SPEED * Time.deltaTime;
			m_moveThing.transform.localPosition = localPosition;
		}

		private GameObject CreateCreditData(CreditsData.CreditSection p_creditBlock)
		{
			m_tempSize = 0f;
			GameObject gameObject = Helper.Instantiate<GameObject>(m_creditDataPrefab);
			UILabel[] componentsInChildren = gameObject.GetComponentsInChildren<UILabel>();
			UILabel uilabel = null;
			UILabel uilabel2 = null;
			foreach (UILabel uilabel3 in componentsInChildren)
			{
				if (uilabel3.name == "Names")
				{
					uilabel = uilabel3;
				}
				else if (uilabel3.name == "SectionHeader")
				{
					uilabel2 = uilabel3;
				}
			}
			m_isSubsection = !String.IsNullOrEmpty(p_creditBlock.m_subHeader);
			if (uilabel2 == null || uilabel == null)
			{
				return null;
			}
			if (!m_isSubsection)
			{
				uilabel2.text = p_creditBlock.m_sectionHeader;
				Int32 num = p_creditBlock.m_sectionHeader.Split(new String[]
				{
					"\\n"
				}, StringSplitOptions.None).Length;
				m_tempSize += 96f * num + 10f;
				StringBuilder stringBuilder = new StringBuilder();
				Int32 num2 = 0;
				if (p_creditBlock.m_entries != null && p_creditBlock.m_entries.Length > 0)
				{
					foreach (CreditsData.CreditEntry creditEntry in p_creditBlock.m_entries)
					{
						stringBuilder.Append(creditEntry.m_name + "\n");
						if (creditEntry.m_name.Contains("\\n"))
						{
							num2 += 2;
						}
						num2++;
					}
					m_tempSize += (num2 + 1) * 38f;
					uilabel.text = stringBuilder.ToString();
				}
				else
				{
					uilabel.text = String.Empty;
					m_tempSize -= 38f;
				}
			}
			else
			{
				m_currentPosition -= 74f;
				uilabel2.text = p_creditBlock.m_subHeader;
				Int32 num3 = p_creditBlock.m_subHeader.Split(new String[]
				{
					"\\n"
				}, StringSplitOptions.None).Length;
				uilabel2.transform.localScale = new Vector3(68f, 68f, 1f);
				m_tempSize += 96f * num3 + 10f;
				StringBuilder stringBuilder2 = new StringBuilder();
				Int32 num4 = 0;
				if (p_creditBlock.m_entries != null && p_creditBlock.m_entries.Length > 0)
				{
					foreach (CreditsData.CreditEntry creditEntry2 in p_creditBlock.m_entries)
					{
						stringBuilder2.Append(creditEntry2.m_name + "\n");
						if (creditEntry2.m_name.Contains("\\n"))
						{
							num4 += 2;
						}
						num4++;
					}
					m_tempSize += (num4 + 1) * 38f;
					uilabel.transform.localScale = new Vector3(42f, 42f, 1f);
					uilabel.text = stringBuilder2.ToString();
				}
				else
				{
					uilabel.text = String.Empty;
					m_tempSize -= 58f;
				}
			}
			NGUITools.SetActive(gameObject.gameObject, true);
			if (uilabel.text.Length == 0)
			{
				NGUITools.SetActive(uilabel.gameObject, false);
			}
			return gameObject;
		}

		public void OnCancel(Object p_sender, HotkeyEventArgs p_args)
		{
			if (LegacyLogic.Instance.WorldManager.IsShowingEndingSequences)
			{
				ContextManager.ChangeContext(EContext.Game);
			}
			else
			{
				ContextManager.ChangeContext(EContext.Mainmenu);
			}
		}

		private enum EState
		{
			FADEIN,
			ACTIVE
		}
	}
}
