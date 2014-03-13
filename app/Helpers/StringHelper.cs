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
            var regex = new Regex(Regex.Escape(replaceable));
            var newTerm = regex.Replace(term, substitute, thisOften);
            return newTerm;
        }

    }
}
