using System;
using System.Collections.Generic;
using System.Linq;
using HttpBucket.Models;

namespace HttpBucket.Stores
{
    public interface IBucketStore
    {
        List<Bucket> Buckets {get;set;}
        void AddEntry(Guid bucketId, BucketEntry bucketEntry);
        List<BucketEntry> Entries(Guid bucketId);
        int Counter(Guid bucketId);
        DateTime Created(Guid bucketId);
    }
}