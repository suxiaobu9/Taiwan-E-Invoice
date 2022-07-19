using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Creater
{
    public class DoubleExtension
    {
        private readonly double dpi;
        public DoubleExtension(double dpi)
        {
            this.dpi = dpi;
        }

        public double ToPixal(double cm)
        {
            var result = cm / 2.54d * dpi;

            return result;
        }
    }
}