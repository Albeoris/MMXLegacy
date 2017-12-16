using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;

namespace Legacy.Core.UpdateLogic
{
	public class UpdateManager
	{
		private MonsterTurnActor m_monsters;

		private PartyTurnActor m_party;

		private SpawnTurnActor m_spawns;

		private InteractionTurnActor m_interactions;

		private SummonTurnActor m_summonsAfterPartyTurn;

		private SummonTurnActor m_summonsAfterMonsterTurn;

		private TurnActor[] m_turnOrder;

		private IEnumerator<TurnActor> m_actorIterator;

		private Boolean m_skipPartyTurn;

		private Boolean m_skipNextPartyTurn;

		private Boolean m_skipUntilNextInteractionTurnActor;

		internal UpdateManager()
		{
			m_party = new PartyTurnActor();
			m_spawns = new SpawnTurnActor();
			m_interactions = new InteractionTurnActor();
			m_summonsAfterPartyTurn = new SummonTurnActor(ESummonExecutionOrder.AFTER_PARTY_TURN);
			m_summonsAfterMonsterTurn = new SummonTurnActor(ESummonExecutionOrder.AFTER_MONSTER_TURN);
			m_monsters = new MonsterTurnActor();
			m_turnOrder = new TurnActor[]
			{
				m_spawns,
				m_party,
				m_interactions,
				m_summonsAfterPartyTurn,
				m_monsters,
				m_summonsAfterMonsterTurn
			};
			m_actorIterator = ((IEnumerable<TurnActor>)m_turnOrder).GetEnumerator();
			m_actorIterator.MoveNext();
			m_actorIterator.Current.StartTurn();
		}

		public MonsterTurnActor MonsterTurnActor => m_monsters;

	    public PartyTurnActor PartyTurnActor => m_party;

	    public SpawnTurnActor SpawnTurnActor => m_spawns;

	    public InteractionTurnActor InteractionsActor => m_interactions;

	    public SummonTurnActor SummonActorAfterPartyTurn => m_summonsAfterPartyTurn;

	    public SummonTurnActor SummonsActorAfterMonsterTurn => m_summonsAfterMonsterTurn;

	    public TurnActor CurrentTurnActor => m_actorIterator.Current;

	    public Boolean SkipPartyTurn
		{
			get => m_skipPartyTurn;
	        set => m_skipPartyTurn = value;
	    }

		public Boolean SkipNextPartyTurn
		{
			get => m_skipNextPartyTurn;
		    set => m_skipNextPartyTurn = value;
		}

		public Boolean SkipUntilNextInteractionTurnActor
		{
			get => m_skipUntilNextInteractionTurnActor;
		    set => m_skipUntilNextInteractionTurnActor = value;
		}

		internal void AddEntity(BaseObject entity)
		{
			for (Int32 i = 0; i < m_turnOrder.Length; i++)
			{
				m_turnOrder[i].AddEntity(entity);
			}
		}

		internal void RemoveEntity(BaseObject entity)
		{
			for (Int32 i = 0; i < m_turnOrder.Length; i++)
			{
				m_turnOrder[i].RemoveEntity(entity);
			}
		}

		internal void Clear()
		{
			for (Int32 i = 0; i < m_turnOrder.Length; i++)
			{
				m_turnOrder[i].Clear();
			}
			ResetTurn();
		}

		internal void ClearAndDestroy()
		{
			for (Int32 i = 0; i < m_turnOrder.Length; i++)
			{
				m_turnOrder[i].ClearAndDestroy();
			}
			ResetTurn();
		}

		internal void Update()
		{
			OnUpdateTurn();
			if (m_skipUntilNextInteractionTurnActor && !(m_actorIterator.Current is InteractionTurnActor))
			{
				if (m_actorIterator.Current is PartyTurnActor)
				{
					m_actorIterator.Current.FinishTurn();
				}
				if (!m_actorIterator.MoveNext())
				{
					ResetTurn();
				}
			}
			else
			{
				m_skipUntilNextInteractionTurnActor = false;
			}
			while (m_actorIterator.Current.IsFinished())
			{
				OnEndTurn();
				if (!m_actorIterator.MoveNext())
				{
					ResetTurn();
				}
				if (m_skipPartyTurn && m_actorIterator.Current is PartyTurnActor)
				{
					m_actorIterator.MoveNext();
					m_skipPartyTurn = false;
				}
				if (m_skipNextPartyTurn)
				{
					m_skipPartyTurn = true;
					m_skipNextPartyTurn = false;
				}
				OnBeginTurn();
			}
			if (m_actorIterator.Current != PartyTurnActor)
			{
				PartyTurnActor.PassiveUpdateTurn();
			}
		}

		internal void ResetTurn()
		{
			m_actorIterator.Reset();
			m_actorIterator.MoveNext();
		}

		internal void ResetCurrentTurnActor()
		{
			m_actorIterator.Current.ExecutionBreak();
		}

		private void OnBeginTurn()
		{
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.UPDATE_HUD_DANGER_SENSE, EventArgs.Empty);
			m_actorIterator.Current.StartTurn();
			m_actorIterator.Current.UpdateTurn();
		}

		private void OnUpdateTurn()
		{
			m_actorIterator.Current.UpdateTurn();
		}

		private void OnEndTurn()
		{
			m_actorIterator.Current.FinishTurn();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.UPDATE_HUD_DANGER_SENSE, EventArgs.Empty);
			LegacyLogic.Instance.GameTime.UpdateIncreaseTurn();
		}
	}
}
