using System.Collections;

namespace MasterDiploma_FuzzyAssociationRules.AssociativeRules
{
    public class Combinatorics
    {
        public static List<string>[] Subsets(string[] source)
        {

            int cnt = 1 << source.Length;
            var result = new List<string>[cnt - 1];
            for (int i = 1; i < cnt; i++) // skip the empty set
            {
                var subset = new List<string>(source.Length);
                BitArray b = new BitArray(new int[] { i });
                for (int j = 0; j < source.Length; j++)
                {
                    if (b[j])
                        subset.Add(source[j]);
                }
                result[i - 1] = subset;
            }
            return result;
        }
    }
}