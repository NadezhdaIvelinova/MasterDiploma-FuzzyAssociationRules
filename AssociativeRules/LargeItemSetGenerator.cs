using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDiploma_FuzzyAssociationRules.AssociativeRules
{
    public class LargeItemSetGenerator
    {

        public IEnumerable<ItemSet> GetFrequentItemSets(DataTable fuzzyData, double treshold)
        {
            var oneItemItemSets = fuzzyData.Columns.GetNames().Select(n => new[] { n });
            var result = GetFrequentItemsetsOfGenerationK(oneItemItemSets, null, fuzzyData, treshold, 1);
            return result;
        }


        private IEnumerable<ItemSet> GetFrequentItemsetsOfGenerationK(
            IEnumerable<IEnumerable<string>> lastGenSubsets,
            IEnumerable<IEnumerable<string>> firstGenSubsets,
            DataTable fuzzyData,
            double threshold,
            int k)
        {
            IEnumerable<IEnumerable<string>> currentGenCandidadateSubsets;
            if (k == 1)
                currentGenCandidadateSubsets = lastGenSubsets;
            else
                currentGenCandidadateSubsets = GenerateCurrentGenCandidates(lastGenSubsets, firstGenSubsets, k);

            var currentGenItemSets = currentGenCandidadateSubsets
                            .Select(subset => (ItemSet: subset, FrequencyPair: IsFrequentItemset(subset, fuzzyData, threshold)))
                            .Where(x => x.FrequencyPair.IsFrequent && (k == 1 || !ShouldPrune(x.ItemSet, lastGenSubsets, k)))
                            .Select(x => new ItemSet(x.ItemSet.ToArray(), x.FrequencyPair.Support))
                            .ToList(); // debug - delete later

            IEnumerable<ItemSet> nextGenItemSets = null;
            if (!currentGenItemSets.Any())
            {
                nextGenItemSets = Enumerable.Empty<ItemSet>();
            }
            else
            {
                if (k == 1)
                    firstGenSubsets = currentGenItemSets.Select(itSet => itSet.Items);

                nextGenItemSets = GetFrequentItemsetsOfGenerationK(
                    currentGenItemSets.Select(itSet => itSet.Items),
                    firstGenSubsets,
                    fuzzyData, threshold, k + 1);
            }


            return currentGenItemSets.Concat(nextGenItemSets);
        }

        private IEnumerable<IEnumerable<string>> GenerateCurrentGenCandidates(IEnumerable<IEnumerable<string>> lastGenSubsets,
            IEnumerable<IEnumerable<string>> firstGenSubsets, int k)
        {
            var result = new List<List<string>>();
            foreach (var lastGenSubset in lastGenSubsets)
            {
                foreach (var firstGenSubset in firstGenSubsets)
                {
                    var goodNewSubset = new List<string>();
                    if (!lastGenSubset.Intersect(firstGenSubset).Any())
                    {
                        goodNewSubset.AddRange(lastGenSubset.Concat(firstGenSubset));
                        if (!result.Any(ss => ss.Intersect(goodNewSubset).Count() == k))
                            result.Add(goodNewSubset);
                    }
                }
            }
            return result;
        }

        private bool ShouldPrune(IEnumerable<string> currentItemSet, IEnumerable<IEnumerable<string>> lastGenItemSets, int k)
        {
            var allCurrentItemSubsets = Combinatorics.Subsets(currentItemSet.ToArray())
                .Where(subset => subset.Count == k);

            bool itsOk = allCurrentItemSubsets.All(ss => lastGenItemSets.Any(kk => !kk.Except(ss).Any()));
            return !itsOk;
        }

        private (bool IsFrequent, double Support) IsFrequentItemset(IEnumerable<string> itemSet, DataTable fuzzyData, double threshold)
        {
            double itemSetSupport = GetSupportForItemSet(itemSet, fuzzyData);
            bool isFrequent = itemSetSupport >= threshold;
            return (isFrequent, itemSetSupport);
        }

        public static double GetSupportForItemSet(IEnumerable<string> itemSet, DataTable fuzzyData)
        {
            var rowCount = fuzzyData.Rows.Count;
            var projectedDt = new DataView(fuzzyData).ToTable(false, itemSet.ToArray());
            double itemSetSupport = CalculateSupportForItemset(projectedDt) / rowCount;
            return itemSetSupport;
        }

        private static double CalculateSupportForItemset(DataTable projectedColumnsFuzzyData)
        {
            double sum = 0;
            var cols = projectedColumnsFuzzyData.Columns;
            foreach (DataRow row in projectedColumnsFuzzyData.Rows)
            {
                double fuzzyIntersection = 1;
                foreach (DataColumn col in cols)
                    fuzzyIntersection = Math.Min(fuzzyIntersection, (double)row[col]);

                sum += fuzzyIntersection;
            }
            return sum;
        }

    }
}
