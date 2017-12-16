using System;
using Legacy.Core.Combat;
using Legacy.Core.Entities.AI.Behaviours;

namespace Legacy.Core.Entities.AI
{
	public class AIBrain
	{
		private MovingEntity m_controller;

		private AIData m_data;

		private BaseBehaviour m_behaviour;

		private Single m_magicFactor = 1f;

		private EDamageType m_damageType;

		private Int32 m_ignoreResistance;

		public AIBrain(MovingEntity p_controller, AIData p_data)
		{
			m_controller = p_controller;
			m_data = p_data;
		}

		public EAIBehaviour Behaviour => m_data.AIBehaviour;

	    public AIData Data
		{
			get => m_data;
	        set
			{
				if (m_data.AIBehaviour != value.AIBehaviour)
				{
					m_data = value;
					CreateBehavior(value.AIBehaviour);
				}
				else
				{
					m_data = value;
				}
			}
		}

		public Single MagicFactor
		{
			get => m_magicFactor;
		    set => m_magicFactor = value;
		}

		public EDamageType DamageType
		{
			get => m_damageType;
		    set => m_damageType = value;
		}

		public Int32 IgnoreResistance
		{
			get => m_ignoreResistance;
		    set => m_ignoreResistance = value;
		}

		private void CreateBehavior(EAIBehaviour p_behaviour)
		{
			switch (p_behaviour)
			{
			case EAIBehaviour.Stationary:
				if (m_controller is Summon)
				{
					m_behaviour = new StationaryBehaviour(this, (Summon)m_controller);
					return;
				}
				throw new NotSupportedException(String.Concat(new Object[]
				{
					"The behaviour '",
					p_behaviour,
					"' supports not the controller ",
					m_controller.GetType().Name
				}));
			case EAIBehaviour.Cyclone:
				if (m_controller is Summon)
				{
					m_behaviour = new CycloneBehaviour(this, (Summon)m_controller);
					return;
				}
				throw new NotSupportedException(String.Concat(new Object[]
				{
					"The behaviour '",
					p_behaviour,
					"' supports not the controller ",
					m_controller.GetType().Name
				}));
			case EAIBehaviour.TimeStop:
				if (m_controller is Summon)
				{
					m_behaviour = new TimeStopBehaviour(this, (Summon)m_controller);
					return;
				}
				throw new NotSupportedException(String.Concat(new Object[]
				{
					"The behaviour '",
					p_behaviour,
					"' supports not the controller ",
					m_controller.GetType().Name
				}));
			case EAIBehaviour.Trap:
				if (m_controller is Summon)
				{
					m_behaviour = new TrapBehaviour(this, (Summon)m_controller);
					return;
				}
				throw new NotSupportedException(String.Concat(new Object[]
				{
					"The behaviour '",
					p_behaviour,
					"' supports not the controller ",
					m_controller.GetType().Name
				}));
			}
			throw new NotSupportedException("This behaviour is not supported: " + p_behaviour);
		}

		public virtual void BeginTurn()
		{
			if (m_behaviour == null)
			{
				CreateBehavior(m_data.AIBehaviour);
			}
			m_behaviour.BeginTurn();
		}

		public virtual void UpdateTurn()
		{
			if (m_behaviour != null)
			{
				m_behaviour.UpdateTurn();
			}
		}

		public virtual void EndTurn()
		{
			if (m_behaviour != null)
			{
				m_behaviour.EndTurn();
			}
		}
	}
}
