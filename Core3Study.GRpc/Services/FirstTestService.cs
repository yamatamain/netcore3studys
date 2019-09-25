using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core3Study.GRpc.Services
{
    public class FirstTestService:FirstTest.FirstTestBase
    {
        private static readonly List<string> Cats = new List<string>() { "英短银渐层", "英短金渐层", "美短", "蓝猫", "狸花猫", "橘猫" };
        private static readonly Random Rand = new Random(DateTime.Now.Millisecond);
        public override Task<Reply> GetCacheValue(Request request, ServerCallContext context)
        {
            return Task.FromResult(new Reply()
            {
                Value = $"您吸了一只{Cats[Rand.Next(0, Cats.Count)]}"
            });
        }
    }
}
