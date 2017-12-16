using System;

namespace Legacy.Core.StaticData
{
	public struct EquipmentData
	{
		public readonly EDataType Type;

		public readonly Int32 StaticId;

		public EquipmentData(EDataType p_type, Int32 p_staticId)
		{
			Type = p_type;
			StaticId = p_staticId;
		}
	}
}
