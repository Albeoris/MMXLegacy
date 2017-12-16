using System;
using System.Collections.Generic;
using System.Globalization;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.UpdateLogic.Preconditions;
using Legacy.Core.Utilities.StateManagement;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public abstract class BaseInteraction
	{
		public const Int32 STATE_IDLE = 0;

		public const Int32 STATE_RUNNING = 1;

		public const Int32 STATE_EXECUTE_SUCCESS = 2;

		public const Int32 STATE_EXECUTE_FAILED = 3;

		public const Int32 STATE_ACTION_FINISHED = 10;

		public const Int32 STATE_ACTION_CANCELLED = 11;

		public const Int32 PARAM_COUNT_TEST = 7;

		public const Int32 PARAM_COUNT_CHALLENGE = 8;

		public const Int32 PARAM_COUNT_SECRET_CHALLENGE = 8;

		public const Int32 PARAM_COUNT_INPUT = 6;

		public const Int32 PARAM_COUNT_PARTY_CHECK = 13;

		public const Int32 PARAM_COUNT_DISARM_TRAP = 7;

		protected Int32 m_targetSpawnID;

		protected String m_extra;

		protected Int32 m_parentID;

		protected Int32 m_commandIndex;

		protected BasePrecondition m_precondition;

		protected String m_preconditionString = "NONE";

		protected Int32 m_activateCount = -1;

		protected StateMachine m_stateMachine;

		protected Boolean m_valid = true;

		public BaseInteraction()
		{
		}

		public BaseInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex)
		{
			m_targetSpawnID = p_command.TargetSpawnID;
			m_extra = p_command.Extra;
			m_activateCount = p_command.ActivateCount;
			m_preconditionString = p_command.Precondition;
			m_parentID = p_parentID;
			m_commandIndex = p_commandIndex;
			m_stateMachine = new StateMachine();
			m_stateMachine.AddState(new State(0));
			m_stateMachine.AddState(new State(1));
			m_stateMachine.AddState(new State(2));
			m_stateMachine.AddState(new State(3));
			m_stateMachine.AddState(new State(10));
			m_stateMachine.AddState(new State(11));
			m_stateMachine.ChangeState(0);
			ParseExtra(p_command.Extra);
		}

		public Boolean Valid => m_valid;

	    protected static Grid Grid => LegacyLogic.Instance.MapLoader.Grid;

	    public Int32 State => m_stateMachine.CurrentState.Id;

	    public Int32 ActivateCount => m_activateCount;

	    public void IncreaseActivateCount()
		{
			m_activateCount++;
		}

		protected abstract void ParseExtra(String p_extra);

		public virtual void Update()
		{
			m_stateMachine.Update();
		}

		public void FinishInteraction()
		{
			m_stateMachine.ChangeState(10);
		}

		public void CancelInteraction()
		{
			m_stateMachine.ChangeState(11);
		}

		protected virtual void Validate(Object p_sender, EventArgs p_args)
		{
			PreconditionEvaluateArgs preconditionEvaluateArgs = (PreconditionEvaluateArgs)p_args;
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PRECONDITION_EVALUATED, new EventHandler(Validate));
			if (preconditionEvaluateArgs.Cancelled)
			{
				CancelInteraction();
			}
			else
			{
				Boolean flag = preconditionEvaluateArgs.Passed;
				if (!flag && this is OpenContainerInteraction && Grid.FindInteractiveObject(m_parentID).State == EInteractiveObjectState.CONTAINER_OPENED)
				{
					flag = true;
				}
				if (flag)
				{
					String successText = GetSuccessText(p_args);
					if (successText == String.Empty)
					{
						StartExecution();
					}
					else if (preconditionEvaluateArgs.ShowMessage)
					{
						ShowSuccess(successText);
					}
					else
					{
						StartExecution();
					}
				}
				else
				{
					String failText = GetFailText(p_args);
					if (failText == String.Empty)
					{
						m_stateMachine.ChangeState(3);
					}
					else
					{
						ShowFailure(failText);
					}
				}
			}
		}

		public virtual void Execute()
		{
			m_precondition = ParsePrecondition(m_preconditionString);
			if (m_precondition != null)
			{
				m_stateMachine.ChangeState(1);
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PRECONDITION_EVALUATED, new EventHandler(Validate));
				m_precondition.Trigger();
			}
			else
			{
				StartExecution();
			}
		}

		protected virtual void StartExecution()
		{
			m_stateMachine.ChangeState(1);
			DoExecute();
		}

		protected virtual void DoExecute()
		{
			FinishExecution();
		}

		public virtual void FinishExecution()
		{
			m_stateMachine.ChangeState(2);
		}

		public virtual void PrewarmAfterCreate()
		{
			BasePrecondition basePrecondition = ParsePrecondition(m_preconditionString);
			if (basePrecondition != null && basePrecondition is PartyCheckPrecondition && !(this is SetEnabledInteraction))
			{
				Int32 requiredTokenID = ((PartyCheckPrecondition)basePrecondition).RequiredTokenID;
				if (requiredTokenID > 0)
				{
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.ANNOUNCE_NEEDED_TOKEN, new TokenEventArgs(requiredTokenID));
				}
			}
		}

		public virtual void ReleaseInteractLock()
		{
		}

		public virtual void TriggerStateChange()
		{
		}

		protected String GetSuccessText(EventArgs p_args)
		{
			String text = null;
			String result = String.Empty;
			if (m_precondition is InputPrecondition)
			{
				text = ((InputPrecondition)m_precondition).SuccessText;
			}
			else if (m_precondition is ChallengePrecondition)
			{
				text = ((ChallengePrecondition)m_precondition).SuccessText;
			}
			else if (m_precondition is SecretChallengePrecondition)
			{
				text = ((SecretChallengePrecondition)m_precondition).SuccessText;
			}
			else if (m_precondition is TestPrecondition)
			{
				text = ((TestPrecondition)m_precondition).SuccessText;
			}
			else if (m_precondition is PartyCheckPrecondition)
			{
				text = ((PartyCheckPrecondition)m_precondition).SuccessText;
			}
			else if (m_precondition is DisarmTrapPrecondition)
			{
				text = ((DisarmTrapPrecondition)m_precondition).SuccessText;
			}
			else if (m_precondition is UnlockKeyPrecondition)
			{
				text = ((UnlockKeyPrecondition)m_precondition).SuccessText;
			}
			if (!String.IsNullOrEmpty(text))
			{
				Character character = ((PreconditionEvaluateArgs)p_args).Character;
				if (character != null)
				{
					if (character.Gender == EGender.MALE)
					{
						result = Localization.Instance.GetText(text + "_M", character.Name);
					}
					else
					{
						result = Localization.Instance.GetText(text + "_F", character.Name);
					}
				}
				else
				{
					result = Localization.Instance.GetText(text);
				}
			}
			return result;
		}

		protected String GetFailText(EventArgs p_args)
		{
			Boolean flag = false;
			String text = null;
			String result = String.Empty;
			if (m_precondition is InputPrecondition)
			{
				text = ((InputPrecondition)m_precondition).FailText;
				flag = true;
			}
			else if (m_precondition is ChallengePrecondition)
			{
				text = ((ChallengePrecondition)m_precondition).FailText;
			}
			else if (m_precondition is SecretChallengePrecondition)
			{
				text = ((SecretChallengePrecondition)m_precondition).FailText;
			}
			else if (m_precondition is TestPrecondition)
			{
				text = ((TestPrecondition)m_precondition).FailText;
			}
			else if (m_precondition is PartyCheckPrecondition)
			{
				text = ((PartyCheckPrecondition)m_precondition).FailText;
				flag = true;
			}
			else if (m_precondition is DisarmTrapPrecondition)
			{
				text = ((DisarmTrapPrecondition)m_precondition).FailText;
			}
			else if (m_precondition is UnlockKeyPrecondition)
			{
				text = ((UnlockKeyPrecondition)m_precondition).FailTextForDisplay;
			}
			if (!String.IsNullOrEmpty(text))
			{
				Character character = ((PreconditionEvaluateArgs)p_args).Character;
				if (character == null)
				{
					if (flag)
					{
						return Localization.Instance.GetText(text);
					}
					List<Character> charactersAlive = LegacyLogic.Instance.WorldManager.Party.GetCharactersAlive();
					if (charactersAlive.Count == 0)
					{
						LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.GAME_OVER, EventArgs.Empty);
						return result;
					}
					character = charactersAlive[Random.Range(0, charactersAlive.Count)];
				}
				if (character == null)
				{
					return Localization.Instance.GetText(text);
				}
				text += ((character.Gender != EGender.MALE) ? "_F" : "_M");
				result = Localization.Instance.GetText(text, character.Name);
			}
			return result;
		}

		protected void ShowSuccess(String p_text)
		{
			StringEventArgs p_eventArgs = new StringEventArgs(p_text);
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PRECONDITION_GUI_DONE, new EventHandler(OnAfterShowSuccess));
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PRECONDITION_SHOW_RESULT, p_eventArgs);
		}

		protected void ShowFailure(String p_text)
		{
			StringEventArgs p_eventArgs = new StringEventArgs(p_text);
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PRECONDITION_GUI_DONE, new EventHandler(OnAfterShowFailure));
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PRECONDITION_SHOW_RESULT, p_eventArgs);
		}

		private void OnAfterShowSuccess(Object p_sender, EventArgs p_args)
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PRECONDITION_GUI_DONE, new EventHandler(OnAfterShowSuccess));
			StartExecution();
		}

		private void OnAfterShowFailure(Object p_sender, EventArgs p_args)
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PRECONDITION_GUI_DONE, new EventHandler(OnAfterShowFailure));
			m_stateMachine.ChangeState(3);
		}

		protected internal static BasePrecondition ParsePrecondition(String p_precondition)
		{
			if (p_precondition == "NONE" || p_precondition == String.Empty)
			{
				return null;
			}
			String[] array = p_precondition.Split(new Char[]
			{
				','
			});
			if (array[0] == "NONE")
			{
				return null;
			}
			BasePrecondition basePrecondition = null;
			String maintext = String.Empty;
			String successText = String.Empty;
			String failText = String.Empty;
			String text = String.Empty;
			EPotionTarget attribute = EPotionTarget.NONE;
			Int32 requiredValue = 0;
			EPreconditionDecision epreconditionDecision = (EPreconditionDecision)Enum.Parse(typeof(EPreconditionDecision), array[1]);
			maintext = array[2];
			if (epreconditionDecision == EPreconditionDecision.TEXT_INPUT && array.Length > 5)
			{
				text = array[5];
			}
			if (epreconditionDecision == EPreconditionDecision.WHO_WILL && array.Length > 5)
			{
				attribute = (EPotionTarget)Enum.Parse(typeof(EPotionTarget), array[5]);
				requiredValue = Convert.ToInt32(array[6]);
			}
			if (array[0] == "PLAIN")
			{
				basePrecondition = new PlainPrecondition();
			}
			else
			{
				successText = array[3];
				failText = array[4];
			}
			if (array[0] == "SELECT_CHARACTER")
			{
				basePrecondition = new SelectCharacterPrecondition();
			}
			if (array[0] == "TEST")
			{
				if (array.Length != 7)
				{
					throw new FormatException(String.Concat(new Object[]
					{
						"Could not parse precondition params ",
						p_precondition,
						" because it contains ",
						array.Length,
						" arguments instead of ",
						7
					}));
				}
				basePrecondition = new TestPrecondition
				{
					Attribute = attribute,
					RequiredValue = requiredValue,
					SuccessText = successText,
					FailText = failText
				};
			}
			if (array[0] == "CHALLENGE")
			{
				if (array.Length != 8 && array.Length != 7)
				{
					throw new FormatException(String.Concat(new Object[]
					{
						"Could not parse precondition params ",
						p_precondition,
						" because it contains ",
						array.Length,
						" arguments instead of ",
						8
					}));
				}
				if (array.Length == 8)
				{
					basePrecondition = new ChallengePrecondition
					{
						Attribute = attribute,
						RequiredValue = requiredValue,
						SuccessText = successText,
						FailText = failText,
						Damage = Convert.ToInt32(array[7])
					};
				}
				else
				{
					basePrecondition = new ChallengePrecondition
					{
						Attribute = attribute,
						RequiredValue = requiredValue,
						SuccessText = successText,
						FailText = failText,
						Damage = 0
					};
				}
			}
			if (array[0] == "SECRET_CHALLENGE")
			{
				if (array.Length != 8)
				{
					throw new FormatException(String.Concat(new Object[]
					{
						"Could not parse precondition params ",
						p_precondition,
						" because it contains ",
						array.Length,
						" arguments instead of ",
						8
					}));
				}
				basePrecondition = new SecretChallengePrecondition
				{
					Attribute = attribute,
					RequiredValue = requiredValue,
					SuccessText = successText,
					FailText = failText,
					Damage = Convert.ToInt32(array[7])
				};
			}
			if (array[0] == "DISARM_TRAP")
			{
				if (array.Length != 7)
				{
					throw new FormatException(String.Concat(new Object[]
					{
						"Could not parse precondition params ",
						p_precondition,
						" because it contains ",
						array.Length,
						" arguments instead of ",
						7
					}));
				}
				basePrecondition = new DisarmTrapPrecondition
				{
					Attribute = attribute,
					RequiredValue = requiredValue,
					SuccessText = successText,
					FailText = failText
				};
			}
			if (array[0] == "INPUT")
			{
				if (array.Length != 6)
				{
					throw new FormatException(String.Concat(new Object[]
					{
						"Could not parse precondition params ",
						p_precondition,
						" because it contains ",
						array.Length,
						" arguments instead of ",
						6
					}));
				}
				basePrecondition = new InputPrecondition
				{
					WantedInput = text,
					SuccessText = successText,
					FailText = failText
				};
			}
			if (array[0] == "UNLOCK_KEY")
			{
				if (array.Length != 6)
				{
					throw new FormatException(String.Concat(new Object[]
					{
						"Could not parse precondition params ",
						p_precondition,
						" because it contains ",
						array.Length,
						" arguments instead of ",
						6
					}));
				}
				Int32 wantedPrivilege = 0;
				if (text != String.Empty)
				{
					wantedPrivilege = Int32.Parse(text, CultureInfo.InvariantCulture.NumberFormat);
				}
				basePrecondition = new UnlockKeyPrecondition
				{
					WantedPrivilege = wantedPrivilege,
					SuccessText = successText,
					FailText = failText
				};
			}
			if (array[0] == "PARTY_CHECK")
			{
				if (array.Length != 13 && array.Length != 10 && array.Length != 9 && array.Length != 8)
				{
					throw new FormatException(String.Concat(new Object[]
					{
						"Could not parse precondition params ",
						p_precondition,
						" because it contains ",
						array.Length,
						" arguments instead of ",
						13
					}));
				}
				if (array.Length > 5)
				{
					basePrecondition = new PartyCheckPrecondition
					{
						SuccessText = successText,
						FailText = failText,
						RequiredTokenID = Convert.ToInt32(array[5]),
						RequiredBlessingID = Convert.ToInt32(array[6]),
						RequiredActiveQuestStepID = Convert.ToInt32(array[7])
					};
					if (array.Length > 8)
					{
						((PartyCheckPrecondition)basePrecondition).RequiredFinishedQuestStepID = Convert.ToInt32(array[8]);
					}
					if (array.Length > 9)
					{
						((PartyCheckPrecondition)basePrecondition).RequiredHirelingID = Convert.ToInt32(array[9]);
					}
					if (array.Length > 12)
					{
						((PartyCheckPrecondition)basePrecondition).RequiredInactiveQuestStepID = Convert.ToInt32(array[10]);
						((PartyCheckPrecondition)basePrecondition).WithoutHirelingID = Convert.ToInt32(array[11]);
						((PartyCheckPrecondition)basePrecondition).WithoutTokenID = Convert.ToInt32(array[12]);
					}
				}
				else
				{
					basePrecondition = new PartyCheckPrecondition
					{
						SuccessText = successText,
						FailText = failText
					};
				}
			}
			basePrecondition.Decision = epreconditionDecision;
			basePrecondition.Maintext = maintext;
			return basePrecondition;
		}
	}
}
