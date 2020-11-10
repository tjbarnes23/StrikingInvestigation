using System;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Models
{
    public class Blow : BlowCore
    {
        public string BellActual { get; set; }

        public int RowNum { get; set; }

        public int Place { get; set; }

        public bool IsHandstroke { get; set; }

        public string BellSound { get; set; }

        public string AudioId { get; set; }

        public int GapCumulativeRow { get; set; }

        public int GapCumulative { get; set; }

        public string GapStr { get; set; }

        public string BellColor { get; set; }

        public DateTime StrikeTime { get; set; }

        public DateTime AltStrikeTime { get; set; }

        public void CreateRandomBlow()
        {
            Random rand = new Random();

            int bellabs = rand.Next(1, 14);

            if (bellabs == 1)
            {
                Bell = "1";
            }
            else if (bellabs == 2)
            {
                Bell = "2s";
            }
            else
            {
                Bell = BellConv.BellStr(bellabs - 1);
            }

            ChangeStr = string.Empty;
            IsTestBell = true;
            IsHighlighted = false;
            
            BellActual = Bell;
            RowNum = 1;
            Place = 1;
            IsHandstroke = false;
            BellSound = "/audio/tws" + BellActual + ".mp3";
            AudioId = "audio1";
        }

        public void LoadBlow(BlowCore blowCore)
        {
            Bell = blowCore.Bell;
            ChangeStr = blowCore.ChangeStr;
            Gap = blowCore.Gap;
            AltGap = blowCore.AltGap;
            IsTestBell = blowCore.IsTestBell;
            IsHighlighted = blowCore.IsHighlighted;

            BellActual = Bell;
            RowNum = 1;
            Place = 1;
            IsHandstroke = false;
            BellSound = "/audio/tws" + BellActual + ".mp3";
            AudioId = "audio1";

            GapCumulativeRow = Gap;
            GapCumulative = Gap;
            GapStr = Gap.ToString();
        }

        public void UpdateGap(int newGap)
        {
            // Only use this method on the last blow in a blowset because it doesn't update the cumulative properties
            // of any subsequent blows, leaving them incorrect
            int oldGap = Gap;
            Gap = newGap;
            GapCumulativeRow += -oldGap + Gap;
            GapCumulative += -oldGap + Gap;
            GapStr = Gap.ToString();
        }
    }
}
