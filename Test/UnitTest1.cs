using NUnit.Framework;
using StackExchange.Redis;

namespace Test
{
    public class Tests
    {
        [SetUp] 
        public void Setup()
        {
            RedisValue value = new RedisValue();
            var str = "test";
            Assert.IsTrue(value.Equals(str));
        } 
    }
}