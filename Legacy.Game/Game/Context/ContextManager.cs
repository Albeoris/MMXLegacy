using System;
using System.Threading;
using UnityEngine;

namespace Legacy.Game.Context
{
	[AddComponentMenu("MM Legacy/Contexts/ContextManager")]
	public class ContextManager : MonoBehaviour
	{
		private EContext m_CurrentContextID = EContext.None;

		private static ContextManager s_Instance;

		private static BaseContext[] s_ContextMap;

		static ContextManager()
		{
			Array values = Enum.GetValues(typeof(EContext));
			s_ContextMap = new BaseContext[values.Length];
		}

	    public static event EventHandler<ContextChangedEventArgs> OnContextChanged;

	    public static event EventHandler<ContextChangedEventArgs> OnContextChanging;

		public static BaseContext ActiveContext { get; private set; }

		public static EContext ActiveContextID
		{
			get => (!(ActiveContext != null)) ? EContext.None : ActiveContext.ID;
		    private set => s_Instance.m_CurrentContextID = value;
		}

		public static Boolean ChangeContext(EContext contextID)
		{
			if (contextID < EContext.DevLogos)
			{
				return false;
			}
			if (ActiveContext != null && ActiveContext.ID == contextID)
			{
				return false;
			}
			EContext activeContextID = ActiveContextID;
			if (OnContextChanging != null)
			{
				OnContextChanging(null, new ContextChangedEventArgs(activeContextID, contextID));
			}
			if (ActiveContext != null)
			{
				try
				{
					ActiveContext.OnDisableContext();
				}
				catch (Exception message)
				{
					Debug.LogError(message);
				}
				ActiveContext = null;
				ActiveContextID = EContext.None;
			}
			BaseContext baseContext = s_ContextMap[(Int32)contextID];
			if (baseContext != null)
			{
				ActiveContext = baseContext;
				try
				{
					ActiveContext.OnEnableContext();
				}
				catch (Exception message2)
				{
					Debug.LogError(message2);
				}
				if (OnContextChanged != null)
				{
					OnContextChanged(null, new ContextChangedEventArgs(activeContextID, contextID));
				}
				return true;
			}
			return false;
		}

		public static void RegisterContext(BaseContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (s_ContextMap[(Int32)context.ID] != null)
			{
				Debug.LogError("Context already registered! " + context.ID, s_ContextMap[(Int32)context.ID]);
				return;
			}
			s_ContextMap[(Int32)context.ID] = context;
		}

		public static void DeregisterContext(EContext contextID)
		{
			if (ActiveContext != null && ActiveContext.ID == contextID)
			{
				try
				{
					ActiveContext.OnDisableContext();
				}
				catch (Exception message)
				{
					Debug.LogError(message);
				}
				ActiveContext = null;
				ActiveContextID = EContext.None;
			}
			s_ContextMap[(Int32)contextID] = null;
		}

		private void Awake()
		{
			if (s_Instance != null)
			{
				throw new Exception("Only one instance allowed!");
			}
			s_Instance = this;
		}
	}
}
