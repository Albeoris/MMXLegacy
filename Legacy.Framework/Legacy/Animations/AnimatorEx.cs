using System;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Animations
{
	public class AnimatorEx
	{
		private Animator m_Animator;

		private Dictionary<String, IUpdateable> m_Properties = new Dictionary<String, IUpdateable>();

		private List<IUpdateable> m_Updates = new List<IUpdateable>();

		public AnimatorEx(Animator animator)
		{
			m_Animator = animator;
		}

		public Property<T> RegisterProperty<T>(String propertyName) where T : struct
		{
		    IUpdateable property;
			if (typeof(T) == typeof(Boolean))
			{
				property = new BoolProperty(this, m_Animator, propertyName);
			}
			else if (typeof(T) == typeof(Int32))
			{
				property = new IntProperty(this, m_Animator, propertyName);
			}
			else if (typeof(T) == typeof(Single))
			{
				property = new FloatProperty(this, m_Animator, propertyName);
			}
			else if (typeof(T) == typeof(Vector3))
			{
				property = new Vector3Property(this, m_Animator, propertyName);
			}
			else
			{
				if (typeof(T) != typeof(Quaternion))
				{
					throw new NotSupportedException("'" + typeof(T).Name + "' Type not supported!");
				}
				property = new QuaternionProperty(this, m_Animator, propertyName);
			}
			m_Properties.Add(propertyName, property);
			return (Property<T>)property;
		}

		public Property<T> GetProperty<T>(String propertyName) where T : struct
		{
			IUpdateable updateable;
			m_Properties.TryGetValue(propertyName, out updateable);
			return updateable as Property<T>;
		}

		public void Update()
		{
			for (Int32 i = m_Updates.Count - 1; i >= 0; i--)
			{
				if (m_Updates[i].Update())
				{
					m_Updates.RemoveAt(i);
				}
			}
		}

		public interface IUpdateable
		{
			Boolean Update();
		}

		[Serializable]
		public abstract class Property<T> : IUpdateable where T : struct
		{
			[SerializeField]
			private String m_PropertyName;

			[SerializeField]
			private T m_Value;

			protected AnimatorEx m_Controller;

			protected Animator m_Animator;

			protected Int32 m_NameHash;

			protected Int32 m_TriggerLayer;

			protected Int32 m_TriggerTagHash;

			protected T m_TriggerResetValue;

			public Property()
			{
			}

			public Property(AnimatorEx controller, Animator animator, String propertyName)
			{
				m_PropertyName = propertyName;
				Init(controller, animator);
			}

			public String Name
			{
				get => m_PropertyName;
			    set => m_PropertyName = value;
			}

			public virtual T RawValue
			{
				get => m_Value;
			    set => m_Value = value;
			}

			public virtual T Value
			{
				get
				{
					T result;
					GetValue(out result);
					return result;
				}
				set => SetValue(ref value);
			}

			public Boolean IsNameDefined => !String.IsNullOrEmpty(m_PropertyName);

		    public virtual void Init(AnimatorEx controller, Animator animator)
			{
				m_Controller = controller;
				m_Animator = animator;
				if (!String.IsNullOrEmpty(m_PropertyName))
				{
					m_NameHash = Animator.StringToHash(m_PropertyName);
					if (m_NameHash == 0)
					{
						Debug.LogError(String.Concat(new String[]
						{
							"Property '",
							m_PropertyName,
							"' not in '",
							m_Animator.runtimeAnimatorController.name,
							"' defined!"
						}));
					}
					else
					{
						SetValue(ref m_Value);
					}
				}
			}

			public abstract void SetValue(ref T value);

			public abstract void GetValue(out T value);

			public void Apply()
			{
				SetValue(ref m_Value);
			}

			public void Trigger(Int32 layer, String tag, T resetValue)
			{
				Trigger(layer, tag, ref resetValue);
			}

			public void Trigger(Int32 layer, String tag, ref T resetValue)
			{
				m_TriggerLayer = layer;
				m_TriggerTagHash = Animator.StringToHash(tag);
				m_TriggerResetValue = resetValue;
				if (!m_Controller.m_Updates.Contains(this))
				{
					m_Controller.m_Updates.Add(this);
				}
			}

			public Boolean Update()
			{
				if (m_Animator.IsInTransition(0) && m_Animator.GetNextAnimatorStateInfo(m_TriggerLayer).tagHash == m_TriggerTagHash)
				{
					m_Value = m_TriggerResetValue;
					SetValue(ref m_TriggerResetValue);
					return true;
				}
				return false;
			}
		}

		[Serializable]
		public class BoolProperty : Property<Boolean>
		{
			public BoolProperty()
			{
			}

			public BoolProperty(AnimatorEx controller, Animator animator, String propertyName) : base(controller, animator, propertyName)
			{
			}

			public override Boolean Value
			{
				get => RawValue;
			    set
				{
					if (RawValue != value)
					{
						RawValue = value;
						SetValue(ref value);
					}
				}
			}

			public override void SetValue(ref Boolean value)
			{
				if (m_NameHash != 0)
				{
					m_Animator.SetBool(m_NameHash, value);
				}
			}

			public override void GetValue(out Boolean value)
			{
				value = RawValue;
				if (m_NameHash != 0)
				{
					value = m_Animator.GetBool(m_NameHash);
				}
			}
		}

		[Serializable]
		public class IntProperty : Property<Int32>
		{
			public IntProperty()
			{
			}

			public IntProperty(AnimatorEx controller, Animator animator, String propertyName) : base(controller, animator, propertyName)
			{
			}

			public override Int32 Value
			{
				get => RawValue;
			    set
				{
					if (RawValue != value)
					{
						RawValue = value;
						SetValue(ref value);
					}
				}
			}

			public override void SetValue(ref Int32 value)
			{
				if (m_NameHash != 0)
				{
					m_Animator.SetInteger(m_NameHash, value);
				}
			}

			public override void GetValue(out Int32 value)
			{
				value = RawValue;
				if (m_NameHash != 0)
				{
					value = m_Animator.GetInteger(m_NameHash);
				}
			}
		}

		[Serializable]
		public class FloatProperty : Property<Single>
		{
			public FloatProperty()
			{
			}

			public FloatProperty(AnimatorEx controller, Animator animator, String propertyName) : base(controller, animator, propertyName)
			{
			}

			public override Single Value
			{
				get => RawValue;
			    set
				{
					if (RawValue != value)
					{
						RawValue = value;
						SetValue(ref value);
					}
				}
			}

			public override void SetValue(ref Single value)
			{
				if (m_NameHash != 0)
				{
					m_Animator.SetFloat(m_NameHash, value);
				}
			}

			public override void GetValue(out Single value)
			{
				value = RawValue;
				if (m_NameHash != 0)
				{
					value = m_Animator.GetFloat(m_NameHash);
				}
			}

			public void SetValue(Single value, Single dampTime, Single deltaTime)
			{
				RawValue = value;
				if (m_NameHash != 0)
				{
					m_Animator.SetFloat(m_NameHash, value, dampTime, deltaTime);
				}
			}

			public void Apply(Single dampTime, Single deltaTime)
			{
				if (m_NameHash != 0)
				{
					m_Animator.SetFloat(m_NameHash, RawValue, dampTime, deltaTime);
				}
			}
		}

		[Serializable]
		public class Vector3Property : Property<Vector3>
		{
			public Vector3Property()
			{
			}

			public Vector3Property(AnimatorEx controller, Animator animator, String propertyName) : base(controller, animator, propertyName)
			{
			}

			public override Vector3 Value
			{
				get => RawValue;
			    set
				{
					if (RawValue != value)
					{
						RawValue = value;
						SetValue(ref value);
					}
				}
			}

			public override void SetValue(ref Vector3 value)
			{
				if (m_NameHash != 0)
				{
					m_Animator.SetVector(m_NameHash, value);
				}
			}

			public override void GetValue(out Vector3 value)
			{
				value = RawValue;
				if (m_NameHash != 0)
				{
					value = m_Animator.GetVector(m_NameHash);
				}
			}
		}

		[Serializable]
		public class QuaternionProperty : Property<Quaternion>
		{
			public QuaternionProperty()
			{
			}

			public QuaternionProperty(AnimatorEx controller, Animator animator, String propertyName) : base(controller, animator, propertyName)
			{
			}

			public override Quaternion Value
			{
				get => RawValue;
			    set
				{
					if (RawValue != value)
					{
						RawValue = value;
						SetValue(ref value);
					}
				}
			}

			public override void SetValue(ref Quaternion value)
			{
				if (m_NameHash != 0)
				{
					m_Animator.SetQuaternion(m_NameHash, value);
				}
			}

			public override void GetValue(out Quaternion value)
			{
				value = RawValue;
				if (m_NameHash != 0)
				{
					value = m_Animator.GetQuaternion(m_NameHash);
				}
			}
		}
	}
}
