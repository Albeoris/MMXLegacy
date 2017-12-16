using System;
using System.Collections.Generic;

public class BetterList<T>
{
	public T[] buffer;

	public Int32 size;

	public IEnumerator<T> GetEnumerator()
	{
		if (buffer != null)
		{
			Int32 i = 0;
			while (i < buffer.Length && i < size)
			{
				yield return buffer[i];
				i++;
			}
		}
		yield break;
	}

	public T this[Int32 i]
	{
		get => buffer[i];
	    set => buffer[i] = value;
	}

	public Int32 capacity
	{
		get => (buffer != null) ? buffer.Length : 0;
	    set
		{
			if (value < size)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (buffer == null || value > buffer.Length)
			{
				AllocateMore(value);
			}
		}
	}

	public void Reserve(Int32 size)
	{
		size += this.size;
		if (size > capacity || buffer == null)
		{
			capacity = size;
		}
	}

	private void AllocateMore(Int32 length)
	{
		T[] destinationArray = new T[length];
		if (buffer != null && size > 0)
		{
			Array.Copy(buffer, destinationArray, size);
		}
		buffer = destinationArray;
	}

	public void Trim()
	{
		if (size > 0 && buffer != null && size < buffer.Length)
		{
			AllocateMore(size);
		}
		else
		{
			buffer = null;
		}
	}

	public void Clear()
	{
		size = 0;
	}

	public void Release()
	{
		size = 0;
		buffer = null;
	}

	public void Add(T item)
	{
		if (buffer == null || size == buffer.Length)
		{
			AllocateMore((buffer != null) ? (buffer.Length << 1) : 32);
		}
		buffer[size++] = item;
	}

	public void AddRanged(T[] items, Int32 index, Int32 length)
	{
		if (buffer == null || size + length > buffer.Length)
		{
			AllocateMore(size + length);
		}
		Array.Copy(items, index, buffer, size, length);
		size += length;
	}

	public void Insert(Int32 index, T item)
	{
		if (buffer == null || size == buffer.Length)
		{
			AllocateMore((buffer != null) ? (buffer.Length << 1) : 32);
		}
		if (index < size)
		{
			Array.Copy(buffer, index - 1, buffer, index, size - index);
			buffer[index] = item;
			size++;
		}
		else
		{
			Add(item);
		}
	}

	public Boolean Contains(T item)
	{
		if (buffer == null)
		{
			return false;
		}
		EqualityComparer<T> @default = EqualityComparer<T>.Default;
		Int32 num = 0;
		while (num < buffer.Length && num < size)
		{
			if (@default.Equals(buffer[num], item))
			{
				return true;
			}
			num++;
		}
		return false;
	}

	public Boolean Remove(T item)
	{
		if (buffer != null)
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			Int32 num = 0;
			while (num < buffer.Length && num < size)
			{
				if (@default.Equals(buffer[num], item))
				{
					size--;
					buffer[num] = default(T);
					Array.Copy(buffer, num + 1, buffer, num, size - num);
					return true;
				}
				num++;
			}
		}
		return false;
	}

	public void RemoveAt(Int32 index)
	{
		if (buffer != null && index < size)
		{
			size--;
			buffer[index] = default(T);
			Array.Copy(buffer, index + 1, buffer, index, size - index);
		}
	}

	public T Pop()
	{
		if (buffer != null && size != 0)
		{
			T result = buffer[--size];
			buffer[size] = default(T);
			return result;
		}
		return default(T);
	}

	public T[] ToArray()
	{
		if (size < 0 || buffer == null)
		{
			return null;
		}
		if (buffer.Length == size)
		{
			return buffer;
		}
		T[] destinationArray = new T[size];
		Array.Copy(buffer, destinationArray, size);
		return buffer = destinationArray;
	}

	public void Sort(Comparison<T> comparer)
	{
		if (buffer == null)
		{
			return;
		}
		Boolean flag = true;
		while (flag)
		{
			flag = false;
			Int32 num = 1;
			while (num < buffer.Length && num < size)
			{
				if (comparer(buffer[num - 1], buffer[num]) > 0)
				{
					T t = buffer[num];
					buffer[num] = buffer[num - 1];
					buffer[num - 1] = t;
					flag = true;
				}
				num++;
			}
		}
	}
}
