using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureSite.Models
{
    public class SiteFile
    {
        public Guid Id { get; set; }
        public string UntrustedName { get; set; }
        public DateTime Time { get; set; }
        public long Size { get; set; }
        public byte[] Content { get; set; }
    }
}
