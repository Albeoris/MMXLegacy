using System;

namespace Legacy.Core.ServiceWrapper
{
	public enum UPLAY_OverlappedResult
	{
		UPLAY_OverlappedResult_Ok,
		UPLAY_OverlappedResult_InvalidArgument,
		UPLAY_OverlappedResult_ConnectionError,
		UPLAY_OverlappedResult_NotFound,
		UPLAY_OverlappedResult_NotAPartyLeader,
		UPLAY_OverlappedResult_PartyFull,
		UPLAY_OverlappedResult_Failed,
		UPLAY_OverlappedResult_AlreadyOpened,
		UPLAY_OverlappedResult_SlotLocked
	}
}
