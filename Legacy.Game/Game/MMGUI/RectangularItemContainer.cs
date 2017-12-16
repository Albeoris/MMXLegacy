using System;
using Legacy.Core.PartyManagement;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/RectangularItemContainer")]
	public class RectangularItemContainer : ItemContainer
	{
		[SerializeField]
		private Int32 m_width = 5;

		[SerializeField]
		private Int32 m_height = 5;

		[SerializeField]
		private Vector2 m_itemSlotSpace = Vector2.one;

		[SerializeField]
		private GameObject m_itemSlotPrefab;

		public override void Init(IInventory p_inventory)
		{
			base.Init(p_inventory);
			Single num = m_itemSlotSpace.x * (m_width - 1) / 2f;
			Single num2 = m_itemSlotSpace.y * (m_height - 1) / 2f;
			for (Int32 i = 0; i < m_height; i++)
			{
				for (Int32 j = 0; j < m_width; j++)
				{
					Single x = -num + j * m_itemSlotSpace.x;
					Single y = num2 - i * m_itemSlotSpace.y;
					GameObject gameObject = NGUITools.AddChild(this.gameObject, m_itemSlotPrefab);
					AddItemSlot(gameObject.GetComponent<ItemSlot>(), j + i * m_width);
					gameObject.transform.localPosition = new Vector3(x, y, 0f);
				}
			}
			InitStartItems();
		}

		public override void CleanUp()
		{
			base.CleanUp();
			for (Int32 i = 0; i < m_itemSlots.Length; i++)
			{
				if (m_itemSlots[i] != null)
				{
					Destroy(m_itemSlots[i].gameObject);
				}
			}
		}
	}
}
