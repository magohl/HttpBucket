using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace HttpBucket.Models
{
    public class BucketEntry
    {
        public int Id { get; set; }
        public DateTime Received { get; set; }
        public int StatusCodeToReturn {get;set;}
        public string Body { get; set; }
        public string Method { get; set; }
        public string Path {get;set;}
        public Dictionary<string, StringValues> Headers {get;set;}
    }
}