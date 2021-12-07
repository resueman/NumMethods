using System.Collections.Generic;

namespace ApproxIntegralCalculationWithHighestAlgAccFormulas
{
    public class RootClarifierMethodAnalytics
    {
        public RootClarifierMethodAnalytics(Segment segment, List<double> initialApproximationToTheRoot,
            int stepsCounts, double finalApproximationToTheRoot, double lengthOfLastApproximationSegment,
            double discrepancyAbsoluteValue)
        {
            Segment = segment;
            InitialApproximationToTheRoot = initialApproximationToTheRoot;
            StepsCounts = stepsCounts;
            Root = finalApproximationToTheRoot;
            LengthOfLastApproximationSegment = lengthOfLastApproximationSegment;
            DiscrepancyAbsoluteValue = discrepancyAbsoluteValue;
        }

        public Segment Segment { get; private set; }

        public List<double> InitialApproximationToTheRoot { get; private set; }

        public int StepsCounts { get; private set; }

        public double Root { get; private set; }

        public double LengthOfLastApproximationSegment { get; private set; }

        public double DiscrepancyAbsoluteValue { get; private set; }
    }
}
