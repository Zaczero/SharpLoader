using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            Encode(ref source);
            Trash(ref source);
            Flow(ref source);
            Swap(ref source);
            Decode(ref source);
        }

        public string GetRandomString(int length)
        {
            const string source = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";

            var rndStrSb = new StringBuilder(length);

            for (var i = 0; i < length; i++)
            {
                var index = _rnd.Next(0, source.Length);
                rndStrSb.Append(source[index]);
            }

            return rndStrSb.ToString();
        }

        private string _getVariableStringLastStr;
        private List<string> _getVariableStringGenerated;
        private string GetVariableName(string str)
        {
            // New source file
            if (_getVariableStringLastStr != str)
            {
                _getVariableStringLastStr = str;
                _getVariableStringGenerated = new List<string>();
            }

            string varName;

            while (true)
            {
                varName = GetRandomString(_rnd.Next(8, 16 + 1));
                if (!str.Contains(varName) && _getVariableStringGenerated.All(g => g != varName))
                {
                    break;
                }
            }

            _getVariableStringGenerated.Add(varName);

            return varName;
        }

        private void Encode(ref string str)
        {
            var resultSb = new StringBuilder(str.Length);

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
                if (!insideString && str[i] == '/' && str[i + 1] == '/' && str[i + 2] == '-')
                {
                    i++;
                    i++;
                    continue;
                }
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

                // Encode non-strings
                if (!insideString)
                {
                    // Encode
                    if (str[i] == '\r' ||
                        str[i] == '\n' ||
                        str[i] == '\t')
                    {
                        continue;
                    }

                    if (str[i] == ' ')
                    {
                        if ((!char.IsLetterOrDigit(str[i - 1]) && str[i - 1] != '_' && str[i - 1] != '@') ||
                            (!char.IsLetterOrDigit(str[i + 1]) && str[i + 1] != '_' && str[i + 1] != '@'))
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
                        resultSb.Append('\0');
                        continue;
                    }
                    if (str[i] == '<')
                    {
                        resultSb.Append('\u0001');
                        continue;
                    }
                    if (str[i] == '>')
                    {
                        resultSb.Append('\u0002');
                        continue;
                    }
                }

                resultSb.Append(str[i]);
            }

            str = resultSb.ToString();
        }

        private void Trash(ref string str)
        {
            while (true)
            {
                // Check for tag
                var tagIndex = str.IndexOf("<trash", StringComparison.Ordinal);
                if (tagIndex == -1)
                {
                    break;
                }

                int trashAmount;

                // Check for close tag
                var tagLength = str.Substring(tagIndex).IndexOf(">", StringComparison.Ordinal);
                if (tagLength == -1)
                {
                    throw new Exception("close tag not found");
                }

                // Are arguments given?
                if (tagLength > 7)
                {
                    // Substring arguments
                    var argStr = str.Substring(tagIndex + 7, tagLength - 7);

                    // Multiple arguments
                    if (argStr.IndexOf(' ') != -1)
                    {
                        var argsStr = argStr.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                        if (argsStr.Length != 2)
                        {
                            throw new Exception($"invalid argument count: {argsStr.Length}");
                        }

                        // Parse
                        var args = new int[argsStr.Length];
                        for (var i = 0; i < argsStr.Length; i++)
                        {
                            if (!int.TryParse(argsStr[i], out args[i]))
                            {
                                throw new Exception($"invalid argument value: {argsStr[i]}");
                            }
                        }

                        // Check values
                        if (args[0] > args[1] + 1)
                        {
                            throw new Exception($"invalid argument value: {args[0]} <= {args[1] + 1}");
                        }

                        trashAmount = _rnd.Next(args[0], args[1] + 1);
                    }
                    // Single argument
                    else
                    {
                        int arg;
                        if (!int.TryParse(argStr, out arg))
                        {
                            throw new Exception($"invalid argument value: {argStr}");
                        }

                        trashAmount = _rnd.Next(1, arg + 1);
                    }
                }
                // No arguments
                else
                {
                    trashAmount = _rnd.Next(1, 6 + 1);
                }

                // Indexes:
                // 0 - string trash
                // 1 - value trash
                // 2 - object trash
                var trashChances = new[] { 25, 60, 15 };
                var allTrashChance = trashChances.Sum();

                // Add all before values
                for (var j = 1; j < trashChances.Length; j++)
                {
                    trashChances[j] += trashChances[j - 1];
                }

                // Add trash
                var trash = string.Empty;
                for (var i = 0; i < trashAmount; i++)
                {
                    var trashChancesIndex = 0;
                    var rndValue = _rnd.Next(0, allTrashChance);

                    if (rndValue < trashChances[trashChancesIndex++])
                    {
                        // string trash
                        var varValue = GetRandomString(_rnd.Next(8, 32 + 1));
                        string operation;

                        switch (_rnd.Next(0, 6))
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
                            case 3:
                            {
                                operation = "ToLowerInvariant()";
                                break;
                            }
                            case 4:
                            {
                                operation = "ToUpperInvariant()";
                                break;
                            }
                            case 5:
                            {
                                operation = "ToCharArray()";
                                break;
                            }
                            default:
                            {
                                throw new Exception("invalid switch value");
                            }
                        }

                        trash += $"\"{varValue}\".{operation};";
                    }
                    else if(rndValue < trashChances[trashChancesIndex++])
                    {
                        // value trash
                        var varName = GetVariableName(str);
                        var operation = _rnd.Next(0, 2) == 0 ? '+' : '-';

                        string varType;
                        int varValue;
                        int varChange;

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
                            default:
                            {
                                throw new Exception("invalid switch value");
                            }
                        }

                        trash += $"{varType} {varName}={varValue}\0{varName}{operation}={varChange};";
                    }
                    else if (rndValue < trashChances[trashChancesIndex++])
                    {
                        // object trash
                        var operation = _rnd.Next(0, 2) == 0 ? "GetHashCode()" : "GetTypeCode()";

                        switch (_rnd.Next(0, 6))
                        {
                            case 0:
                                {
                                    trash += $"new byte().{operation};";
                                    break;
                                }
                            case 1:
                                {
                                    trash += $"new bool().{operation};";
                                    break;
                                }
                            case 2:
                                {
                                    trash += $"new char().{operation};";
                                    break;
                                }
                            case 3:
                                {
                                    trash += $"new short().{operation};";
                                    break;
                                }
                            case 4:
                                {
                                    trash += $"new int().{operation};";
                                    break;
                                }
                            case 5:
                                {
                                    trash += $"new long().{operation};";
                                    break;
                                }
                            default:
                                {
                                    throw new Exception("invalid switch value");
                                }
                        }
                    }
                }

                // Replace
                str = str.Remove(tagIndex, tagLength + 1).Insert(tagIndex, trash);
            }
        }

        private void Flow(ref string str)
        {
            while (true)
            {
                // Check for tag
                var tagIndex = str.IndexOf("<flow>", StringComparison.Ordinal);
                if (tagIndex == -1)
                {
                    break;
                }

                const int tagLength = 6;
                const int endTagLength = 7;

                var afterStr = str.Substring(tagIndex + tagLength);

                // Check for close tag
                var endTagIndex = afterStr.IndexOf("<flow/>", StringComparison.Ordinal);
                if (endTagIndex == -1)
                {
                    throw new Exception("close tag not found");
                }

                // Substring inner content
                var innerStr = afterStr.Substring(0, endTagIndex);

                var rawBlocks = GetCodeBlocks(innerStr);
                var emptyBlocks = _rnd.Next(0, rawBlocks.Length / 2 + 1);
                var blocks = new string[rawBlocks.Length + emptyBlocks];

                // Fill blocks
                for (var i = 0; i < rawBlocks.Length; i++)
                {
                    blocks[i] = rawBlocks[i];
                }
                for (var i = rawBlocks.Length; i < blocks.Length; i++)
                {
                    blocks[i] = string.Empty;
                }

                var leftXors = new int[blocks.Length];
                var rightXors = new int[blocks.Length];
                var switchValues = new int[blocks.Length];

                // Generate switch values
                for (var i = 0; i < switchValues.Length; i++)
                {
                    while (true)
                    {
                        var leftXor = _rnd.Next(int.MinValue, int.MaxValue);
                        var rightXor = _rnd.Next(int.MinValue, int.MaxValue);
                        var switchValue = leftXor ^ rightXor;

                        if (switchValues.All(v => v != switchValue))
                        {
                            leftXors[i] = leftXor;
                            rightXors[i] = rightXor;
                            switchValues[i] = switchValue;
                            break;
                        }
                    }
                }

                // Generate variable names
                var switchVarName = GetVariableName(str);
                var exitLoopVarName = GetVariableName(str);

                var funcName = GetVariableName(str);
                var leftName = GetVariableName(str);
                var rightName = GetVariableName(str);

                // Generate xor function
                var xorFunc = $"Action<int,int>{funcName}=({leftName},{rightName})=>{switchVarName}={leftName}^{rightName};";

                var cases = new string[switchValues.Length];

                // Fill cases
                for (var i = 0; i < cases.Length; i++)
                {
                    // Last
                    if (i + 1 == cases.Length)
                    {
                        cases[i] = $"case {switchValues[i]}:{{{blocks[i]}{exitLoopVarName}=false\0break\0}}";
                    }
                    // Not last
                    else
                    {
                        cases[i] = $"case {switchValues[i]}:{{{blocks[i]}{funcName}({leftXors[i + 1]},{rightXors[i + 1]});\0break\0}}<block>";
                    }
                }

                // Generate output
                var caseOutput = cases.Aggregate(string.Empty, (current, c) => current + c);
                var output = $"<swap>int {switchVarName}={switchValues[0]}\0{xorFunc}bool {exitLoopVarName}=true;<swap/>while({exitLoopVarName}){{switch({switchVarName}){{<swap>{caseOutput}<swap/>}}}}";

                // Remove old
                str = str.Remove(tagIndex, tagLength + endTagIndex + endTagLength);

                // Insert new
                str = str.Insert(tagIndex, output);
            }
        }

        private void Swap(ref string str)
        {
            while (true)
            {
                // Check for tag
                var tagIndex = str.IndexOf("<swap>", StringComparison.Ordinal);
                if (tagIndex == -1)
                {
                    break;
                }

                const int tagLength = 6;
                const int endTagLength = 7;

                var afterStr = str.Substring(tagIndex + tagLength);
                
                // Check for close tag
                var endTagIndex = afterStr.IndexOf("<swap/>", StringComparison.Ordinal);
                if (endTagIndex == -1)
                {
                    throw new Exception("close tag not found");
                }

                // Substring inner content
                var innerStr = afterStr.Substring(0, endTagIndex);

                var blocks = GetCodeBlocks(innerStr);

                var swapped = new string[blocks.Length];

                // Swap
                foreach (var b in blocks)
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

                    swapped[swapIndex] = b;
                }

                // Remove old
                str = str.Remove(tagIndex, tagLength + endTagIndex + endTagLength);

                // Insert new
                var output = swapped.Aggregate(string.Empty, (current, s) => current + s);
                str = str.Insert(tagIndex, output);
            }
        }

        private void Decode(ref string str)
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

                if (str[i] == '\0')
                {
                    str = str.Remove(i, 1).Insert(i, ";");
                }

                // Skip non-strings
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

        private string[] GetCodeBlocks(string str)
        {
            string[] blocks;

            // Get blocks
            if (str.IndexOf("<block>", StringComparison.Ordinal) != -1)
            {
                var blockList = new List<string>();

                // Find blocks
                while (true)
                {
                    // Are blocks available?
                    var blockIndex = str.IndexOf("<block>", StringComparison.Ordinal);
                    if (blockIndex == -1)
                    {
                        // Add last block
                        blockList.Add(str);
                        break;
                    }

                    const int blockTagLength = 7;

                    // Get block's content
                    var innerBlockStr = str.Substring(0, blockIndex);
                    blockList.Add(innerBlockStr);

                    // Remove from search
                    str = str.Remove(0, blockIndex + blockTagLength);
                }

                blocks = blockList.ToArray();
            }
            // Get lines
            else
            {
                // Split
                var lines = str.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                for (var i = 0; i < lines.Length; i++)
                {
                    lines[i] += ';';
                }

                blocks = lines;
            }

            return blocks;
        }
    }
}
