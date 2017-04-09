using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SharpLoader.Core
{
    public class SourceRandomizer
    {
        public readonly List<string> Inject;

        private readonly Random _rnd;
        private readonly int _seed;

        public SourceRandomizer(int seed)
        {
            Inject = new List<string>();

            _rnd = new Random(seed);
            _seed = seed;
        }

        public void Randomize(ref string source)
        {
            Encode(ref source);
            Replace(ref source);
            RandomS(ref source);
            Random(ref source);
            Trash(ref source);
            Encrypt(ref source);
            Proxy(ref source);
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
            var insideString = 0;

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
                if (insideString == 0 && str[i] == '/' && str[i + 1] == '/' && str[i + 2] == '-')
                {
                    i++;
                    i++;
                    continue;
                }
                if (insideString == 0 && str[i] == '/' && str[i + 1] == '/')
                {
                    i++;
                    insideComment = 1;
                    continue;
                }
                if (insideString == 0 && str[i] == '/' && str[i + 1] == '*')
                {
                    i++;
                    insideComment = 2;
                    continue;
                }

                // Check insideString
                if (insideString == 0)
                {
                    if (str[i] == '\"' && str[i - 1] != '\\')
                    {
                        insideString = 1;
                    }
                    else if (str[i] == '\'' && str[i - 1] != '\\')
                    {
                        insideString = 2;
                    }
                }
                else if (insideString == 1)
                {
                    if (str[i] == '\"' && str[i - 1] != '\\')
                    {
                        insideString = 0;
                    }
                }
                else if (insideString == 2)
                {
                    if (str[i] == '\'' && str[i - 1] != '\\')
                    {
                        insideString = 0;
                    }
                }
                
                // Encode non-strings
                if (insideString == 0)
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

        private void Replace(ref string str)
        {
            str = str.Replace("<seed>", _seed.ToString());
        }

        private void Random(ref string str)
        {
            while (true)
            {
                // Check for tag
                var tagIndex = str.IndexOf("<rnd", StringComparison.Ordinal);
                if (tagIndex == -1)
                {
                    break;
                }

                // Check for close tag
                var tagLength = str.Substring(tagIndex).IndexOf(">", StringComparison.Ordinal);
                if (tagLength == -1)
                {
                    throw new Exception("close tag not found");
                }

                int outputValue;

                // Are arguments given?
                if (tagLength > 5)
                {
                    // Substring arguments
                    var argStr = str.Substring(tagIndex + 5, tagLength - 5);

                    // Multiple arguments
                    if (argStr.IndexOf(' ') != -1)
                    {
                        var argsStr = argStr.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
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

                        outputValue = _rnd.Next(args[0], args[1] + 1);
                    }
                    // Single argument
                    else
                    {
                        if (!int.TryParse(argStr, out int arg))
                        {
                            throw new Exception($"invalid argument value: {argStr}");
                        }

                        outputValue = _rnd.Next(0, arg + 1);
                    }
                }
                // No arguments
                else
                {
                    outputValue = _rnd.Next(int.MinValue, int.MaxValue);
                }

                // Replace
                str = str.Remove(tagIndex, tagLength + 1).Insert(tagIndex, outputValue.ToString());
            }
        }

        private void RandomS(ref string str)
        {
            while (true)
            {
                // Check for tag
                var tagIndex = str.IndexOf("<rnds", StringComparison.Ordinal);
                if (tagIndex == -1)
                {
                    break;
                }

                // Check for close tag
                var tagLength = str.Substring(tagIndex).IndexOf(">", StringComparison.Ordinal);
                if (tagLength == -1)
                {
                    throw new Exception("close tag not found");
                }

                string output;

                // Are arguments given?
                if (tagLength > 6)
                {
                    // Substring arguments
                    var argStr = str.Substring(tagIndex + 6, tagLength - 6);

                    // Multiple arguments
                    if (argStr.IndexOf(' ') != -1)
                    {
                        var argsStr = argStr.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
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

                        output = GetRandomString(_rnd.Next(args[0], args[1] + 1));
                    }
                    // Single argument
                    else
                    {
                        if (!int.TryParse(argStr, out int arg))
                        {
                            throw new Exception($"invalid argument value: {argStr}");
                        }

                        output = GetRandomString(_rnd.Next(1, arg + 1));
                    }
                }
                // No arguments
                else
                {
                    output = GetRandomString(_rnd.Next(8, 16 + 1));
                }

                // Replace
                str = str.Remove(tagIndex, tagLength + 1).Insert(tagIndex, output);
            }
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

                // Check for close tag
                var tagLength = str.Substring(tagIndex).IndexOf(">", StringComparison.Ordinal);
                if (tagLength == -1)
                {
                    throw new Exception("close tag not found");
                }

                int trashAmount;

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
                        if (!int.TryParse(argStr, out int arg))
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

        private bool _stringDecryptorInjected;
        private string _stringDecryptorFunction;
        private bool _valueDecryptorInjected;
        private string _valueDecryptorFunction;
        private void Encrypt(ref string str)
        {
            while (true)
            {
                // Check for tag
                var tagIndex = str.IndexOf("<enc", StringComparison.Ordinal);
                if (tagIndex == -1)
                {
                    break;
                }

                // Check for close tag
                var tagLength = str.Substring(tagIndex).IndexOf(">", StringComparison.Ordinal);
                if (tagLength == -1)
                {
                    throw new Exception("close tag not found");
                }

                // Substring arguments
                var arg = str.Substring(tagIndex + 5, tagLength - 5);

                // String
                if (arg.Contains('"'))
                {
                    // Inject decryptor
                    if (!_stringDecryptorInjected)
                    {
                        var namespaceName = GetVariableName(str);
                        var className = GetVariableName(str);
                        var funcName = GetVariableName(str);

                        var encryptedArgName = GetVariableName(str);
                        var keyArgName = GetVariableName(str);
                        var deltaArgName = GetVariableName(str);

                        var encrypted = GetVariableName(str);
                        var decrypted = GetVariableName(str);
                        var i = GetVariableName(str);

                        var c1ArgName = GetVariableName(str);
                        var c2ArgName = GetVariableName(str);
                        var c3ArgName = GetVariableName(str);
                        var c4ArgName = GetVariableName(str);

                        Inject.Add($"using System;" +
                                   $"using System.Text;" +
                                   $"namespace {namespaceName}" +
                                   $"{{" +
                                   $"public static class {className}" +
                                   $"{{" +
                                   $"public static string {funcName}(string {encryptedArgName}, int {keyArgName}, byte {deltaArgName})" +
                                   $"{{" +
                                   $"byte[] {encrypted} = Convert.FromBase64String({encryptedArgName});" +
                                   $"<swap>" +
                                   $"byte[] {decrypted} = new byte[{encrypted}.Length];" +
                                   $"int {c1ArgName} = 0;" +
                                   $"int {c2ArgName} = 0;" +
                                   $"int {c3ArgName} = 0;" +
                                   $"int {c4ArgName} = 0;" +
                                   $"<trash>" +
                                   $"<swap/>" +
                                   $"for(int {i} = 0; {i} < {encrypted}.Length; {i}++)" +
                                   $"{{" +
                                   $"<flow>" +
                                   $"{c1ArgName} = {keyArgName} % {deltaArgName};" +
                                   $"{c2ArgName} = {i} + {deltaArgName};" +
                                   $"{c3ArgName} = {c1ArgName} * {c2ArgName};" +
                                   $"{c4ArgName} = {c3ArgName} ^ {encrypted}[{i}];" +
                                   $"{decrypted}[{i}] = (byte){c4ArgName};" +
                                   $"<trash>" +
                                   $"<flow/>" +
                                   $"}}" +
                                   $"<flow>" +
                                   $"<trash>" +
                                   $"return Encoding.Unicode.GetString({decrypted});" +
                                   $"<flow/>" +
                                   $"return string.Empty;" +
                                   $"}}" +
                                   $"}}" +
                                   $"}}");

                        _stringDecryptorInjected = true;
                        _stringDecryptorFunction = $"{namespaceName}.{className}.{funcName}";
                    }

                    string rawString;

                    // Are arguments given?
                    if (tagLength > 6)
                    {
                        // Substring arguments
                        rawString = str.Substring(tagIndex + 5, tagLength - 5).Trim('"');
                    }
                    // No arguments
                    else
                    {
                        throw new Exception($"invalid argument value: null");
                    }

                    var key = _rnd.Next(int.MinValue, int.MaxValue);
                    var delta = _rnd.Next(1, byte.MaxValue);

                    var rawBytes = Encoding.Unicode.GetBytes(rawString);
                    var encBytes = new byte[rawBytes.Length];

                    for (var i = 0; i < rawBytes.Length; i++)
                    {
                        encBytes[i] = (byte)(key % delta * (i + delta) ^ rawBytes[i]);
                    }

                    var encString = Convert.ToBase64String(encBytes);

                    var output = $"{_stringDecryptorFunction}(\"{encString}\",{key},{delta})";

                    // Replace
                    str = str.Remove(tagIndex, tagLength + 1).Insert(tagIndex, output);
                }
                // Value
                else
                {
                    // Inject decryptor
                    if (!_valueDecryptorInjected)
                    {
                        var namespaceName = GetVariableName(str);
                        var className = GetVariableName(str);
                        var funcName = GetVariableName(str);

                        var encryptedArgName = GetVariableName(str);
                        var keyArgName = GetVariableName(str);
                        var deltaArgName = GetVariableName(str);

                        var c1ArgName = GetVariableName(str);
                        var c2ArgName = GetVariableName(str);
                        var c3ArgName = GetVariableName(str);

                        Inject.Add($"using System;" +
                                   $"namespace {namespaceName}" +
                                   $"{{" +
                                   $"public static class {className}" +
                                   $"{{" +
                                   $"public static int {funcName}(int {encryptedArgName}, int {keyArgName}, byte {deltaArgName})" +
                                   $"{{" +
                                   $"<swap>" +
                                   $"int {c1ArgName} = 0;" +
                                   $"int {c2ArgName} = 0;" +
                                   $"int {c3ArgName} = 0;" +
                                   $"<trash>" +
                                   $"<swap/>" +
                                   $"<flow>" +
                                   $"{c1ArgName} = {keyArgName} % {deltaArgName};" +
                                   $"{c2ArgName} = {c1ArgName} * {deltaArgName};" +
                                   $"{c3ArgName} = {c2ArgName} ^ {encryptedArgName};" +
                                   $"<trash>" +
                                   $"return {c3ArgName};" +
                                   $"<flow/>" +
                                   $"return 0;" +
                                   $"}}" +
                                   $"}}" +
                                   $"}}");

                        _valueDecryptorInjected = true;
                        _valueDecryptorFunction = $"{namespaceName}.{className}.{funcName}";
                    }

                    int rawValue;

                    // Are arguments given?
                    if (tagLength > 5)
                    {
                        // Hex
                        if (arg.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                        {
                            arg = arg.Remove(0, 2);
                            if (!int.TryParse(arg, NumberStyles.HexNumber, null, out rawValue))
                            {
                                throw new Exception($"invalid argument value: {str.Substring(tagIndex + 5, tagLength - 5)}");
                            }
                        }
                        // Dec
                        else
                        {
                            if (!int.TryParse(arg, out rawValue))
                            {
                                throw new Exception($"invalid argument value: {str.Substring(tagIndex + 5, tagLength - 5)}");
                            }
                        }
                    }
                    // No arguments
                    else
                    {
                        throw new Exception($"invalid argument value: null");
                    }

                    var key = _rnd.Next(int.MinValue, int.MaxValue);
                    var delta = _rnd.Next(1, byte.MaxValue);

                    var encValue = key % delta * delta ^ rawValue;

                    var output = $"{_valueDecryptorFunction}({encValue},{key},{delta})";

                    // Replace
                    str = str.Remove(tagIndex, tagLength + 1).Insert(tagIndex, output);
                }
            }
        }

        private void Proxy(ref string str)
        {
            while (true)
            {
                // Check for tag
                var tagIndex = str.IndexOf("<proxy", StringComparison.Ordinal);
                if (tagIndex == -1)
                {
                    break;
                }

                // Check for close tag
                var tagLength = str.Substring(tagIndex).IndexOf(">", StringComparison.Ordinal);
                if (tagLength == -1)
                {
                    throw new Exception("close tag not found");
                }

                var argNamespace = string.Empty;

                // Are arguments given?
                if (tagLength > 7)
                {
                    // Substring arguments
                    argNamespace = str.Substring(tagIndex + 7, tagLength - 7);
                }
                // No arguments
                else
                {

                }

                const int endTagLength = 8;

                var afterStr = str.Substring(tagIndex + tagLength + 1);

                // Check for close tag
                var endTagIndex = afterStr.IndexOf("<proxy/>", StringComparison.Ordinal);
                if (endTagIndex == -1)
                {
                    throw new Exception("close tag not found");
                }

                // Substring inner content
                var innerStr = afterStr.Substring(0, endTagIndex);

                var blocks = GetCodeBlocks(innerStr);

                var funcs = new string[blocks.Length];

                for (var i = 0; i < blocks.Length; i++)
                {
                    var namespaceName = argNamespace == string.Empty ? GetVariableName(str) : argNamespace;
                    var className = GetVariableName(str);
                    var funcName = GetVariableName(str);

                    str += $"namespace {namespaceName}" +
                           $"{{" +
                           $"public static class {className}" +
                           $"{{" +
                           $"public static void {funcName}()" +
                           $"{{" +
                           $"{blocks[i]}" +
                           $"}}" +
                           $"}}" +
                           $"}}";

                    funcs[i] = $"{namespaceName}.{className}.{funcName}();";
                }

                var output = funcs.Aggregate(string.Empty, (current, func) => current + func);

                // Replace
                str = str.Remove(tagIndex, tagLength + 1 + endTagIndex + endTagLength).Insert(tagIndex, output);
            }
        }

        private bool _xorDecryptorInjected;
        private string _xorDecryptorFunction;
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

                // Inject decryptor
                if (!_xorDecryptorInjected)
                {
                    var namespaceName = GetVariableName(str);
                    var className = GetVariableName(str);
                    var funcName = GetVariableName(str);

                    var leftArgName = GetVariableName(str);
                    var rightArgName = GetVariableName(str);

                    var calcVarName = GetVariableName(str);

                    Inject.Add($"using System;" +
                               $"namespace {namespaceName}" +
                               $"{{" +
                               $"public static class {className}" +
                               $"{{" +
                               $"public static int {funcName}(int {leftArgName}, int {rightArgName})" +
                               $"{{" +
                               $"<swap>" +
                               $"int {calcVarName} = {leftArgName} ^ {rightArgName};" +
                               $"<trash>" +
                               $"<swap/>" +
                               $"return {calcVarName};" +
                               $"}}" +
                               $"}}" +
                               $"}}");

                    _xorDecryptorInjected = true;
                    _xorDecryptorFunction = $"{namespaceName}.{className}.{funcName}";
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
                        cases[i] = $"case {switchValues[i]}:{{{blocks[i]}{switchVarName}={_xorDecryptorFunction}({leftXors[i + 1]},{rightXors[i + 1]})\0break\0}}<block>";
                    }
                }

                // Generate output
                var caseOutput = cases.Aggregate(string.Empty, (current, c) => current + c);
                var innerOutput = cases.Length < 2 ? caseOutput : $"<swap>{caseOutput}<swap/>";
                var output = $"<swap>int {switchVarName}={switchValues[0]};bool {exitLoopVarName}=true;<swap/>while({exitLoopVarName}){{switch({switchVarName}){{{innerOutput}}}}}";

                // Replace
                str = str.Remove(tagIndex, tagLength + endTagIndex + endTagLength).Insert(tagIndex, output);
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

                var output = swapped.Aggregate(string.Empty, (current, s) => current + s);

                // Replace
                str = str.Remove(tagIndex, tagLength + endTagIndex + endTagLength).Insert(tagIndex, output);
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
