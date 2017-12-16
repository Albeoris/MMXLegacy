using System;

namespace Legacy
{
	public class Singleton<T> where T : class, new()
	{
		public static readonly T Instance = Activator.CreateInstance<T>();
	}
}
