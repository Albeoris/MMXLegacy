using System;
using Legacy.Core.Entities.Skills;

namespace Legacy.Core.Entities.Items
{
	public interface ISkillDependant
	{
		Int32 GetRequiredSkillID();

		ETier GetRequiredSkillTier();
	}
}
