using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDiploma_FuzzyAssociationRules.AssociativeRules
{
    public class APrioriAlgorithm
    {
        public APrioriAlgorithm(DataTable fuzzyData)
        {
            _fuzzyData = fuzzyData;
        }

        private readonly DataTable _fuzzyData;

        public IEnumerable<(AssociativeRule Rule, double CertaintyFactor)> GetBestRules(double treshold, IEnumerable<ItemSet> mostFrequentItemSets)
        {
            var allRules = GetAllAssociativeRules(mostFrequentItemSets);
            foreach (var rule in allRules)
            {
                double cf = rule.GetCertaintyFactor();
                if (cf >= treshold)
                    yield return (rule, cf);
            }
        }

        private List<AssociativeRule> GetAllAssociativeRules(IEnumerable<ItemSet> mostFrequentItemSets)
        {
            var result = new List<AssociativeRule>();
            foreach (var itemSet in mostFrequentItemSets)
            {
                var associativeRulesForSet = GetAssociativeRuleForItemSet(itemSet, mostFrequentItemSets);
                result.AddRange(associativeRulesForSet);
            }
            return result;
        }

        private List<AssociativeRule> GetAssociativeRuleForItemSet(ItemSet target, IEnumerable<ItemSet> allItemSets)
        {
            var result = new List<AssociativeRule>();
            if (target.ItemCount == 1)
                return result;

            var allItemSubsets = Combinatorics.Subsets(target.Items);
            foreach (var lhsSubset in allItemSubsets)
            {
                foreach (var rhsSubset in allItemSubsets)
                {
                    if (!lhsSubset.Intersect(rhsSubset).Any())
                    {
                        var actualLhsSubset = allItemSets.First(itSet => itSet.Equals(lhsSubset));
                        var actualRhsSubset = allItemSets.First(itSet => itSet.Equals(rhsSubset));
                        result.Add(new AssociativeRule(actualLhsSubset, actualRhsSubset, _fuzzyData));
                    }

                }
            }

            return result;
        }

    }
}
