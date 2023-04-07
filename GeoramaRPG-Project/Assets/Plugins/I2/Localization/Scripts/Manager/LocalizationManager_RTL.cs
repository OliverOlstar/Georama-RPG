using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;
using System.Collections;
using RTLTMPro;

namespace I2.Loc
{
    public static partial class LocalizationManager
    {
        static string[] LanguagesRTL = {"ar-DZ", "ar","ar-BH","ar-EG","ar-IQ","ar-JO","ar-KW","ar-LB","ar-LY","ar-MA","ar-OM","ar-QA","ar-SA","ar-SY","ar-TN","ar-AE","ar-YE",
                                        "fa", "he","ur","ji"};

        private static FastStringBuilder m_StringBuilder;

        public static string ApplyRTLfixByRTLTMP(string line, bool ignoreNumbers = true)
        {
            m_StringBuilder ??= new FastStringBuilder(1000);
            m_StringBuilder.Clear();

            RTLSupport.FixRTL(line, m_StringBuilder, preserveNumbers: ignoreNumbers);
            return m_StringBuilder.ToString();
        }
        
        public static string ApplyRTLfix(string line) { return ApplyRTLfix(line, 0, true); }
        public static string ApplyRTLfix(string line, int maxCharacters, bool ignoreNumbers)
        {
            if (string.IsNullOrEmpty(line))
                return line;

            // Fix !, ? and . signs not set correctly
            char firstC = line[0];
            if (firstC == '!' || firstC == '.' || firstC == '?')
                line = line.Substring(1) + firstC;

            int tagStart = -1, tagEnd = 0;

            // Find all Tags (and Numbers if ignoreNumbers is true)
            int tagBase = 40000;
            tagEnd = 0;
            var tags = new List<string>();
            while (I2Utils.FindNextTag(line, tagEnd, out tagStart, out tagEnd))
            {
                string tag = "@@" + (char)(tagBase + tags.Count) + "@@";
                tags.Add(line.Substring(tagStart, tagEnd - tagStart + 1));

                line = line.Substring(0, tagStart) + tag + line.Substring(tagEnd + 1);
                tagEnd = tagStart + 5;
            }

            // Split into lines and fix each line
            line = line.Replace("\r\n", "\n");
            line = I2Utils.SplitLine(line, maxCharacters);
            line = RTLFixer.Fix(line, true, !ignoreNumbers);


            // Restore all tags
  
            for (int i = 0; i < tags.Count; i++)
            {
                var len = line.Length;
  
                for (int j = 0; j < len; ++j)
                {
                    if (line[j] == '@' && line[j + 1] == '@' && line[j + 2] >= tagBase && line[j + 3] == '@' && line[j + 4] == '@')
                    {
                        int idx = line[j + 2] - tagBase;
                        if (idx % 2 == 0) idx++;
                        else idx--;
                        if (idx >= tags.Count) idx = tags.Count - 1;

                        line = line.Substring(0, j) + tags[idx] + line.Substring(j + 5);

                        break;
                    }
                }
            }

            return line;
        }

       
        public static string FixRTL_IfNeeded(string text, int maxCharacters = 0, bool ignoreNumber=false)
		{
			if (IsRight2Left)
				return ApplyRTLfixByRTLTMP(text, ignoreNumber);
			else
				return text;
		}

		public static bool IsRTL(string Code)
		{
			return System.Array.IndexOf(LanguagesRTL, Code)>=0;
		}
    }

}
