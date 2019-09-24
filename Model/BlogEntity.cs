using System;

namespace Model
{
    public class BlogEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Auther { get; set; } 
        public DateTime CreateTime { get; set; } 
        public byte Status { get; set; }
    }
}
