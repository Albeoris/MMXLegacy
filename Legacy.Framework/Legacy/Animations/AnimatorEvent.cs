using System;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Animations
{
	public class AnimatorEvent : MonoBehaviour
	{
		private static readonly Event[] m_Empty = new Event[0];

		private Int32 m_LastStateHash;

		private Animator m_Animator;

		[SerializeField]
		internal GameObject m_BaseEventTarget;

		[SerializeField]
		internal Event[] m_Events = m_Empty;

		public GameObject BaseEventTarget
		{
			get => m_BaseEventTarget;
		    set => m_BaseEventTarget = value;
		}

		private void Awake()
		{
			m_Animator = this.GetComponent<Animator>(true);
			for (Int32 i = 0; i < m_Events.Length; i++)
			{
				Event @event = m_Events[i];
				@event.StateHash = Animator.StringToHash(@event.StateName);
			}
		}

		private void Update()
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
			Single num;
			if (!currentAnimatorStateInfo.loop && currentAnimatorStateInfo.normalizedTime > 1f)
			{
				num = 1f;
			}
			else
			{
				num = currentAnimatorStateInfo.normalizedTime - (Single)Math.Truncate(currentAnimatorStateInfo.normalizedTime);
			}
			Boolean flag = m_LastStateHash != currentAnimatorStateInfo.nameHash;
			for (Int32 i = 0; i < m_Events.Length; i++)
			{
				Event @event = m_Events[i];
				if (flag)
				{
					@event.LastTime = 0f;
				}
				else if (@event.StateHash == currentAnimatorStateInfo.nameHash)
				{
					if (num < @event.LastTime)
					{
						if (@event.LastTime <= @event.Time || num >= @event.Time)
						{
							_HandleEvent(@event, num);
						}
					}
					else if (num > @event.Time && @event.LastTime <= @event.Time)
					{
						_HandleEvent(@event, num);
					}
					@event.LastTime = num;
				}
			}
			m_LastStateHash = currentAnimatorStateInfo.nameHash;
		}

		private void _HandleEvent(Event data, Single time)
		{
			EventMessage message = data.Message;
			if (String.IsNullOrEmpty(message.MethodName))
			{
				return;
			}
			GameObject gameObject = message.Target;
			if (gameObject == null)
			{
				gameObject = m_BaseEventTarget;
			}
			if (gameObject == null)
			{
				gameObject = this.gameObject;
			}
			Object obj = null;
			switch (message.ParamType)
			{
			case EParamType.Int:
				obj = message.ParamInt;
				break;
			case EParamType.Float:
				obj = message.ParamFloat;
				break;
			case EParamType.String:
				obj = message.ParamString;
				break;
			case EParamType.Object:
				obj = message.ParamObject;
				break;
			}
			try
			{
				switch (message.Type)
				{
				case EEventType.SendMessage:
					gameObject.SendMessage(message.MethodName, obj, message.Option);
					break;
				case EEventType.SendMessageUpwards:
					gameObject.SendMessageUpwards(message.MethodName, obj, message.Option);
					break;
				case EEventType.BroadcastMessage:
					gameObject.BroadcastMessage(message.MethodName, obj, message.Option);
					break;
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(String.Concat(new Object[]
				{
					"MethodName: ",
					message.MethodName,
					"\nValue: ",
					obj,
					"\n",
					ex
				}), this);
			}
		}

		public enum EEventType
		{
			SendMessage,
			SendMessageUpwards,
			BroadcastMessage
		}

		public enum EParamType : byte
		{
			None,
			Int,
			Float,
			String,
			Object
		}

		[Serializable]
		public class Event
		{
			[NonSerialized]
			public Single LastTime;

			[NonSerialized]
			public Int32 StateHash;

			public String StateName;

			public Single Time;

			public EventMessage Message;
		}

		[Serializable]
		public class EventMessage
		{
			public EEventType Type;

			public GameObject Target;

			public SendMessageOptions Option = SendMessageOptions.DontRequireReceiver;

			public String MethodName;

			public EParamType ParamType;

			public Int32 ParamInt;

			public Single ParamFloat;

			public String ParamString;

			public UnityEngine.Object ParamObject;
		}
	}
}
