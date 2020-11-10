using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrikingInvestigation.Utilities
{
    public static class Diam
    {
        public static double Diameter(string bellActual)
        {
            var bell = bellActual switch
            {
                "1" => 1,
                "2s" => 2,
                "0" => 11,
                "E" => 12,
                "T" => 13,
                _ => Convert.ToInt32(bellActual) + 1,
            };

            double diameter = (0.01241259 * Math.Pow(bell, 3)) + (-0.04985015 * Math.Pow(bell, 2)) +
                    (0.5313437 * bell) + 25.0451;
            
            return diameter;
        }
    }
}
