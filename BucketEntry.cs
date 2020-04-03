using System;

namespace HttpBucket
{
    public class BucketEntry
    {
        public int Id {get;set;}
        public DateTime Received { get; set; }
        public Guid Bucket { get; set; }
        public string Message { get; set; }
    }
}