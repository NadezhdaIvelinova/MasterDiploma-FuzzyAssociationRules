using MasterDiploma_FuzzyAssociationRules.Fuzzy_Sets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDiploma_FuzzyAssociationRules
{
    public class FuzzyParameter
    {
        public IEnumerable<string> AppliesToColumns { get; set; }
        public IEnumerable<SetParameter> Sets { get; set; }
    }

    public class SetParameter
    {
        public MembershipFuctionType MembershipFunctionType { get; set; }
        public string Label { get; set; }
        public double[] Points { get; set; }
    }
}
