using System;
using System.Runtime.InteropServices;

namespace Legacy.Bink
{
	internal static class BinkApi
	{
		private const String DLL_NAME = "bink2w32";

		public const String BinkDate = "2013-08-22";

		public const String Bink2Version = "2.2d";

		public const String Bink1Version = "1.992d";

		public const String BinkVersion = "2.2d/1.992d";

		public const UInt32 BINKYAINVERT = 2048u;

		public const UInt32 BINKFRAMERATE = 4096u;

		public const UInt32 BINKPRELOADALL = 8192u;

		public const UInt32 BINKSNDTRACK = 16384u;

		public const UInt32 BINKOLDFRAMEFORMAT = 32768u;

		public const UInt32 BINKRBINVERT = 65536u;

		public const UInt32 BINKGRAYSCALE = 131072u;

		public const UInt32 BINKNOSKIP = 524288u;

		public const UInt32 BINKALPHA = 1048576u;

		public const UInt32 BINKNOFILLIOBUF = 2097152u;

		public const UInt32 BINKSIMULATE = 4194304u;

		public const UInt32 BINKFILEHANDLE = 8388608u;

		public const UInt32 BINKIOSIZE = 16777216u;

		public const UInt32 BINKIOPROCESSOR = 33554432u;

		public const UInt32 BINKFROMMEMORY = 67108864u;

		public const UInt32 BINKNOTHREADEDIO = 134217728u;

		public const UInt32 BINKNOFRAMEBUFFERS = 1024u;

		public const UInt32 BINKNOYPLANE = 512u;

		public const UInt32 BINKRUNNINGASYNC = 256u;

		public const UInt32 BINKWILLLOOP = 128u;

		public const UInt32 BINKDONTCLOSETHREADS = 64u;

		public const UInt32 BINKFILEOFFSET = 32u;

		public const UInt32 BINKSURFACEFAST = 0u;

		public const UInt32 BINKSURFACESLOW = 134217728u;

		public const UInt32 BINKSURFACEDIRECT = 67108864u;

		public const UInt32 BINKCOPYALL = 2147483648u;

		public const UInt32 BINKCOPY2XH = 268435456u;

		public const UInt32 BINKCOPY2XHI = 536870912u;

		public const UInt32 BINKCOPY2XW = 805306368u;

		public const UInt32 BINKCOPY2XWH = 1073741824u;

		public const UInt32 BINKCOPY2XWHI = 1342177280u;

		public const UInt32 BINKCOPY1XI = 1610612736u;

		public const UInt32 BINKCOPYNOSCALING = 1879048192u;

		public const UInt32 BINKSURFACEP8 = 0u;

		public const UInt32 BINKSURFACE24 = 1u;

		public const UInt32 BINKSURFACE24R = 2u;

		public const UInt32 BINKSURFACE32 = 3u;

		public const UInt32 BINKSURFACE32R = 4u;

		public const UInt32 BINKSURFACE32A = 5u;

		public const UInt32 BINKSURFACE32RA = 6u;

		public const UInt32 BINKSURFACE4444 = 7u;

		public const UInt32 BINKSURFACE5551 = 8u;

		public const UInt32 BINKSURFACE555 = 9u;

		public const UInt32 BINKSURFACE565 = 10u;

		public const UInt32 BINKSURFACE655 = 11u;

		public const UInt32 BINKSURFACE664 = 12u;

		public const UInt32 BINKSURFACEYUY2 = 13u;

		public const UInt32 BINKSURFACEUYVY = 14u;

		public const UInt32 BINKSURFACEYV12 = 15u;

		public const UInt32 BINKSURFACEMASK = 15u;

		public const UInt32 BINKGOTOQUICK = 1u;

		public const UInt32 BINKGOTOQUICKSOUND = 2u;

		public const UInt32 BINKGETKEYPREVIOUS = 0u;

		public const UInt32 BINKGETKEYNEXT = 1u;

		public const UInt32 BINKGETKEYCLOSEST = 2u;

		public const UInt32 BINKGETKEYNOTEQUAL = 128u;

		public const UInt32 BINKDOFRAMEHALF1 = 1u;

		public const UInt32 BINKDOFRAMEHALF2 = 2u;

		public const UInt32 BINKDOFRAMESTART = 256u;

		public const UInt32 BINKDOFRAMEEND = 512u;

		public const UInt32 BINKPLATFORMSOUNDTHREAD = 1u;

		public const UInt32 BINKPLATFORMIOTHREAD = 2u;

		public const UInt32 BINKPLATFORMSUBMITTHREADCOUNT = 3u;

		public const UInt32 BINKPLATFORMSUBMITTHREADS = 4u;

		public const UInt32 BINKPLATFORMASYNCTHREADS = 1024u;

		public const UInt32 BINKBGIOSUSPEND = 1u;

		public const UInt32 BINKBGIORESUME = 2u;

		public const UInt32 BINKBGIOWAIT = 2147483648u;

		public const UInt32 BINKBGIOTRYWAIT = 1073741824u;

		public const UInt32 BINK_CPU_MMX = 1u;

		public const UInt32 BINK_CPU_3DNOW = 2u;

		public const UInt32 BINK_CPU_SSE = 3u;

		public const UInt32 BINK_CPU_SSE2 = 8u;

		public const UInt32 BINK_CPU_ALTIVEC = 1u;

		[DllImport("bink2w32")]
		public static extern IntPtr BinkLogoAddress();

		[DllImport("bink2w32")]
		public static extern void BinkSetError([MarshalAs(UnmanagedType.LPStr)] String str);

		[DllImport("bink2w32")]
		public static extern IntPtr BinkGetError();

		public static String GetError()
		{
			IntPtr ptr = BinkGetError();
			return Marshal.PtrToStringAnsi(ptr);
		}

		[DllImport("bink2w32")]
		public static extern IntPtr BinkOpen(String name, UInt32 flags);

		[DllImport("bink2w32")]
		public static extern IntPtr BinkOpen(IntPtr ptr, UInt32 flags);

		[DllImport("bink2w32")]
		public static extern IntPtr BinkOpenWithOptions(String name, ref BINK_OPEN_OPTIONS boo, UInt32 flags);

		[DllImport("bink2w32")]
		public static extern Int32 BinkDoFrame(IntPtr bnk);

		[DllImport("bink2w32")]
		public static extern Int32 BinkDoFramePlane(IntPtr bnk, UInt32 which_planes);

		[DllImport("bink2w32")]
		public static extern void BinkNextFrame(IntPtr bnk);

		[DllImport("bink2w32")]
		public static extern Int32 BinkWait(IntPtr bnk);

		[DllImport("bink2w32")]
		public static extern void BinkClose(IntPtr bnk);

		[DllImport("bink2w32")]
		public static extern Int32 BinkPause(IntPtr bnk, Int32 pause);

		[DllImport("bink2w32")]
		public static extern Int32 BinkCopyToBuffer(IntPtr bnk, IntPtr dest, Int32 destpitch, UInt32 destheight, UInt32 destx, UInt32 desty, UInt32 flags);

		[DllImport("bink2w32")]
		public static extern Int32 BinkCopyToBufferRect(IntPtr bnk, IntPtr dest, Int32 destpitch, UInt32 destheight, UInt32 destx, UInt32 desty, UInt32 srcx, UInt32 srcy, UInt32 srcw, UInt32 srch, UInt32 flags);

		[DllImport("bink2w32")]
		public static extern Int32 BinkGetRects(IntPtr bnk, UInt32 flags);

		[DllImport("bink2w32")]
		public static extern void BinkGoto(IntPtr bnk, UInt32 frame, UInt32 flags);

		[DllImport("bink2w32")]
		public static extern UInt32 BinkGetKeyFrame(IntPtr bnk, UInt32 frame, Int32 flags);

		[DllImport("bink2w32")]
		public static extern void BinkFreeGlobals();

		[DllImport("bink2w32")]
		public static extern Int32 BinkSetVideoOnOff(IntPtr bnk, Int32 onoff);

		[DllImport("bink2w32")]
		public static extern Int32 BinkSetSoundOnOff(IntPtr bnk, Int32 onoff);

		[DllImport("bink2w32")]
		public static extern void BinkSetVolume(IntPtr bnk, UInt32 trackid, Int32 volume);

		[DllImport("bink2w32")]
		public static extern void BinkSetPan(IntPtr bnk, UInt32 trackid, Int32 pan);

		[DllImport("bink2w32")]
		public static extern void BinkService(IntPtr bink);

		[DllImport("bink2w32")]
		public static extern Int32 BinkShouldSkip(IntPtr bink);

		[DllImport("bink2w32")]
		public static extern Int32 BinkControlBackgroundIO(IntPtr bink, UInt32 control);

		[DllImport("bink2w32")]
		public static extern void BinkSetWillLoop(IntPtr bink, Int32 onoff);

		[DllImport("bink2w32")]
		public static extern void BinkFlipEndian32(Byte[] buffer, UInt32 bytes);

		[DllImport("bink2w32")]
		public static extern void BinkOpenTrack(IntPtr bnk, UInt32 trackindex);

		[DllImport("bink2w32")]
		public static extern void BinkCloseTrack(ref BINKTRACK bnkt);

		[DllImport("bink2w32")]
		public static extern UInt32 BinkGetTrackData(ref BINKTRACK bnkt, Byte[] dest);

		[DllImport("bink2w32")]
		public static extern UInt32 BinkGetTrackType(IntPtr bnk, UInt32 trackindex);

		[DllImport("bink2w32")]
		public static extern UInt32 BinkGetTrackMaxSize(IntPtr bnk, UInt32 trackindex);

		[DllImport("bink2w32")]
		public static extern UInt32 BinkGetTrackID(IntPtr bnk, UInt32 trackindex);

		[DllImport("bink2w32")]
		public static extern void BinkGetSummary(IntPtr bnk, out BINKSUMMARY sum);

		[DllImport("bink2w32")]
		public static extern void BinkGetRealtime(IntPtr bnk, out BINKREALTIME run, UInt32 frames);

		[DllImport("bink2w32")]
		public static extern void BinkSetFileOffset(UInt64 offset);

		[DllImport("bink2w32")]
		public static extern void BinkSetSoundTrack(UInt32 total_tracks, UInt32[] tracks);

		[DllImport("bink2w32")]
		public static extern void BinkSetIO(BINKIOOPEN io);

		[DllImport("bink2w32")]
		public static extern void BinkSetFrameRate(UInt32 forcerate, UInt32 forceratediv);

		[DllImport("bink2w32")]
		public static extern void BinkSetSimulate(UInt32 sim);

		[DllImport("bink2w32")]
		public static extern void BinkSetIOSize(UInt32 iosize);

		[DllImport("bink2w32")]
		public static extern Int32 BinkSetSoundSystem(BINKSNDSYSOPEN open, UInt32 param);

		[DllImport("bink2w32")]
		public static extern Int32 BinkControlPlatformFeatures(Int32 use, Int32 dont_use);

		[DllImport("bink2w32")]
		public static extern BINKSNDOPEN BinkOpenDirectSound(UInt32 param);

		public static Int32 BinkSoundUseDirectSound(UInt32 lpDS)
		{
			return BinkSetSoundSystem(new BINKSNDSYSOPEN(BinkOpenDirectSound), lpDS);
		}

		[DllImport("bink2w32")]
		public static extern BINKSNDOPEN BinkOpenWaveOut(UInt32 param);

		public static Int32 BinkSoundUseWaveOut(UInt32 lpDS)
		{
			return BinkSetSoundSystem(new BINKSNDSYSOPEN(BinkOpenWaveOut), lpDS);
		}

		[DllImport("bink2w32")]
		public static extern void BinkUseTelemetry(Byte[] context);

		[DllImport("bink2w32")]
		public static extern void BinkUseTmLite(Byte[] context);

		public delegate Int32 BINKIOOPEN(IntPtr Bnkio, String name, UInt32 flags);

		public delegate Int32 BINKSNDOPEN(IntPtr BnkSnd, UInt32 freq, Int32 bits, Int32 chans, UInt32 flags, IntPtr bink);

		public delegate BINKSNDOPEN BINKSNDSYSOPEN(UInt32 param);

		public delegate BINKSNDOPEN BINKSNDSYSOPEN2(UInt32 param1, UInt32 param2);

		public struct BINKSUMMARY
		{
			public UInt32 Width;

			public UInt32 Height;

			public UInt32 TotalTime;

			public UInt32 FileFrameRate;

			public UInt32 FileFrameRateDiv;

			public UInt32 FrameRate;

			public UInt32 FrameRateDiv;

			public UInt32 TotalOpenTime;

			public UInt32 TotalFrames;

			public UInt32 TotalPlayedFrames;

			public UInt32 SkippedFrames;

			public UInt32 SkippedBlits;

			public UInt32 SoundSkips;

			public UInt32 TotalBlitTime;

			public UInt32 TotalReadTime;

			public UInt32 TotalVideoDecompTime;

			public UInt32 TotalAudioDecompTime;

			public UInt32 TotalIdleReadTime;

			public UInt32 TotalBackReadTime;

			public UInt32 TotalReadSpeed;

			public UInt32 SlowestFrameTime;

			public UInt32 Slowest2FrameTime;

			public UInt32 SlowestFrameNum;

			public UInt32 Slowest2FrameNum;

			public UInt32 AverageDataRate;

			public UInt32 AverageFrameSize;

			public UInt32 HighestMemAmount;

			public UInt32 TotalIOMemory;

			public UInt32 HighestIOUsed;

			public UInt32 Highest1SecRate;

			public UInt32 Highest1SecFrame;
		}

		public struct BINKREALTIME
		{
			public UInt32 FrameNum;

			public UInt32 FrameRate;

			public UInt32 FrameRateDiv;

			public UInt32 Frames;

			public UInt32 FramesTime;

			public UInt32 FramesVideoDecompTime;

			public UInt32 FramesAudioDecompTime;

			public UInt32 FramesReadTime;

			public UInt32 FramesIdleReadTime;

			public UInt32 FramesThreadReadTime;

			public UInt32 FramesBlitTime;

			public UInt32 ReadBufferSize;

			public UInt32 ReadBufferUsed;

			public UInt32 FramesDataRate;
		}

		public struct BINK_OPEN_OPTIONS
		{
			public UInt64 FileOffset;

			public Int32 ForceRate;

			public Int32 ForceRateDiv;

			public Int32 IOBufferSize;

			public Int32 Simulate;

			public UInt32 TotTracks;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			private Int32[] TrackNums;

			private BINKIOOPEN UserOpen;
		}

		public struct BINKTRACK
		{
			private UInt32 Frequency;

			private UInt32 Bits;

			private UInt32 Channels;

			private UInt32 MaxSize;

			private IntPtr bink;

			private UInt32 sndcomp;

			private Int32 trackindex;
		}
	}
}
