using System;
using System.Collections.Generic;
using System.IO;

namespace Dumper.Core
{
	public class CsvFile
	{
		private List<String[]> m_Values;

		private CsvFile()
            : this((Stream)null)
		{
		}

		public CsvFile(String fileName)
            : this(File.OpenRead(fileName))
		{
		}

		public CsvFile(Stream stream)
		{
			try
			{
				if (stream != null)
				{
					m_Values = CsvHelper.Parse(stream);
					CsvHelper.ErrorCheck(m_Values);
					if (m_Values.Count > 0)
					{
						Columns = m_Values[0];
						m_Values.RemoveAt(0);
						IsNull = false;
					}
					else
					{
						Columns = new String[0];
						IsNull = true;
					}
				}
				else
				{
					Columns = new String[0];
					m_Values = new List<String[]>();
					IsNull = true;
				}
				Values = m_Values.AsReadOnly();
			}
			finally
			{
				if (stream != null)
				{
					((IDisposable)stream).Dispose();
				}
			}
		}

		public Boolean IsNull { get; private set; }

		public String[] Columns { get; private set; }

		public IList<String[]> Values { get; private set; }

		public void Cleanup()
		{
			if (!IsNull)
			{
				CsvHelper.Cleanup(m_Values);
			}
		}

		public static CsvFile Merge(CsvFile fileA, CsvFile fileB)
		{
			if (fileA.Columns.Length < fileB.Columns.Length)
			{
				CsvFile csvFile = fileB;
				fileB = fileA;
				fileA = csvFile;
			}
			List<String[]> list = new List<String[]>(fileA.m_Values);
			List<String[]> list2 = new List<String[]>(fileB.m_Values);
			CsvHelper.Cleanup(list);
			CsvHelper.Cleanup(list2);
			List<String> list3 = new List<String>(fileA.Columns.Length + fileB.Columns.Length);
			list3.AddRange(fileA.Columns);
			list3.AddRange(fileB.Columns);
			CsvFile csvFile2 = new CsvFile();
			csvFile2.IsNull = false;
			csvFile2.Columns = list3.ToArray();
			list3.Clear();
			for (Int32 i = 0; i < list.Count; i++)
			{
				list3.AddRange(list[i]);
				if (i < list2.Count)
				{
					list3.AddRange(list2[i]);
				}
				else
				{
					Int32 num = fileB.Columns.Length;
					while (num-- >= 0)
					{
						list3.Add(String.Empty);
					}
				}
				csvFile2.m_Values.Add(list3.ToArray());
				list3.Clear();
			}
			return csvFile2;
		}
	}
}
