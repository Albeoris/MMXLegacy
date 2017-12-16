using System;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;
using Legacy.Views;
using UnityEngine;

namespace Legacy.EffectEngine
{
	public static class FXHelper
	{
		public static PlayerEntityView GetPlayerEntity()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			return ViewManager.Instance.FindViewAndGetComponent<PlayerEntityView>(party);
		}

		public static GameObject GetCharacterGO(Int32 pCharIndex)
		{
			return GetPlayerEntity().GetMemberGameObject(pCharIndex);
		}
	}
}
