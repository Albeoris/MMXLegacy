using System;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/PopupItemSplitter")]
	public class PopupItemSplitter : MonoBehaviour
	{
		[SerializeField]
		private UIScrollBar m_slider;

		[SerializeField]
		private UILabel m_maxCount;

		[SerializeField]
		private UILabel m_price;

		[SerializeField]
		private UIInput m_input;

		private Int32 m_max;

		private Int32 m_count;

		private Mode m_mode;

		private BaseItem m_item;

		private Int32 m_itemSlotIndex;

		private IInventory m_targetInventory;

		private Int32 m_targetSlotIndex;

		public Int32 Count => m_count;

	    public BaseItem Item => m_item;

	    public Int32 ItemSlotIndex => m_itemSlotIndex;

	    public IInventory TargetInventory => m_targetInventory;

	    public Int32 TargetSlotIndex => m_targetSlotIndex;

	    private void Awake()
		{
			UIScrollBar slider = m_slider;
			slider.onChange = (UIScrollBar.OnScrollBarChange)Delegate.Combine(slider.onChange, new UIScrollBar.OnScrollBarChange(OnScrollbarChange));
			m_input.onInputChanged += OnInputChange;
			UIInput input = m_input;
			input.validator = (UIInput.Validator)Delegate.Combine(input.validator, new UIInput.Validator(Validate));
		}

		private void OnDestroy()
		{
			UIScrollBar slider = m_slider;
			slider.onChange = (UIScrollBar.OnScrollBarChange)Delegate.Remove(slider.onChange, new UIScrollBar.OnScrollBarChange(OnScrollbarChange));
			m_input.onInputChanged -= OnInputChange;
			UIInput input = m_input;
			input.validator = (UIInput.Validator)Delegate.Remove(input.validator, new UIInput.Validator(Validate));
		}

		public void Open(Mode p_mode, Int32 p_max, BaseItem p_item, Int32 p_itemSlotIndex, IInventory p_targetInventory, Int32 p_targetSlotIndex)
		{
			NGUITools.SetActiveSelf(gameObject, true);
			m_max = p_max;
			m_mode = p_mode;
			m_item = p_item;
			m_itemSlotIndex = p_itemSlotIndex;
			m_targetInventory = p_targetInventory;
			m_targetSlotIndex = p_targetSlotIndex;
			m_maxCount.text = "/ " + p_max;
			m_input.text = "1";
			m_count = 1;
			m_slider.scrollValue = 0f;
			UpdatePrice();
		}

		public void Finish()
		{
			m_item = null;
		}

		private void OnScrollbarChange(UIScrollBar sb)
		{
			m_count = (Int32)Math.Round(m_slider.scrollValue * (m_max - 1f)) + 1;
			m_input.text = m_count.ToString();
			UpdatePrice();
			m_slider.scrollValue = (m_count - 1) / (Single)(m_max - 1);
		}

		private void OnInputChange(String inputString)
		{
			if (m_input.text.Length > 0)
			{
				ValidateInput();
				m_slider.scrollValue = (m_count - 1) / (Single)(m_max - 1);
				m_input.text = m_count.ToString();
			}
		}

		public Char Validate(String currentText, Char nextChar)
		{
			if (Char.IsNumber(nextChar))
			{
				return nextChar;
			}
			return '\0';
		}

		public void FinalizeInput()
		{
			if (m_input.text.Length > 0)
			{
				ValidateInput();
			}
			else
			{
				m_count = 0;
			}
		}

		private void ValidateInput()
		{
			Int32.TryParse(m_input.text, out m_count);
			if (m_count < 1)
			{
				m_count = 1;
			}
			else if (m_count > m_max)
			{
				m_count = m_max;
			}
		}

		private void UpdatePrice()
		{
			if (m_mode == Mode.BUY)
			{
				m_price.text = LocaManager.GetText("POPUP_REQUEST_ITEMS_BUY_PRICE", m_count * m_item.Price);
			}
			else
			{
				m_price.text = LocaManager.GetText("POPUP_REQUEST_ITEMS_BUY_PRICE", m_count * m_item.Price);
			}
		}

		public enum Mode
		{
			SELL,
			BUY
		}
	}
}
