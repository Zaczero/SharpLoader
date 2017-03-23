using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLoader.Core
{
    class SourceRandomizer
    {
        private readonly Random _rnd;

        public SourceRandomizer(int seed)
        {
            _rnd = new Random(seed);
        }

        public void Randomize(ref string source)
        {
            source = Format(source);
            source = Trash(source);
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

        private string Format(string str)
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
                if (str[i] == '/' && str[i + 1] == '/')
                {
                    i++;
                    insideComment = 1;
                    continue;
                }
                if (str[i] == '/' && str[i + 1] == '*')
                {
                    i++;
                    insideComment = 2;
                    continue;
                }

                // Check insideString
                if (str[i] == '\\' && str[i + 1] != '\"')
                {
                    insideString = !insideString;
                }
                else if (str[i] == '\\' && str[i + 1] != '\'')
                {
                    insideString = !insideString;
                }

                // Don't format strings
                if (!insideString)
                {
                    // Format
                    if (str[i] == '\r' ||
                        str[i] == '\n' ||
                       (str[i] == ' ' && str[i + 1] == ' '))
                    {
                        continue;
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
                var tagIndex = result.IndexOf("[trash", StringComparison.Ordinal);
                if (tagIndex == -1)
                {
                    break;
                }

                // Get arguments
                var arg = 8;
                var tagLength = result.Substring(tagIndex).IndexOf("]", StringComparison.Ordinal);
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
                    switch (_rnd.Next(0, 3))
                    {
                        case 0:
                        {
                            var varName = GetVariableName(result);
                            var change = _rnd.Next(1, byte.MaxValue);
                            trash += $"byte {varName} = {_rnd.Next(byte.MinValue, byte.MaxValue - change)};{varName} += {change};";
                            break;
                        }
                        case 1:
                        {
                            var varName = GetVariableName(result);
                            var change = _rnd.Next(1, int.MaxValue);
                            trash += $"int {varName} = {_rnd.Next(int.MinValue, int.MaxValue - change)};{varName} += {change};";
                            break;
                        }
                        case 2:
                        {
                            var varName = GetVariableName(result);
                            trash += $"string {varName} = \"{GetRandomString(_rnd.Next(8, 32))}\";{varName}.Normalize();";
                            break;
                        }
                    }
                }

                // Replace
                result = result.Remove(tagIndex, tagLength + 1).Insert(tagIndex, trash);
            }

            return result;
        }
    }
}
