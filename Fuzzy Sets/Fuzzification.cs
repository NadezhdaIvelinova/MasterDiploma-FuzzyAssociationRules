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

        private readonly IEnumerable<FuzzyParameter> _fuzzyParameters;

        public Fuzzification(IEnumerable<FuzzyParameter> fuzzyParameters)
        {
            _fuzzyParameters = fuzzyParameters;
        }


        public Fuzzification(MembershipFuctionType membershipFuctionType, IEnumerable<(string label, double[] definingPoints)> setParameters)
        {
            InitializeSets(membershipFuctionType, setParameters);
        }

        public Fuzzification()
        {
            InitializeSets(MembershipFuctionType.Trapezoidal, TrapezoidalParameters);
        }

        public List<FuzzySet> FuzzySets { get; set; }


        public DataTable FuzzifyTable(DataTable dataTable)
        {
            var newDataTable = new DataTable();
            var oldColumnNames = dataTable.Columns.GetNames().ToArray();

            InitializeColumns(newDataTable, oldColumnNames);
            InitializeRows(dataTable, newDataTable, oldColumnNames);
            return newDataTable;
        }


        //Define Trapezoidal Membership Fuction
        public IEnumerable<(string label, double[] definingPoints)> TrapezoidalParameters { get; } = new[]
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
            bool initializeThroughFuzzyParameters = _fuzzyParameters != null;
            if (initializeThroughFuzzyParameters)
                InitializeColumnsThroughFuzzyParameters(newDataTable, oldColumnNames);
            else
                InitializeColumnsThroughFuzzySets(newDataTable, oldColumnNames);
        }

        private void InitializeColumnsThroughFuzzyParameters(DataTable newDataTable, string[] oldColumnNames)
        {
            foreach (var oldColumnName in oldColumnNames)
            {
                var parameter = _fuzzyParameters.First(p => p.AppliesToColumns.Contains(oldColumnName));
                foreach (var fuzzySetParameter in parameter.Sets)
                {
                    var newColumnName = NewColumnName(oldColumnName, fuzzySetParameter.Label);
                    newDataTable.Columns.Add(new DataColumn(newColumnName, typeof(double)));
                }
            }
        }

        private void InitializeColumnsThroughFuzzySets(DataTable newDataTable, string[] oldColumnNames)
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
            bool initializeThroughFuzzyParameters = _fuzzyParameters != null;
            if (initializeThroughFuzzyParameters)
                InitializeRowsThroughFuzzyParameters(oldDataTable, newDataTable, oldColumnNames);
            else
                InitializeRowsThroughFuzzySets(oldDataTable, newDataTable, oldColumnNames);

        }

        private void InitializeRowsThroughFuzzyParameters(DataTable oldDataTable, DataTable newDataTable, string[] oldColumnNames)
        {
            foreach (DataRow row in oldDataTable.Rows)
            {
                var newRow = newDataTable.NewRow();
                foreach (var oldColumnName in oldColumnNames)
                {
                    var parameter = _fuzzyParameters.First(p => p.AppliesToColumns.Contains(oldColumnName));
                    foreach (var fuzzySetParameter in parameter.Sets)
                    {
                        var newColumnName = NewColumnName(oldColumnName, fuzzySetParameter.Label);
                        byte oldValue = (byte)(int)row[oldColumnName];

                        var temporaryFuzzySet = new FuzzySet(fuzzySetParameter.Label, fuzzySetParameter.Points, fuzzySetParameter.MembershipFunctionType);
                        var fuzzyValue = temporaryFuzzySet.MembershipFunction(oldValue);
                        newRow[newColumnName] = fuzzyValue;
                    }
                }
                newDataTable.Rows.Add(newRow);
            }
        }

        private void InitializeRowsThroughFuzzySets(DataTable oldDataTable, DataTable newDataTable, string[] oldColumnNames)
        {
            foreach (DataRow row in oldDataTable.Rows)
            {
                var newRow = newDataTable.NewRow();
                foreach (var oldColumnName in oldColumnNames)
                {
                    foreach (var fuzzySet in FuzzySets)
                    {
                        var newColumnName = NewColumnName(oldColumnName, fuzzySet.Label);
                        byte oldValue = (byte)(int)row[oldColumnName];
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
