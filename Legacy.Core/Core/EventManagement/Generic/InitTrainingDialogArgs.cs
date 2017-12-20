using System;
using Legacy.Core.Entities.Skills;
using Legacy.Core.StaticData;

namespace Legacy.Core.EventManagement
{
    public class InitTrainingDialogArgs : EventArgs
    {
        public String Caption { get; }
        public ETier SkillRank { get; }
        public SkillStaticData SkillData { get; }
        public String SkillName { get; }

        public InitTrainingDialogArgs(String caption, ETier skillRank, SkillStaticData skillData, String skillName)
        {
            Caption = caption;
            SkillRank = skillRank;
            SkillData = skillData;
            SkillName = skillName;
        }
    }
}
