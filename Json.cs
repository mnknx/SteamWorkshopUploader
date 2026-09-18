using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SteamWorkshopUploader
{

    internal static class Json
    {

        public static object Parse(string text)
        {
            int i = 0;
            object value = Value(text, ref i);
            SkipWhitespace(text, ref i);
            return value;
        }

        private static object Value(string s, ref int i)
        {
            SkipWhitespace(s, ref i);
            if (i >= s.Length) throw new FormatException("Unexpected end of file.");

            char c = s[i];
            if (c == '{') return ObjectValue(s, ref i);
            if (c == '[') return ArrayValue(s, ref i);
            if (c == '"') return StringValue(s, ref i);

            if (Match(s, ref i, "true")) return true;
            if (Match(s, ref i, "false")) return false;
            if (Match(s, ref i, "null")) return null;

            return NumberValue(s, ref i);
        }

        private static JsonObject ObjectValue(string s, ref int i)
        {
            JsonObject objectValue = new JsonObject();
            i++;
            SkipWhitespace(s, ref i);
            if (i < s.Length && s[i] == '}') { i++; return objectValue; }

            while (true)
            {
                SkipWhitespace(s, ref i);
                if (i >= s.Length || s[i] != '"') throw new FormatException("Expected a key at position " + i);
                string key = StringValue(s, ref i);

                SkipWhitespace(s, ref i);
                if (i >= s.Length || s[i] != ':') throw new FormatException("Expected ':' at position " + i);
                i++;

                objectValue.Set(key, Value(s, ref i));

                SkipWhitespace(s, ref i);
                if (i >= s.Length) throw new FormatException("Unclosed object.");
                if (s[i] == ',') { i++; continue; }
                if (s[i] == '}') { i++; return objectValue; }
                throw new FormatException("Expected ',' or '}' at position " + i);
            }
        }

        private static List<object> ArrayValue(string s, ref int i)
        {
            List<object> list = new List<object>();
            i++;
            SkipWhitespace(s, ref i);
            if (i < s.Length && s[i] == ']') { i++; return list; }

            while (true)
            {
                list.Add(Value(s, ref i));
                SkipWhitespace(s, ref i);
                if (i >= s.Length) throw new FormatException("Unclosed array.");
                if (s[i] == ',') { i++; continue; }
                if (s[i] == ']') { i++; return list; }
                throw new FormatException("Expected ',' or ']' at position " + i);
            }
        }

        private static string StringValue(string s, ref int i)
        {
            StringBuilder b = new StringBuilder();
            i++;

            while (i < s.Length)
            {
                char c = s[i++];
                if (c == '"') return b.ToString();

                if (c != '\\') { b.Append(c); continue; }

                if (i >= s.Length) break;
                char k = s[i++];
                switch (k)
                {
                    case '"': b.Append('"'); break;
                    case '\\': b.Append('\\'); break;
                    case '/': b.Append('/'); break;
                    case 'b': b.Append('\b'); break;
                    case 'f': b.Append('\f'); break;
                    case 'n': b.Append('\n'); break;
                    case 'r': b.Append('\r'); break;
                    case 't': b.Append('\t'); break;
                    case 'u':
                        if (i + 4 > s.Length) throw new FormatException("Incomplete \\u escape.");
                        b.Append((char)Convert.ToInt32(s.Substring(i, 4), 16));
                        i += 4;
                        break;
                    default:
                        throw new FormatException("Unknown escape: \\" + k);
                }
            }

            throw new FormatException("Unclosed string.");
        }

        private static object NumberValue(string s, ref int i)
        {
            int bas = i;
            while (i < s.Length && "+-.eE0123456789".IndexOf(s[i]) >= 0) i++;
            if (i == bas) throw new FormatException("Expected a number at position " + i);

            string raw = s.Substring(bas, i - bas);
            double d;
            if (!double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out d))
                throw new FormatException("Invalid number: " + raw);
            return d;
        }

        private static bool Match(string s, ref int i, string sozcuk)
        {
            if (i + sozcuk.Length > s.Length) return false;
            if (string.CompareOrdinal(s, i, sozcuk, 0, sozcuk.Length) != 0) return false;
            i += sozcuk.Length;
            return true;
        }

        private static void SkipWhitespace(string s, ref int i)
        {
            while (i < s.Length && char.IsWhiteSpace(s[i])) i++;
        }

        public static string Write(object value)
        {
            StringBuilder b = new StringBuilder();
            Log(value, b, 0);
            b.Append(Environment.NewLine);
            return b.ToString();
        }

        private static void Log(object value, StringBuilder b, int depth)
        {
            if (value == null) { b.Append("null"); return; }

            JsonObject objectValue = value as JsonObject;
            if (objectValue != null) { WriteObject(objectValue, b, depth); return; }

            List<object> arrayValue = value as List<object>;
            if (arrayValue != null) { WriteArray(arrayValue, b, depth); return; }

            if (value is bool) { b.Append((bool)value ? "true" : "false"); return; }

            if (value is double || value is int || value is long || value is ulong)
            {
                double d = Convert.ToDouble(value, CultureInfo.InvariantCulture);

                if (Math.Abs(d - Math.Floor(d)) < double.Epsilon && Math.Abs(d) < 1e15)
                    b.Append(((long)d).ToString(CultureInfo.InvariantCulture));
                else
                    b.Append(d.ToString("R", CultureInfo.InvariantCulture));
                return;
            }

            WriteString(Convert.ToString(value, CultureInfo.InvariantCulture), b);
        }

        private static void WriteObject(JsonObject objectValue, StringBuilder b, int depth)
        {
            if (objectValue.Count == 0) { b.Append("{}"); return; }

            b.Append('{').Append(Environment.NewLine);
            int n = 0;
            foreach (KeyValuePair<string, object> pair in objectValue)
            {
                Indent(b, depth + 1);
                WriteString(pair.Key, b);
                b.Append(": ");
                Log(pair.Value, b, depth + 1);
                if (++n < objectValue.Count) b.Append(',');
                b.Append(Environment.NewLine);
            }
            Indent(b, depth);
            b.Append('}');
        }

        private static void WriteArray(List<object> arrayValue, StringBuilder b, int depth)
        {
            if (arrayValue.Count == 0) { b.Append("[]"); return; }

            b.Append('[').Append(Environment.NewLine);
            for (int i = 0; i < arrayValue.Count; i++)
            {
                Indent(b, depth + 1);
                Log(arrayValue[i], b, depth + 1);
                if (i + 1 < arrayValue.Count) b.Append(',');
                b.Append(Environment.NewLine);
            }
            Indent(b, depth);
            b.Append(']');
        }

        private static void WriteString(string text, StringBuilder b)
        {
            b.Append('"');
            foreach (char c in text ?? "")
            {
                switch (c)
                {
                    case '"': b.Append("\\\""); break;
                    case '\\': b.Append("\\\\"); break;
                    case '\b': b.Append("\\b"); break;
                    case '\f': b.Append("\\f"); break;
                    case '\n': b.Append("\\n"); break;
                    case '\r': b.Append("\\r"); break;
                    case '\t': b.Append("\\t"); break;
                    default:
                        if (c < ' ') b.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                        else b.Append(c);
                        break;
                }
            }
            b.Append('"');
        }

        private static void Indent(StringBuilder b, int depth)
        {
            b.Append('\t', depth);
        }
    }
    internal sealed class JsonObject : List<KeyValuePair<string, object>>
    {
        public bool Has(string key)
        {
            return IndexOf(key) >= 0;
        }

        public object Get(string key)
        {
            int i = IndexOf(key);
            return i < 0 ? null : this[i].Value;
        }

        public string Text(string key, string defaultValue = "")
        {
            object d = Get(key);
            if (d == null) return defaultValue;
            if (d is double) return Json.Write(d).Trim();
            if (d is bool) return (bool)d ? "true" : "false";
            return Convert.ToString(d, CultureInfo.InvariantCulture);
        }

        public int Number(string key, int defaultValue)
        {
            object d = Get(key);
            if (d is double) return (int)(double)d;
            int n;
            if (d is string && int.TryParse((string)d, NumberStyles.Integer, CultureInfo.InvariantCulture, out n)) return n;
            return defaultValue;
        }

        public List<string> Strings(string key)
        {
            List<string> result = new List<string>();
            List<object> arrayValue = Get(key) as List<object>;
            if (arrayValue == null) return result;

            foreach (object d in arrayValue)
            {
                string s = Convert.ToString(d, CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(s)) result.Add(s);
            }
            return result;
        }

        public void Set(string key, object value)
        {
            int i = IndexOf(key);
            KeyValuePair<string, object> pair = new KeyValuePair<string, object>(key, value);
            if (i < 0) Add(pair); else this[i] = pair;
        }

        public void Remove(string key)
        {
            int i = IndexOf(key);
            if (i >= 0) RemoveAt(i);
        }

        private int IndexOf(string key)
        {
            for (int i = 0; i < Count; i++)
                if (string.Equals(this[i].Key, key, StringComparison.OrdinalIgnoreCase)) return i;
            return -1;
        }
    }
}
