using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace RuleValidator.Rules
{
    public class RuleIsEnum<T> : BaseEnumerableRule<object, RuleIsEnum<T>>
        where T : struct
    {
        internal RuleIsEnum(object value) : base(value, null)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("Type is not enum");

            base.ValidValues = Enum.GetValues(typeof(T)).Cast<object>();
        }

        protected override bool ValidateInternal()
        {
            return Enum.IsDefined(typeof(T), _Value);
        }
    }   
}
