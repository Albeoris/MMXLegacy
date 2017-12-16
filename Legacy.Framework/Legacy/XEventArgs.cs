using System;

namespace Legacy
{
	public class XEventArgs<T> : EventArgs
	{
		public XEventArgs(T value)
		{
			Value = value;
		}

		public T Value { get; private set; }
	}
}
