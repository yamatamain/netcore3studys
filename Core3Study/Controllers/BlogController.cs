using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Model;

namespace Core3Study.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly RedisHelper _redisHelper;
        private RedisEndPoint _endPoint;
        public BlogController(IOptions<RedisEndPoint> option)
        {
            _endPoint = option.Value;
            if (_endPoint == null)
                throw new ArgumentNullException("redis endpoint connection is null");
            if (_redisHelper == null)
            { 
                _redisHelper = new RedisHelper(_endPoint.Value);
            }
        }

        [HttpPost]
        [Route("add")]
        public async Task<bool> Insert(BlogEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity is null");

            var str = JsonSerializer.Serialize(entity);

            _redisHelper.Set(GetMd5Key(entity), str);
            return true;
        }

        [HttpGet]
        [Route("get")]
        public async Task<string> Get(string key = "test")
        {
            return _redisHelper.Get(key);
        }


        private string GetMd5Key(BlogEntity entity)
        {
            var md5Str = string.Empty;
            var str = $"{entity.Auther}-{entity.Content}-{entity.Status}-{entity.Title}";
            using (var md5 = MD5.Create())
            {
                var data = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    builder.Append(data[i].ToString("X2"));
                }
                md5Str = builder.ToString();
            }
            return md5Str;
        }
    }
}