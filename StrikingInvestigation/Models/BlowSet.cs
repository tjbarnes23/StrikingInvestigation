using System;
using System.Collections.Generic;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Models
{
    public class BlowSet : BlowSetCore
    {
        public BlowSet(int stage, int numRows, int tenorWeight, int errorType, bool hasErrors)
        {
            Stage = stage;
            NumRows = numRows;
            TenorWeight = tenorWeight;
            ErrorType = errorType;
            HasErrors = hasErrors;
            NumBells = Stage + (Stage % 2);

            Blows = new List<Blow>();
        }

        public int NumBells { get; }

        public List<Blow> Blows { get; }

        public void PopulateBlows(Block block, int testPlace, string audioIdSuffix)
        {
            int count = 1;

            foreach (Row row in block.Rows)
            {
                for (int i = 1; i <= NumBells; i++)
                {
                    Blow blow = new Blow
                    {
                        Bell = row.RowArr[i],
                        ChangeStr = row.ChangeStr
                    };

                    // If this is the test place and the last row, set IsTestBell to true and IsLastBlow to true;
                    if (row.RowNum == block.NumRows && i == testPlace)
                    {
                        blow.IsTestBell = true;
                    }
                    else
                    {
                        blow.IsTestBell = false;
                    }

                    blow.BellActual = BellAct.DeriveBellActual(Stage, TenorWeight, blow.Bell);
                    blow.RowNum = row.RowNum;
                    blow.Place = i;

                    if (blow.RowNum % 2 == 1)
                    {
                        blow.IsHandstroke = true;
                    }
                    else
                    {
                        blow.IsHandstroke = false;
                    }

                    blow.BellSound = "/audio/tws" + blow.BellActual + ".mp3";

                    blow.AudioId = "audio" + count.ToString();

                    // If an audioIdSuffix parameter was provided, add it to the end of AudioId
                    if (audioIdSuffix != string.Empty)
                    {
                        blow.AudioId += audioIdSuffix;
                    }

                    count++;

                    Blows.Add(blow);

                    // Exit if test blow reached
                    if (blow.IsTestBell == true)
                    {
                        break;
                    }
                }
            }
        }

        public void CreateEvenSpacing(int gapRound)
        {
            int gap;
            int gapCumulativeRow = 0;
            int gapCumulative = 0;

            int baseGap = BaseGaps.BaseGap(Stage, TenorWeight, gapRound);

            foreach (Blow blow in Blows)
            {
                // if 1st's place and handstroke, double the gap
                if (blow.Place == 1 && blow.IsHandstroke == true)
                {
                    gap = baseGap * 2;
                }
                else
                {
                    gap = baseGap;
                }

                if (blow.Place == 1)
                {
                    gapCumulativeRow = gap;
                }
                else
                {
                    gapCumulativeRow += gap;
                }

                gapCumulative += gap;

                blow.Gap = gap;
                blow.GapCumulativeRow = gapCumulativeRow;
                blow.GapCumulative = gapCumulative;
                blow.GapStr = gap.ToString();
            }
        }

        public void CreateRandomSpacing(int errorSize, int gapRound)
        {
            int gap;
            int gapCumulativeRow = 0;
            int gapCumulative = 0;

            // Although we round the resulting gap later, we still need to round basegap because
            // otherwise we won't get even striking when errorSize is zero
            // This is because (baseGap * 2) might round to something other than (baseGap rounded) * 2
            // and also because gapReversion will affect whether an indivdiual gap is rounded up or down
            int baseGap = BaseGaps.BaseGap(Stage, TenorWeight, gapRound);

            int baseGapCumulative = 0;
            int gapReversion = 0;

            Random rand = new Random();
            
            foreach (Blow blow in Blows)
            {
                // if 1st's place and handstroke, double the gap
                if (blow.Place == 1 && blow.IsHandstroke == true)
                {
                    gap = rand.Next((baseGap * 2) - errorSize, (baseGap * 2) + (errorSize + 1));
                    baseGapCumulative += (baseGap * 2);
                }
                else
                {
                    gap = rand.Next(baseGap - errorSize, baseGap + (errorSize + 1));
                    baseGapCumulative += baseGap;
                }

                if (gapReversion < 0)
                {
                    gap += rand.Next(gapReversion, 1);
                }
                else if (gapReversion > 0)
                {
                    gap += rand.Next(0, gapReversion + 1);
                }

                if (gapRound > 1)
                {
                    gap = Convert.ToInt32((double)gap / gapRound) * gapRound;
                }

                if (blow.Place == 1)
                {
                    gapCumulativeRow = gap;
                }
                else
                {
                    gapCumulativeRow += gap;
                }

                gapCumulative += gap;

                gapReversion = baseGapCumulative - gapCumulative;

                blow.Gap = gap;
                blow.GapCumulativeRow = gapCumulativeRow;
                blow.GapCumulative = gapCumulative;
                blow.GapStr = gap.ToString();
            }
        }

        public void CreateStrikingError(int gapError)
        {
            // This method adjusts previously generated even striking
            // This method can only be called when Blows contains a complete set of rows (i.e. no partial rows) -- i.e.
            //    this is for use in the AB Test
            // The gap of one blow in each row is either increased or decreased by the gapError parameter
            // The following blow will also be adjusted to give a net zero cumulative adjustment
            for (int i = 1; i <= NumRows; i++)
            {
                Random rand = new Random();
                int place;
                int index;

                // Don't pick the first blow in BlowSet since there isn't an adjacent blow
                // to properly hear the effect of the striking error
                // Also, don't pick the last blow of a row as this also changes the first blow of the
                // next row, and this blow could also be picked as a blow to adjust in the next row
                if (i == 1)
                {
                    place = rand.Next(2, NumBells);
                }
                else
                {
                    place = rand.Next(1, NumBells);
                }

                // Convert row and place to list index
                index = ((i - 1) * NumBells) + place - 1;

                // Randomly choose whether to add error or subtract error
                bool up = rand.Next(0, 2) == 1;

                if (up == true)
                {
                    Blows[index].Gap += gapError;
                    Blows[index].GapCumulativeRow += gapError;
                    Blows[index].GapCumulative += gapError;
                    Blows[index + 1].Gap -= gapError;
                }
                else
                {
                    Blows[index].Gap -= gapError;
                    Blows[index].GapCumulativeRow -= gapError;
                    Blows[index].GapCumulative -= gapError;
                    Blows[index + 1].Gap += gapError;
                }

                Blows[index].GapStr = Blows[index].Gap.ToString();
                Blows[index + 1].GapStr = Blows[index + 1].Gap.ToString();

                // Mark the two places to be highlighted
                Blows[index].IsHighlighted = true;
                Blows[index + 1].IsHighlighted = true;
            }
        }

        public void CreateCompassError(int compassError)
        {
            // This method adjusts previously generated even striking
            // Blows contains a complete set of rows (i.e. no partial rows)
            // Gaps for a random row are either increased or decreased by the compassError parameter
            Random rand = new Random();
            int errorRow = rand.Next(1, NumRows + 1);

            // Work out the starting index
            int index = (errorRow - 1) * NumBells;

            // Pick random add error or subtract error
            bool up = rand.Next(0, 2) == 1;

            for (int i = 1; i <= NumBells; i++)
            {
                if (up == true)
                {
                    Blows[index].Gap += compassError;
                }
                else
                {
                    Blows[index].Gap -= compassError;
                }

                if (index == 0)
                {
                    Blows[index].GapCumulativeRow = Blows[index].Gap;
                    Blows[index].GapCumulative = Blows[index].Gap;
                }
                else if (i == 1)
                {
                    Blows[index].GapCumulativeRow = Blows[index].Gap;
                    Blows[index].GapCumulative = Blows[index - 1].GapCumulative + Blows[index].Gap;
                }
                else
                {
                    Blows[index].GapCumulativeRow = Blows[index - 1].GapCumulativeRow + Blows[index].Gap;
                    Blows[index].GapCumulative = Blows[index - 1].GapCumulative + Blows[index].Gap;
                }

                Blows[index].GapStr = Blows[index].Gap.ToString();

                // Mark the place to be highlighted
                Blows[index].IsHighlighted = true;

                index += 1;
            }
        }

        public void SetUnstruck()
        {
            foreach (Blow blow in Blows)
            {
                if (blow.IsTestBell == true)
                {
                    blow.BellColor = Constants.UnstruckTestBellColor;
                }
                else
                {
                    blow.BellColor = Constants.UnstruckBellColor;
                }
            }
        }

        public void LoadBlows(BlowSetCore blowSetCore, string audioIdSuffix)
        {
            int rowNum = 1;
            int place = 1;

            int currRowNum = 0;
            int gapCumulativeRow = 0;
            int gapCumulative = 0;

            int count = 1;

            foreach (BlowCore blowCore in blowSetCore.BlowsCore)
            {
                Blow blow = new Blow
                {
                    Bell = blowCore.Bell,
                    ChangeStr = blowCore.ChangeStr,
                    Gap = blowCore.Gap,
                    AltGap = blowCore.AltGap,
                    IsTestBell = blowCore.IsTestBell,
                    IsHighlighted = blowCore.IsHighlighted
                };

                blow.BellActual = BellAct.DeriveBellActual(Stage, TenorWeight, blow.Bell);
                blow.RowNum = rowNum;
                blow.Place = place;

                place += 1;
                
                if (place > NumBells)
                {
                    rowNum += 1;
                    place = 1;
                }
                
                if (blow.RowNum % 2 == 1)
                {
                    blow.IsHandstroke = true;
                }
                else
                {
                    blow.IsHandstroke = false;
                }

                blow.BellSound = "/audio/tws" + blow.BellActual + ".mp3";
                
                blow.AudioId = "audio" + count.ToString();

                // If an audioIdSuffix parameter was provided, add it to the end of AudioId
                if (audioIdSuffix != string.Empty)
                {
                    blow.AudioId += audioIdSuffix;
                }

                count++;

                // Update GapCumulativeRow
                if (blow.RowNum == currRowNum)
                {
                    gapCumulativeRow += blow.Gap;
                }
                else
                {
                    gapCumulativeRow = blow.Gap;
                    currRowNum = blow.RowNum;
                }

                blow.GapCumulativeRow = gapCumulativeRow;

                // Update GapCumulative
                gapCumulative += blow.Gap;
                blow.GapCumulative = gapCumulative;

                // Update GapStr
                blow.GapStr = blow.Gap.ToString();

                // Add blow to Blows list
                Blows.Add(blow);
            }
        }
    }
}
