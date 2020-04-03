using System;

namespace HttpBucket.Models
{
    public class BucketEntry
    {
        public DateTime Received { get; set; }
        public string Message { get; set; }
    }
}