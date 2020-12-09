using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace iot.solution.data
{
    public static class StringUtils
    {
        #region Basic String Tasks
        public static string TrimStart(string text, string textToTrim, bool caseInsensitive)
        {            
            while (true)
            {
                string match = text.Substring(0, textToTrim.Length);

                if (match == textToTrim ||
                    (caseInsensitive && match.ToLower() == textToTrim.ToLower()))
                {
                    if (text.Length <= match.Length)
                        text = "";
                    else
                        text = text.Substring(textToTrim.Length);
                }
                else
                    break;
            }
            return text;
        }
        [Obsolete("Please use the StringUtils.Truncate() method instead.")]
        public static string TrimTo(string value, int charCount)
        {
            if (value == null)
                return value;

            if (value.Length > charCount)
                return value.Substring(0, charCount);

            return value;
        }
        public static string Replicate(string input, int charCount)
        {
            return new StringBuilder().Insert(0, input, charCount).ToString();
        }
        public static string Replicate(char character, int charCount)
        {
            return new StringBuilder().Insert(0, character.ToString(), charCount).ToString();
        }
        public static int IndexOfNth(this string source, string matchString, int stringInstance, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            if (string.IsNullOrEmpty(source))
                return -1;

            int lastPos = 0;
            int count = 0;
           
            while (count < stringInstance )
            {
                var len = source.Length - lastPos;
                lastPos = source.IndexOf(matchString, lastPos,len,stringComparison);
                if (lastPos == -1)
                    break;

                count++;
                if (count == stringInstance)
                    return lastPos;

                lastPos += matchString.Length;
            }
            return -1;
        }
        public static int IndexOfNth(this string source, char matchChar, int charInstance)        
        {
            if (string.IsNullOrEmpty(source))
                return -1;

            if (charInstance < 1)
                return -1;

            int count = 0;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] == matchChar)
                {
                    count++;
                    if (count == charInstance)                 
                        return i;                 
                }
            }
            return -1;
        }
        public static int LastIndexOfNth(this string source, string matchString, int charInstance, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            if (string.IsNullOrEmpty(source))
                return -1;

            int lastPos = source.Length;
            int count = 0;

            while (count < charInstance)
            {                
                lastPos = source.LastIndexOf(matchString, lastPos, lastPos, stringComparison);
                if (lastPos == -1)
                    break;

                count++;
                if (count == charInstance)
                    return lastPos;                
            }
            return -1;
        }
        public static int LastIndexOfNth(this string source, char matchChar, int charInstance)
        {
            if (string.IsNullOrEmpty(source))
                return -1;

            int count = 0;
            for (int i = source.Length-1 ; i > -1; i--)
            {
                if (source[i] == matchChar)
                {
                    count++;
                    if (count == charInstance)
                        return i;
                }
            }
            return -1;
        }
        #endregion

        #region String Casing
        public static string ProperCase(string Input)
        {
            if (Input == null)
                return null;
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(Input);
        }
        public static string ToCamelCase(string phrase)
        {
            if (phrase == null)
                return string.Empty;

            StringBuilder sb = new StringBuilder(phrase.Length);

            // First letter is always upper case
            bool nextUpper = true;

            foreach (char ch in phrase)
            {
                if (char.IsWhiteSpace(ch) || char.IsPunctuation(ch) || char.IsSeparator(ch) || ch > 32 && ch < 48)
                {
                    nextUpper = true;
                    continue;
                }
                if (char.IsDigit(ch))
                {
                    sb.Append(ch);
                    nextUpper = true;
                    continue;
                }                       

                if (nextUpper)
                    sb.Append(char.ToUpper(ch));
                else
                    sb.Append(char.ToLower(ch));

                nextUpper = false;
            }

            return sb.ToString();
        }
        public static string FromCamelCase(string camelCase)
        {
            if (string.IsNullOrEmpty(camelCase))
                return camelCase;

            StringBuilder sb = new StringBuilder(camelCase.Length + 10);
            bool first = true;
            char lastChar = '\0';

            foreach (char ch in camelCase)
            {
                if (!first &&
                    (char.IsUpper(ch) ||
                     char.IsDigit(ch) && !char.IsDigit(lastChar)))
                    sb.Append(' ');

                sb.Append(ch);
                first = false;
                lastChar = ch;
            }

            return sb.ToString(); ;
        }
        #endregion

        #region String Manipulation
        public static string ExtractString(this string source,
            string beginDelim,
            string endDelim,
            bool caseSensitive = false,
            bool allowMissingEndDelimiter = false,
            bool returnDelimiters = false)
        {
            int at1, at2;

            if (string.IsNullOrEmpty(source))
                return string.Empty;

            if (caseSensitive)
            {
                at1 = source.IndexOf(beginDelim,StringComparison.CurrentCulture);
                if (at1 == -1)
                    return string.Empty;

                at2 = source.IndexOf(endDelim, at1 + beginDelim.Length,StringComparison.CurrentCulture);
            }
            else
            {
                //string Lower = source.ToLower();
                at1 = source.IndexOf(beginDelim, 0, source.Length, StringComparison.OrdinalIgnoreCase);
                if (at1 == -1)
                    return string.Empty;

                at2 = source.IndexOf(endDelim, at1 + beginDelim.Length, StringComparison.OrdinalIgnoreCase);
            }

            if (allowMissingEndDelimiter && at2 < 0)
            {
                if (!returnDelimiters)
                    return source.Substring(at1 + beginDelim.Length);

                return source.Substring(at1);
            }

            if (at1 > -1 && at2 > 1)
            {
                if (!returnDelimiters)
                    return source.Substring(at1 + beginDelim.Length, at2 - at1 - beginDelim.Length);

                return source.Substring(at1, at2 - at1 + endDelim.Length);
            }

            return string.Empty;
        }
        public static string ReplaceStringInstance(string origString, string findString,
            string replaceWith, int instance, bool caseInsensitive)
        {
            if (instance == -1)
                return ReplaceString(origString, findString, replaceWith, caseInsensitive);

            int at1 = 0;
            for (int x = 0; x < instance; x++)
            {
                if (caseInsensitive)
                    at1 = origString.IndexOf(findString, at1, origString.Length - at1, StringComparison.OrdinalIgnoreCase);
                else
                    at1 = origString.IndexOf(findString, at1);

                if (at1 == -1)
                    return origString;

                if (x < instance - 1)
                    at1 += findString.Length;
            }

            return origString.Substring(0, at1) + replaceWith + origString.Substring(at1 + findString.Length);
        }
        public static string ReplaceString(string origString, string findString,
            string replaceString, bool caseInsensitive)
        {
            int at1 = 0;
            while (true)
            {
                if (caseInsensitive)
                    at1 = origString.IndexOf(findString, at1, origString.Length - at1, StringComparison.OrdinalIgnoreCase);
                else
                    at1 = origString.IndexOf(findString, at1);

                if (at1 == -1)
                    break;

                origString = origString.Substring(0, at1) + replaceString + origString.Substring(at1 + findString.Length);

                at1 += replaceString.Length;
            }

            return origString;
        }
        public static string Truncate(this string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return text.Length <= maxLength ? text : text.Substring(0, maxLength);
        }
        public static string TextAbstract(string text, int length)
        {
            if (text == null)
                return string.Empty;

            if (text.Length <= length)
                return text;

            text = text.Substring(0, length);

            text = text.Substring(0, text.LastIndexOf(" "));
            return text + "...";
        }
        public static string TerminateString(string value, string terminator)
        {
            if (string.IsNullOrEmpty(value))
                return terminator;
                    
            if(value.EndsWith(terminator))
                return value;

            return value + terminator;
        }
        public static string Right(string full, int rightCharCount)
        {
            if (string.IsNullOrEmpty(full) || full.Length < rightCharCount || full.Length - rightCharCount < 0)
                return full;

            return full.Substring(full.Length - rightCharCount);
        }
        #endregion

        #region String Parsing
        public static bool Inlist(string s, params string[] list)
        {
            return list.Contains(s);
        }
        public static bool Contains(this string text, string searchFor, StringComparison stringComparison)
        {
            return text.IndexOf(searchFor, stringComparison) > -1;
        }
        public static string[] GetLines(this string s, int maxLines = 0)
        {
            if (s == null)
                return null;
           
            s = s.Replace("\r\n", "\n");

            if (maxLines <  1)
                return s.Split(new char[] { '\n' });

            return s.Split(new char[] {'\n'}).Take(maxLines).ToArray();
        }
        public static int CountLines(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;

            return s.Split('\n').Length;
        }
        public static string StripNonNumber(string input)
        {
            var chars = input.ToCharArray();
            StringBuilder sb = new StringBuilder();
            foreach (var chr in chars)
            {
                if (char.IsNumber(chr) || char.IsSeparator(chr))
                    sb.Append(chr);
            }

            return sb.ToString();
        }
        static Regex tokenizeRegex = new Regex("{{.*?}}");
        public static List<string> TokenizeString(ref string text, string start, string end, string replaceDelimiter = "#@#")
        {
            var strings = new List<string>();            
            var matches = tokenizeRegex.Matches(text);

            int i = 0;
            foreach (Match match in matches)
            {
                tokenizeRegex = new Regex(Regex.Escape(match.Value));
                text = tokenizeRegex.Replace(text, $"{replaceDelimiter}{i}{replaceDelimiter}", 1);
                strings.Add(match.Value);
                i++;
            }

            return strings;
        }
        public static string DetokenizeString(string text, List<string> tokens, string replaceDelimiter = "#@#")
        {
            int i = 0;
            foreach (string token in tokens)
            {
                text = text.Replace($"{replaceDelimiter}{i}{replaceDelimiter}", token);
                i++;
            }
            return text;
        }
        public static int ParseInt(string input, int defaultValue=0, IFormatProvider numberFormat = null)
        {
            if (numberFormat == null)
                numberFormat = CultureInfo.CurrentCulture.NumberFormat;

            int val = defaultValue;
            if (!int.TryParse(input, NumberStyles.Any, numberFormat, out val))
                return defaultValue;
            return val;
        }
        public static decimal ParseDecimal(string input, decimal defaultValue = 0M, IFormatProvider numberFormat = null)
        {
            numberFormat = numberFormat ?? CultureInfo.CurrentCulture.NumberFormat;
            decimal val = defaultValue;
            if (!decimal.TryParse(input, NumberStyles.Any, numberFormat, out val))
                return defaultValue;
            return val;
        }
        #endregion

        #region String Ids
        public static string NewStringId()
        {
            return Guid.NewGuid().ToString().GetHashCode().ToString("x");
        }

        public static string RandomString(int size, bool includeNumbers = false)
        {
            StringBuilder builder = new StringBuilder(size);
            char ch;
            int num;

            for (int i = 0; i < size; i++)
            {
                if (includeNumbers)
                    num = Convert.ToInt32(Math.Floor(62 * random.NextDouble()));
                else
                    num = Convert.ToInt32(Math.Floor(52 * random.NextDouble()));

                if (num < 26)
                    ch = Convert.ToChar(num + 65);
                // lower case
                else if (num > 25 && num < 52)
                    ch = Convert.ToChar(num - 26 + 97);
                // numbers
                else
                    ch = Convert.ToChar(num - 52 + 48);

                builder.Append(ch);
            }

            return builder.ToString();
        }
        private static Random random = new Random((int)DateTime.Now.Ticks);

        #endregion

        #region Encodings
        // [Obsolete("Use System.Uri.EscapeDataString instead")]
        public static string UrlEncode(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            
            return Uri.EscapeDataString(text);
        }
        public static string UrlEncodePathSafe(string text)
        { 
            string escaped = UrlEncode(text);
            return escaped.Replace(".", "%2E").Replace("#", "%23");
        }
        public static string UrlDecode(string text)
        {
            // pre-process for + sign space formatting since System.Uri doesn't handle it
            // plus literals are encoded as %2b normally so this should be safe
            text = text.Replace("+", " ");
            string decoded = Uri.UnescapeDataString(text);
            return decoded;
        }
        public static string GetUrlEncodedKey(string urlEncoded, string key)
        {
            urlEncoded = "&" + urlEncoded + "&";

            int Index = urlEncoded.IndexOf("&" + key + "=", StringComparison.OrdinalIgnoreCase);
            if (Index < 0)
                return string.Empty;

            int lnStart = Index + 2 + key.Length;

            int Index2 = urlEncoded.IndexOf("&", lnStart);
            if (Index2 < 0)
                return string.Empty;

            return UrlDecode(urlEncoded.Substring(lnStart, Index2 - lnStart));
        }
        public static string SetUrlEncodedKey(string urlEncoded, string key, string value)
        {
            if (!urlEncoded.EndsWith("?") && !urlEncoded.EndsWith("&"))
                urlEncoded += "&";

            Match match = Regex.Match(urlEncoded, "[?|&]" + key + "=.*?&");

            if (match == null || string.IsNullOrEmpty(match.Value))
                urlEncoded = urlEncoded + key + "=" + UrlEncode(value) + "&";
            else
                urlEncoded = urlEncoded.Replace(match.Value, match.Value.Substring(0, 1) + key + "=" + UrlEncode(value) + "&");

            return urlEncoded.TrimEnd('&');
        }
        #endregion

        #region Binary Encoding
        public static byte[] BinHexToBinary(string hex)
        {
            int offset = hex.StartsWith("0x") ? 2 : 0;
            if ((hex.Length % 2) != 0)
                throw new ArgumentException(String.Format("InvalidHexStringLength", hex.Length));

            byte[] ret = new byte[(hex.Length - offset) / 2];

            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = (byte)((ParseHexChar(hex[offset]) << 4)
                                | ParseHexChar(hex[offset + 1]));
                offset += 2;
            }
            return ret;
        }
        public static string BinaryToBinHex(byte[] data)
        {
            if (data == null)
                return null;

            StringBuilder sb = new StringBuilder(data.Length * 2);
            foreach (byte val in data)
            {
                sb.AppendFormat("{0:x2}", val);
            }
            return sb.ToString();
        }
        public static byte[] StringToBytes(string text, Encoding encoding = null)
        {
            if (text == null)
                return null;

            if (encoding == null)
                encoding = Encoding.Unicode;

            return encoding.GetBytes(text);
        }
        public static string BytesToString(byte[] buffer, Encoding encoding = null)
        {
            if (buffer == null)
                return null;

            if (encoding == null)
                encoding = Encoding.Unicode;

            return encoding.GetString(buffer);            
        }
        static int ParseHexChar(char c)
        {
            if (c >= '0' && c <= '9')
                return c - '0';
            if (c >= 'A' && c <= 'F')
                return c - 'A' + 10;
            if (c >= 'a' && c <= 'f')
                return c - 'a' + 10;

            throw new ArgumentException("InvalidHexDigit" + c);
        }
        static char[] base36CharArray = "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray();
        static string base36Chars = "0123456789abcdefghijklmnopqrstuvwxyz";
        public static string Base36Encode(long value)
        {
            string returnValue = "";
            bool isNegative = value < 0;
            if (isNegative)
                value = value * -1;

            do
            {
                returnValue = base36CharArray[value % base36CharArray.Length] + returnValue;
                value /= 36;
            } while (value != 0);

            return isNegative ? returnValue + "-" : returnValue;
        }
        public static long Base36Decode(string input)
        {
            bool isNegative = false;
            if (input.EndsWith("-"))
            {
                isNegative = true;
                input = input.Substring(0, input.Length - 1);
            }

            char[] arrInput = input.ToCharArray();
            Array.Reverse(arrInput);
            long returnValue = 0;
            for (long i = 0; i < arrInput.Length; i++)
            {
                long valueindex = base36Chars.IndexOf(arrInput[i]);
                returnValue += Convert.ToInt64(valueindex * Math.Pow(36, i));
            }
            return isNegative ? returnValue * -1 : returnValue;
        }
        #endregion

        #region Miscellaneous
        public static string NormalizeLineFeeds(string text, LineFeedTypes type = LineFeedTypes.Auto)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            if (type == LineFeedTypes.Auto)
            {
                if (Environment.NewLine.Contains('\r'))
                    type = LineFeedTypes.CrLf;
                else
                    type = LineFeedTypes.Lf;
            }

            if (type == LineFeedTypes.Lf)            
                return text.Replace("\r\n", "\n");            
            
            return text.Replace("\r\n", "*@\r@*").Replace("\n","\r\n").Replace("*@\r@*","\r\n");
        }
        public static string NormalizeIndentation(string code)
        {
            // normalize tabs to 3 spaces
            string text = code.Replace("\t", "   ");

            string[] lines = text.Split(new string[3] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            // keep track of the smallest indent
            int minPadding = 1000;

            foreach (var line in lines)
            {
                if (line.Length == 0)  // ignore blank lines
                    continue;

                int count = 0;
                foreach (char chr in line)
                {
                    if (chr == ' ' && count < minPadding)
                        count++;
                    else
                        break;
                }
                if (count == 0)
                    return code;

                minPadding = count;
            }

            string strip = new String(' ', minPadding);

            StringBuilder sb = new StringBuilder();
            foreach (var line in lines)
            {
                sb.AppendLine(StringUtils.ReplaceStringInstance(line, strip, "", 1, false));
            }

            return sb.ToString();
        }
        public static void LogString(string output, string filename, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            StreamWriter Writer = new StreamWriter(filename, true, encoding);
            Writer.WriteLine(DateTime.Now + " - " + output);
            Writer.Close();
        }
        public static Stream StringToStream(string text, Encoding encoding =null)
        {
            if (encoding == null)
                encoding = Encoding.Default;

            MemoryStream ms = new MemoryStream(text.Length * 2);
            byte[] data = encoding.GetBytes(text);
            ms.Write(data, 0, data.Length);
            ms.Position = 0;
            return ms;
        }
        public static string GetProperty(string propertyString, string key)
        {
            return StringUtils.ExtractString(propertyString, "<" + key + ">", "</" + key + ">");
        }
        public static string SetProperty(string propertyString, string key, string value)
        {
            string extract = StringUtils.ExtractString(propertyString, "<" + key + ">", "</" + key + ">");

            if (string.IsNullOrEmpty(value) && extract != string.Empty)
            {
                return propertyString.Replace(extract, "");
            }

            string xmlLine = "<" + key + ">" + value + "</" + key + ">";

            // replace existing
            if (extract != string.Empty)
                return propertyString.Replace(extract, xmlLine);

            // add new
            return propertyString + xmlLine + "\r\n";
        }
        #endregion
    }

    public enum LineFeedTypes
    {
        // Linefeed \n only
        Lf,
        // Carriage Return and Linefeed \r\n
        CrLf,
        // Platform default Environment.NewLine
        Auto
    }
}