using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Dumper.Core
{
	public class CsvSerializer
	{
		private Dictionary<String, FieldInfo> mColumnToField = new Dictionary<String, FieldInfo>();

		private Type mObjectType;

		public CsvSerializer(Type type)
		{
			if (!type.IsArray)
			{
				throw new ArgumentException("Only array type supported", "type");
			}
			mObjectType = type.GetElementType();
			FindAllAttributes();
		}

	    public event EventHandler<ColumnValueResolveEventArg> ColumnValueResolve;

		public Array Deserialize(Stream stream)
		{
			List<String[]> list = CsvHelper.Parse(stream);
			CsvHelper.Cleanup(list);
			if (list.Count > 1)
			{
				Int32 i;
				for (i = 0; i < list.Count; i++)
				{
					if (list[i] != null && !CsvHelper.IsComment(list[i]))
					{
						break;
					}
				}
				return Deserialize(list[i], list, i + 1);
			}
			return null;
		}

	    public void Deserialize<T>(Stream stream, Dictionary<Int32, T> output)
	    {
	        List<String[]> list = CsvHelper.Parse(stream);
	        CsvHelper.Cleanup(list);
	        if (list.Count > 1)
	        {
	            Int32 i;
	            for (i = 0; i < list.Count; i++)
	            {
	                if (list[i] != null && !CsvHelper.IsComment(list[i]))
	                    break;
	            }
	            Deserialize<T>(list[i], list, i + 1, output);
	        }
        }

        public void Serialize(Stream stream, Array obj)
		{
			throw new NotImplementedException();
		}

		private void FindAllAttributes()
		{
			FieldInfo[] fields = mObjectType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				Object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(CsvColumnAttribute), false);
				if (customAttributes.Length > 0)
				{
					CsvColumnAttribute csvColumnAttribute = (CsvColumnAttribute)customAttributes[0];
					if (String.IsNullOrEmpty(csvColumnAttribute.Column))
					{
						throw new Exception(String.Format("{0}\nColumn name at field '{1}' is empty.", mObjectType.FullName, fieldInfo.Name));
					}
					if (mColumnToField.ContainsKey(csvColumnAttribute.Column))
					{
						throw new Exception(String.Format("{0}\nDouble Column name attribute '{1}.'", mObjectType.FullName, csvColumnAttribute.Column));
					}
					mColumnToField.Add(csvColumnAttribute.Column, fieldInfo);
				}
			}
			if (mColumnToField.Count == 0)
			{
				throw new Exception(String.Format("No CsvColumnAttribute definition in class '{0}.'", mObjectType.FullName));
			}
		}

		private void Deserialize<T>(String[] columns, List<String[]> values, Int32 valueOffset, Dictionary<Int32, T> output)
		{
			FieldInfo[] columns2 = FindColumnInfos(columns);
			for (Int32 i = valueOffset; i < values.Count; i++)
			{
			    String[] data = values[i];
                if (data != null && !CsvHelper.IsComment(data))
                {
                    Int32 staticId = Int32.Parse(data[0], CultureInfo.InvariantCulture);

                    if (!output.TryGetValue(staticId, out var obj))
                    {
                        obj = (T)Activator.CreateInstance(mObjectType);
                        output.Add(staticId, obj);
                    }

                    SetFieldValues(i + 1, obj, columns2, data);
				}
			}
		}

	    private Array Deserialize(String[] columns, List<String[]> values, Int32 valueOffset)
	    {
	        FieldInfo[] columns2 = FindColumnInfos(columns);
	        ArrayList arrayList = new ArrayList(values.Count - valueOffset);
	        for (Int32 i = valueOffset; i < values.Count; i++)
	        {
	            if (values[i] != null && !CsvHelper.IsComment(values[i]))
	            {
	                Object obj = Activator.CreateInstance(mObjectType);
	                SetFieldValues(i + 1, obj, columns2, values[i]);
	                arrayList.Add(obj);
	            }
	        }
	        Array array = Array.CreateInstance(mObjectType, arrayList.Count);
	        arrayList.CopyTo(array);
	        return array;
	    }

        private void SetFieldValues(Int32 lineNum, Object instance, FieldInfo[] columns, String[] columnValues)
		{
			for (Int32 i = 0; i < columns.Length; i++)
			{
				FieldInfo fieldInfo = columns[i];
				if (fieldInfo != null)
				{
					SetFieldValue(lineNum, instance, fieldInfo, columnValues[i]);
				}
			}
		}

		private void SetFieldValue(Int32 lineNum, Object instance, FieldInfo field, Object value)
		{
			try
			{
				Type fieldType = field.FieldType;
				TypeCode typeCode;
				if (fieldType.IsArray)
				{
					typeCode = Type.GetTypeCode(fieldType.GetElementType());
				}
				else
				{
					typeCode = Type.GetTypeCode(fieldType);
				}
				if (typeCode == TypeCode.Object && ColumnValueResolve != null)
				{
					ColumnValueResolveEventArg columnValueResolveEventArg = new ColumnValueResolveEventArg(field.FieldType, (String)value);
					ColumnValueResolve(this, columnValueResolveEventArg);
					value = columnValueResolveEventArg.Output;
				}
				else
				{
					Boolean flag = ConvertEnum(field, ref value);
					if (!(flag | ConvertArray(field, ref value)))
					{
						try
						{
							value = Convert.ChangeType(value, field.FieldType, CultureInfo.InvariantCulture);
						}
						catch (Exception innerException)
						{
							throw new InvalidDataException(String.Format("Convert error value '{0}' to {1} [{2}]", value, field.FieldType.Name, field.Name), innerException);
						}
					}
				}
			}
			catch (Exception innerException2)
			{
				throw new InvalidDataException("Error in file a line " + lineNum, innerException2);
			}
			field.SetValue(instance, value);
		}

		private FieldInfo[] FindColumnInfos(String[] columnNames)
		{
			FieldInfo[] array = new FieldInfo[columnNames.Length];
			for (Int32 i = 0; i < array.Length; i++)
			{
				mColumnToField.TryGetValue(columnNames[i], out array[i]);
			}
			return array;
		}

		private static void CastEnum(Type targetType, ref Object value)
		{
			switch (Type.GetTypeCode(value.GetType()))
			{
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
			{
				value = Enum.ToObject(targetType, value);
				Int32 num;
				if (!Int32.TryParse(value.ToString(), out num))
				{
					return;
				}
				break;
			}
			case TypeCode.String:
				try
				{
					value = Enum.Parse(targetType, (String)value, true);
				}
				catch (Exception innerException)
				{
					throw new InvalidCastException(String.Format("Fail cast Value='{0}' to Enum='{1}'", value, targetType), innerException);
				}
				return;
			}
			throw new InvalidCastException(String.Format("Fail cast Value='{0}' to Enum='{1}'", value, targetType));
		}

		private static Boolean ConvertEnum(FieldInfo targetField, ref Object value)
		{
			if (targetField.FieldType.IsEnum)
			{
				CastEnum(targetField.FieldType, ref value);
				return true;
			}
			return false;
		}

		private static Boolean ConvertArray(FieldInfo targetField, ref Object value)
		{
			Type fieldType = targetField.FieldType;
			Type elementType = fieldType.GetElementType();
			if (fieldType.IsArray && elementType.IsEnum)
			{
				String value2 = (String)value;
				String[] array = Util.Split(value2);
				Array array2 = Array.CreateInstance(elementType, array.Length);
				for (Int32 i = 0; i < array.Length; i++)
				{
					try
					{
						Object value3 = array[i];
						CastEnum(elementType, ref value3);
						array2.SetValue(value3, i);
					}
					catch (Exception innerException)
					{
						throw new Exception(String.Concat(new Object[]
						{
							"Cast error '",
							array[i],
							"' to ",
							elementType
						}), innerException);
					}
				}
				value = array2;
				return true;
			}
			if (fieldType.IsArray && elementType.IsPrimitive)
			{
				String text = (String)value;
				if (!Util.IsCsvNumbers(text))
				{
					throw new InvalidCastException("Csv cast to " + fieldType.FullName + "Only Numbers\nValue: " + text);
				}
				String[] array3 = Util.Split(text);
				if (elementType.IsUnsignedValueType())
				{
					foreach (String text2 in array3)
					{
						if (text2.StartsWith("-", true, CultureInfo.InvariantCulture))
						{
							throw new InvalidCastException("Parsing CSV\n Value '" + text2 + "' to " + fieldType.Name);
						}
					}
				}
				Array array5 = Array.CreateInstance(elementType, array3.Length);
				for (Int32 k = 0; k < array3.Length; k++)
				{
					try
					{
						array5.SetValue(Convert.ChangeType(array3[k], elementType, CultureInfo.InvariantCulture), k);
					}
					catch (Exception innerException2)
					{
						throw new Exception(String.Concat(new Object[]
						{
							"Cast error '",
							array3[k],
							"' to ",
							elementType
						}), innerException2);
					}
				}
				value = array5;
				return true;
			}
			else
			{
				if (fieldType.IsArray && elementType == typeof(String))
				{
					String value4 = (String)value;
					value = Util.Split(value4, StringSplitOptions.None);
					return true;
				}
				return false;
			}
		}
	}
}
