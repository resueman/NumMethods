namespace InverseInterpolation
{
    public class InverseInterpolationResult
    {
        public InverseInterpolationResult(double argumentValue, double abcoluteDisrepancyValue)
        {
            ArgumentValue = argumentValue;
            AbcoluteDisrepancyValue = abcoluteDisrepancyValue;
        }

        public double ArgumentValue { get; private set; }

        public double AbcoluteDisrepancyValue { get; private set; }
    }
}
