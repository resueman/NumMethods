using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpolation
{
    class Segment
    {
        public double Left { get; private set; }

        public double Right { get; private set; }

        public Segment(double left, double right)
        {
            Left = Math.Min(left, right);
            Right = Math.Max(left, right);
        }
    }
}
