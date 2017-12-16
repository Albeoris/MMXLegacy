using System;
using System.Xml.Serialization;
using Legacy.Core.Api;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class CheckNotOnMapCondition : DialogCondition
	{
		private String m_map;

		[XmlAttribute("map")]
		public String Map
		{
			get => m_map;
		    set => m_map = value;
		}

		public override EDialogState CheckCondition(Npc p_npc)
		{
			String gridFileName = LegacyLogic.Instance.MapLoader.GridFileName;
			if (gridFileName != Map)
			{
				return EDialogState.NORMAL;
			}
			return FailState;
		}
	}
}
