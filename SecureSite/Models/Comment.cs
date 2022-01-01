using System;

namespace SecureSite.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public DateTime Time { get; set; }
        public string Content { get; set; }
    }
}