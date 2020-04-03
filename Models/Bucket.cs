using System;
using System.Collections.Generic;

namespace HttpBucket.Models
{
    public class Bucket
    {
        public Bucket()
        {
            Created = DateTime.Now;
            Entries = new List<BucketEntry>();
        }
        public Guid Id {get;set;}
        public DateTime Created {get;private set;}
        public List<BucketEntry> Entries { get; set; }
    }
}