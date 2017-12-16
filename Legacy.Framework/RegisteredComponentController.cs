using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public static class RegisteredComponentController
{
	private static Dictionary<Type, InstanceContainer> _instanceContainers = new Dictionary<Type, InstanceContainer>();

	public static T[] GetAllOfType<T>() where T : IRegisteredComponent
	{
		InstanceContainer instanceContainer;
		if (!_instanceContainers.TryGetValue(typeof(T), out instanceContainer))
		{
			return new T[0];
		}
		T[] array = new T[instanceContainer.Count];
		Int32 num = 0;
		foreach (IRegisteredComponent registeredComponent in instanceContainer)
		{
			array[num++] = (T)registeredComponent;
		}
		return array;
	}

	public static Object[] GetAllOfType(Type type)
	{
		InstanceContainer instanceContainer;
		if (!_instanceContainers.TryGetValue(type, out instanceContainer))
		{
			return new Object[0];
		}
		Object[] array = new Object[instanceContainer.Count];
		Int32 num = 0;
		foreach (IRegisteredComponent registeredComponent in instanceContainer)
		{
			array[num++] = registeredComponent;
		}
		return array;
	}

	public static HashSet<IRegisteredComponent> GetCollectionOfType(Type type)
	{
		InstanceContainer result;
		if (!_instanceContainers.TryGetValue(type, out result))
		{
			return null;
		}
		return result;
	}

	public static Int32 InstanceCountOfType<T>() where T : IRegisteredComponent
	{
		InstanceContainer instanceContainer;
		if (!_instanceContainers.TryGetValue(typeof(T), out instanceContainer))
		{
			return 0;
		}
		return instanceContainer.Count;
	}

	private static InstanceContainer _GetInstanceContainer(Type type)
	{
		InstanceContainer instanceContainer;
		if (_instanceContainers.TryGetValue(type, out instanceContainer))
		{
			return instanceContainer;
		}
		instanceContainer = new InstanceContainer();
		_instanceContainers.Add(type, instanceContainer);
		return instanceContainer;
	}

	private static void _RegisterType(IRegisteredComponent component, Type type)
	{
		InstanceContainer instanceContainer = _GetInstanceContainer(type);
		if (!instanceContainer.Add(component))
		{
			Debug.LogError("RegisteredComponentController error: Tried to register same instance twice");
		}
	}

	internal static void _Register(IRegisteredComponent component)
	{
		Type type = component.GetType();
		do
		{
			_RegisterType(component, type);
			type = type.BaseType;
		}
		while (type != component.GetRegisteredComponentBaseClassType());
	}

	internal static void _UnregisterType(IRegisteredComponent component, Type type)
	{
		InstanceContainer instanceContainer = _GetInstanceContainer(type);
		if (!instanceContainer.Remove(component))
		{
			Debug.LogError("RegisteredComponentController error: Tried to unregister unknown instance");
		}
	}

	internal static void _Unregister(IRegisteredComponent component)
	{
		Type type = component.GetType();
		do
		{
			_UnregisterType(component, type);
			type = type.BaseType;
		}
		while (type != component.GetRegisteredComponentBaseClassType());
	}

	public class InstanceContainer : HashSet_Flash<IRegisteredComponent>
	{
	}
}
