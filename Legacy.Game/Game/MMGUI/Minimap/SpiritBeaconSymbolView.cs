using System;
using Legacy.Core.Map;

namespace Legacy.Game.MMGUI.Minimap
{
	public class SpiritBeaconSymbolView : SimpleSymbolView
	{
		public override Position MyControllerGridPosition => ControllerGridPosition;

	    public override EDirection MyControllerGridDirection => ControllerGridDirection;

	    public Position ControllerGridPosition { get; set; }

		public EDirection ControllerGridDirection { get; set; }
	}
}
