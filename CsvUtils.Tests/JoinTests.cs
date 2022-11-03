namespace CsvUtils.Tests
{
    public class JoinTests
    {
        private const char DQ = '"';

        [Fact]
        public void JoinTest_null()
        {
#pragma warning disable CS8625 // null ���e������ null �񋖗e�Q�ƌ^�ɕϊ��ł��܂���B
            new Func<string?>(() => CsvUtils.Join(null))
                .Should().Throw<ArgumentNullException>("���͂�null�Ȃ�ArgumentNullException����");
#pragma warning restore CS8625 // null ���e������ null �񋖗e�Q�ƌ^�ɕϊ��ł��܂���B
        }

        [Fact]
        public void JoinTest_Empty()
        {
            // string.Empty��Ԃ������������悤�ȋC�����邪�A���͂�string.Empty�������ꍇ�Ƌ�ʂł��Ȃ��̂�
            // Split()�Ƃ̑Ώ̐����l���āAnull�ԋp���邱�Ƃ�
            CsvUtils.Join(Enumerable.Empty<string>())
                .Should().BeNull("���͂���Ȃ猋�ʂ�null");
        }

        [Fact]
        public void JoinTest_a_special_letter()
        {
            // �_�u���N�H�[�g���K�v�ƂȂ镶���̓_�u���N�H�[�g�A���s�����A�J���}(,)��3���

            CsvUtils.Join(new string[] { $"{DQ}", })
                .Should().Be($"{DQ}{DQ}{DQ}{DQ}", "���͂��_�u���N�H�[�g1�Ȃ�_�u���N�H�[�g�ŃG�X�P�[�v���ꂽ�_�u���N�H�[�g���_�u���N�H�[�g�ň͂��������񂪕ԋp�����"); // " -> """"

            CsvUtils.Join(new string[] { Environment.NewLine, })
                .Should().Be($"{DQ}{Environment.NewLine}{DQ}", "���͂����s�����Ȃ�P���Ƀ_�u���N�H�[�g�ň͂��������񂪕ԋp�����");

            CsvUtils.Join(new string[] { ",", })
                .Should().Be($"{DQ},{DQ}", "���͂��J���}�Ȃ�P���Ƀ_�u���N�H�[�g�ň͂��������񂪕ԋp�����");
        }

        [Fact]
        public void JoinTest_a_not_special_letter()
        {
            // �󕶎����(�ꉞ)����ꕶ��
            CsvUtils.Join(new string[] { string.Empty, })
                .Should().BeEmpty("���͂��󕶎���1�Ȃ�󕶎��񂪕ԋp�����");

            CsvUtils.Join(new string[] { "a", })
                .Should().Be("a", "���͂����ꕶ���łȂ���΂��̂��̂��ԋp�����");

            CsvUtils.Join(new string[] { " ", })
                .Should().Be(" ", "���͂����p�󔒂Ȃ炻�̂��̂��ԋp�����");

            CsvUtils.Join(new string[] { "\t", })
                .Should().Be("\t", "���͂�TAB�Ȃ炻�̂��̂��ԋp�����");
        }

        [Fact]
        public void JoinTest_with_spaces()
        {
            // ���ꕶ������

            CsvUtils.Join(new string[] { $" {DQ} ", })
                .Should().Be($"{DQ} {DQ}{DQ} {DQ}", "�_�u���N�H�[�g�̑}���ʒu�ɒ���");

            CsvUtils.Join(new string[] { $" {Environment.NewLine} ", })
                .Should().Be($"{DQ} {Environment.NewLine} {DQ}", "�_�u���N�H�[�g�̑}���ʒu�ɒ���");

            CsvUtils.Join(new string[] { " , ", })
                .Should().Be($"{DQ} , {DQ}", "�_�u���N�H�[�g�̑}���ʒu�ɒ���");

            // ����ꕶ�������̓_�u���N�H�[�g�Ȃ�

            CsvUtils.Join(new string[] { " a ", })
                .Should().Be(" a ", "���͂����ꕶ���łȂ���΂��̂��̂��ԋp�����");

            CsvUtils.Join(new string[] { "   ", })
                .Should().Be("   ", "���͂����p�󔒂Ȃ炻�̂��̂��ԋp�����");

            CsvUtils.Join(new string[] { " \t ", })
                .Should().Be(" \t ", "���͂�TAB�Ȃ炻�̂��̂��ԋp�����");
        }

        [Fact]
        public void JoinTest_some_cols()
        {
            CsvUtils.Join(new string[] { "a", "b", "c", })
                .Should().Be("a,b,c", "�ʏ핶��3�g");

            CsvUtils.Join(new string[] { string.Empty, "b", "c", })
                .Should().Be(",b,c", "�擪���󕶎���");

            CsvUtils.Join(new string[] { "a", string.Empty, "c", })
                .Should().Be("a,,c", "�^�񒆂��󕶎���");

            CsvUtils.Join(new string[] { "a", "b", string.Empty, })
                .Should().Be("a,b,", "�Ōオ�󕶎���");

            CsvUtils.Join(new string[] { string.Empty, string.Empty, string.Empty, })
                .Should().Be(",,", "�S�����󕶎���");
        }

        [Fact]
        public void JoinTest_sequencial_DQs()
        {
            CsvUtils.Join(new string[] { $"{DQ}{DQ}", $" {DQ}{DQ}{DQ} ", })
                .Should().Be($"{DQ}{DQ}{DQ}{DQ}{DQ}{DQ},{DQ} {DQ}{DQ}{DQ}{DQ}{DQ}{DQ} {DQ}", "string.Replace()�̓���m�F");
        }
    }
}