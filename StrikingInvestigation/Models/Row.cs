using System;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Models
{
    public class Row
    {
        internal Row(int stage, int rowNum)
        {
            Stage = stage;
            RowNum = rowNum;
            NumBells = Stage + (Stage % 2);
            ChangeArr = new int[NumBells + 1];
            RowArr = new string[NumBells + 1];
        }

        public int Stage { get; }

        public int RowNum { get; }

        public int NumBells { get; }

        public int[] ChangeArr { get; }

        public string[] RowArr { get; }

        public string ChangeStr { get; private set; } = string.Empty;

        public string RowStr { get; private set; } = string.Empty;

        public void CreateRandomChange()
        {
            Random rand = new Random();

            int i = 1;
            int j;

            do
            {
                if (i == Stage)
                {
                    ChangeArr[i] = i;
                    i += 1;
                }
                else
                {
                    // i is less than Stage
                    j = rand.Next(0, Stage / 2);

                    if (j == 0)
                    {
                        // Make a place at i
                        ChangeArr[i] = i;
                        i += 1;
                    }
                    else
                    {
                        // Cross at i and i+1
                        ChangeArr[i] = i + 1;
                        ChangeArr[i + 1] = i;
                        i += 2;
                    }
                }
            }
            while (i <= Stage);

            // Create a string with the change
            ChangeStr = string.Empty;

            for (i = 1; i <= Stage; i++)
            {
                if (ChangeArr[i] == i)
                {
                    ChangeStr += BellConv.BellStr(i);
                }
            }

            if (ChangeStr == string.Empty)
            {
                ChangeStr = "x";
            }

            // If Stage is odd, add a place to the Change array for the tenor behind
            if (Stage < NumBells)
            {
                ChangeArr[NumBells] = NumBells;
            }
        }

        public void CreateRandomRow()
        {
            // Initialize rounds for the stage
            int[] rounds = new int[Stage + 1];

            // Put numbers 1 - stage in an array with the same indices
            for (int i = 1; i <= Stage; i++)
            {
                rounds[i] = i;
            }

            int place = 1;
            Random rand = new Random();

            for (int i = Stage; i >= 2; i--)
            {
                // Pick a random number from 1 to i
                int j = rand.Next(1, i + 1);

                // Put that bell into the row array
                RowArr[place] = BellConv.BellStr(rounds[j]);

                // Move the bells with higher indices down one position
                for (int k = j + 1; k <= i; k++)
                {
                    rounds[k - 1] = rounds[k];
                }

                // Delete the bell in ith's place
                rounds[i] = 0;

                // Increment rowPos
                place++;
            }

            // Put last remaining bell into row
            RowArr[place] = BellConv.BellStr(rounds[1]);

            // Populate RowStr
            RowStr = string.Empty;

            for (int i = 1; i <= Stage; i++)
            {
                RowStr += RowArr[i];
            }

            // If Stage is odd, add a bell to the RowArr array for the tenor behind
            if (Stage < NumBells)
            {
                RowArr[NumBells] = BellConv.BellStr(NumBells);
            }
        }

        public void CalculateRow(Row prevRow)
        {
            for (int i = 1; i <= Stage; i++)
            {
                RowArr[i] = prevRow.RowArr[ChangeArr[i]];
            }

            // Populate RowStr
            RowStr = string.Empty;

            for (int i = 1; i <= Stage; i++)
            {
                RowStr += RowArr[i];
            }

            // If Stage is odd, add a bell to the RowArr array for the tenor behind
            if (Stage < NumBells)
            {
                RowArr[NumBells] = BellConv.BellStr(NumBells);
            }
        }
    }
}
