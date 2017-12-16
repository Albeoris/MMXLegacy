using System;
using System.Collections;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Views/SceneView Restore Object")]
	public class PrefabContainerViewRestoreObject : BaseView
	{
		[SerializeField]
		private String m_viewListenCommandName;

		[SerializeField]
		private RestoreObjectChunk[] m_chunks;

		[SerializeField]
		private AnimationCurve m_speedCurve;

		protected override void Awake()
		{
			base.Awake();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PREFAB_CONTAINER_TRIGGER_ANIM, new EventHandler(OnAnimTriggered));
		}

		private void Start()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PREFAB_CONTAINER_TRIGGER_ANIM, new EventHandler(OnAnimTriggered));
		}

		private void OnAnimTriggered(Object p_sender, EventArgs p_args)
		{
			StringEventArgs stringEventArgs = p_args as StringEventArgs;
			if (stringEventArgs != null)
			{
				String[] array = stringEventArgs.text.Split(new Char[]
				{
					'_'
				});
				if (array.Length > 1 && array[0] == m_viewListenCommandName && array[1] == "Activate")
				{
					StartChunkHandling();
				}
				else if (array.Length <= 1 || !(array[0] == m_viewListenCommandName) || array[1] == "Deactivate")
				{
				}
			}
		}

		private void SetActiveRecursively(GameObject obj, Boolean state)
		{
			if (obj != null)
			{
				obj.SetActive(state);
			}
		}

		[ContextMenu("Activate")]
		private void Activate()
		{
			StartChunkHandling();
		}

		private void StartChunkHandling()
		{
			foreach (RestoreObjectChunk restoreObjectChunk in m_chunks)
			{
				if (restoreObjectChunk.ChunkObject != null)
				{
					StartCoroutine(HandleChunk(restoreObjectChunk));
				}
			}
		}

		private IEnumerator HandleChunk(RestoreObjectChunk p_chunkObject)
		{
			yield return new WaitForSeconds(p_chunkObject.Delay);
			if (p_chunkObject.Mode == ChunkMode.Enable || p_chunkObject.Mode == ChunkMode.Disable)
			{
				p_chunkObject.ChunkObject.SetActive(p_chunkObject.Mode == ChunkMode.Enable);
				yield break;
			}
			Single time = 0f;
			Vector3 origPos = p_chunkObject.ChunkObject.transform.position;
			Vector3 origRot = p_chunkObject.ChunkObject.transform.eulerAngles;
			while (time <= p_chunkObject.Time)
			{
				Single curveTime = m_speedCurve.Evaluate(time / p_chunkObject.Time);
				p_chunkObject.ChunkObject.transform.position = Vector3.Lerp(origPos, p_chunkObject.TargetPosition.position, curveTime);
				if (p_chunkObject.Mode == ChunkMode.FlyRotate)
				{
					p_chunkObject.ChunkObject.transform.eulerAngles = Vector3.Lerp(origRot, p_chunkObject.TargetPosition.eulerAngles + p_chunkObject.SpinRotation, curveTime);
				}
				yield return new WaitForEndOfFrame();
				time += Time.deltaTime;
				if (time >= p_chunkObject.Time)
				{
					p_chunkObject.ChunkObject.transform.position = p_chunkObject.TargetPosition.position;
					if (p_chunkObject.Mode == ChunkMode.FlyRotate)
					{
						p_chunkObject.ChunkObject.transform.rotation = p_chunkObject.TargetPosition.rotation;
					}
					if (p_chunkObject.DisableAtEnd)
					{
						p_chunkObject.ChunkObject.SetActive(false);
					}
					if (p_chunkObject.EnableTarget != null)
					{
						p_chunkObject.EnableTarget.SetActive(true);
					}
				}
			}
			yield break;
		}

		public enum ChunkMode
		{
			Enable,
			FlyStraight,
			FlyRotate,
			Disable
		}

		[Serializable]
		public class RestoreObjectChunk
		{
			public Single Delay;

			public Single Time = 1f;

			public Vector3 SpinRotation = Vector3.zero;

			public ChunkMode Mode = ChunkMode.FlyStraight;

			public Transform TargetPosition;

			public GameObject ChunkObject;

			public Boolean DisableAtEnd;

			public GameObject EnableTarget;
		}
	}
}
