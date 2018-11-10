using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using RuleValidator;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var ob = new TestClass
            {
                Code = "AB",
                EnumChar = null,
                EnumInt = "2",
            };

            var results = ob.Validate().ToList();
        }

        private enum TestEnumInt
        {
            value1 = 1,
            value2 = 2,
            value3 = 3
        }

        private enum TestEnumChar
        {
            value1 = 'A',
            value2 = 'B',
            value3 = 'C'
        }

        private class TestClass
        {
            public string Code { get; set; }

            public string EnumInt { get; set; }
            public string EnumChar { get; set; }

            public IEnumerable<ValidationResult> Validate()
            {
                var results = new List<ValidationResult>();
                ValidationResult result;

                if (!Rule.IsEnum<TestEnumChar>(EnumChar)
                        .NullIsValid()
                        .HandleError(nameof(this.EnumChar))
                        .Validate(out result))
                    results.Add(result);

                if (!Rule.IsEnum<TestEnumInt>(EnumInt)
                        .NullIsValid()
                        .HandleError(nameof(this.EnumInt))
                        .Validate(out result))
                    results.Add(result);

                if(!Rule.IsTrue(Code == "CD")
                        .HandleError(nameof(this.Code), "Value is not 'AB'")
                        .Validate(out result))
                    results.Add(result);

                if (!Rule.IsIn(Code, new[] { "CD", "XY" })
                        .HandleError(nameof(this.Code))
                        .Validate(out result))
                    results.Add(result);

                return results;
            }
        }
    }
}
