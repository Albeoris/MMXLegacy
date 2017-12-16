using System;

namespace Legacy.MMGUI
{
	public class GUIDialogParameters
	{
		private String m_title;

		private String m_text;

		private EDialogType m_type;

		private Callback m_callback;

		private Boolean m_isPopup = true;

		public GUIDialogParameters(String p_title, String p_text, EDialogType p_type, Callback p_callback, Boolean p_isPopup)
		{
			m_title = p_title;
			m_text = p_text;
			m_type = p_type;
			m_callback = p_callback;
			m_isPopup = p_isPopup;
		}

		public String Title => m_title;

	    public String Text => m_text;

	    public EDialogType Type => m_type;

	    public Callback CallbackFunc => m_callback;

	    public Boolean IsPopup => m_isPopup;

	    public override String ToString()
		{
			String text = "DialogParameters:\nTitle: " + ((Title != null) ? Title : "NULL") + "\nText: " + ((Text != null) ? Text : "NULL");
			text = text + "\nType: " + Type;
			text = text + "\nCallbackFunc: " + ((CallbackFunc != null) ? CallbackFunc.ToString() : "NULL");
			return text + "\nIsPopup: " + IsPopup;
		}

		public enum EDialogType
		{
			DIALOG_OK,
			DIALOG_YES_NO
		}

		public enum EDialogAnswer
		{
			OK,
			YES,
			NO
		}

		public delegate void Callback(EDialogAnswer p_answer);
	}
}
