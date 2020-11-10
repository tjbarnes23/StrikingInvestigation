using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrikingInvestigation.Utilities
{
    public static class BellAct
    {
        public static string DeriveBellActual(int stage, int tenorWeight, string bell)
        {
            string bellActual;

            int bellnum = BellConv.BellInt(bell);

            switch (stage)
            {
                case 5:
                case 6:
                    bellActual = BellConv.BellStr(bellnum + 2);
                    break;

                case 7:
                case 8:
                    if (tenorWeight == 23)
                    {
                        bellActual = BellConv.BellStr(bellnum + 4);
                    }
                    else
                    {
                        if (bellnum == 2)
                        {
                            bellActual = "2s";
                        }
                        else
                        {
                            bellActual = BellConv.BellStr(bellnum);
                        }
                    }
                    break;

                case 9:
                case 10:
                    bellActual = BellConv.BellStr(bellnum + 2);
                    break;

                case 11:
                case 12:
                    bellActual = BellConv.BellStr(bellnum);
                    break;

                default:
                    bellActual = BellConv.BellStr(bellnum);
                    break;
            }
                        
            return bellActual;
        }
    }
}
