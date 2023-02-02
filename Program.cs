using MasterDiploma_FuzzyAssociationRules;
using MasterDiploma_FuzzyAssociationRules.Fuzzy_Sets;
using System.Data;

class Program
{
    static void Main(string[] args)
    {

        var settings = DataSettings.ReadFromFile("DataSettings.json");
        var repo = new Repository(settings.DbConnectionString);

        var rawTable = repo.LoadTableFromSql(settings.RawDataTableName, settings.Columns);

        DataTable fuzzyfiedTable;
        if (settings.ShouldFuzzify)
        {
            Fuzzification fuzzification;
            if (settings.FuzzyParameters != null)
                fuzzification = new Fuzzification(settings.FuzzyParameters);
            else
                fuzzification = new Fuzzification();

            fuzzyfiedTable = fuzzification.FuzzifyTable(rawTable);


            if (settings.AddFuzzifiedTableToDb)
                repo.CreateFuzzySqlTable(fuzzyfiedTable);
        }
        else
        {
            fuzzyfiedTable = rawTable;
        }
        /*
        double frequentItemSetsTreshold = settings.LargeItemSetTreshold;
        IEnumerable<ItemSet> mostFrequentItemSets;
        if (settings.UsePreComputedRules)
            mostFrequentItemSets = ItemSet.LoadFromFile(@"Data\Pre-loaded-rules-first 25 cols.json");
        else
            mostFrequentItemSets = new LargeItemSetGenerator().GetFrequentItemSets(fuzzyfiedTable, frequentItemSetsTreshold);

        var aPrioriAlgorhitm = new APrioriAlgorithm(fuzzyfiedTable);

        IEnumerable<(AssociativeRule Rule, double CertaintyFactor)> bestRulePairs =
            aPrioriAlgorhitm.GetBestRules(settings.RuleTreshold, mostFrequentItemSets)
            .OrderByDescending(p => p.CertaintyFactor);

        if (settings.GroupRulesByTheirRHS)
        {
            int topNFromEachGroup = settings.TakeTopNFromEachGroup;
            bestRulePairs = bestRulePairs
                    .GroupBy(rp => rp.Rule.RHS)
                    .SelectMany(g => g.Take(topNFromEachGroup));
        }

        if (settings.OutputTopNRules != 0)
            bestRulePairs = bestRulePairs.Take(settings.OutputTopNRules);

        foreach (var rulePair in bestRulePairs)
            Console.WriteLine($"CF: {rulePair.CertaintyFactor}; Rule: " +
                $"{string.Join(',', rulePair.Rule.LHS.Items)} => {string.Join(',', rulePair.Rule.RHS.Items)}");
        */
    }
}
