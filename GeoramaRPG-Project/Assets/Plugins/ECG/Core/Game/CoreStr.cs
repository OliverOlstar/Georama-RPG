
using UnityEngine;
using System.Runtime.CompilerServices;
using System.Text;

namespace Core
{
	public static class Str
	{
		public static readonly string EMPTY = "";

		public static bool Equals(string s1, string s2)
		{
			if (object.ReferenceEquals(s1, null) || object.ReferenceEquals(s2, null))
			{
				return object.ReferenceEquals(s1, null) && object.ReferenceEquals(s2, null);
			}
			int s1Len = s1.Length;
			if (s1Len != s2.Length)
			{
				return false;
			}
			for (int i = 0; i < s1Len; i++)
			{
				if (s1[i] != s2[i])
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsEmpty(string s)
		{
			return object.ReferenceEquals(s, null) || s.Length == 0;
		}

		public static bool Contains(string s1, string s2)
		{
			if (object.ReferenceEquals(s1, null) || object.ReferenceEquals(s2, null))
			{
				return false;
			}
			int s2Len = s2.Length;
			int s1Len = s1.Length - s2Len + 1;
			for (int i = 0; i < s1Len; ++i)
			{
				for (int j = 0; j < s2Len; ++j)
				{
					if (s1[i + j] != s2[j])
					{
						break;
					}
					if (j == s2Len - 1)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool Contains(string s, char c)
		{
			if (object.ReferenceEquals(s, null))
			{
				return false;
			}
			int length = s.Length;
			for (int i = 0; i < length; ++i)
			{
				if (s[i] == c)
				{
					return true;
				}
			}
			return false;
		}

		public static int FirstIndexOf(string s, char c)
		{
			if (object.ReferenceEquals(s, null))
			{
				return -1;
			}
			int length = s.Length;
			for (int i = 0; i < length; ++i)
			{
				if (s[i] == c)
				{
					return i;
				}
			}
			return -1;
		}

		public static int LastIndexOf(string s, char c)
		{
			if (object.ReferenceEquals(s, null))
			{
				return -1;
			}
			int length = s.Length;
			for (int i = length-1; i >= 0; --i)
			{
				if (s[i] == c)
				{
					return i;
				}
			}
			return -1;
		}

		public static bool StartsWith(string s1, string s2)
		{
			if (object.ReferenceEquals(s1, null) || object.ReferenceEquals(s2, null))
			{
				return false;
			}
			int s1Len = s1.Length;
			int s2Len = s2.Length;
			if (s1Len < s2Len)
			{
				return false;
			}
			for (int i = 0; i < s2Len; ++i)
			{
				if (s1[i] != s2[i])
				{
					return false;
				}
			}
			return true;
		}

		public static bool EndsWith(string s1, string s2)
		{
			if (object.ReferenceEquals(s1, null) || object.ReferenceEquals(s2, null))
			{
				return false;
			}
			int s1Len = s1.Length;
			int s2Len = s2.Length;
			if (s1Len < s2Len)
			{
				return false;
			}
			for (int i = 1; i <= s2Len; ++i)
			{
				if (s1[s1Len - i] != s2[s2Len - i])
				{
					return false;
				}
			}
			return true;
		}

		public static bool StartsWith(string s1, char c)
		{
			if (object.ReferenceEquals(s1, null) || s1.Length == 0)
			{
				return false;
			}
			return s1[0] == c;
		}

		public static bool EndsWith(string s1, char c)
		{
			if (object.ReferenceEquals(s1, null) || s1.Length == 0)
			{
				return false;
			}
			return s1[s1.Length - 1] == c;
		}

		private static StringBuilder s_BuildBuilder = new StringBuilder();
		public static string Build(string s1, string s2)
		{
			s_BuildBuilder.Clear();
			return s_BuildBuilder.Append(s1).Append(s2).ToString();
		}

		public static string Build(string s1, string s2, string s3)
		{
			s_BuildBuilder.Clear();
			return s_BuildBuilder.Append(s1).Append(s2).Append(s3).ToString();
		}

		public static string Build(string s1, string s2, string s3, string s4)
		{
			s_BuildBuilder.Clear();
			return s_BuildBuilder.Append(s1).Append(s2).Append(s3).Append(s4).ToString();
		}

		public static string Build(string s1, string s2, string s3, string s4, string s5)
		{
			s_BuildBuilder.Clear();
			return s_BuildBuilder.Append(s1).Append(s2).Append(s3).Append(s4).Append(s5).ToString();
		}

		public static string Build(params string[] list)
		{
			s_BuildBuilder.Clear();
			for (int i = 0; i < list.Length; i++)
			{
				s_BuildBuilder.Append(list[i]);
			}
			return s_BuildBuilder.ToString();
		}

		public static string BuildWithBetweens(string between, params string[] list)
		{
			s_BuildBuilder.Clear();
			s_BuildBuilder.Append(list[0]);
			for (int i = 1; i < list.Length; i++)
			{
				s_BuildBuilder.Append(between);
				s_BuildBuilder.Append(list[i]);
			}
			return s_BuildBuilder.ToString();
		}

		static StringBuilder s_AddBuilder = new StringBuilder();
		public static void Add(string s1) =>
			s_AddBuilder.Append(s1);
		public static void Add(string s1, string s2) =>
			s_AddBuilder.Append(s1).Append(s2);
		public static void Add(string s1, string s2, string s3) =>
			s_AddBuilder.Append(s1).Append(s2).Append(s3);
		public static void Add(string s1, string s2, string s3, string s4) => 
			s_AddBuilder.Append(s1).Append(s2).Append(s3).Append(s4);
		public static void Add(string s1, string s2, string s3, string s4, string s5) => 
			s_AddBuilder.Append(s1).Append(s2).Append(s3).Append(s4).Append(s5);
		public static void Add(string s1, string s2, string s3, string s4, string s5, string s6) =>
			s_AddBuilder.Append(s1).Append(s2).Append(s3).Append(s4).Append(s5).Append(s6);
		public static void Add(string s1, string s2, string s3, string s4, string s5, string s6, string s7) =>
			s_AddBuilder.Append(s1).Append(s2).Append(s3).Append(s4).Append(s5).Append(s6).Append(s7);
		public static void Add(string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8) => 
			s_AddBuilder.Append(s1).Append(s2).Append(s3).Append(s4).Append(s5).Append(s6).Append(s7).Append(s8);
		public static void Add(params string[] list)
		{
			int length = list.Length;
			for (int i = 0; i < length; i++)
			{
				s_AddBuilder.Append(list[i]);
			}
		}
		public static void AddLine() => s_AddBuilder.Append("\n");
		public static void AddLine(string s1) =>
			s_AddBuilder.Append(s1).Append("\n");
		public static void AddLine(string s1, string s2) =>
			s_AddBuilder.Append(s1).Append(s2).Append("\n");
		public static void AddLine(string s1, string s2, string s3) =>
			s_AddBuilder.Append(s1).Append(s2).Append(s3).Append("\n");
		public static void AddLine(string s1, string s2, string s3, string s4) =>
			s_AddBuilder.Append(s1).Append(s2).Append(s3).Append(s4).Append("\n");
		public static void AddLine(string s1, string s2, string s3, string s4, string s5) =>
			s_AddBuilder.Append(s1).Append(s2).Append(s3).Append(s4).Append(s5).Append("\n");
		public static void AddLine(string s1, string s2, string s3, string s4, string s5, string s6) =>
			s_AddBuilder.Append(s1).Append(s2).Append(s3).Append(s4).Append(s5).Append(s6).Append("\n");
		public static void AddLine(string s1, string s2, string s3, string s4, string s5, string s6, string s7) =>
			s_AddBuilder.Append(s1).Append(s2).Append(s3).Append(s4).Append(s5).Append(s6).Append(s7).Append("\n");
		public static void AddLine(string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8) =>
			s_AddBuilder.Append(s1).Append(s2).Append(s3).Append(s4).Append(s5).Append(s6).Append(s7).Append(s8).Append("\n");
		public static void AddLine(params string[] list)
		{
			Add(list);
			s_AddBuilder.Append("\n");
		}

		public static void AddNewLine() => s_AddBuilder.Append("\n");
		public static void AddNewLine(string s1) =>
			s_AddBuilder.Append("\n").Append(s1);
		public static void AddNewLine(string s1, string s2) =>
			s_AddBuilder.Append("\n").Append(s1).Append(s2);
		public static void AddNewLine(string s1, string s2, string s3) =>
			s_AddBuilder.Append("\n").Append(s1).Append(s2).Append(s3);
		public static void AddNewLine(string s1, string s2, string s3, string s4) =>
			s_AddBuilder.Append("\n").Append(s1).Append(s2).Append(s3).Append(s4);
		public static void AddNewLine(string s1, string s2, string s3, string s4, string s5) =>
			s_AddBuilder.Append("\n").Append(s1).Append(s2).Append(s3).Append(s4).Append(s5);
		public static void AddNewLine(string s1, string s2, string s3, string s4, string s5, string s6) =>
			s_AddBuilder.Append("\n").Append(s1).Append(s2).Append(s3).Append(s4).Append(s5).Append(s6);
		public static void AddNewLine(string s1, string s2, string s3, string s4, string s5, string s6, string s7) =>
			s_AddBuilder.Append("\n").Append(s1).Append(s2).Append(s3).Append(s4).Append(s5).Append(s6).Append(s7);
		public static void AddNewLine(string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8) =>
			s_AddBuilder.Append("\n").Append(s1).Append(s2).Append(s3).Append(s4).Append(s5).Append(s6).Append(s7).Append(s8);
		public static void AddNewLine(params string[] list)
		{
			s_AddBuilder.Append("\n");
			Add(list);
		}

		public static string Finish()
		{
			string finish = Core.Str.EMPTY;
			if (s_AddBuilder != null)
			{
				finish = s_AddBuilder.ToString();
				s_AddBuilder.Clear();
			}
			return finish;
		}

        public static void Flush()
        {
            if (s_AddBuilder != null)
			{
				s_AddBuilder.Clear();
			}
        }

		public static bool IsEmpty()
		{
			return s_AddBuilder != null;
		}
	}
}
