#region License and Terms
// MoreLINQ - Extensions to LINQ to Objects
// Copyright (c) 2010 Leopold Bushkin. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

namespace MoreLinq.Test
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    /// <summary>
    /// Tests of the various overloads of <see cref="MoreEnumerable"/>.Random()
    /// </summary>
    [TestFixture]
    public class RandomTest
    {
        const int RandomTrials = 10000;

        /// <summary>
        /// Verify that passing a negative maximum value yields an exception
        /// </summary>
        [Test]
        public void TestNegativeMaxValueException()
        {
            const int maxValue = -10;
            Assert.Less(maxValue, 0);

            AssertThrowsArgument.OutOfRangeException("maxValue",() =>
                MoreEnumerable.Random(maxValue));
        }

        /// <summary>
        /// Verify that passing lower bound that is greater than the upper bound results
        /// in an exception.
        /// </summary>
        [Test]
        public void TestMinValueGreaterThanMaxValueException()
        {
            const int minValue = 100;
            const int maxValue = 10;

            Assert.Greater(minValue, maxValue);

            AssertThrowsArgument.OutOfRangeException("minValue",() =>
                MoreEnumerable.Random(minValue, maxValue));
        }

        /// <summary>
        /// Verify that we can produce a valid sequence or random doubles between 0.0 and 1.0
        /// </summary>
        [Test]
        public void TestRandomDouble()
        {
            var resultA = MoreEnumerable.RandomDouble().Take(RandomTrials);
            var resultB = MoreEnumerable.RandomDouble(new Random()).Take(RandomTrials);

            // NOTE: Unclear what should actually be verified here... some additional thought needed.
            Assert.AreEqual(RandomTrials, resultA.Count());
            Assert.AreEqual(RandomTrials, resultB.Count());
            Assert.IsTrue(resultA.All(x => x is >= 0.0 and < 1.0));
            Assert.IsTrue(resultB.All(x => x is >= 0.0 and < 1.0));
        }

        /// <summary>
        /// Verify that the max constraint is preserved by the sequence generator.
        /// </summary>
        [Test]
        public void TestRandomMaxConstraint()
        {
            const int max = 100;
            var resultA = MoreEnumerable.Random(max).Take(RandomTrials);
            var resultB = MoreEnumerable.Random(new Random(), max).Take(RandomTrials);

            Assert.AreEqual(RandomTrials, resultA.Count());
            Assert.AreEqual(RandomTrials, resultB.Count());
            Assert.IsTrue(resultA.All(x => x < max));
            Assert.IsTrue(resultB.All(x => x < max));
        }

        /// <summary>
        /// Verify that the min/max constraints are preserved by the sequence generator.
        /// </summary>
        [Test]
        public void TestRandomMinMaxConstraint()
        {
            const int min = 0;
            const int max = 100;
            var resultA = MoreEnumerable.Random(min, max).Take(RandomTrials);
            var resultB = MoreEnumerable.Random(new Random(), min, max).Take(RandomTrials);

            Assert.AreEqual(RandomTrials, resultA.Count());
            Assert.AreEqual(RandomTrials, resultB.Count());
            Assert.IsTrue(resultA.All(x => x is >= min and < max));
            Assert.IsTrue(resultB.All(x => x is >= min and < max));
        }

        /// <summary>
        /// Evaluate that using a random sequence (with a given generator)
        /// is equivalent to a for loop accessing the same random generator.
        /// </summary>
        [Test]
        public void TestRandomEquivalence()
        {
            const int seed = 12345;
            // must use a specific seed to ensure sequences will be identical
            var randA = new Random(seed);
            var randB = new Random(seed);

            var valuesA = new List<int>();
            for (var i = 0; i < RandomTrials; i++)
                valuesA.Add(randA.Next());

            var randomSeq = MoreEnumerable.Random(randB);
            var valuesB = randomSeq.Take(RandomTrials);

            Assert.That(valuesA, Is.EqualTo(valuesB));
        }
    }
}
