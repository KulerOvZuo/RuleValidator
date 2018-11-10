using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RuleValidator
{
    using RuleValidator.Internal;
    using System;
    using System.Linq.Expressions;

    public static class Rule
    {
        public static IRule IsEnum<T>(object value) where T : struct
            => new RuleIsEnum<T>(value);
        public static IRule IsTrue(bool value) => new RuleIsTrue(value);
        public static IRule IsFalse(bool value) => IsTrue(!value);
        public static IRule IsIn<T>(T value, IEnumerable<T> isIn) => new RuleIsIn<T>(value, isIn);
    }

    public interface IRule
    {
        bool NullISValid { get; }
        string PropertyName { get; }
        string ErrorMessage { get; }
        object Value { get; }

        IRule NullIsValid(bool valid = true);

        IRuleValidator HandleError(string propertyName, string errorMessage);

        IRuleValidator HandleError(string propertyName);
    }

    public interface IRuleValidator
    {
        bool Validate(out ValidationResult result);
        ValidationResult Validate();
    }
}
