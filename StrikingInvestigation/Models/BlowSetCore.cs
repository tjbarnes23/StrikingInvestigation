using System.Collections.Generic;

namespace StrikingInvestigation.Models
{
    public class BlowSetCore
    {
        // Constructor not used for this class because Json deserializer needs parameterless constructor
        
        public int Stage { get; set; }

        public int NumRows { get; set; }

        public int TenorWeight { get; set; }

        public int ErrorType { get; set; }

        public bool HasErrors { get; set; }

        public List<BlowCore> BlowsCore { get; set; }

        public void LoadBlowsCore(BlowSet blowSet)
        {
            BlowsCore = new List<BlowCore>();

            foreach (Blow blow in blowSet.Blows)
            {
                // Implicit cast from child to parent
                BlowsCore.Add(blow);
            }
        }
    }
}
