using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace FullSerializer {
    public static class fsJsonPrinter {
        /// <summary>
        /// Inserts the given number of indents into the builder.
        /// 与えられた数のインデントをビルダーに挿入します。
        /// </summary>
        private static void InsertSpacing(TextWriter stream, int count) {
            for (int i = 0; i < count; ++i) {
                stream.Write("    ");
            }
        }

        /// <summary>
        /// Escapes a string.
        /// 文字列をエスケープします。
        /// </summary>
        private static string EscapeString(string str) {
            // Escaping a string is pretty allocation heavy, so we try hard to not do it.
            //文字列をエスケープするのはかなり重いので、私たちはそれをしないように努力しています。
            //キーボードから入力できない文字を他の文字の組み合わせで表現したものをエスケープシーケンスといいます
            //https://www.javadrive.jp/start/num/index4.html参照

            bool needsEscape = false;
            for (int i = 0; i < str.Length; ++i) {
                char c = str[i];

                // unicode code point
                //ユニコードコードポイント
                int intChar = Convert.ToInt32(c);
                if (intChar < 0 || intChar > 127) {
                    needsEscape = true;
                    break;
                }

                // standard escape character
                //標準エスケープ文字
                switch (c) {
                    case '"':
                    case '\\':
                    case '\a':
                    case '\b':
                    case '\f':
                    case '\n':
                    case '\r':
                    case '\t':
                    case '\0':
                        needsEscape = true;
                        break;
                }

                if (needsEscape) {
                    break;
                }
            }

            if (needsEscape == false) {
                return str;
            }


            StringBuilder result = new StringBuilder();

            for (int i = 0; i < str.Length; ++i) {
                char c = str[i];

                // unicode code point
                //ユニコードコードポイント
                int intChar = Convert.ToInt32(c);
                if (intChar < 0 || intChar > 127) {
                    result.Append(string.Format("\\u{0:x4} ", intChar).Trim());
                    continue;
                }

                // standard escape character
                //標準エスケープ文字
                switch (c) {
                    case '"': result.Append("\\\""); continue;
                    case '\\': result.Append(@"\\"); continue;
                    case '\a': result.Append(@"\a"); continue;
                    case '\b': result.Append(@"\b"); continue;
                    case '\f': result.Append(@"\f"); continue;
                    case '\n': result.Append(@"\n"); continue;
                    case '\r': result.Append(@"\r"); continue;
                    case '\t': result.Append(@"\t"); continue;
                    case '\0': result.Append(@"\0"); continue;
                }

                // no escaping needed
                //エスケープする必要はありません
                result.Append(c);
            }
            return result.ToString();
        }

        private static void BuildCompressedString(fsData data, TextWriter stream) {
            switch (data.Type) {
                case fsDataType.Null:
                    stream.Write("null");
                    break;

                case fsDataType.Boolean:
                    if (data.AsBool) stream.Write("true");
                    else stream.Write("false");
                    break;

                case fsDataType.Double:
                    // doubles must *always* include a decimal
                    //doubleは常に*小数を含める必要があります
                    stream.Write(ConvertDoubleToString(data.AsDouble));
                    break;

                case fsDataType.Int64:
                    stream.Write(data.AsInt64);
                    break;

                case fsDataType.String:
                    stream.Write('"');
                    stream.Write(EscapeString(data.AsString));
                    stream.Write('"');
                    break;

                case fsDataType.Object: {
                        stream.Write('{');
                        bool comma = false;
                        foreach (var entry in data.AsDictionary) {
                            if (comma) stream.Write(',');
                            comma = true;
                            stream.Write('"');
                            stream.Write(entry.Key);
                            stream.Write('"');
                            stream.Write(":");
                            BuildCompressedString(entry.Value, stream);
                        }
                        stream.Write('}');
                        break;
                    }

                case fsDataType.Array: {
                        stream.Write('[');
                        bool comma = false;
                        foreach (var entry in data.AsList) {
                            if (comma) stream.Write(',');
                            comma = true;
                            BuildCompressedString(entry, stream);
                        }
                        stream.Write(']');
                        break;
                    }
            }
        }

        /// <summary>
        /// Formats this data into the given builder.
        /// このデータを指定されたBuilderにフォーマットします。
        /// </summary>
        private static void BuildPrettyString(fsData data, TextWriter stream, int depth) {
            switch (data.Type) {
                case fsDataType.Null:
                    stream.Write("null");
                    break;

                case fsDataType.Boolean:
                    if (data.AsBool) stream.Write("true");
                    else stream.Write("false");
                    break;

                case fsDataType.Double:
                    stream.Write(ConvertDoubleToString(data.AsDouble));
                    break;

                case fsDataType.Int64:
                    stream.Write(data.AsInt64);
                    break;


                case fsDataType.String:
                    stream.Write('"');
                    stream.Write(EscapeString(data.AsString));
                    stream.Write('"');
                    break;

                case fsDataType.Object: {
                        stream.Write('{');
                        stream.WriteLine();
                        bool comma = false;
                        foreach (var entry in data.AsDictionary) {
                            if (comma) {
                                stream.Write(',');
                                stream.WriteLine();
                            }
                            comma = true;
                            InsertSpacing(stream, depth + 1);
                            stream.Write('"');
                            stream.Write(entry.Key);
                            stream.Write('"');
                            stream.Write(": ");
                            BuildPrettyString(entry.Value, stream, depth + 1);
                        }
                        stream.WriteLine();
                        InsertSpacing(stream, depth);
                        stream.Write('}');
                        break;
                    }

                case fsDataType.Array:
                    // special case for empty lists; we don't put an empty line between the brackets
                    //空リストの特別な場合。 括弧の間に空行を入れません
                    if (data.AsList.Count == 0) {
                        stream.Write("[]");
                    }

                    else {
                        bool comma = false;

                        stream.Write('[');
                        stream.WriteLine();
                        foreach (var entry in data.AsList) {
                            if (comma) {
                                stream.Write(',');
                                stream.WriteLine();
                            }
                            comma = true;
                            InsertSpacing(stream, depth + 1);
                            BuildPrettyString(entry, stream, depth + 1);
                        }
                        stream.WriteLine();
                        InsertSpacing(stream, depth);
                        stream.Write(']');
                    }
                    break;
            }
        }

        /// <summary>
        /// Writes the pretty JSON output data to the given stream.
        /// かなりのJSON出力データを指定されたストリームに書き込みます。
        /// </summary>
        /// <param name="data">The data to print.</param>
        /// <param name="outputStream">Where to write the printed data.</param>
        public static void PrettyJson(fsData data, TextWriter outputStream) {
            BuildPrettyString(data, outputStream, 0);
        }

        /// <summary>
        /// Returns the data in a pretty printed JSON format.
        /// きれいに印刷されたJSON形式のデータを返します。
        /// </summary>
        public static string PrettyJson(fsData data) {
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb)) {
                BuildPrettyString(data, writer, 0);
                return sb.ToString();
            }
        }

        /// <summary>
        /// Writes the compressed JSON output data to the given stream.
        /// 圧縮されたJSON出力データを指定されたストリームに書き込みます。
        /// </summary>
        /// <param name="data">The data to print.</param>
        /// <param name="outputStream">Where to write the printed data.</param>
        public static void CompressedJson(fsData data, StreamWriter outputStream) {
            BuildCompressedString(data, outputStream);
        }

        /// <summary>
        /// Returns the data in a relatively compressed JSON format.
        /// 相対的に圧縮されたJSON形式でデータを返します。
        /// </summary>
        public static string CompressedJson(fsData data) {
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb)) {
                BuildCompressedString(data, writer);
                return sb.ToString();
            }
        }

        /// <summary>
        /// Utility method that converts a double to a string.
        /// doubleを文字列に変換するユーティリティメソッド。
        /// </summary>
        private static string ConvertDoubleToString(double d) {
            if (Double.IsInfinity(d) || Double.IsNaN(d))
                return d.ToString(CultureInfo.InvariantCulture);

            string doubledString = d.ToString(CultureInfo.InvariantCulture);

            // NOTE/HACK: If we don't serialize with a period or an exponent,
            // then the number will be deserialized as an Int64, not a double.
            //NOTE / HACK：期間や指数でシリアル化しないと、
            //その数値は、倍精度ではなくInt64として逆シリアル化されます。
            if (doubledString.Contains(".") == false &&
                doubledString.Contains("e") == false &&
                doubledString.Contains("E") == false) {
                doubledString += ".0";
            }

            return doubledString;
        }

    }
}