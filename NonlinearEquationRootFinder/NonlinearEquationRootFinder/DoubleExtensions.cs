namespace NonlinearEquationRootFinder
{
    public static class DoubleExtensions
    {
        public static string ToFormattedString(this double number, int count = 8) =>
            number.ToString($"F{count}");
    }
}
