using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ComparisonExtensions
{
    public enum ConditionComparison
    {
        LessThan,
        LessThanOrEqualTo,
        EqualTo,
        GreaterThanOrEqualTo,
        GreaterThan,
    }

    public static bool CompareValues<T>(this T actualValue, ConditionComparison condition, T expectedValue)
        where T : IComparable
    {
        switch (condition)
        {
            case ConditionComparison.EqualTo:
            case ConditionComparison.GreaterThan:
            case ConditionComparison.GreaterThanOrEqualTo:
            case ConditionComparison.LessThan:
            case ConditionComparison.LessThanOrEqualTo:
                break;
            default:
                break;
        }

        return false;
    }
}
