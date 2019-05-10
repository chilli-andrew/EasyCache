using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NUnit.Framework;

namespace EasyCache.Tests
{
    [TestFixture]
    public class EasyCacheTests
    {
        [TestFixture]
        public class Set
        {
            [Test]
            public void GivenNullKey_ShouldThrow()
            {
                // Arrange
                var easyCache = CreateEasyCache();
                // Act

                var ex = Assert.Throws<InvalidOperationException>(() => easyCache.Set(null, 1));
                // Assert
                Assert.AreEqual("A key must be supplied", ex.Message);
            }

            [Test]
            public void GivenEmptyKey_ShouldThrow()
            {
                // Arrange
                var easyCache = CreateEasyCache();
                // Act

                var ex = Assert.Throws<InvalidOperationException>(() => easyCache.Set(String.Empty, 1));
                // Assert
                Assert.AreEqual("A key must be supplied", ex.Message);
            }

            [TestFixture]
            public class GivenValidInputs
            {
                [Test]
                public void ShouldSetSerializedValueInCache()
                {
                    // Arrange
                    var key = "person/1";
                    var person = new Person {FirstName = "John", LastName = "Jones", Age = 45};
                    var actualJson = "";
                    var expectedJson = "{\"Age\":45,\"LastName\":\"Jones\",\"FirstName\":\"John\"}";
                    var distributedCache = Substitute.For<IDistributedCache>();
                    distributedCache.When(x => x.Set(key, Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>()))
                        .Do((callInfo) =>
                        {
                            actualJson = System.Text.Encoding.UTF8.GetString(callInfo.Arg<byte[]>());
                        });
                    var easyCache = CreateEasyCache(distributedCache);
                    // Act
                    easyCache.Set(key, person);
                    // Assert
                    Assert.AreEqual(expectedJson, actualJson);
                }
            }
        }

        [TestFixture]
        public class Get
        {
            [Test]
            public void GivenKeyIsNull_ShouldThrow()
            {
                // Arrange
                var easyCache = CreateEasyCache();
                // Act
                var ex = Assert.Throws<InvalidOperationException>(() => easyCache.Get<Person>(null));
                // Assert
                Assert.AreEqual("A key must be supplied", ex.Message);
            }

            [Test]
            public void GivenKeyIsEmptyString_ShouldThrow()
            {
                // Arrange
                var easyCache = CreateEasyCache();
                // Act
                var ex = Assert.Throws<InvalidOperationException>(() => easyCache.Get<Person>(String.Empty));
                // Assert
                Assert.AreEqual("A key must be supplied", ex.Message);
            }

            [TestFixture]
            public class GivenValidKey
            {
                [Test]
                public void ShouldReturnObjectFromCache()
                {
                    // Arrange
                    var key = "person/1";
                    var person = new Person { FirstName = "John", LastName = "Jones", Age = 45 };
                    var distributedCache = Substitute.For<IDistributedCache>();
                    distributedCache.Get(key)
                        .Returns(Encoding.UTF8.GetBytes("{\"Age\":45,\"LastName\":\"Jones\",\"FirstName\":\"John\"}"));
                    var easyCache = CreateEasyCache(distributedCache);

                    // Act
                    var result = easyCache.Get<Person>(key);
                    Assert.AreEqual(person.FirstName, result.FirstName);
                    Assert.AreEqual(person.LastName, result.LastName);
                    Assert.AreEqual(person.Age, result.Age);
                }
            }

        }

        private static EasyCache CreateEasyCache()
        {
            var distributedCache = Substitute.For<IDistributedCache>();
            var compressionService = Substitute.For<ICompressionService>();
            return new EasyCache(distributedCache, compressionService);
        }

        private static EasyCache CreateEasyCache(IDistributedCache distributedCache)
        {
            var compressionService = Substitute.For<ICompressionService>();
            return new EasyCache(distributedCache, compressionService);
        }

        internal class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
        }

    }
}
