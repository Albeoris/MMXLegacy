using System;

namespace Legacy
{
	public class SingletonLazy<T> where T : class, new()
	{
		private static T s_Instance;

		public static T Instance
		{
			get
			{
				T result;
				if ((result = s_Instance) == null)
				{
					result = (s_Instance = Activator.CreateInstance<T>());
				}
				return result;
			}
		}
	}
}
