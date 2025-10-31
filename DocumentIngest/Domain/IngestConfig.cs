using System.Collections.Generic;

namespace DocumentIngestApp
{
    public class IngestConfig
    {
        public long MaxContentLength { get; set; }
        public HashSet<string> AcceptedMimes { get; set; } = new HashSet<string>();
    }
}