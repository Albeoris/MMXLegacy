using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Object = System.Object;

namespace AssetBundles.Core.Loader
{
	public abstract class AssetBundleLoader : MonoBehaviour, IEnumerable<AssetBundleRequest>, IEnumerable
	{
		private const Int32 START_REQUEST_FRAME_DELAY = 6;

		[SerializeField]
		internal String m_BaseAddress;

		[SerializeField]
		private Int32 m_MaxLoads = -1;

		[SerializeField]
		private Boolean m_BroadcastEvent;

		private List<AssetBundleRequest> m_Requests = new List<AssetBundleRequest>();

		private Boolean m_SortRequests;

		private Int32 m_NextRequestStart;

	    public event EventHandler<LoadStartedEventArgs> LoadStarted;
	    public event EventHandler<LoadCompletedEventArgs> LoadCompleted;
	    public event EventHandler<LoadProgressChangedEventArgs> LoadProgressChanged;

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_Requests.GetEnumerator();
		}

		public String BaseAddress
		{
			get => m_BaseAddress;
		    set
			{
				if (m_BaseAddress != value)
				{
					m_BaseAddress = value;
					ParsedBaseAddress = ((m_BaseAddress == null) ? null : m_BaseAddress.Replace("{streamPath}", Application.streamingAssetsPath).Replace("{dataPath}", Application.dataPath));
				}
			}
		}

		public String ParsedBaseAddress { get; private set; }

		public Int32 MaximalDownloads
		{
			get => m_MaxLoads;
		    set => m_MaxLoads = ((value >= -1) ? value : -1);
		}

		public virtual void Load(String address, Int32 priority, Int32 fileSize, Int32 version, UInt32 crcValue, Object userToken)
		{
			AssetBundleRequest assetBundleRequest = GetAssetBundleRequest(address, priority, fileSize, version, crcValue);
			assetBundleRequest.UserToken = userToken;
			m_Requests.Add(assetBundleRequest);
		}

		public virtual void Abort()
		{
			foreach (AssetBundleRequest assetBundleRequest in m_Requests)
			{
				assetBundleRequest.Abort();
				OnLoadCompleted(assetBundleRequest, true, null);
			}
			m_Requests.Clear();
		}

		protected virtual void Awake()
		{
			MaximalDownloads = m_MaxLoads;
			ParsedBaseAddress = m_BaseAddress.Replace("{streamPath}", Application.streamingAssetsPath).Replace("{dataPath}", Application.dataPath);
		}

		protected virtual void Update()
		{
			Int32 num = 0;
			for (Int32 i = m_Requests.Count - 1; i >= 0; i--)
			{
				AssetBundleRequest assetBundleRequest = m_Requests[i];
				if (assetBundleRequest.IsStarted)
				{
					num++;
					Single num2 = assetBundleRequest.Update();
					if (num2 != 0f || assetBundleRequest.IsDone || assetBundleRequest.IsError)
					{
						OnLoadProgressChanged(assetBundleRequest);
					}
					if (assetBundleRequest.IsDone || assetBundleRequest.IsError)
					{
						OnLoadCompleted(assetBundleRequest, false, assetBundleRequest.ErrorText);
						m_Requests.RemoveAt(i);
					}
				}
			}
			Boolean flag = m_NextRequestStart < Time.frameCount;
			if ((num < m_MaxLoads || m_MaxLoads == -1) && flag)
			{
				for (Int32 j = m_Requests.Count - 1; j >= 0; j--)
				{
					AssetBundleRequest assetBundleRequest2 = m_Requests[j];
					if (!assetBundleRequest2.IsStarted)
					{
						m_NextRequestStart = Time.frameCount + 6;
						assetBundleRequest2.Start();
						OnLoadStarted(assetBundleRequest2);
						break;
					}
				}
			}
		}

		protected abstract AssetBundleRequest GetAssetBundleRequest(String address, Int32 priority, Int32 fileSize, Int32 version, UInt32 crcValue);

		protected virtual void OnLoadStarted(AssetBundleRequest request)
		{
			LoadStartedEventArgs loadStartedEventArgs = (LoadCompleted == null && !m_BroadcastEvent) ? null : new LoadStartedEventArgs(request);
			if (LoadStarted != null)
			{
				LoadStarted(this, loadStartedEventArgs);
			}
			if (m_BroadcastEvent)
			{
				SendMessage("OnAssetBundleLoadStarted", new UnityEventArgs<LoadStartedEventArgs>(this, loadStartedEventArgs), SendMessageOptions.DontRequireReceiver);
			}
		}

		protected virtual void OnLoadCompleted(AssetBundleRequest request, Boolean cancelled, String error)
		{
			LoadCompletedEventArgs loadCompletedEventArgs = (LoadCompleted == null && !m_BroadcastEvent) ? null : new LoadCompletedEventArgs(request, cancelled, error);
			if (LoadCompleted != null)
			{
				LoadCompleted(this, loadCompletedEventArgs);
			}
			if (m_BroadcastEvent)
			{
				SendMessage("OnAssetBundleLoadCompleted", new UnityEventArgs<LoadCompletedEventArgs>(this, loadCompletedEventArgs), SendMessageOptions.DontRequireReceiver);
			}
		}

		protected virtual void OnLoadProgressChanged(AssetBundleRequest request)
		{
			LoadProgressChangedEventArgs loadProgressChangedEventArgs = (LoadCompleted == null && !m_BroadcastEvent) ? null : new LoadProgressChangedEventArgs(request);
			if (LoadProgressChanged != null)
			{
				LoadProgressChanged(this, loadProgressChangedEventArgs);
			}
			if (m_BroadcastEvent)
			{
				SendMessage("OnAssetBundleLoadProgressChanged", new UnityEventArgs<LoadProgressChangedEventArgs>(this, loadProgressChangedEventArgs), SendMessageOptions.DontRequireReceiver);
			}
		}

		public IEnumerator<AssetBundleRequest> GetEnumerator()
		{
			return m_Requests.GetEnumerator();
		}

		private class AssetBundleRequestComparer : IComparer<AssetBundleRequest>
		{
			public static AssetBundleRequestComparer Default = new AssetBundleRequestComparer();

			public Int32 Compare(AssetBundleRequest x, AssetBundleRequest y)
			{
				return y.Priority.CompareTo(x.Priority);
			}
		}
	}
}
