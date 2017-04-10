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
        public readonly List<string> InjectSources = new List<string>();
        public readonly List<string> InjectAssemblies = new List<string>();

        public readonly string InjectBytesNamespace;
        public readonly string InjectBytesClass;
        public readonly string InjectBytesProperty;
        public readonly List<byte> InjectBytes = new List<byte>();

        public readonly string InjectBoolsNamespace;
        public readonly string InjectBoolsClass;
        public readonly string InjectBoolsProperty;
        public readonly List<bool> InjectBools = new List<bool>();

        public readonly string InjectIntsNamespace;
        public readonly string InjectIntsClass;
        public readonly string InjectIntsProperty;
        public readonly List<int> InjectInts = new List<int>();

        public readonly string InjectStringsNamespace;
        public readonly string InjectStringsClass;
        public readonly string InjectStringsProperty;
        public readonly List<string> InjectStrings = new List<string>();

        private readonly Random _rnd;
        private readonly int _seed;

        public SourceRandomizer(int seed)
        {
            _rnd = new Random(seed);
            _seed = seed;

            InjectBytesNamespace = GetRandomName();
            InjectBytesClass = GetRandomName();
            InjectBytesProperty = GetRandomName();

            InjectBoolsNamespace = GetRandomName();
            InjectBoolsClass = GetRandomName();
            InjectBoolsProperty = GetRandomName();

            InjectIntsNamespace = GetRandomName();
            InjectIntsClass = GetRandomName();
            InjectIntsProperty = GetRandomName();

            InjectStringsNamespace = GetRandomName();
            InjectStringsClass = GetRandomName();
            InjectStringsProperty = GetRandomName();
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
            Swap(ref source);
            Flow(ref source);
            Swap(ref source);
            Decode(ref source);
        }

        public string GetRandomString(int length)
        {
            //const string source = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";

            var rndStrSb = new StringBuilder(length);

            for (var i = 0; i < length; i++)
            {
                rndStrSb.Append((char)_rnd.Next(0x30a1, 0x30fb));
            }

            return rndStrSb.ToString();
        }

        private readonly List<string> _namesGenerated = new List<string>();
        private string GetRandomName()
        {
            string varName;

            while (true)
            {
                varName = GetRandomString(_rnd.Next(8, 16 + 1));
                if (_namesGenerated.All(g => g != varName))
                {
                    break;
                }
            }

            _namesGenerated.Add(varName);

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
                str = str.Remove(tagIndex, tagLength + 1).Insert(tagIndex, Inject(outputValue));
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
                str = str.Remove(tagIndex, tagLength + 1).Insert(tagIndex, Inject(output));
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

                        trash += $"{Inject(varValue)}.{operation};";
                    }
                    else if(rndValue < trashChances[trashChancesIndex++])
                    {
                        // value trash
                        var varName = GetRandomName();
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

                        trash += $"{varType} {varName}=({varType}){Inject(varValue)}\0{varName}{operation}=({varType}){Inject(varChange)};";
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
        private bool _charDecryptorInjected;
        private string _charDecryptorFunction;
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
                if (arg.EndsWith("\""))
                {
                    // Inject decryptor
                    if (!_stringDecryptorInjected)
                    {
                        var namespaceName = GetRandomName();
                        var className = GetRandomName();
                        var funcName = GetRandomName();

                        var funcArg = GetRandomName();

                        var result = GetRandomName();
                        var i = GetRandomName();

                        var a = GetRandomName();
                        var b = GetRandomName();
                        var c = GetRandomName();
                        var d = GetRandomName();

                        InjectSources.Add($"using System;" +
                                   $"using System.Text;" +
                                   $"namespace {namespaceName}" +
                                   $"{{" +
                                   $"public static class {className}" +
                                   $"{{" +
                                   $"private static byte[] {result};" +
                                   $"private static int {a};" +
                                   $"private static int {b};" +
                                   $"private static int {c};" +
                                   $"private static int {d};" +
                                   $"public static string {funcName}(byte[] {funcArg})" +
                                   $"{{" +
                                   $"<flow>" +
                                   $"<swap>" +
                                   $"{result} = new byte[{funcArg}.Length - {Inject(2)}];" +
                                   $"<trash>" +
                                   $"<swap/>" +
                                   $"<flow/>" +
                                   $"for(int {i} = {Inject(0)}; {i} < {result}.Length; {i}++)" +
                                   $"{{" +
                                   $"<flow>" +
                                   $"{a} = {funcArg}[{Inject(0)}] % {funcArg}[{Inject(1)}];" +
                                   $"<trash 4>" +
                                   $"{b} = {i} + {funcArg}[{Inject(1)}];" +
                                   $"<trash 4>" +
                                   $"{c} = {a} * {b};" +
                                   $"<trash 4>" +
                                   $"{d} = {c} ^ {funcArg}[{i} + {Inject(2)}];" +
                                   $"<trash 4>" +
                                   $"{result}[{i}] = (byte){d};" +
                                   $"<trash 4>" +
                                   $"<flow/>" +
                                   $"}}" +
                                   $"<flow>" +
                                   $"<trash>" +
                                   $"return Encoding.Unicode.GetString({result});" +
                                   $"<flow/>" +
                                   $"return string.Empty;" +
                                   $"}}" +
                                   $"}}" +
                                   $"}}");

                        InjectAssemblies.Add("System.dll");

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

                    {
                        var stringBytes = Encoding.Unicode.GetBytes(rawString);

                        var tmp1 = new byte[stringBytes.Length + 2];

                        tmp1[0] = (byte)_rnd.Next(byte.MinValue, byte.MaxValue);
                        tmp1[1] = (byte)_rnd.Next(1, byte.MaxValue);

                        for (var i = 0; i < stringBytes.Length; i++)
                        {
                            tmp1[i + 2] = (byte)(tmp1[0] % tmp1[1] * (i + tmp1[1]) ^ stringBytes[i]);
                        }

                        var funcArg = ByteArrayToString(tmp1);

                        var output = $"{_stringDecryptorFunction}({funcArg})";

                        // Replace
                        str = str.Remove(tagIndex, tagLength + 1).Insert(tagIndex, output);
                    }
                }
                // Char
                else if (arg.EndsWith("'"))
                {
                    // Inject decryptor
                    if (!_charDecryptorInjected)
                    {
                        var namespaceName = GetRandomName();
                        var className = GetRandomName();
                        var funcName = GetRandomName();

                        var funcArg = GetRandomName();

                        var result = GetRandomName();
                        var i = GetRandomName();

                        var a = GetRandomName();
                        var b = GetRandomName();
                        var c = GetRandomName();
                        var d = GetRandomName();

                        InjectSources.Add($"using System;" +
                                   $"using System.Text;" +
                                   $"namespace {namespaceName}" +
                                   $"{{" +
                                   $"public static class {className}" +
                                   $"{{" +
                                   $"private static byte[] {result};" +
                                   $"private static int {a};" +
                                   $"private static int {b};" +
                                   $"private static int {c};" +
                                   $"private static int {d};" +
                                   $"public static char {funcName}(byte[] {funcArg})" +
                                   $"{{" +
                                   $"<flow>" +
                                   $"<swap>" +
                                   $"{result} = new byte[{funcArg}.Length - {Inject(2)}];" +
                                   $"<trash>" +
                                   $"<swap/>" +
                                   $"<flow/>" +
                                   $"for(int {i} = {Inject(0)}; {i} < {result}.Length; {i}++)" +
                                   $"{{" +
                                   $"<flow>" +
                                   $"{a} = {funcArg}[{Inject(0)}] % {funcArg}[{Inject(1)}];" +
                                   $"<trash 4>" +
                                   $"{b} = {i} + {funcArg}[{Inject(1)}];" +
                                   $"<trash 4>" +
                                   $"{c} = {a} * {b};" +
                                   $"<trash 4>" +
                                   $"{d} = {c} ^ {funcArg}[{i} + {Inject(2)}];" +
                                   $"<trash 4>" +
                                   $"{result}[{i}] = (byte){d};" +
                                   $"<trash 4>" +
                                   $"<flow/>" +
                                   $"}}" +
                                   $"<flow>" +
                                   $"<trash>" +
                                   $"return BitConverter.ToChar({result}, 0);" +
                                   $"<flow/>" +
                                   $"return \'\\0\';" +
                                   $"}}" +
                                   $"}}" +
                                   $"}}");

                        InjectAssemblies.Add("System.dll");

                        _charDecryptorInjected = true;
                        _charDecryptorFunction = $"{namespaceName}.{className}.{funcName}";
                    }

                    char rawChar;

                    // Are arguments given?
                    if (tagLength > 6)
                    {
                        // Substring arguments
                        rawChar = str.Substring(tagIndex + 5, tagLength - 5).Trim('\'')[0];
                    }
                    // No arguments
                    else
                    {
                        throw new Exception($"invalid argument value: null");
                    }

                    {
                        var stringBytes = BitConverter.GetBytes(rawChar);

                        var tmp1 = new byte[stringBytes.Length + 2];

                        tmp1[0] = (byte)_rnd.Next(byte.MinValue, byte.MaxValue);
                        tmp1[1] = (byte)_rnd.Next(1, byte.MaxValue);

                        for (var i = 0; i < stringBytes.Length; i++)
                        {
                            tmp1[i + 2] = (byte)(tmp1[0] % tmp1[1] * (i + tmp1[1]) ^ stringBytes[i]);
                        }

                        var funcArg = ByteArrayToString(tmp1);

                        var output = $"{_charDecryptorFunction}({funcArg})";

                        // Replace
                        str = str.Remove(tagIndex, tagLength + 1).Insert(tagIndex, output);
                    }
                }
                // Value
                else
                {
                    // Inject decryptor
                    if (!_valueDecryptorInjected)
                    {
                        var namespaceName = GetRandomName();
                        var className = GetRandomName();
                        var funcName = GetRandomName();

                        var funcArg = GetRandomName();

                        var encrypted = GetRandomName();

                        var a = GetRandomName();
                        var b = GetRandomName();
                        var c = GetRandomName();

                        InjectSources.Add($"using System;" +
                                   $"namespace {namespaceName}" +
                                   $"{{" +
                                   $"public static class {className}" +
                                   $"{{" +
                                   $"private static int {encrypted};" +
                                   $"private static int {a};" +
                                   $"private static int {b};" +
                                   $"private static int {c};" +
                                   $"public static int {funcName}(byte[] {funcArg})" +
                                   $"{{" +
                                   $"<flow>" +
                                   $"<swap>" +
                                   $"{encrypted} = BitConverter.ToInt32({funcArg}, {Inject(2)});" +
                                   $"<trash>" +
                                   $"<swap/>" +
                                   $"{a} = {funcArg}[{Inject(0)}] % {funcArg}[{Inject(1)}];" +
                                   $"<trash 4>" +
                                   $"{b} = {a} * {funcArg}[{Inject(1)}];" +
                                   $"<trash 4>" +
                                   $"{c} = {b} ^ {encrypted};" +
                                   $"<trash 4>" +
                                   $"return {c};" +
                                   $"<flow/>" +
                                   $"return 0;" +
                                   $"}}" +
                                   $"}}" +
                                   $"}}");

                        InjectAssemblies.Add("System.dll");

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

                    {
                        var tmp1 = new byte[6];

                        tmp1[0] = (byte)_rnd.Next(byte.MinValue, byte.MaxValue);
                        tmp1[1] = (byte)_rnd.Next(1, byte.MaxValue);

                        var encrypted = tmp1[0] % tmp1[1] * tmp1[1] ^ rawValue;

                        var tmp2 = BitConverter.GetBytes(encrypted);
                        Array.Copy(tmp2, 0, tmp1, 2, 4);

                        var funcArg = ByteArrayToString(tmp1);

                        var output = $"{_valueDecryptorFunction}({funcArg})";

                        // Replace
                        str = str.Remove(tagIndex, tagLength + 1).Insert(tagIndex, output);
                    }
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
                    var argStr = str.Substring(tagIndex + 7, tagLength - 7);

                    // String proxy
                    if (argStr.EndsWith("\""))
                    {
                        var output = Inject(argStr.Trim('"'));

                        // Replace
                        str = str.Remove(tagIndex, tagLength + 1).Insert(tagIndex, output);

                        continue;
                    }
                    // Char proxy
                    if (argStr.EndsWith("'"))
                    {
                        var output = $"{Inject(argStr.Trim('\''))}[{Inject(0)}]";

                        // Replace
                        str = str.Remove(tagIndex, tagLength + 1).Insert(tagIndex, output);

                        continue;
                    }
                    // Value proxy
                    if (argStr.ToCharArray().All(char.IsDigit))
                    {
                        int arg;

                        // Hex
                        if (argStr.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                        {
                            argStr = argStr.Remove(0, 2);
                            if (!int.TryParse(argStr, NumberStyles.HexNumber, null, out arg))
                            {
                                throw new Exception($"invalid argument value: {str.Substring(tagIndex + 5, tagLength - 5)}");
                            }
                        }
                        // Dec
                        else
                        {
                            if (!int.TryParse(argStr, out arg))
                            {
                                throw new Exception($"invalid argument value: {str.Substring(tagIndex + 5, tagLength - 5)}");
                            }
                        }

                        var output = Inject(arg);

                        // Replace
                        str = str.Remove(tagIndex, tagLength + 1).Insert(tagIndex, output);

                        continue;
                    }
                    // Namespace
                    argNamespace = argStr;
                }
                // No arguments
                else
                {
                    argNamespace = GetRandomName();
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
                    var namespaceName = argNamespace;
                    var className = GetRandomName();
                    var funcName = GetRandomName();

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

                {
                    var output = funcs.Aggregate(string.Empty, (current, func) => current + func);

                    // Replace
                    str = str.Remove(tagIndex, tagLength + 1 + endTagIndex + endTagLength).Insert(tagIndex, output);
                }
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
                    var namespaceName = GetRandomName();
                    var className = GetRandomName();
                    var funcName = GetRandomName();

                    var baseDecryptor = GetRandomName();

                    var tmp1 = GetRandomName();
                    var tmp2 = GetRandomName();

                    var decryptorOne = GetRandomName();
                    var decryptorTwo = GetRandomName();

                    var result = GetRandomName();

                    InjectSources.Add($"using System;" +
                               $"namespace {namespaceName}" +
                               $"{{" +
                               $"public static class {className}" +
                               $"{{" +
                               $"private static byte[] {tmp1} = new byte[{Inject(4)}];" +
                               $"private static byte[] {tmp2} = new byte[{Inject(4)}];" +
                               $"private static int {decryptorOne};" +
                               $"private static int {decryptorTwo};" +
                               $"private static int {result};" +
                               $"public static int {funcName}(byte[] {baseDecryptor})" +
                               $"{{" +
                               $"<swap>" +
                               $"Array.Copy({baseDecryptor}, {Inject(0)}, {tmp1}, {Inject(0)}, {Inject(4)});" +
                               $"Array.Copy({baseDecryptor}, {Inject(4)}, {tmp2}, {Inject(0)}, {Inject(4)});" +
                               $"<trash>" +
                               $"<swap/>" +
                               $"<swap>" +
                               $"{decryptorOne} = BitConverter.ToInt32({tmp1}, {Inject(0)});" +
                               $"{decryptorTwo} = BitConverter.ToInt32({tmp2}, {Inject(0)});" +
                               $"<trash>" +
                               $"<swap/>" +
                               $"<swap>" +
                               $"{result} = {decryptorOne} ^ {decryptorTwo};" +
                               $"<trash>" +
                               $"<swap/>" +
                               $"return {result};" +
                               $"}}" +
                               $"}}" +
                               $"}}");

                    InjectAssemblies.Add("System.dll");

                    _xorDecryptorInjected = true;
                    _xorDecryptorFunction = $"{namespaceName}.{className}.{funcName}";
                }

                // Substring inner content
                var innerStr = afterStr.Substring(0, endTagIndex);

                var rawBlocks = GetCodeBlocks(innerStr);
                var emptyBlocks = _rnd.Next(0, rawBlocks.Length / 2 + 1);
                var finalBlocks = new string[rawBlocks.Length + emptyBlocks];

                // Fill blocks
                for (var i = 0; i < rawBlocks.Length; i++)
                {
                    finalBlocks[i] = rawBlocks[i];
                }
                for (var i = rawBlocks.Length; i < finalBlocks.Length; i++)
                {
                    finalBlocks[i] = string.Empty;
                }

                {
                    var leftXors = new int[finalBlocks.Length];
                    var rightXors = new int[finalBlocks.Length];
                    var switchValues = new int[finalBlocks.Length];

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
                    var switchName = GetRandomName();
                    var doLoopName = GetRandomName();

                    var cases = new string[switchValues.Length];

                    // Fill cases
                    for (var i = 0; i < cases.Length; i++)
                    {
                        // Last
                        if (i + 1 == cases.Length)
                        {
                            cases[i] = $"case {switchValues[i]}:{{{finalBlocks[i]}{doLoopName}={Inject(false)};break;}}";
                        }
                        // Not last
                        else
                        {
                            var tmp2 = BitConverter.GetBytes(leftXors[i + 1]);
                            var tmp3 = BitConverter.GetBytes(rightXors[i + 1]);
                            var tmp1 = new byte[8];
                            Array.Copy(tmp2, 0, tmp1, 0, 4);
                            Array.Copy(tmp3, 0, tmp1, 4, 4);
                            var baseDecryptor = ByteArrayToString(tmp1);

                            cases[i] = $"case {switchValues[i]}:{{{finalBlocks[i]}{switchName}={_xorDecryptorFunction}({baseDecryptor});break;}}<block>";
                        }
                    }

                    // Generate output
                    var caseOutput = cases.Aggregate(string.Empty, (current, c) => current + c);
                    var innerOutput = cases.Length < 2 ? caseOutput : $"<swap>{caseOutput}<swap/>";
                    var output = $"<swap>int {switchName}={Inject(switchValues[0])};bool {doLoopName}={Inject(true)};<swap/>while({doLoopName}){{switch({switchName}){{{innerOutput}}}}}";

                    // Replace
                    str = str.Remove(tagIndex, tagLength + endTagIndex + endTagLength).Insert(tagIndex, output);
                }
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

        private string ByteArrayToString(byte[] byteArray)
        {
            var returnSb = new StringBuilder("new byte[] {");
            foreach (var b in byteArray)
            {
                returnSb.Append($"{Inject(b)},");
            }
            returnSb.Remove(returnSb.Length - 1, 1);
            returnSb.Append("}");
            return returnSb.ToString();
        }

        private string Inject(byte val)
        {
            int index;

            if (InjectBytes.Any(b => b == val))
            {
                index = InjectBytes.IndexOf(val);
            }
            else
            {
                InjectBytes.Add(val);
                index = InjectBytes.Count - 1;
            }

            return $"{InjectBytesNamespace}.{InjectBytesClass}.{InjectBytesProperty}[{index}]";
        }

        private string Inject(bool val)
        {
            int index;

            if (InjectBools.Any(b => b == val))
            {
                index = InjectBools.IndexOf(val);
            }
            else
            {
                InjectBools.Add(val);
                index = InjectBools.Count - 1;
            }

            return $"{InjectBoolsNamespace}.{InjectBoolsClass}.{InjectBoolsProperty}[{index}]";
        }

        private string Inject(int val)
        {
            int index;

            if (InjectInts.Any(b => b == val))
            {
                index = InjectInts.IndexOf(val);
            }
            else
            {
                InjectInts.Add(val);
                index = InjectInts.Count - 1;
            }

            return $"{InjectIntsNamespace}.{InjectIntsClass}.{InjectIntsProperty}[{index}]";
        }

        private string Inject(string val)
        {
            int index;

            if (InjectStrings.Any(b => b == val))
            {
                index = InjectStrings.IndexOf(val);
            }
            else
            {
                InjectStrings.Add(val);
                index = InjectStrings.Count - 1;
            }

            return $"{InjectStringsNamespace}.{InjectStringsClass}.{InjectStringsProperty}[{index}]";
        }
    }
}
