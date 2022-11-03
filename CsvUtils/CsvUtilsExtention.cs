namespace CsvUtils
{
    static class CsvUtilsExtention
    {
        public static string? CsvJoin(this IEnumerable<string> columns) => CsvUtils.Join(columns);

        public static IEnumerable<string> CsvSplit(this string line) => CsvUtils.Split(line);
        public static IEnumerable<string> CsvSplit(this TextReader tr) => CsvUtils.Split(tr);
    }
}
