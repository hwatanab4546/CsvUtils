using System.Text;
using System.Text.RegularExpressions;

namespace CsvUtils
{
    public static class CsvUtils
    {
        private const char DQ = '"';
        private const char CM = ',';
        private const char CR = '\r';
        private const char LF = '\n';

        private static readonly Regex RequresDqRegex = new($"[{DQ}{CM}{CR}{LF}]", RegexOptions.Compiled);

        /// <summary>
        /// string型のコレクションに含まれる全ての文字列要素をカンマ(,)で連結する。
        /// </summary>
        /// <param name="columns">string型のコレクション</param>
        /// <returns>全ての文字列要素をカンマ(,)で連結した文字列</returns>
        /// <exception cref="ArgumentNullException">引数としてnullが指定された</exception>
        public static string? Join(IEnumerable<string> columns)
        {
            if (columns is null)
            {
                throw new ArgumentNullException(nameof(columns));
            }

            if (!columns.Any())
            {
                return null;
            }

            var sb = new StringBuilder();
            foreach (var col in columns)
            {
                if (RequresDqRegex.IsMatch(col))
                {
                    sb.Append(DQ);
                    sb.Append(col.Replace($"{DQ}", $"{DQ}{DQ}"));
                    sb.Append(DQ);
                }
                else
                {
                    sb.Append(col);
                }
                sb.Append(CM);
            }
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        private enum ParsingState
        {
            Start,
            DQ,
            ClosingDQorEscapingDQ,
            noDQ,
        }

        /// <summary>
        /// 指定された文字列をカンマ(,)を区切り文字として分割する。
        /// </summary>
        /// <param name="str">分割対象文字列</param>
        /// <returns>分割後の各フィールドのコレクション</returns>
        /// <exception cref="ArgumentException">ダブルクォート(")で囲まれていないフィールドに改行コードが含まれていた</exception>
        public static IEnumerable<string> Split(string str)
        {
            if (str is null)
            {
                yield break;
            }

            var pst = ParsingState.Start;
            var sb = new StringBuilder();
            foreach (var ch in str)
            {
                switch (pst)
                {
                    case ParsingState.Start:
                    case ParsingState.ClosingDQorEscapingDQ:
                    case ParsingState.noDQ:
                        if (ch == CR || ch == LF)
                        {
                            throw new ArgumentException("found a newline not enclosed by double-quotes");
                        }
                        break;
                }

                switch (pst)
                {
                    case ParsingState.Start:
                        sb.Clear();
                        if (ch == CM)
                        {
                            yield return string.Empty;
                        }
                        else if (ch == DQ)
                        {
                            pst = ParsingState.DQ;
                        }
                        else
                        {
                            sb.Append(ch);
                            pst = ParsingState.noDQ;
                        }
                        break;

                    case ParsingState.DQ:
                        if (ch == DQ)
                        {
                            pst = ParsingState.ClosingDQorEscapingDQ;
                        }
                        else
                        {
                            sb.Append(ch);
                        }
                        break;
                    case ParsingState.ClosingDQorEscapingDQ:
                        if (ch == DQ)
                        {
                            sb.Append(DQ);
                            pst = ParsingState.DQ;
                        }
                        else if (ch == CM)
                        {
                            yield return sb.ToString();
                            pst = ParsingState.Start;
                        }
                        else
                        {
                            sb.Append(ch);
                            pst = ParsingState.noDQ;
                        }
                        break;

                    case ParsingState.noDQ:
                        if (ch == CM)
                        {
                            yield return sb.ToString();
                            pst = ParsingState.Start;
                        }
                        else
                        {
                            sb.Append(ch);
                        }
                        break;
                }
            }
            yield return sb.ToString();
        }

        /// <summary>
        /// 指定された読込み元からCSV形式1行分の文字列を読み出し、カンマ(,)を区切り文字として分割する。
        /// 読込み元が末尾に達した後は、空のコレクションを返す。
        /// 
        /// 改行コードをCSV形式1行分の末尾として扱う。ただし、ダブルクォート(")で囲まれたフィールド内の改行コードは除く。
        /// </summary>
        /// <param name="tr">分割対象文字列の読込み元</param>
        /// <returns>分割後の各フィールドのコレクション。読込み元が末尾に達した後は空コレクション</returns>
        /// <exception cref="ArgumentException">引数としてnullが指定された</exception>
        public static IEnumerable<string> Split(TextReader tr)
        {
            if (tr is null)
            {
                throw new ArgumentNullException(nameof(tr));
            }

            var pst = ParsingState.Start;
            var sb = new StringBuilder();
            while (true)
            {
                var ch = tr.Read();
                if (ch < 0)
                {
                    break;
                }

                switch (pst)
                {
                    case ParsingState.Start:
                    case ParsingState.ClosingDQorEscapingDQ:
                    case ParsingState.noDQ:
                        if (ch == CR)
                        {
                            if (tr.Peek() == LF)
                            {
                                _ = tr.Read();
                            }
                            goto LOOP_END;
                        }
                        else if (ch == LF)
                        {
                            goto LOOP_END;
                        }
                        break;
                }

                switch (pst)
                {
                    case ParsingState.Start:
                        sb.Clear();
                        if (ch == CM)
                        {
                            yield return string.Empty;
                        }
                        else if (ch == DQ)
                        {
                            pst = ParsingState.DQ;
                        }
                        else
                        {
                            sb.Append((char)ch);
                            pst = ParsingState.noDQ;
                        }
                        break;

                    case ParsingState.DQ:
                        if (ch == DQ)
                        {
                            pst = ParsingState.ClosingDQorEscapingDQ;
                        }
                        else
                        {
                            sb.Append((char)ch);
                        }
                        break;
                    case ParsingState.ClosingDQorEscapingDQ:
                        if (ch == DQ)
                        {
                            sb.Append(DQ);
                            pst = ParsingState.DQ;
                        }
                        else if (ch == CM)
                        {
                            yield return sb.ToString();
                            pst = ParsingState.Start;
                        }
                        else
                        {
                            sb.Append((char)ch);
                            pst = ParsingState.noDQ;
                        }
                        break;

                    case ParsingState.noDQ:
                        if (ch == CM)
                        {
                            yield return sb.ToString();
                            pst = ParsingState.Start;
                        }
                        else
                        {
                            sb.Append((char)ch);
                        }
                        break;
                }
            }
        LOOP_END:
            yield return sb.ToString();
        }
    }
}
