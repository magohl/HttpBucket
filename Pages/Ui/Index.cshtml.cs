using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttpBucket.Models;
using HttpBucket.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace HttpBucket.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IBucketStore _store;
        public List<BucketEntry> HistoricEntries;
        public Guid BucketId;
        public DateTime Created;
        public IndexModel(ILogger<IndexModel> logger, IBucketStore store)
        {
            _logger = logger;
            _store = store;
        }

        public void OnGet(Guid bucketId)
        {
            BucketId= bucketId;
            HistoricEntries = _store.Entries(bucketId);
            Created = _store.Created(bucketId);
        }
    }
}
