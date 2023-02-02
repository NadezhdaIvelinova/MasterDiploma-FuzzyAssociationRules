using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDiploma_FuzzyAssociationRules.AssociativeRules
{
    [JsonObject]
    public struct ItemSet : IEnumerable<string>, IEquatable<ICollection<string>>
    {
        public ItemSet(string[] items, double support)
        {
            Items = items;
            Support = support;
        }
        public string[] Items { get; set; }
        public double Support { get; set; }

        public int ItemCount { get => Items.Length; }

        public bool Equals([AllowNull] ICollection<string> other)
        {
            return ItemCount == other.Count && Items.Intersect(other).Count() == ItemCount;
        }

        public override int GetHashCode()
        {
            return 17 * Items.Aggregate(1, (agg, curr) => agg *= curr.GetHashCode());
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (var item in Items)
                yield return item;
        }

        public static List<ItemSet> LoadFromFile(string filePath)
        {
            using var sr = new StreamReader(filePath);
            var fileText = sr.ReadToEnd();
            var result = JsonConvert.DeserializeObject<List<ItemSet>>(fileText);
            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}
