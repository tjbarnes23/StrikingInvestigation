using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrikingInvestigation.Utilities
{
    public static class BellConv
    {
        public static string BellStr(int bell)
        {
            return bell switch
            {
                10 => "0",
                11 => "E",
                12 => "T",
                _ => bell.ToString()
            };
        }

        public static int BellInt(string bell)
        {
            return bell switch
            {
                "0" => 10,
                "E" => 11,
                "T" => 12,
                _ => Convert.ToInt32(bell)
            };
        }
    }

    public static class StageConv
    {
        public static string StageStr(int stage)
        {
            return stage switch
            {
                5 => "Doubles",
                6 => "Minor",
                7 => "Triples",
                8 => "Major",
                9 => "Caters",
                10 => "Royal",
                11 => "Cinques",
                12 => "Maximus",
                _ => stage.ToString()
            };
        }

        public static int StageInt(string stage)
        {
            return stage switch
            {
                "Doubles" => 5,
                "Minor" => 6,
                "Triples" => 7,
                "Major" => 8,
                "Caters" => 9,
                "Royal" => 10,
                "Cinques" => 11,
                "Maximus" => 12,
                _ => Convert.ToInt32(stage)
            };
        }
    }

    public static class TenorWeightConv
    {
        public static string TenorWeightStr(int tenorWeight)
        {
            return tenorWeight switch
            {
                8 => "8cwt",
                23 => "23cwt",
                _ => "0cwt"
            };
        }

        public static int TenorWeightInt(string tenorWeight)
        {
            return tenorWeight switch
            {
                "8cwt" => 8,
                "23cwt" => 23,
                _ => 0
            };
        }
    }

    public static class TestBellLocConv
    {
        public static string TestBellLocStr(int testBellLoc)
        {
            return testBellLoc switch
            {
                1 => "Last blow of last row",
                2 => "Any blow of last row",
                _ => "0"
            };
        }

        public static int TestBellLocInt(string testBellLoc)
        {
            return testBellLoc switch
            {
                "Last blow of last row" => 1,
                "Any blow of last row" => 2,
                _ => 0
            };
        }
    }

    public static class ErrorSizeConv
    {
        public static string ErrorSizeStr(int errorSize)
        {
            return errorSize.ToString() + "ms";
        }

        public static int ErrorSizeInt(string errorSize)
        {
            return Convert.ToInt32(errorSize[0..^2]);
        }
    }

    public static class ErrorTypeConv
    {
        public static string ErrorTypeStr(int errorType)
        {
            return errorType switch
            {
                1 => "Striking error",
                2 => "Compass error",
                _ => "0"
            };
        }

        public static int ErrorTypeInt(string errorType)
        {
            return errorType switch
            {
                "Striking error" => 1,
                "Compass error" => 2,
                _ => 0
            };
        }
    }
}
