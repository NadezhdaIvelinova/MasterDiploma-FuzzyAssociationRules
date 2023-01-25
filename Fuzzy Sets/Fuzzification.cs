using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDiploma_FuzzyAssociationRules.Fuzzy_Sets
{
    public class Fuzzification
    {

        public Fuzzification(MembershipFuctionType membershipFuctionType, IEnumerable<(string label, double[] definingPoints)> setParameters)
        {
            InitializeSets(membershipFuctionType, setParameters);
        }

        public Fuzzification()
        {
            InitializeSets(MembershipFuctionType.Trapezoidal, TrapezoidalParameters);
        }

        public List<FuzzySet> FuzzySets { get; set; }

        /*
        public DataTable FuzzifyTable(DataTable dataTable)
        {
            var newDataTable = new DataTable();
            var oldColumnNames = dataTable.Columns.GetNames().ToArray();

            InitializeColumns(newDataTable, oldColumnNames);
            InitializeRows(dataTable, newDataTable, oldColumnNames);
            return newDataTable;
        }
        */

        //Define Trapezoidal Membership Fuction
        public IEnumerable<(string label, double[] definingPoints)> TrapezoidalParameters { get; } = new []
        {
            ("DISAGREE", new []{1, 1, 1.84, 4}),
            ("NEUTRAL", new []{1.84, 3.08, 4.92, 6.16}),
            ("AGREE", new []{4, 6.16, 7, 7})
        };

        private void InitializeSets(MembershipFuctionType membershipFuctionType, IEnumerable<(string label, double[] definingPoints)> setParameters)
        {
            FuzzySets = new List<FuzzySet>();
            foreach (var setParameter in setParameters)
            {
                var set = new FuzzySet(setParameter.label, setParameter.definingPoints, membershipFuctionType);
                FuzzySets.Add(set);
            }
        }

        private void InitializeColumns(DataTable newDataTable, string[] oldColumnNames)
        {
            foreach (var oldColumnName in oldColumnNames)
            {
                foreach (var fuzzySet in FuzzySets)
                {
                    var newColumnName = NewColumnName(oldColumnName, fuzzySet.Label);
                    newDataTable.Columns.Add(new DataColumn(newColumnName, typeof(double)));
                }
            }
        }

        private void InitializeRows(DataTable oldDataTable, DataTable newDataTable, string[] oldColumnNames)
        {
            foreach (DataRow row in oldDataTable.Rows)
            {
                var newRow = newDataTable.NewRow();
                foreach (var oldColumnName in oldColumnNames)
                {
                    foreach (var fuzzySet in FuzzySets)
                    {
                        var newColumnName = NewColumnName(oldColumnName, fuzzySet.Label);
                        byte oldValue = (byte)row[oldColumnName];
                        var fuzzyValue = fuzzySet.MembershipFunction(oldValue);
                        newRow[newColumnName] = fuzzyValue;
                    }
                }
                newDataTable.Rows.Add(newRow);
            }

        }

        private string NewColumnName(string oldColumnName, string label)
        {
            return $"{oldColumnName}-{label}";
        }
    }
}
