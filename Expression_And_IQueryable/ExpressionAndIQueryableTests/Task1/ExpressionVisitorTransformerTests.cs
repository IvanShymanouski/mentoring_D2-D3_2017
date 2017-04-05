using ExpressionAndIQueryable.Task1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionAndIQueryableTests.Task1
{
    [TestClass]
    public class ExpressionVisitorTransformerTests
    {
        [TestMethod]
        public void TransformToIncrementTest()
        {
            Expression<Func<double, double>> source = (x) => x + 1.0;

            var actual = new ExpressionVisitorTransformer().VisitAndConvert(source, "");

            Assert.AreEqual("x => Increment(x)", actual?.ToString());
        }

        [TestMethod]
        public void TransformToDecrementTest()
        {
            Expression<Func<int, int>> source = (x) => x - 1;

            var actual = new ExpressionVisitorTransformer().VisitAndConvert(source, "");

            Assert.AreEqual("x => Decrement(x)", actual?.ToString());
        }

        [TestMethod]
        public void NoTransformationTest()
        {
            Expression<Func<int, int>> source = (x) => x - 2;

            var actual = new ExpressionVisitorTransformer().VisitAndConvert(source, "");

            Assert.AreEqual("x => (x - 2)", actual?.ToString());
        }

        [TestMethod]
        public void ParameterIsExistExpression()
        {
            Expression<Func<int, int, int, int>> source = (a, b, c) => a + b + c;

            var actual = new ExpressionVisitorTransformer().Transform(source, new Dictionary<string, object>
            {
                { "a", 1993 },
                { "c", 2020 }
            });

            Assert.AreEqual("b => 1993 + b + 2020", actual?.ToString().Replace("(", "").Replace(")",""));
        }
    }
}
