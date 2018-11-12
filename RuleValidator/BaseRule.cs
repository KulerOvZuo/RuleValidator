using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace RuleValidator
{
    public abstract class BaseRule<TRule>
        where TRule : BaseRule<TRule>
    {
        protected bool _NullIsValid { get; set; } = true;
        protected bool? _ValidateIfTrue { get; set; } = null;

        protected string[] _PropertyNames { get; set; }
        protected bool _CustomErrorMessage { get; set; }
        protected string _ErrorMessage { get; set; }
        protected object _Value { get; set; }

        public BaseRule(object value)
        {
            this._Value = value;
        }

        #region Helpers
        protected string ValueToString(object value)
        {
            if (value == null)
                return "<null>";

            var type = value.GetType();
            if (type == typeof(string) && (string)value == string.Empty)
                return "<string.Empty>";

            return value.ToString();
        }
        #endregion

        #region General
        protected TRule This => (TRule)this;

        public TRule NullIsValid(bool valid = true)
        {
            this._NullIsValid = valid;
            return This;
        }

        public TRule ValidateIfTrue(bool value)
        {
            this._ValidateIfTrue = value;
            return This;
        }
        #endregion

        #region Property
        public TRule PropertyName(string propertyName)
        {
            this._PropertyNames = new[] { propertyName };
            return This;
        }

        public TRule PropertyName(params string[] propertyNames)
        {
            this._PropertyNames = propertyNames;
            return This;
        }
        #endregion

        #region ErrorMessage
        public TRule CustomErrorMessage(string errorMessage)
        {
            this._CustomErrorMessage = true;
            this._ErrorMessage = errorMessage;
            return This;
        }

        protected virtual string ErrorMessageInfo() => $"Value {ValueToString(_Value)} is not valid.";

        private string GetErrorMessage()
        {
            if (_CustomErrorMessage)
                return _ErrorMessage;
            return ErrorMessageInfo();
        }
        #endregion
        
        #region Validate
        public bool Validate(out ValidationResult result)
        {
            result = Validate();
            return result == null || result == ValidationResult.Success;
        }

        public virtual ValidationResult Validate()
        {
            if (this._ValidateIfTrue == false)
                return ValidationResult.Success;

            if (this._Value == null)
            {
                if (this._NullIsValid)
                    return ValidationResult.Success;
                return new ValidationResult("Field value is required", this._PropertyNames);
            }

            if (this.ValidateInternal())
                return ValidationResult.Success;

            return new ValidationResult(GetErrorMessage(), this._PropertyNames);
        }

        protected abstract bool ValidateInternal();
        #endregion
    }

    public abstract class BaseEnumerableRule<TEnumerable, TRule> : BaseRule<TRule>
        where TRule : BaseEnumerableRule<TEnumerable, TRule>
    {
        protected IEnumerable<TEnumerable> ValidValues;

        protected bool _ShowValidValues { get; set; } = false;

        public BaseEnumerableRule(object value, IEnumerable<TEnumerable> validValues) : base(value)
        {
            this.ValidValues = validValues;
        }

        public TRule ShowValidValues(bool show = true)
        {
            _ShowValidValues = show;
            return This;
        }

        protected override bool ValidateInternal()
        {
            return ValidValues.Contains((TEnumerable)this._Value);
        }

        protected override string ErrorMessageInfo()
        {
            var error = $"Value '{ValueToString(_Value)}' is not in range of valid context.";

            if (_ShowValidValues)
                error += $" Valid: {string.Join(", ", ValidValues.Select(c => "'" + ValueToString(c) + "'"))}.";

            return error;
        }
    }
}
