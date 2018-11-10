using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace RuleValidator.Internal
{
    public abstract class BaseRule : IRule, IRuleValidator
    {
        public bool NullISValid { get; private set; } = false;
        public string PropertyName { get; private set; }
        public string ErrorMessage { get; private set; }
        public object Value { get; private set; }

        public BaseRule(object value)
        {
            this.Value = value;
        }

        #region IRule
        public IRule NullIsValid(bool valid = true)
        {
            this.NullISValid = valid;
            return this;
        }

        public IRuleValidator HandleError(string propertyName, string errorMessage)
        {
            this.PropertyName = propertyName;
            this.ErrorMessage = errorMessage;
            return this;
        }

        public IRuleValidator HandleError(string propertyName)
        {
            this.PropertyName = propertyName;
            this.ErrorMessage = ErrorMessageInfo();
            return this;
        }

        protected virtual string ErrorMessageInfo() => $"Value {this.Value?.ToString()} is not valid";
        #endregion

        #region IValidator
        public bool Validate(out ValidationResult result)
        {
            result = Validate();
            return result == null || result == ValidationResult.Success;
        }

        public ValidationResult Validate()
        {
            if (this.Value == null)
            {
                if (this.NullISValid)
                    return ValidationResult.Success;
                return new ValidationResult("Field value is required", new[] { this.PropertyName });
            }

            if (this.ValidateInternal())
                return ValidationResult.Success;

            return new ValidationResult(this.ErrorMessage, new[] { this.PropertyName });
        }

        protected abstract bool ValidateInternal();
        #endregion
    }

    public class RuleIsEnum<T> : BaseRule, IRule
        where T : struct
    {
        internal RuleIsEnum(object value) : base(value) { }

        protected override bool ValidateInternal()
        {
            return Enum.IsDefined(typeof(T), this.Value);
        }

        protected override string ErrorMessageInfo() =>
            $"Value '{this.Value?.ToString()}' is not valid in context of enum type '{typeof(T).Name}'";
    }

    public class RuleIsTrue : BaseRule, IRule
    {
        internal RuleIsTrue(bool value) : base(value) { }

        protected override bool ValidateInternal()
        {
            return ((Nullable<bool>)this.Value) == true;
        }

        protected override string ErrorMessageInfo() =>
            $"Value '{this.Value?.ToString()}' is not 'True'";
    }

    public class RuleIsIn<T> : BaseRule, IRule
    {
        private IEnumerable<T> IsIn;

        internal RuleIsIn(T value, IEnumerable<T> isIn) : base(value)
        {
            this.IsIn = isIn;
        }

        protected override bool ValidateInternal()
        {
            return IsIn.Contains((T)this.Value);
        }

        protected override string ErrorMessageInfo() =>
            $"Value '{this.Value?.ToString()}' is not in range of valid context";
    }
}
