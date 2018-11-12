using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using RuleValidator;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class RuleValidatorTest
    {
        [TestMethod]
        public void TestMethod_AllValid()
        {
            var ob = GetValidObject();

            var results = ob.Validate().ToList();
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void TestMethod_NullValidAndInvalid()
        {
            var ob = GetValidObject();
            ob.EnumChar = null;
            ob.EnumInt = null;

            var results = ob.Validate().ToList();

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(nameof(ob.EnumChar), results[0].MemberNames.First());
            Assert.AreEqual(true, results[0].ErrorMessage.Contains("required"));
        }

        [TestMethod]
        public void TestMethod_InvalidEnum()
        {
            var ob = GetValidObject();
            ob.EnumChar = "value4";

            var results = ob.Validate().ToList();

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(nameof(ob.EnumChar), results[0].MemberNames.First());
            Assert.AreEqual(true, results[0].ErrorMessage.Contains("is not in range of valid context"));
        }

        [TestMethod]
        public void TestMethod_InvalidEnumerable()
        {
            var ob = GetValidObject();
            ob.Code = "XY";

            var results = ob.Validate().ToList();

            Assert.AreEqual(2, results.Count);

            Assert.AreEqual(nameof(ob.Code), results[0].MemberNames.First());
            Assert.AreEqual(true, results[0].ErrorMessage.Contains("is not equal to expected"));

            Assert.AreEqual(nameof(ob.Code), results[1].MemberNames.First());
            Assert.AreEqual(true, results[1].ErrorMessage.Contains("is not in range of valid context"));
            Assert.AreEqual(true, results[1].ErrorMessage.Contains(string.Join(", ",
                TestStruct.Values.Select(c => "'" + c + "'"))));
        }

        [TestMethod]
        public void TestMethod_InvalidDecimal()
        { 
            var ob = GetValidObject();
            ob.DecimalValue = 10m;

            var results = ob.Validate().ToList();

            Assert.AreEqual(1, results.Count);

            Assert.AreEqual(nameof(ob.DecimalValue), results[0].MemberNames.First());
            Assert.AreEqual(true, results[0].ErrorMessage.Contains("is less than"));
        }

        [TestMethod]
        public void TestMethod_MasterSlave()
        {
            var ob = GetValidObject();
            ob.BoolMaster = false;
            ob.BoolSlave = true;

            var results = ob.Validate().ToList();

            Assert.AreEqual(0, results.Count);

            ob.BoolMaster = true;
            ob.BoolSlave = false;

            results = ob.Validate().ToList();

            Assert.AreEqual(1, results.Count);

            Assert.AreEqual(nameof(ob.BoolSlave), results[0].MemberNames.First());
            Assert.AreEqual(true, results[0].ErrorMessage.Contains("is not 'True'"));
        }

        #region TestClass
        private TestClass GetValidObject()
        {
            return new TestClass
            {
                Code = TestStruct.Value1,
                EnumChar = TestEnumChar.value1.ToString(),
                EnumInt = TestEnumInt.value2.ToString(),
                BoolMaster = true,
                BoolSlave = true,
                DecimalValue = 60m
            };
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

        private struct TestStruct
        {
            public const string Value1 = "AB";
            public const string Value2 = "CD";
            public const string Value3 = "EF";

            public static string[] Values => new[]
            {
                Value1,
                Value2,
                Value3
            };
        }

        private class TestClass
        {
            public string Code { get; set; }

            public string EnumInt { get; set; }
            public string EnumChar { get; set; }

            public bool BoolMaster { get; set; }
            public bool BoolSlave { get; set; }

            public decimal DecimalValue { get; set; }

            public IEnumerable<ValidationResult> Validate()
            {
                var results = new List<ValidationResult>();
                ValidationResult result;

                if (!Rule.IsEnum<TestEnumChar>(EnumChar)
                        .PropertyName(nameof(this.EnumChar))
                        .NullIsValid(false)
                        .Validate(out result))
                    results.Add(result);

                if (!Rule.IsEnum<TestEnumInt>(EnumInt)
                        .PropertyName(nameof(this.EnumInt))
                        .ShowValidValues()
                        .Validate(out result))
                    results.Add(result);

                if(!Rule.AreEqual(TestStruct.Value1, Code)
                        .PropertyName(nameof(this.Code))
                        .Validate(out result))
                    results.Add(result);

                if (!Rule.IsIn(Code, TestStruct.Values)
                        .PropertyName(nameof(this.Code))
                        .ShowValidValues()
                        .Validate(out result))
                    results.Add(result);

                if (!Rule.IsTrue(DecimalValue > 50)
                        .PropertyName(nameof(this.DecimalValue))
                        .CustomErrorMessage("Value is less than 50")
                        .Validate(out result))
                    results.Add(result);

                if (!Rule.IsTrue(BoolSlave)
                        .ValidateIfTrue(BoolMaster)
                        .PropertyName(nameof(this.BoolSlave))
                        .Validate(out result))
                    results.Add(result);

                return results;
            }
        }
        #endregion
    }
}
