using System;
using System.Xml.Serialization;
using Legacy.Core.Api;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class DayTimeEqualsCondition : DialogCondition
	{
		private EDayState m_dayTime;

		[XmlAttribute("dayTime")]
		public EDayState DayTime
		{
			get => m_dayTime;
		    set => m_dayTime = value;
		}

		public override EDialogState CheckCondition(Npc p_npc)
		{
			if (LegacyLogic.Instance.GameTime.DayState == m_dayTime)
			{
				return EDialogState.NORMAL;
			}
			return FailState;
		}
	}
}
