using System.Collections.Generic;

namespace RuleValidator
{
    using RuleValidator.Rules;

    public static class Rule
    {
        public static RuleIsEnum<T> IsEnum<T>(object value) where T : struct
            => new RuleIsEnum<T>(value);

        public static RuleIsTrue IsTrue(bool value) => new RuleIsTrue(value);

        public static RuleAreEqual<T> AreEqual<T>(T expected, T value) => new RuleAreEqual<T>(expected, value);

        public static RuleIsIn<T> IsIn<T>(T value, IEnumerable<T> isIn) => new RuleIsIn<T>(value, isIn);
    }
}
