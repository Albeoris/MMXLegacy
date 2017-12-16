using System;
using Legacy.Animations;
using Legacy.EffectEngine.Effects;
using Legacy.Views;
using UnityEngine;

namespace Legacy
{
	public class CommunityDungeonBossRevealView : BaseView
	{
		[SerializeField]
		private AnimatorControl Model;

		[SerializeField]
		private String PrefabPath;

		public void RunFX()
		{
			GameObject gameObject = Helper.Instantiate<GameObject>(Helper.ResourcesLoad<GameObject>(PrefabPath));
			Destroy(gameObject, 10f);
			CharacterReplacementFX component = gameObject.GetComponent<CharacterReplacementFX>();
			component.Model = Model;
		}
	}
}
