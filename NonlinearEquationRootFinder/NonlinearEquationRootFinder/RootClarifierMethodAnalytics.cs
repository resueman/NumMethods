using System.Collections.Generic;

namespace NonlinearEquationRootFinder
{
    class RootClarifierMethodAnalytics
    {
        public RootClarifierMethodAnalytics(Segment segment, List<double> initialApproximationToTheRoot, 
            int stepsCounts, double finalApproximationToTheRoot, double lengthOfLastApproximationSegment,
            double discrepancyAbsoluteValue)
        {
            Segment = segment;
            InitialApproximationToTheRoot = initialApproximationToTheRoot;
            StepsCounts = stepsCounts;
            FinalApproximationToTheRoot = finalApproximationToTheRoot;
            LengthOfLastApproximationSegment = lengthOfLastApproximationSegment;
            DiscrepancyAbsoluteValue = discrepancyAbsoluteValue;
        }

        public Segment Segment { get; private set; }

        public List<double> InitialApproximationToTheRoot { get; private set; }

        public int StepsCounts { get; private set; }

        public double FinalApproximationToTheRoot { get; private set; }

        public double LengthOfLastApproximationSegment { get; private set; }

        public double DiscrepancyAbsoluteValue { get; private set; }
    }
}
