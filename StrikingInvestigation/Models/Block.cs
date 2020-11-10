using System.Collections.Generic;

namespace StrikingInvestigation.Models
{
    public class Block
    {
        public Block(int stage, int numRows)
        {
            Stage = stage;
            NumRows = numRows;
            Rows = new List<Row>();
        }

        public int Stage { get; }

        public int NumRows { get; }

        public List<Row> Rows { get; }

        public void CreateRandomBlock()
        {
            Row row = new Row(Stage, 1);
            row.CreateRandomRow();
            Rows.Add(row);

            Row prevRow = row;
            
            for (int i = 2; i <= NumRows; i++)
            {
                row = new Row(Stage, i);
                row.CreateRandomChange();
                row.CalculateRow(prevRow);
                Rows.Add(row);

                prevRow = row;
            }
        }
    }
}
