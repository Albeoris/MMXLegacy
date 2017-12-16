using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Legacy.Bink
{
	public class BinkMedia : IDisposable
	{
		private IntPtr m_binkPtr;

		private Texture2D m_Texture;

		private Color32[] m_Pixels;

		private Boolean m_isLoop;

		private Boolean m_isPause = true;

		private Single m_Volume = 1f;

		private Single m_Pan;

		private UInt32 m_FrameRate;

		private UInt32 m_FrameRateDiv;

		private UInt32 m_PositionFrames;

		public BinkMedia(String filename, Boolean alphablend, Boolean autoplay, Boolean playLoop)
		{
			if (String.IsNullOrEmpty(filename))
			{
				throw new ArgumentException("filename");
			}
			if (!File.Exists(filename))
			{
				throw new FileNotFoundException(filename);
			}
			if (BinkApi.BinkSoundUseDirectSound(0u) == 0)
			{
				throw new Exception("Fail init BinkSoundUseDirectSound ");
			}
			UInt32 num = 0u;
			if (alphablend)
			{
				num |= 1048576u;
			}
			String currentDirectory = Environment.CurrentDirectory;
			try
			{
				Environment.CurrentDirectory = Path.GetDirectoryName(Path.GetFullPath(filename));
				m_binkPtr = BinkApi.BinkOpen(Path.GetFileName(filename), num);
			}
			finally
			{
				Environment.CurrentDirectory = currentDirectory;
			}
			if (m_binkPtr == IntPtr.Zero)
			{
				throw new Exception(String.Format("BinkOpen failed to open '{0}'\nBinkError: {1}", filename, BinkApi.GetError()));
			}
			BinkApi.BINKSUMMARY binksummary;
			BinkApi.BinkGetSummary(m_binkPtr, out binksummary);
			m_Texture = new Texture2D((Int32)binksummary.Width, (Int32)binksummary.Height, TextureFormat.RGBA32, false);
			m_Texture.name = "BinkOutputTexture";
			m_Pixels = new Color32[binksummary.Width * binksummary.Height];
			Loop = playLoop;
			IsAlphaBlended = alphablend;
			Filename = filename;
			Width = (Int32)binksummary.Width;
			Height = (Int32)binksummary.Height;
			m_FrameRate = binksummary.FrameRate;
			m_FrameRateDiv = binksummary.FrameRateDiv;
			DurationFrames = binksummary.TotalFrames;
			if (!autoplay)
			{
				Pause();
			}
		}

		public Boolean IsDisposed { get; private set; }

		public String Filename { get; private set; }

		public Int32 Width { get; private set; }

		public Int32 Height { get; private set; }

		public Single AspectRatio => Width / (Single)Height;

	    public Single FrameRate => m_FrameRate / m_FrameRateDiv;

	    public Single DurationSeconds => DurationFrames / (m_FrameRate / m_FrameRateDiv);

	    public UInt32 DurationFrames { get; private set; }

		public Boolean IsAlphaBlended { get; private set; }

		public Texture OutputTexture
		{
			get
			{
				CheckDisposed();
				return m_Texture;
			}
		}

		public Boolean Loop
		{
			get => m_isLoop;
		    set
			{
				CheckDisposed();
				if (m_isLoop != value)
				{
					m_isLoop = value;
					BinkApi.BinkSetWillLoop(m_binkPtr, (!m_isLoop) ? 0 : 1);
				}
			}
		}

		public Boolean IsPlaying => !m_isPause;

	    public Boolean IsFinishedPlaying { get; private set; }

		public Single Volume
		{
			get => m_Volume;
		    set
			{
				CheckDisposed();
				if (m_Volume != value)
				{
					m_Volume = value;
					Int32 num = (Int32)(32768f * m_Volume);
					num = Mathf.Clamp(num, 0, 65536);
					BinkApi.BinkSetVolume(m_binkPtr, 0u, num);
				}
			}
		}

		public Single AudioBalance
		{
			get => m_Pan;
		    set
			{
				CheckDisposed();
				if (m_Pan != value)
				{
					m_Pan = value;
					Int32 num = (Int32)(32768f * m_Pan) + 32768;
					num = Mathf.Clamp(num, 0, 65536);
					BinkApi.BinkSetPan(m_binkPtr, 0u, num);
				}
			}
		}

		public Single PositionSeconds
		{
			get => m_PositionFrames / FrameRate;
		    set => PositionFrames = (UInt32)(value * FrameRate);
		}

		public UInt32 PositionFrames
		{
			get => m_PositionFrames;
		    set
			{
				CheckDisposed();
				m_PositionFrames = value;
				BinkApi.BinkGoto(m_binkPtr, value, 0u);
			}
		}

		~BinkMedia()
		{
			Dispose();
		}

		public void Update()
		{
			CheckDisposed();
			if (BinkApi.BinkWait(m_binkPtr) == 0)
			{
				BinkApi.BinkDoFrame(m_binkPtr);
				while (BinkApi.BinkShouldSkip(m_binkPtr) != 0)
				{
					BinkApi.BinkNextFrame(m_binkPtr);
					BinkApi.BinkDoFrame(m_binkPtr);
				}
				GCHandle gchandle = GCHandle.Alloc(m_Pixels, GCHandleType.Pinned);
				try
				{
					UInt32 num = 0u;
					if (IsAlphaBlended)
					{
						num |= 6u;
					}
					else
					{
						num |= 4u;
					}
					BinkApi.BinkCopyToBuffer(m_binkPtr, gchandle.AddrOfPinnedObject(), m_Texture.width * 4, (UInt32)m_Texture.height, 0u, 0u, num);
					m_Texture.SetPixels32(m_Pixels, 0);
					m_Texture.Apply();
				}
				finally
				{
					gchandle.Free();
				}
				BinkApi.BINKREALTIME binkrealtime;
				BinkApi.BinkGetRealtime(m_binkPtr, out binkrealtime, 0u);
				m_PositionFrames = binkrealtime.FrameNum;
				if (!m_isLoop && m_PositionFrames >= DurationFrames)
				{
					IsFinishedPlaying = true;
					Pause();
					return;
				}
				BinkApi.BinkNextFrame(m_binkPtr);
			}
		}

		public void Play()
		{
			CheckDisposed();
			BinkApi.BinkPause(m_binkPtr, 0);
			m_isPause = false;
			if (IsFinishedPlaying)
			{
				m_PositionFrames = 0u;
				BinkApi.BinkGoto(m_binkPtr, 0u, 1u);
				IsFinishedPlaying = false;
			}
		}

		public void Pause()
		{
			CheckDisposed();
			BinkApi.BinkPause(m_binkPtr, 1);
			m_isPause = true;
		}

		public void Dispose()
		{
			if (!IsDisposed)
			{
				IsDisposed = true;
				if (m_binkPtr != IntPtr.Zero)
				{
					BinkApi.BinkClose(m_binkPtr);
					m_binkPtr = IntPtr.Zero;
				}
				Helper.DestroyImmediate<Texture2D>(ref m_Texture);
				m_Pixels = null;
			}
		}

		private void CheckDisposed()
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException("BinkMedia object already disposed!");
			}
		}
	}
}
