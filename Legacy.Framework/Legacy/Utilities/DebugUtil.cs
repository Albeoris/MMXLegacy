using System;
using System.Collections;
using System.Reflection;
using System.Text;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Utilities
{
	public static class DebugUtil
	{
		private static StringBuilder m_TextBuilder = new StringBuilder();

		public static String DumpObjectText(Object obj)
		{
			m_TextBuilder.Length = 0;
			m_TextBuilder.AppendLine("*******************************************");
			if (obj != null)
			{
				Type type = obj.GetType();
				m_TextBuilder.Append("* Dump - ");
				if (typeof(IList).IsAssignableFrom(type))
				{
					WriteList(0, (IList)obj);
				}
				else
				{
					m_TextBuilder.AppendLine(type.ToString());
					FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (fields.Length != 0)
					{
						Int32 num = MaxPadSize(fields);
						Int32 num2 = MaxPadSize(fields);
						m_TextBuilder.AppendLine("### Fields ###");
						try
						{
							foreach (FieldInfo fieldInfo in fields)
							{
								m_TextBuilder.Append("* ");
								m_TextBuilder.Append(fieldInfo.FieldType.Name.PadRight(num));
								m_TextBuilder.Append(" | ");
								m_TextBuilder.Append(fieldInfo.Name.PadRight(num2));
								m_TextBuilder.Append(" = ");
								Object value = fieldInfo.GetValue(obj);
								if (value != null)
								{
									if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
									{
										WriteList(6 + num + num2, (IList)value);
									}
									else
									{
										m_TextBuilder.AppendLine(value.ToString());
									}
								}
								else
								{
									m_TextBuilder.AppendLine("NULL");
								}
							}
						}
						catch (Exception ex)
						{
							m_TextBuilder.Append("\n* Exception - ");
							m_TextBuilder.AppendLine(ex.Message);
						}
					}
					PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (properties.Length != 0)
					{
						Int32 num = MaxPadSize(properties);
						Int32 num2 = MaxPadSize(properties);
						m_TextBuilder.AppendLine("### Properties ###");
						try
						{
							foreach (PropertyInfo propertyInfo in properties)
							{
								if (propertyInfo.CanRead)
								{
									m_TextBuilder.Append("* ");
									m_TextBuilder.Append(propertyInfo.PropertyType.Name.PadRight(num));
									m_TextBuilder.Append(" | ");
									m_TextBuilder.Append(propertyInfo.Name.PadRight(num2));
									m_TextBuilder.Append(" = ");
									try
									{
										Object value2 = propertyInfo.GetValue(obj, null);
										if (value2 != null)
										{
											if (typeof(IList).IsAssignableFrom(propertyInfo.PropertyType))
											{
												WriteList(6 + num + num2, (IList)value2);
											}
											else
											{
												m_TextBuilder.AppendLine(value2.ToString());
											}
										}
										else
										{
											m_TextBuilder.AppendLine("NULL");
										}
									}
									catch (Exception ex2)
									{
										m_TextBuilder.Append("Exception: ");
										m_TextBuilder.AppendLine(ex2.Message);
									}
								}
							}
						}
						catch (Exception ex3)
						{
							m_TextBuilder.Append("\n* Exception - ");
							m_TextBuilder.AppendLine(ex3.Message);
						}
					}
				}
			}
			else
			{
				m_TextBuilder.AppendLine("* Dump - Object is NULL");
			}
			m_TextBuilder.AppendLine("*******************************************");
			return m_TextBuilder.ToString();
		}

		public static void LogObject(Object obj)
		{
			Debug.Log(DumpObjectText(obj));
		}

		private static void WriteList(Int32 paddingLeft, IList list)
		{
			m_TextBuilder.Append("Element Length = ").Append(list.Count).AppendLine();
			if (list.Count != 0)
			{
				for (Int32 i = 0; i < list.Count; i++)
				{
					Object obj = list[i];
					m_TextBuilder.Append(' ', paddingLeft);
					m_TextBuilder.Append(i);
					m_TextBuilder.Append(". ");
					m_TextBuilder.AppendLine((obj == null) ? "NULL" : obj.ToString());
				}
			}
		}

		private static Int32 MaxPadSize(MemberInfo[] array)
		{
			Int32 num = 9;
			if (array == null)
			{
				return num;
			}
			for (Int32 i = 0; i < array.Length; i++)
			{
				num = Math.Max(num, array[i].Name.Length);
			}
			return num;
		}

		private static Int32 MaxPadSize(FieldInfo[] array)
		{
			Int32 num = 9;
			if (array == null)
			{
				return num;
			}
			for (Int32 i = 0; i < array.Length; i++)
			{
				num = Math.Max(num, array[i].FieldType.Name.Length);
			}
			return num;
		}

		private static Int32 MaxPadSize(PropertyInfo[] array)
		{
			Int32 num = 9;
			if (array == null)
			{
				return num;
			}
			for (Int32 i = 0; i < array.Length; i++)
			{
				num = Math.Max(num, array[i].PropertyType.Name.Length);
			}
			return num;
		}
	}
}
