namespace CsvUtils.Tests
{
    public class TextReaderSplitTests
    {
        private const char DQ = '"';

        [Fact]
        public void TextReaderSplitTest_null()
        {
#pragma warning disable CS8625 // null リテラルを null 非許容参照型に変換できません。
#pragma warning disable CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
            new Func<IEnumerable<string>>(() => CsvUtils.Split((TextReader)null))
                .Enumerating()
                .Should().Throw<ArgumentNullException>("入力がnullならArgumentNullException発生");
#pragma warning restore CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
#pragma warning restore CS8625 // null リテラルを null 非許容参照型に変換できません。
        }

        class TestItem
        {
            public string input { get; }
            public IEnumerable<string> expected { get; }
            public string because { get; }

            public TestItem(string input, IEnumerable<string> expected, string because = "")
            {
                this.input = input;
                this.expected = expected;
                this.because = because;
            }
        }

        static readonly TestItem[] TestItems = new[]
        {
            new TestItem(string.Empty, new[] { string.Empty, }, "入力がDQで囲まれた空文字列の場合と一緒"),
            new TestItem(",", Enumerable.Repeat(string.Empty, 2), "2つの空文字列"),

            new TestItem($"{DQ}{DQ}", new[] { string.Empty, }, "DQで囲まれた空文字列の入力"),
            new TestItem($"{DQ}{DQ}{DQ}{DQ}", new[] { $"{DQ}", }, "DQで囲まれたDQ"), // """" -> "
            new TestItem($"{DQ},{DQ}", new[] { ",", }, "DQで囲まれたカンマ"), // "," -> ,
            new TestItem($"{DQ}{Environment.NewLine}{DQ}", new[] { $"{Environment.NewLine}", }, "DQで囲まれたNL"), // "NL" -> NL

            new TestItem("a,b,c", new[] { "a", "b", "c", }),
            new TestItem($"{DQ}a{DQ},{DQ}b{DQ},{DQ}c{DQ}", new[] { "a", "b", "c", }),
            new TestItem("あ,い,う", new[] { "あ", "い", "う", }),
            new TestItem($"{DQ}あ{DQ},{DQ}い{DQ},{DQ}う{DQ}", new[] { "あ", "い", "う", }),
            new TestItem(" , , ", new[] { " ", " ", " ", }),
            new TestItem($"{DQ} {DQ},{DQ} {DQ},{DQ} {DQ}", new[] { " ", " ", " ", }),

            new TestItem($" {DQ} , {DQ}{DQ} , {DQ}{DQ}{DQ} ", new[] { $" {DQ} ", $" {DQ}{DQ} ", $" {DQ}{DQ}{DQ} ", }, "各カラムの2文字目以降に初めてDQが現れる場合はDQを特殊文字扱いしない"),
            new TestItem($"{DQ}a{DQ}b{DQ}c{DQ}d", new[] { @"ab""c""d", }, "閉じDQ後に文字列があってもエラーにしない"),
        };

        [Fact]
        public void TextReaderSplitTest_SingleLine()
        {
            foreach (var item in TestItems)
            {
                using var sr = new StringReader(item.input);

                CsvUtils.Split(sr)
                    .Should().Equal(item.expected, item.because);
            }
        }

        [Fact]
        public void TextReaderSplitTest_SingleLineWithNewline()
        {
            foreach (var item in TestItems)
            {
                using var sr = new StringReader($"{item.input}{Environment.NewLine}");

                CsvUtils.Split(sr)
                    .Should().Equal(item.expected, item.because);
            }
        }

        [Fact]
        public void TextReaderSplitTest_TwoLines()
        {
            for (var i = 0; i < TestItems.Length; ++i)
            {
                for (var j = 0; j < TestItems.Length; ++j)
                {
                    using var sr = new StringReader($"{TestItems[i].input}{Environment.NewLine}{TestItems[j].input}");

                    CsvUtils.Split(sr)
                        .Should().Equal(TestItems[i].expected, TestItems[i].because);
                    CsvUtils.Split(sr)
                        .Should().Equal(TestItems[j].expected, TestItems[j].because);
                }
            }
        }

        [Fact]
        public void TextReaderSplitTest_withoutClosingDQ()
        {
            var sr = new StringReader($"{DQ}a{Environment.NewLine},");

            CsvUtils.Split(sr)
                .Should().Equal(new[] { $"a{Environment.NewLine},", }, "閉じDQなし+EOFは正常系とする");
        }

        [Fact]
        public void TextReaderSplitTest_illegalDQ()
        {
            var sr = new StringReader($" {DQ}a{Environment.NewLine}{DQ},b");

            CsvUtils.Split(sr)
                .Should().Equal(new[] { $" {DQ}a", }, "DQ外のNLなので・・・");
            CsvUtils.Split(sr)
                .Should().Equal(new[] { ",b", }, "DQ外のNLなので・・・");
        }

        [Fact]
        public void TextReaderSplitTest_withNL()
        {
            var sr = new StringReader($"{DQ},{Environment.NewLine},{Environment.NewLine}{DQ}{DQ}{DQ}");

            CsvUtils.Split(sr)
                .Should().Equal(new[] { $",{Environment.NewLine},{Environment.NewLine}{DQ}", });
        }
    }
}
