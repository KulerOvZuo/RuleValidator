using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace RuleValidator.Rules
{
    public class RuleIsIn<T> : BaseEnumerableRule<T, RuleIsIn<T>>
    {
        internal RuleIsIn(T value, IEnumerable<T> isIn) : base(value, isIn)
        {

        }
    }
}
