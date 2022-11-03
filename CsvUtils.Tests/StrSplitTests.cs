namespace CsvUtils.Tests
{
    public class StrSplitTests
    {
        private const char DQ = '"';

        [Fact]
        public void StrSplitTest_null()
        {
#pragma warning disable CS8625 // null リテラルを null 非許容参照型に変換できません。
#pragma warning disable CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
#if false
            new Func<IEnumerable<string>>(() => CsvUtils.Split((string)null))
                .Enumerating()
                .Should().Throw<ArgumentNullException>("入力がnullならArgumentNullException発生");
#else
            CsvUtils.Split((string)null)
                .Should().Equal(Enumerable.Empty<string>());
#endif
#pragma warning restore CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
#pragma warning restore CS8625 // null リテラルを null 非許容参照型に変換できません。
        }

        [Fact]
        public void StrSplitTest_Empty()
        {
            CsvUtils.Split(string.Empty)
                .Should().Equal(new[] { string.Empty, }, "入力がDQで囲まれた空文字列の場合と一緒");

            CsvUtils.Split(",")
                .Should().Equal(Enumerable.Repeat(string.Empty, 2), "2つの空文字列");
        }

        [Fact]
        public void StrSplitTest_enclosed()
        {
            CsvUtils.Split($"{DQ}{DQ}")
                .Should().Equal(new[] { string.Empty, }, "DQで囲まれた空文字列の入力");

            CsvUtils.Split($"{DQ}{DQ}{DQ}{DQ}")
                .Should().Equal(new[] { $"{DQ}", }, "DQで囲まれたDQ"); // """" -> "

            CsvUtils.Split($"{DQ},{DQ}")
                .Should().Equal(new[] { ",", }, "DQで囲まれたカンマ"); // "," -> ,

            CsvUtils.Split($"{DQ}{Environment.NewLine}{DQ}")
                .Should().Equal(new[] { $"{Environment.NewLine}", }, "DQで囲まれたNL"); // "NL" -> NL
        }

        [Fact]
        public void StrSplitTest_normal()
        {
            CsvUtils.Split("a,b,c")
                .Should().Equal(new[] { "a", "b", "c", });

            CsvUtils.Split($"{DQ}a{DQ},{DQ}b{DQ},{DQ}c{DQ}")
                .Should().Equal(new[] { "a", "b", "c", });

            CsvUtils.Split("あ,い,う")
                .Should().Equal(new[] { "あ", "い", "う", });

            CsvUtils.Split($"{DQ}あ{DQ},{DQ}い{DQ},{DQ}う{DQ}")
                .Should().Equal(new[] { "あ", "い", "う", });

            CsvUtils.Split(" , , ")
                .Should().Equal(new[] { " ", " ", " ", });

            CsvUtils.Split($"{DQ} {DQ},{DQ} {DQ},{DQ} {DQ}")
                .Should().Equal(new[] { " ", " ", " ", });
        }

        [Fact]
        public void StrSplitTest_illegal()
        {
            CsvUtils.Split($" {DQ} , {DQ}{DQ} , {DQ}{DQ}{DQ} ")
                .Should().Equal(new[] { $" {DQ} ", $" {DQ}{DQ} ", $" {DQ}{DQ}{DQ} ", }, "各カラムの2文字目以降に初めてDQが現れる場合はDQを特殊文字扱いしない");

            CsvUtils.Split($"{DQ}a")
                .Should().Equal(new[] { "a", }, "閉じDQなしは正常系とする");

            CsvUtils.Split($"{DQ},")
                .Should().Equal(new[] { ",", }, "閉じDQなしは正常系とする");

            CsvUtils.Split($"{DQ}a{DQ}b{DQ}c{DQ}d")
                .Should().Equal(new[] { @"ab""c""d", }, "閉じDQ後に文字列があってもエラーにしない");

            CsvUtils.Split($"{DQ}{Environment.NewLine}")
                .Should().Equal(new[] { $"{Environment.NewLine}", }, "閉じDQなしは正常系とする"); // TextReader版は挙動が異なる

            new Func<IEnumerable<string>>(() => CsvUtils.Split($"{Environment.NewLine}"))
                .Enumerating()
                .Should().Throw<ArgumentException>("DQ外の改行はArgumentException発生"); // TextReader版は挙動が異なる
        }
    }
}
