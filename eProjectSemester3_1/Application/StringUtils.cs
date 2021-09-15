using HtmlAgilityPack;
using Microsoft.Security.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace eProjectSemester3_1.Application
{
    public static class StringUtils
    {

        #region Extension Methods
        /// <summary>
        /// Checks whether the string is Null Or Empty
        /// </summary>
        /// <param name="theInput"></param>
        /// <returns></returns>
        public static bool IsNullEmpty(this string theInput)
        {
            return string.IsNullOrEmpty(theInput);
        }

        /// <summary>
        /// Converts the string to Int32
        /// </summary>
        /// <param name="theInput"></param>
        /// <returns></returns>
        public static int ToInt32(this string theInput)
        {
            return !string.IsNullOrEmpty(theInput) ? Convert.ToInt32(theInput) : 0;
        }

        /// <summary>
        /// Removes all line breaks from a string
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static string RemoveLineBreaks(this string lines)
        {
            return lines.Replace("\r\n", "")
                        .Replace("\r", "")
                        .Replace("\n", "");
        }

        // Gets the full url including 
        public static string ReturnCurrentDomain()
        {
            var r = HttpContext.Current.Request;
            var builder = new UriBuilder(r.Url.Scheme, r.Url.Host, r.Url.Port);
            return builder.Uri.ToString().TrimEnd('/');
        }

        /// <summary>
        /// Removes all line breaks from a string and replaces them with specified replacement
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceLineBreaks(this string lines, string replacement)
        {
            return lines.Replace(Environment.NewLine, replacement);
        }

        /// <summary>
        /// Does a case insensitive contains
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ContainsCaseInsensitive(this string source, string value)
        {
            var results = source.IndexOf(value, StringComparison.CurrentCultureIgnoreCase);
            return results != -1;
        }
        #endregion

        #region Misc

        /// <summary>
        /// Create a salt for the password hash (just makes it a bit more complex)
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string CreateSalt(int size)
        {
            // Generate a cryptographic random number.
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number.
            return Convert.ToBase64String(buff);
        }

        /// <summary>
        /// Generate a hash for a password, adding a salt value
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string GenerateSaltedHash(string plainText, string salt)
        {
            // http://stackoverflow.com/questions/2138429/hash-and-salt-passwords-in-c-sharp

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            var saltBytes = Encoding.UTF8.GetBytes(salt);

            // Combine the two lists
            var plainTextWithSaltBytes = new List<byte>(plainTextBytes.Length + saltBytes.Length);
            plainTextWithSaltBytes.AddRange(plainTextBytes);
            plainTextWithSaltBytes.AddRange(saltBytes);

            // Produce 256-bit hashed value i.e. 32 bytes
            HashAlgorithm algorithm = new SHA256Managed();
            var byteHash = algorithm.ComputeHash(plainTextWithSaltBytes.ToArray());
            return Convert.ToBase64String(byteHash);
        }

        public static string PostForm(string url, string poststring)
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);

            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/x-www-form-urlencoded";

            var bytedata = Encoding.UTF8.GetBytes(poststring);
            httpRequest.ContentLength = bytedata.Length;

            var requestStream = httpRequest.GetRequestStream();
            requestStream.Write(bytedata, 0, bytedata.Length);
            requestStream.Close();

            var httpWebResponse = (HttpWebResponse)httpRequest.GetResponse();
            var responseStream = httpWebResponse.GetResponseStream();

            var sb = new StringBuilder();

            if (responseStream != null)
            {
                using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        sb.Append(line);
                    }
                }
            }

            return sb.ToString();

        }


        #endregion


        #region String content helpers

        private static readonly Random _rng = new Random();
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string RandomString(int size)
        {
            var buffer = new char[size];
            for (var i = 0; i < size; i++)
            {
                buffer[i] = _chars[_rng.Next(_chars.Length)];
            }
            return new string(buffer);
        }

        /// <summary>
        /// Returns the number of occurances of one string within another
        /// </summary>
        /// <param name="text"></param>
        /// <param name="stringToFind"></param>
        /// <returns></returns>
        public static int NumberOfOccurrences(string text, string stringToFind)
        {
            if (text == null || stringToFind == null)
            {
                return 0;
            }

            var reg = new Regex(stringToFind, RegexOptions.IgnoreCase);

            return reg.Matches(text).Count;
        }

        /// <summary>
        /// reverses a string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StringReverse(string str)
        {
            var len = str.Length;
            var arr = new char[len];
            for (var i = 0; i < len; i++)
            {
                arr[i] = str[len - 1 - i];
            }
            return new string(arr);
        }

        /// <summary>
        /// Returns a capitalised version of words in the string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CapitalizeWords(string value)
        {
            if (value == null)
                return null;
            if (value.Length == 0)
                return value;

            var result = new StringBuilder(value);
            result[0] = char.ToUpper(result[0]);
            for (var i = 1; i < result.Length; ++i)
            {
                if (char.IsWhiteSpace(result[i - 1]))
                    result[i] = char.ToUpper(result[i]);
                else
                    result[i] = char.ToLower(result[i]);
            }
            return result.ToString();
        }


        /// <summary>
        /// Returns the amount of individual words in a string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int CountWordsInString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }
            var tmpStr = text.Replace("\t", " ").Trim();
            tmpStr = tmpStr.Replace("\n", " ");
            tmpStr = tmpStr.Replace("\r", " ");
            while (tmpStr.IndexOf("  ") != -1)
                tmpStr = tmpStr.Replace("  ", " ");
            return tmpStr.Split(' ').Length;
        }

        /// <summary>
        /// Returns a specified amount of words from a string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="wordAmount"></param>
        /// <returns></returns>
        public static string ReturnAmountWordsFromString(string text, int wordAmount)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            string tmpStr;
            string[] stringArray;
            var tmpStrReturn = "";
            tmpStr = text.Replace("\t", " ").Trim();
            tmpStr = tmpStr.Replace("\n", " ");
            tmpStr = tmpStr.Replace("\r", " ");

            while (tmpStr.IndexOf("  ") != -1)
            {
                tmpStr = tmpStr.Replace("  ", " ");
            }
            stringArray = tmpStr.Split(' ');

            if (stringArray.Length < wordAmount)
            {
                wordAmount = stringArray.Length;
            }
            for (int i = 0; i < wordAmount; i++)
            {
                tmpStrReturn += stringArray[i] + " ";
            }
            return tmpStrReturn;
        }

        /// <summary>
        /// Returns a string to do a related question/search lookup
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public static string ReturnSearchString(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return searchTerm;
            }

            // Lower case
            searchTerm = searchTerm.ToLower();

            // Firstly strip non alpha numeric charactors out
            searchTerm = Regex.Replace(searchTerm, @"[^\w\.@\- ]", "");

            // Now strip common words out and retun the final result
            return string.Join(" ", searchTerm.Split().Where(w => !CommonWords().Contains(w)).ToArray());
        }

        /// <summary>
        /// Returns a list of the most common english words
        /// TODO: Need to put this in something so people can add other language lists of common words
        /// </summary>
        /// <returns></returns>
        public static IList<string> CommonWords()
        {
            return new List<string>
                {
                    "the", "be",  "to",
                    "of",
                    "and",
                    "a",
                    "in",
                    "that",
                    "have",
                    "i",
                    "it",
                    "for",
                    "not",
                    "on",
                    "with",
                    "he",
                    "as",
                    "you",
                    "do",
                    "at",
                    "this",
                    "but",
                    "his",
                    "by",
                    "from",
                    "they",
                    "we",
                    "say",
                    "her",
                    "she",
                    "or",
                    "an",
                    "will",
                    "my",
                    "one",
                    "all",
                    "would",
                    "there",
                    "their",
                    "what",
                    "so",
                    "up",
                    "out",
                    "if",
                    "about",
                    "who",
                    "get",
                    "which",
                    "go",
                    "me",
                    "when",
                    "make",
                    "can",
                    "like",
                    "time",
                    "no",
                    "just",
                    "him",
                    "know",
                    "take",
                    "people",
                    "into",
                    "year",
                    "your",
                    "good",
                    "some",
                    "could",
                    "them",
                    "see",
                    "other",
                    "than",
                    "then",
                    "now",
                    "look",
                    "only",
                    "come",
                    "its",
                    "over",
                    "think",
                    "also",
                    "back",
                    "after",
                    "use",
                    "two",
                    "how",
                    "our",
                    "work",
                    "first",
                    "well",
                    "way",
                    "even",
                    "new",
                    "want",
                    "because",
                    "any",
                    "these",
                    "give",
                    "day",
                    "most",
                    "cant",
                    "us"
                };
        }

        #endregion


        #region Sanitising

        /// <summary>
        /// Strips all non alpha/numeric charators from a string
        /// </summary>
        /// <param name="strInput"></param>
        /// <param name="replaceWith"></param>
        /// <returns></returns>
        public static string StripNonAlphaNumeric(string strInput, string replaceWith)
        {
            strInput = Regex.Replace(strInput, "[^\\w]", replaceWith);
            strInput = strInput.Replace(string.Concat(replaceWith, replaceWith, replaceWith), replaceWith)
                                .Replace(string.Concat(replaceWith, replaceWith), replaceWith)
                                .TrimStart(Convert.ToChar(replaceWith))
                                .TrimEnd(Convert.ToChar(replaceWith));
            return strInput;
        }

        /// <summary>
        /// Get the current users IP address
        /// </summary>
        /// <returns></returns>
        public static string GetUsersIpAddress()
        {
            var context = HttpContext.Current;
            var serverName = context.Request.ServerVariables["SERVER_NAME"];
            if (serverName.ToLower().Contains("localhost"))
            {
                return serverName;
            }
            var ipList = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            return !string.IsNullOrEmpty(ipList) ? ipList.Split(',')[0] : context.Request.ServerVariables["REMOTE_ADDR"];
        }

        /// <summary>
        /// Used to pass all string input in the system  - Strips all nasties from a string/html
        /// </summary>
        /// <param name="html"></param>
        /// <param name="useXssSantiser"></param>
        /// <returns></returns>
        public static string GetSafeHtml(string html, bool useXssSantiser = false)
        {
            // Scrub html
            html = ScrubHtml(html, useXssSantiser);

            // remove unwanted html
            html = RemoveUnwantedTags(html);

            return html;
        }

        /// <summary>
        /// Takes in HTML and returns santized Html/string
        /// </summary>
        /// <param name="html"></param>
        /// <param name="useXssSantiser"></param>
        /// <returns></returns>
        public static string ScrubHtml(string html, bool useXssSantiser = false)
        {
            if (string.IsNullOrEmpty(html))
            {
                return html;
            }

            // clear the flags on P so unclosed elements in P will be auto closed.
            HtmlNode.ElementsFlags.Remove("p");

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var finishedHtml = html;

            // Embed Urls
            if (doc.DocumentNode != null)
            {
                // Get all the links we are going to 
                var tags = doc.DocumentNode.SelectNodes("//a[contains(@href, 'youtube.com')]|//a[contains(@href, 'youtu.be')]|//a[contains(@href, 'vimeo.com')]|//a[contains(@href, 'screenr.com')]|//a[contains(@href, 'instagram.com')]");

                if (tags != null)
                {
                    // find formatting tags
                    foreach (var item in tags)
                    {
                        if (item.PreviousSibling == null)
                        {
                            // Prepend children to parent node in reverse order
                            foreach (var node in item.ChildNodes.Reverse())
                            {
                                item.ParentNode.PrependChild(node);
                            }
                        }
                        else
                        {
                            // Insert children after previous sibling
                            foreach (var node in item.ChildNodes)
                            {
                                item.ParentNode.InsertAfter(node, item.PreviousSibling);
                            }
                        }

                        // remove from tree
                        item.Remove();
                    }
                }


                //Remove potentially harmful elements
                var nc = doc.DocumentNode.SelectNodes("//script|//link|//iframe|//frameset|//frame|//applet|//object|//embed");
                if (nc != null)
                {
                    foreach (var node in nc)
                    {
                        node.ParentNode.RemoveChild(node, false);

                    }
                }

                //remove hrefs to java/j/vbscript URLs
                nc = doc.DocumentNode.SelectNodes("//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'javascript')]|//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'jscript')]|//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'vbscript')]");
                if (nc != null)
                {

                    foreach (var node in nc)
                    {
                        node.SetAttributeValue("href", "#");
                    }
                }

                //remove img with refs to java/j/vbscript URLs
                nc = doc.DocumentNode.SelectNodes("//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'javascript')]|//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'jscript')]|//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'vbscript')]");
                if (nc != null)
                {
                    foreach (var node in nc)
                    {
                        node.SetAttributeValue("src", "#");
                    }
                }

                //remove on<Event> handlers from all tags
                nc = doc.DocumentNode.SelectNodes("//*[@onclick or @onmouseover or @onfocus or @onblur or @onmouseout or @ondblclick or @onload or @onunload or @onerror]");
                if (nc != null)
                {
                    foreach (var node in nc)
                    {
                        node.Attributes.Remove("onFocus");
                        node.Attributes.Remove("onBlur");
                        node.Attributes.Remove("onClick");
                        node.Attributes.Remove("onMouseOver");
                        node.Attributes.Remove("onMouseOut");
                        node.Attributes.Remove("onDblClick");
                        node.Attributes.Remove("onLoad");
                        node.Attributes.Remove("onUnload");
                        node.Attributes.Remove("onError");
                    }
                }

                // remove any style attributes that contain the word expression (IE evaluates this as script)
                nc = doc.DocumentNode.SelectNodes("//*[contains(translate(@style, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'expression')]");
                if (nc != null)
                {
                    foreach (var node in nc)
                    {
                        node.Attributes.Remove("stYle");
                    }
                }

                // build a list of nodes ordered by stream position
                var pos = new NodePositions(doc);

                // browse all tags detected as not opened
                foreach (var error in doc.ParseErrors.Where(e => e.Code == HtmlParseErrorCode.TagNotOpened))
                {
                    // find the text node just before this error
                    var last = pos.Nodes.OfType<HtmlTextNode>().LastOrDefault(n => n.StreamPosition < error.StreamPosition);
                    if (last != null)
                    {
                        // fix the text; reintroduce the broken tag
                        last.Text = error.SourceText.Replace("/", "") + last.Text + error.SourceText;
                    }
                }

                finishedHtml = doc.DocumentNode.WriteTo();
            }


            // The reason we have this option, is using the santiser with the MarkDown editor 
            // causes problems with line breaks.
            if (useXssSantiser)
            {
                return SanitizerCompatibleWithForiegnCharacters(Sanitizer.GetSafeHtmlFragment(finishedHtml));
            }

            return finishedHtml;
        }

        public static string RemoveUnwantedTags(string html)
        {

            var unwantedTagNames = new List<string>
            {
                "div",
                "font",
                "table",
                "tbody",
                "tr",
                "td",
                "th",
                "thead"
            };

            return RemoveUnwantedTags(html, unwantedTagNames);
        }

        public static string RemoveUnwantedTags(string html, List<string> unwantedTagNames)
        {
            if (string.IsNullOrEmpty(html))
            {
                return html;
            }

            var htmlDoc = new HtmlDocument();

            // load html
            htmlDoc.LoadHtml(html);

            var tags = (from tag in htmlDoc.DocumentNode.Descendants()
                        where unwantedTagNames.Contains(tag.Name)
                        select tag).Reverse();


            // find formatting tags
            foreach (var item in tags)
            {
                if (item.PreviousSibling == null)
                {
                    // Prepend children to parent node in reverse order
                    foreach (var node in item.ChildNodes.Reverse())
                    {
                        item.ParentNode.PrependChild(node);
                    }
                }
                else
                {
                    // Insert children after previous sibling
                    foreach (var node in item.ChildNodes)
                    {
                        item.ParentNode.InsertAfter(node, item.PreviousSibling);
                    }
                }

                // remove from tree
                item.Remove();
            }

            // return transformed doc
            return htmlDoc.DocumentNode.WriteContentTo().Trim();
        }

        /// <summary>
        /// Url Encodes a string using the XSS library
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string UrlEncode(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return Microsoft.Security.Application.Encoder.UrlEncode(input);
            }
            return input;
        }

        /// <summary>
        /// Decode a url
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string UrlDecode(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return HttpUtility.UrlDecode(input);
            }
            return input;
        }

        /// <summary>
        /// decode a chunk of html or url
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string HtmlDecode(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return HttpUtility.HtmlDecode(input);
            }
            return input;
        }

        /// <summary>
        /// Uses regex to strip HTML from a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StripHtmlFromString(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = Regex.Replace(input, @"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", string.Empty, RegexOptions.Singleline);
                input = Regex.Replace(input, @"\[[^]]+\]", "");
            }
            return input;
        }

        /// <summary>
        /// Returns safe plain text using XSS library
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SafePlainText(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = StripHtmlFromString(input);
                input = GetSafeHtml(input, true);
            }
            return input;
        }
        #endregion


        #region Sanitizer Compatible With Chinese Characters
        private static readonly Dictionary<string, string> HbjDictionaryFx = new Dictionary<string, string>();
        /// <summary>
        /// 微软的AntiXSS v4.0 让部分汉字乱码,这里将乱码部分汉字转换回来
        /// Microsoft AntiXSS Library Sanitizer causes some Chinese characters become "encoded",
        /// use this function to replace them back.
        /// source:http://www.zhaoshu.net/bbs/read10.aspx?TieID=b1745a9c-03a6-4367-93e0-114707aff3e3
        /// </summary>
        /// <returns></returns>
        public static string SanitizerCompatibleWithForiegnCharacters(string originalString)
        {
            var returnString = originalString;

            //returnString = returnString.Replace("\r\n", "");
            if (returnString.Contains("&#"))
            {
                //Initialize the dictionary, if it doesn't contain anything. 
                if (HbjDictionaryFx.Keys.Count == 0)
                {
                    lock (HbjDictionaryFx)
                    {
                        if (HbjDictionaryFx.Keys.Count == 0)
                        {
                            HbjDictionaryFx.Clear();
                            HbjDictionaryFx.Add("&#20028;", "丼");
                            HbjDictionaryFx.Add("&#20284;", "似");
                            HbjDictionaryFx.Add("&#20540;", "值");
                            HbjDictionaryFx.Add("&#20796;", "儼");
                            HbjDictionaryFx.Add("&#21052;", "刼");
                            HbjDictionaryFx.Add("&#21308;", "匼");
                            HbjDictionaryFx.Add("&#21564;", "吼");
                            HbjDictionaryFx.Add("&#21820;", "唼");
                            HbjDictionaryFx.Add("&#22076;", "嘼");
                            HbjDictionaryFx.Add("&#22332;", "圼");
                            HbjDictionaryFx.Add("&#22588;", "堼");
                            HbjDictionaryFx.Add("&#23612;", "尼");
                            HbjDictionaryFx.Add("&#26684;", "格");
                            HbjDictionaryFx.Add("&#22844;", "夼");
                            HbjDictionaryFx.Add("&#23100;", "娼");
                            HbjDictionaryFx.Add("&#23356;", "嬼");
                            HbjDictionaryFx.Add("&#23868;", "崼");
                            HbjDictionaryFx.Add("&#24124;", "帼");
                            HbjDictionaryFx.Add("&#24380;", "弼");
                            HbjDictionaryFx.Add("&#24636;", "怼");
                            HbjDictionaryFx.Add("&#24892;", "愼");
                            HbjDictionaryFx.Add("&#25148;", "戼");
                            HbjDictionaryFx.Add("&#25404;", "挼");
                            HbjDictionaryFx.Add("&#25660;", "搼");
                            HbjDictionaryFx.Add("&#25916;", "攼");
                            HbjDictionaryFx.Add("&#26172;", "昼");
                            HbjDictionaryFx.Add("&#26428;", "朼");
                            HbjDictionaryFx.Add("&#26940;", "椼");
                            HbjDictionaryFx.Add("&#27196;", "樼");
                            HbjDictionaryFx.Add("&#27452;", "欼");
                            HbjDictionaryFx.Add("&#27708;", "氼");
                            HbjDictionaryFx.Add("&#27964;", "洼");
                            HbjDictionaryFx.Add("&#28220;", "渼");
                            HbjDictionaryFx.Add("&#28476;", "漼");
                            HbjDictionaryFx.Add("&#28732;", "瀼");
                            HbjDictionaryFx.Add("&#28988;", "焼");
                            HbjDictionaryFx.Add("&#29244;", "爼");
                            HbjDictionaryFx.Add("&#29500;", "猼");
                            HbjDictionaryFx.Add("&#29756;", "琼");
                            HbjDictionaryFx.Add("&#30012;", "甼");
                            HbjDictionaryFx.Add("&#30268;", "瘼");
                            HbjDictionaryFx.Add("&#30524;", "眼");
                            HbjDictionaryFx.Add("&#30780;", "砼");
                            HbjDictionaryFx.Add("&#31036;", "礼");
                            HbjDictionaryFx.Add("&#31292;", "稼");
                            HbjDictionaryFx.Add("&#31548;", "笼");
                            HbjDictionaryFx.Add("&#31804;", "簼");
                            HbjDictionaryFx.Add("&#32060;", "紼");
                            HbjDictionaryFx.Add("&#32316;", "縼");
                            HbjDictionaryFx.Add("&#32572;", "缼");
                            HbjDictionaryFx.Add("&#32828;", "耼");
                            HbjDictionaryFx.Add("&#33084;", "脼");
                            HbjDictionaryFx.Add("&#33340;", "舼");
                            HbjDictionaryFx.Add("&#33596;", "茼");
                            HbjDictionaryFx.Add("&#33852;", "萼");
                            HbjDictionaryFx.Add("&#34108;", "蔼");
                            HbjDictionaryFx.Add("&#36156;", "贼");
                            HbjDictionaryFx.Add("&#39740;", "鬼");

                            // Also add russion
                            HbjDictionaryFx.Add("&#1084;", "м");
                        }
                    }

                }

                //start to replace "encoded" Chinese characters.
                lock (HbjDictionaryFx)
                {
                    foreach (string key in HbjDictionaryFx.Keys)
                    {
                        if (returnString.Contains(key))
                        {
                            returnString = returnString.Replace(key, HbjDictionaryFx[key]);
                        }
                    }
                }
            }

            return returnString;
        }
        #endregion
    }

    public class NodePositions
    {
        public NodePositions(HtmlDocument doc)
        {
            AddNode(doc.DocumentNode);
            Nodes.Sort(new NodePositionComparer());
        }

        private void AddNode(HtmlNode node)
        {
            Nodes.Add(node);
            foreach (HtmlNode child in node.ChildNodes)
            {
                AddNode(child);
            }
        }

        private class NodePositionComparer : IComparer<HtmlNode>
        {
            public int Compare(HtmlNode x, HtmlNode y)
            {
                return x.StreamPosition.CompareTo(y.StreamPosition);
            }
        }

        public List<HtmlNode> Nodes = new List<HtmlNode>();
    }
}