using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDiploma_FuzzyAssociationRules.AssociativeRules
{
    public class AssociativeRule
    {
        public AssociativeRule(ItemSet LHS, ItemSet RHS, DataTable fuzzyData)
        {
            this.LHS = LHS;
            this.RHS = RHS;
            this.FuzzyData = fuzzyData;
        }

        public ItemSet LHS { get; }
        public ItemSet RHS { get; }
        public DataTable FuzzyData { get; set; }


        public double GetCertaintyFactor()
        {
            double rhsSupport = RHS.Support;
            double confidenceLevel = GetConfidenceLevel();

            double certaintyFactor;
            if (confidenceLevel >= rhsSupport)
                certaintyFactor = (confidenceLevel - rhsSupport) / (1 - rhsSupport);
            else
                certaintyFactor = (confidenceLevel - rhsSupport) / rhsSupport;

            return certaintyFactor;

        }

        private double GetConfidenceLevel()
        {
            double result = 0;
            foreach (DataRow row in FuzzyData.Rows)
                result += LhsRhsUnion(row);
            return (result / FuzzyData.Rows.Count) / LHS.Support;
        }

        private double LhsIntersect(DataRow dataRow)
        {
            double minSoFar = 1;
            foreach (var item in LHS)
            {

                minSoFar = Math.Min(minSoFar, (double)dataRow[item]);
                
            }
            return minSoFar;
        }
        private double RhsIntersect(DataRow dataRow)
        {
            double minSoFar = 1;
            foreach (var item in RHS)
            {
                minSoFar = Math.Min(minSoFar, (double)dataRow[item]);
            }
            return minSoFar;
        }

        private double LhsRhsUnion(DataRow dataRow)
        {
            var lhsIntersect = LhsIntersect(dataRow);
            var rhsIntersect = RhsIntersect(dataRow);
            return Math.Max(lhsIntersect, rhsIntersect);
        }
    }
}
