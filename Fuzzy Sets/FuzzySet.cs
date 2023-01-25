using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDiploma_FuzzyAssociationRules.Fuzzy_Sets
{
    public enum MembershipFuctionType
    {
        Trapezoidal
    }

    public class FuzzySet
    {
        
        public FuzzySet(string label, double[] points, MembershipFuctionType membershipFuctionType)
        {
            Label = label;
            Points = points.OrderBy(p => p).Take(SetType[MembershipFuctionType]).ToArray();
            MembershipFuctionType = membershipFuctionType;
        }

        public string Label { get; }

        public double[] Points { get; }

        public MembershipFuctionType MembershipFuctionType { get; }

        public Dictionary<MembershipFuctionType, int> SetType { get; } = new Dictionary<MembershipFuctionType, int>
        {
            {MembershipFuctionType.Trapezoidal, 4}
        };

        public double MembershipFunction(double point)
        {
            return MembershipFuctionType switch
            {
                MembershipFuctionType.Trapezoidal => TrapezoidalFuction(point),
                _ => throw new InvalidOperationException(),
            };
        }

        private double TrapezoidalFuction(double x)
        {
            double a = Points[0];
            double b = Points[1];
            double c = Points[2];
            double d = Points[3];

            double result;
            if (a == b)
            {
                if (x < a || x > d)
                    result = 0;
                else if (x >= b && x < c)
                    result = 1;
                else if (x >= c && x <= d)
                    result = (d - x) / (d - c);
                else
                    throw new ArgumentOutOfRangeException("X was out of range when a == b");
            }
            else if (c == d)
            {
                if (x < a || x > d)
                    result = 0;
                else if (x >= a && x < b)
                    result = (x - a) / (b - a);
                else if (x >= b && x <= c)
                    result = 1;
                else
                    throw new ArgumentOutOfRangeException("X was out of range when c == d");
            }
            else
            {
                if (x < a || x > d)
                    result = 0;
                else if (x >= a && x < b)
                    result = (x - a) / (b - a);
                else if (x >= b && x < c)
                    result = 1;
                else
                    result = (d - x) / (d - c);
            }

            return result;
        }
    }
}
