using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.ActionLogging
{
	public class SkillEffectEventArgs : LogEntryEventArgs
	{
		public SkillEffectEventArgs(SkillEffectStaticData p_skillEffectData)
		{
			EffectData = p_skillEffectData;
		}

		public SkillEffectStaticData EffectData { get; private set; }
	}
}
