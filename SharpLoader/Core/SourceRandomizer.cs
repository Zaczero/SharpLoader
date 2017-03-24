using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLoader.Core
{
    public class SourceRandomizer
    {
        private readonly Random _rnd;

        public SourceRandomizer(int seed)
        {
            _rnd = new Random(seed);
        }

        public void Randomize(ref string source)
        {
            source = Encode(source);
            source = Trash(source);
            source = Swap(source);
            source = Decode(source);
        }

        public string GetRandomString(int length)
        {
            const string source = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";

            var result = new StringBuilder(length);

            for (var i = 0; i < length; i++)
            {
                var index = _rnd.Next(0, source.Length);
                result.Append(source[index]);
            }

            return result.ToString();
        }

        private string GetVariableName(string str)
        {
            string varName;
            while (true)
            {
                varName = GetRandomString(_rnd.Next(8, 16));
                if (!str.Contains(varName))
                {
                    break;
                }
            }

            return varName;
        }

        private string Encode(string str)
        {
            var result = new StringBuilder(str.Length);
            var insideComment = 0;
            var insideString = false;

            for (var i = 0; i < str.Length; i++)
            {
                // Check comments (inside)
                if (insideComment == 1)
                {
                    if (str[i] == '\n')
                    {
                        insideComment = 0;
                    }

                    continue;
                }
                if (insideComment == 2)
                {
                    if (str[i] == '*' && str[i + 1] == '/')
                    {
                        i++;
                        insideComment = 0;
                    }

                    continue;
                }

                // Check comments (outside)
                if (!insideString && str[i] == '/' && str[i + 1] == '/')
                {
                    i++;
                    insideComment = 1;
                    continue;
                }
                if (!insideString && str[i] == '/' && str[i + 1] == '*')
                {
                    i++;
                    insideComment = 2;
                    continue;
                }

                // Check insideString
                if (str[i] == '\"' && str[i - 1] != '\\')
                {
                    insideString = !insideString;
                }
                else if (str[i] == '\'' && str[i - 1] != '\\')
                {
                    insideString = !insideString;
                }

                // Skip strings
                if (!insideString)
                {
                    // Encode
                    if (str[i] == '\r' ||
                        str[i] == '\n')
                    {
                        continue;
                    }

                    if (str[i] == ' ')
                    {
                        if (!char.IsLetterOrDigit(str[i - 1]) ||
                            !char.IsLetterOrDigit(str[i + 1]))
                        {
                            continue;
                        }
                    }
                }
                // Encode strings
                else
                {
                    if (str[i] == ';')
                    {
                        str = str.Remove(i, 1).Insert(i, "\u0000");
                    }
                    else if (str[i] == '<')
                    {
                        str = str.Remove(i, 1).Insert(i, "\u0001");
                    }
                    else if (str[i] == '>')
                    {
                        str = str.Remove(i, 1).Insert(i, "\u0002");
                    }
                }

                result.Append(str[i]);
            }

            return result.ToString();
        }

        private string Trash(string str)
        {
            var result = str;

            while (true)
            {
                // Check for tag
                var tagIndex = result.IndexOf("<trash", StringComparison.Ordinal);
                if (tagIndex == -1)
                {
                    break;
                }

                // Get arguments
                var arg = 8;
                var tagLength = result.Substring(tagIndex).IndexOf(">", StringComparison.Ordinal);
                if (tagLength == -1)
                {
                    throw new Exception();
                }
                if (tagLength > 7)
                {
                    arg = int.Parse(result.Substring(tagIndex + 7, tagLength - 7));
                }

                // Add trash
                var trash = string.Empty;
                for (var i = 0; i < arg; i++)
                {
                    // 25%
                    switch (_rnd.Next(0, 4))
                    {
                        case 0:
                        {
                            // string value
                            var varValue = GetRandomString(_rnd.Next(8, 32));
                            var operation = string.Empty;

                            switch (_rnd.Next(0, 3))
                            {
                                case 0:
                                {
                                    operation = "Normalize()";
                                    break;
                                }
                                case 1:
                                {
                                    operation = "ToLower()";
                                    break;
                                }
                                case 2:
                                {
                                    operation = "ToUpper()";
                                    break;
                                }
                            }
 
                            trash += $"\"{varValue}\".{operation};";
                            break;
                        }
                        default:
                        {
                            // numeric value
                            var varName = GetVariableName(result);
                            var operation = _rnd.Next(0, 2) == 0 ? '+' : '-';

                            var varType = string.Empty;
                            var varValue = 0;
                            var varChange = 0;

                            switch (_rnd.Next(0, 3))
                            {
                                case 0:
                                {
                                    varType = "byte";
                                    varChange = _rnd.Next(1, byte.MaxValue);
                                    varValue = operation == '+' ? _rnd.Next(byte.MinValue, byte.MaxValue - varChange) : _rnd.Next(byte.MinValue + varChange, byte.MaxValue);
                                    break;
                                }
                                case 1:
                                {
                                    varType = "short";
                                    varChange = _rnd.Next(1, short.MaxValue);
                                    varValue = operation == '+' ? _rnd.Next(short.MinValue, short.MaxValue - varChange) : _rnd.Next(short.MinValue + varChange, short.MaxValue);
                                    break;
                                }
                                case 2:
                                {
                                    varType = "int";
                                    varChange = _rnd.Next(1, int.MaxValue);
                                    varValue = operation == '+' ? _rnd.Next(int.MinValue, int.MaxValue - varChange) : _rnd.Next(int.MinValue + varChange, int.MaxValue);
                                    break;
                                }
                            }

                            trash += $"{varType} {varName} = {varValue}\u0000{varName} {operation}= {varChange};";
                            break;
                        }
                    }
                }

                // Replace
                result = result.Remove(tagIndex, tagLength + 1).Insert(tagIndex, trash);
            }

            return result;
        }

        private string Swap(string str)
        {
            var result = str;

            while (true)
            {
                // Check for tag
                var tagIndex = result.IndexOf("<swap>", StringComparison.Ordinal);
                if (tagIndex == -1)
                {
                    break;
                }

                var tagLength = 6;
                var endTagLength = 7;

                var afterStr = str.Substring(tagIndex + tagLength);
                var endTagIndex = afterStr.IndexOf("<swap/>", StringComparison.Ordinal);
                if (endTagIndex == -1)
                {
                    throw new Exception("<swap/> not found");
                }
                var innerStr = afterStr.Substring(0, endTagIndex);

                if (innerStr.IndexOf("<block>", StringComparison.Ordinal) != -1)
                {
                    var blocks = new List<string>();

                    // Find blocks
                    while (true)
                    {
                        var blockIndex = innerStr.IndexOf("<block>", StringComparison.Ordinal);
                        if (blockIndex == -1)
                        {
                            // Add last block
                            blocks.Add(innerStr);
                            break;
                        }

                        var blockTagLength = 7;

                        // Get block
                        var innerBlockStr = innerStr.Substring(0, blockIndex);
                        blocks.Add(innerBlockStr);

                        // Remove from search
                        innerStr = innerStr.Remove(0, blockIndex + blockTagLength);
                    }

                    var swapped = new string[blocks.Count];

                    // Swap
                    foreach (var block in blocks)
                    {
                        // Find index to swap into
                        int swapIndex;
                        while (true)
                        {
                            swapIndex = _rnd.Next(0, swapped.Length);
                            if (swapped[swapIndex] == null)
                            {
                                break;
                            }
                        }

                        swapped[swapIndex] = block;
                    }

                    // Remove old
                    result = result.Remove(tagIndex, tagLength + endTagIndex + endTagLength);

                    // Insert new
                    var output = swapped.Aggregate(string.Empty, (current, s) => current + s);
                    result = result.Insert(tagIndex, output);
                }
                else
                {
                    // Split
                    var blocks = innerStr.Split(new [] {';'}, StringSplitOptions.RemoveEmptyEntries);
                    var swapped = new string[blocks.Length];

                    // Swap
                    foreach (var block in blocks)
                    {
                        // Find index to swap into
                        int swapIndex;
                        while (true)
                        {
                            swapIndex = _rnd.Next(0, swapped.Length);
                            if (swapped[swapIndex] == null)
                            {
                                break;
                            }
                        }

                        swapped[swapIndex] = block;
                    }

                    // Remove old
                    result = result.Remove(tagIndex, tagLength + endTagIndex + endTagLength);

                    // Insert new
                    var output = swapped.Aggregate(string.Empty, (current, s) => current + (s + ';'));
                    result = result.Insert(tagIndex, output);
                }
            }

            return result;
        }

        private string Decode(string str)
        {
            var insideString = false;

            for (var i = 0; i < str.Length; i++)
            {
                // Check insideString
                if (str[i] == '\"' && str[i - 1] != '\\')
                {
                    insideString = !insideString;
                }
                else if (str[i] == '\'' && str[i - 1] != '\\')
                {
                    insideString = !insideString;
                }

                if (str[i] == '\u0000')
                {
                    str = str.Remove(i, 1).Insert(i, ";");
                }
                else
                {
                    // Skip not-strings
                    if (!insideString)
                    {
                        continue;
                    }

                    if (str[i] == '\u0001')
                    {
                        str = str.Remove(i, 1).Insert(i, "<");
                    }
                    else if (str[i] == '\u0002')
                    {
                        str = str.Remove(i, 1).Insert(i, ">");
                    }
                }
            }

            return str;
        }
    }
}
