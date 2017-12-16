using System;
using System.Xml.Serialization;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.Entities
{
	public class SpawnCommand : ISaveGameObject
	{
		private EInteraction m_type;

		private Int32 m_targetSpawnID;

		private String m_extra = String.Empty;

		private String m_precondition = "NONE";

		private EInteractionTiming m_timing;

		private EInteractiveObjectState m_requiredState;

		private Int32 m_activateCount = -1;

		[XmlAttribute("Type")]
		public EInteraction Type
		{
			get => m_type;
		    set => m_type = value;
		}

		[XmlAttribute("TargetSpawnID")]
		public Int32 TargetSpawnID
		{
			get => m_targetSpawnID;
		    set => m_targetSpawnID = value;
		}

		[XmlAttribute("Extra")]
		public String Extra
		{
			get => m_extra;
		    set => m_extra = value;
		}

		[XmlAttribute("Precondition")]
		public String Precondition
		{
			get => m_precondition;
		    set => m_precondition = value;
		}

		[XmlAttribute("Timing")]
		public EInteractionTiming Timing
		{
			get => m_timing;
		    set => m_timing = value;
		}

		[XmlAttribute("RequiredState")]
		public EInteractiveObjectState RequiredState
		{
			get => m_requiredState;
		    set => m_requiredState = value;
		}

		[XmlAttribute("ActivateCount")]
		public Int32 ActivateCount
		{
			get => m_activateCount;
		    set => m_activateCount = value;
		}

		public void Load(SaveGameData p_data)
		{
			m_type = p_data.Get<EInteraction>("Type", EInteraction.NONE);
			m_extra = p_data.Get<String>("Extra", String.Empty);
			m_targetSpawnID = p_data.Get<Int32>("TargetSpawnID", 0);
			m_precondition = p_data.Get<String>("Precondition", String.Empty);
			m_timing = p_data.Get<EInteractionTiming>("Timing", EInteractionTiming.NEVER);
			m_requiredState = p_data.Get<EInteractiveObjectState>("RequiredState", EInteractiveObjectState.NONE);
			m_activateCount = p_data.Get<Int32>("ActivateCount", 0);
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("Type", (Int32)m_type);
			p_data.Set<String>("Extra", m_extra);
			p_data.Set<Int32>("TargetSpawnID", m_targetSpawnID);
			p_data.Set<String>("Precondition", m_precondition);
			p_data.Set<Int32>("Timing", (Int32)m_timing);
			p_data.Set<Int32>("RequiredState", (Int32)m_requiredState);
			p_data.Set<Int32>("ActivateCount", m_activateCount);
		}

		public override String ToString()
		{
			return String.Format("[SpawnCommand: Type={0}, TargetSpawnID={1}, Extra={2}, Precondition={3}, Timing={4}, RequiredState={5}, ActivateCount={6}]", new Object[]
			{
				Type,
				TargetSpawnID,
				Extra,
				Precondition,
				Timing,
				RequiredState,
				ActivateCount
			});
		}
	}
}
