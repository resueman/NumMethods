using System;

namespace Common
{
    public class Function
    {
        public Function(string funcStringRepresentation,
            Func<double, double> func,
            Func<double, double> funcIntegral,
            Func<double, double> M1, Func<double, double> M2, Func<double, double> M4)
        {
            Func = func;
            integralFunction = funcIntegral;
            StringRepresentation = funcStringRepresentation;
            this.M1 = M1;
            this.M2 = M2;
            this.M4 = M4;
        }

        public Function(string stringRepresentation, 
            Func<double, double> func, Func<double, double> integralFunction)
        {
            this.integralFunction = integralFunction;
            Func = func;
            StringRepresentation = stringRepresentation;
        }

        private Func<double, double> integralFunction;

        public Func<double, double> Func { get; private set; }
        
        public Func<double, double> M1 { get; private set; }
        
        public Func<double, double> M2 { get; private set; }
        
        public Func<double, double> M4 { get; private set; }

        public string StringRepresentation { get; private set; }

        public double CountIntegral(Segment segment)
            => integralFunction(segment.Right) - integralFunction(segment.Left);
    }
}
