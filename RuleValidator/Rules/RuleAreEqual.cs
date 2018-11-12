using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace RuleValidator.Rules
{
    public class RuleAreEqual<T> : BaseRule<RuleAreEqual<T>>
    {
        protected T _Expected { get; set; }

        internal RuleAreEqual(T expected, T value) : base(value)
        {
            this._Expected = expected;
        }

        protected override bool ValidateInternal()
        {
            return _Expected.Equals((T)this._Value);
        }

        protected override string ErrorMessageInfo() =>
            $"Value '{ValueToString(_Value)}' is not equal to expected '{ValueToString(_Expected)}'.";
    }
}
