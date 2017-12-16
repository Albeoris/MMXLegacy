using System;
using Legacy.Core.Internationalization;
using Legacy.Core.StaticData;

namespace Legacy.Core.Hints
{
	public class Hint
	{
		private HintStaticData staticData;

		private Boolean m_shown;

		public Hint(HintStaticData sd)
		{
			staticData = sd;
			m_shown = false;
		}

		public Int32 StaticID => staticData.StaticID;

	    public EHintType Type => staticData.Type;

	    public EHintCategory Category => staticData.Category;

	    public String Title => Localization.Instance.GetText(staticData.Title);

	    public String Text => Localization.Instance.GetText(staticData.Text);

	    public Boolean Shown
		{
			get => m_shown;
	        set => m_shown = value;
	    }
	}
}
