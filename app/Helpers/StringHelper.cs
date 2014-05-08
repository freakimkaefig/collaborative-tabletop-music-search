using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Helpers
{
    public class StringHelper
    {
        public static String lowerToUpper(String term)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string str in term.Split(' '))
            {
                if (str.Length > 0)
                {
                    sb.Append(str.Substring(0, 1).ToUpper() + str.Substring(1) + " ");
                }
            }
            return sb.ToString();
        }

        public static String replacePartialString(String term, String replaceable, String substitute, int thisOften)
        {
            var newTerm = "";
            var regex = new Regex(Regex.Escape(replaceable));
            if(term != null && substitute != null && thisOften >= 0)
            {
                newTerm = regex.Replace(term, substitute, thisOften);
            }
            
            if (newTerm != null && newTerm != "")
            {
                return newTerm;
            }
            else
            {
                return term;
            }
            
        }

        public static String cleanText(String text)
        {
            String temp = text;
            temp = StringHelper.replacePartialString(temp, "&#34;", "\"", 10000);
            temp = StringHelper.replacePartialString(temp, "&quot;", "\"", 10000);
            temp = StringHelper.replacePartialString(temp, "&#38;", "&", 10000);
            temp = StringHelper.replacePartialString(temp, "&#39;", "'", 10000);
            temp = StringHelper.replacePartialString(temp, "\t", "", 100000);
            temp = StringHelper.replacePartialString(temp, "\n", "", 10000);
            return temp;
        }

    }
}
