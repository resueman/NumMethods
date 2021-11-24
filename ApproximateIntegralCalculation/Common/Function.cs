using System;

namespace Common
{
    public class Function
    {
        public Function(string funcStringRepresentation,
            Func<double, double> func,
            Func<double, double> funcIntegral)
        {
            Func = func;
            integralFunction = funcIntegral;
            StringRepresentation = funcStringRepresentation;
        }

        private Func<double, double> integralFunction;

        public Func<double, double> Func { get; private set; }

        public string StringRepresentation { get; private set; }

        public double CountIntegral(Segment segment)
            => integralFunction(segment.Right) - integralFunction(segment.Left);
    }
}
