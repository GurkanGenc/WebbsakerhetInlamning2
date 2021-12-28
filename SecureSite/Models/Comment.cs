using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureSite.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public DateTime Time { get; set; }
        public string Content { get; set; }
    }
}
