using System;
using System.Collections.Generic;
using Legacy.Core.Spells;

[Serializable]
public class TestSpell
{
	public String Name;

	public String EffectPath;

	public List<String> SorcererBuffEffectPath = new List<String>();

	public List<String> TargetBuffEffectPath = new List<String>();

	public Int32 AnimationNum;

	public ETargetType TargetType;
}
