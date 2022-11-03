namespace CsvUtils.Tests
{
    public class JoinTests
    {
        private const char DQ = '"';

        [Fact]
        public void JoinTest_null()
        {
#pragma warning disable CS8625 // null リテラルを null 非許容参照型に変換できません。
            new Func<string?>(() => CsvUtils.Join(null))
                .Should().Throw<ArgumentNullException>("入力がnullならArgumentNullException発生");
#pragma warning restore CS8625 // null リテラルを null 非許容参照型に変換できません。
        }

        [Fact]
        public void JoinTest_Empty()
        {
            // string.Emptyを返した方がいいような気もするが、入力がstring.Emptyだった場合と区別できないのと
            // Split()との対称性を考えて、null返却することに
            CsvUtils.Join(Enumerable.Empty<string>())
                .Should().BeNull("入力が空なら結果はnull");
        }

        [Fact]
        public void JoinTest_a_special_letter()
        {
            // ダブルクォートが必要となる文字はダブルクォート、改行文字、カンマ(,)の3種類

            CsvUtils.Join(new string[] { $"{DQ}", })
                .Should().Be($"{DQ}{DQ}{DQ}{DQ}", "入力がダブルクォート1つならダブルクォートでエスケープされたダブルクォートをダブルクォートで囲った文字列が返却される"); // " -> """"

            CsvUtils.Join(new string[] { Environment.NewLine, })
                .Should().Be($"{DQ}{Environment.NewLine}{DQ}", "入力が改行文字なら単純にダブルクォートで囲った文字列が返却される");

            CsvUtils.Join(new string[] { ",", })
                .Should().Be($"{DQ},{DQ}", "入力がカンマなら単純にダブルクォートで囲った文字列が返却される");
        }

        [Fact]
        public void JoinTest_a_not_special_letter()
        {
            // 空文字列は(一応)非特殊文字
            CsvUtils.Join(new string[] { string.Empty, })
                .Should().BeEmpty("入力が空文字列1つなら空文字列が返却される");

            CsvUtils.Join(new string[] { "a", })
                .Should().Be("a", "入力が特殊文字でなければそのものが返却される");

            CsvUtils.Join(new string[] { " ", })
                .Should().Be(" ", "入力が半角空白ならそのものが返却される");

            CsvUtils.Join(new string[] { "\t", })
                .Should().Be("\t", "入力がTABならそのものが返却される");
        }

        [Fact]
        public void JoinTest_with_spaces()
        {
            // 特殊文字たち

            CsvUtils.Join(new string[] { $" {DQ} ", })
                .Should().Be($"{DQ} {DQ}{DQ} {DQ}", "ダブルクォートの挿入位置に注意");

            CsvUtils.Join(new string[] { $" {Environment.NewLine} ", })
                .Should().Be($"{DQ} {Environment.NewLine} {DQ}", "ダブルクォートの挿入位置に注意");

            CsvUtils.Join(new string[] { " , ", })
                .Should().Be($"{DQ} , {DQ}", "ダブルクォートの挿入位置に注意");

            // 非特殊文字たちはダブルクォートなし

            CsvUtils.Join(new string[] { " a ", })
                .Should().Be(" a ", "入力が特殊文字でなければそのものが返却される");

            CsvUtils.Join(new string[] { "   ", })
                .Should().Be("   ", "入力が半角空白ならそのものが返却される");

            CsvUtils.Join(new string[] { " \t ", })
                .Should().Be(" \t ", "入力がTABならそのものが返却される");
        }

        [Fact]
        public void JoinTest_some_cols()
        {
            CsvUtils.Join(new string[] { "a", "b", "c", })
                .Should().Be("a,b,c", "通常文字3つ組");

            CsvUtils.Join(new string[] { string.Empty, "b", "c", })
                .Should().Be(",b,c", "先頭が空文字列");

            CsvUtils.Join(new string[] { "a", string.Empty, "c", })
                .Should().Be("a,,c", "真ん中が空文字列");

            CsvUtils.Join(new string[] { "a", "b", string.Empty, })
                .Should().Be("a,b,", "最後が空文字列");

            CsvUtils.Join(new string[] { string.Empty, string.Empty, string.Empty, })
                .Should().Be(",,", "全部が空文字列");
        }

        [Fact]
        public void JoinTest_sequencial_DQs()
        {
            CsvUtils.Join(new string[] { $"{DQ}{DQ}", $" {DQ}{DQ}{DQ} ", })
                .Should().Be($"{DQ}{DQ}{DQ}{DQ}{DQ}{DQ},{DQ} {DQ}{DQ}{DQ}{DQ}{DQ}{DQ} {DQ}", "string.Replace()の動作確認");
        }
    }
}