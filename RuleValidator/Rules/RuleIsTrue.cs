using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace RuleValidator.Rules
{
    public class RuleIsTrue : BaseRule<RuleIsTrue>
    {
        internal RuleIsTrue(bool value) : base(value) { }

        protected override bool ValidateInternal()
        {
            return ((Nullable<bool>)this._Value) == true;
        }

        protected override string ErrorMessageInfo() =>
            $"Value '{ValueToString(_Value)}' is not 'True'.";
    }
}
