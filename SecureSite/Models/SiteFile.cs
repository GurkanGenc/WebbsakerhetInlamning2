using System;
using System.ComponentModel.DataAnnotations;

namespace SecureSite.Models
{
    public class SiteFile
    {
        public Guid Id { get; set; }

        [Display(Name = "File Name")]
        public string UntrustedName { get; set; }

        [Display(Name = "Time (Local)")]
        public DateTime Time { get; set; }

        [Display(Name = "Size (Bytes)")]
        public long Size { get; set; }

        public byte[] Content { get; set; }
    }
}