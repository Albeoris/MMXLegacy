using System;

namespace Legacy.Core.ServiceWrapper
{
	public enum UPLAY_EventType
	{
		UPLAY_Event_FriendsFriendListUpdated = 10000,
		UPLAY_Event_FriendsFriendUpdated,
		UPLAY_Event_FriendsGameInviteAccepted,
		UPLAY_Event_FriendsMenuItemSelected,
		UPLAY_Event_PartyMemberListChanged = 20000,
		UPLAY_Event_PartyMemberUserDataUpdated,
		UPLAY_Event_PartyLeaderChanged,
		UPLAY_Event_PartyGameInviteReceived,
		UPLAY_Event_PartyGameInviteAccepted,
		UPLAY_Event_PartyMemberMenuItemSelected,
		UPLAY_Event_PartyMemberUpdated,
		UPLAY_Event_PartyInviteReceived,
		UPLAY_Event_PartyMemberJoined,
		UPLAY_Event_PartyMemberLeft,
		UPLAY_Event_OverlayActivated = 30000,
		UPLAY_Event_OverlayHidden,
		UPLAY_Event_RewardRedeemed = 40000,
		UPLAY_Event_UserAccountSharing = 50000,
		UPLAY_Event_UserConnectionLost,
		UPLAY_Event_UserConnectionRestored
	}
}
