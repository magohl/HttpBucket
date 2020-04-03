using System;
using System.Collections.Generic;
using System.Linq;
using HttpBucket.Models;

namespace HttpBucket.Stores
{
    public class BucketStore : IBucketStore
    {
        public List<Bucket> Buckets {get;set;} = new List<Bucket>();
        public List<BucketEntry> Entries(Guid bucketId)
        {
            var bucket = Buckets.FirstOrDefault(f=>f.Id==bucketId);
            if (bucket == null)
            {
                Buckets.Add(new Bucket{ Id=bucketId});
            }
            return Buckets.FirstOrDefault(f=>f.Id==bucketId).Entries;
        }

        public void AddEntry(Guid bucketId, BucketEntry bucketEntry)
        {
            var bucket = Buckets.FirstOrDefault(f=>f.Id==bucketId);
            if (bucket != null)
            {
                bucket.Entries.Add(bucketEntry);
            }
            else
            {
                Buckets.Add(new Bucket{Id=bucketId});
                Buckets.FirstOrDefault(f=>f.Id==bucketId).Entries.Add(bucketEntry);
            }
        }

        public int Counter(Guid bucketId)
        {
            return Buckets.FirstOrDefault(f=>f.Id==bucketId).Entries.Count;
        }
        
        public DateTime Created(Guid bucketId)
        {
            return Buckets.FirstOrDefault(f=>f.Id==bucketId).Created;
        }
    }
}