using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrikingInvestigation.Utilities
{
    public static class TenorWeightSelect
    {
        public static bool TenorWeightDisabled (int stage)
        {
            if (stage < 7)
            {
                return true;
            }
            else if (stage > 8)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
