using System.Collections.Generic;

namespace DocumentIngestApp
{
    public class IngestConfig
    {
        public long MaxContentLength { get; }
        public HashSet<string> AcceptedMimes { get; }

        public IngestConfig(long maxContentLength, HashSet<string> acceptedMimes)
        {
            MaxContentLength = maxContentLength;
            AcceptedMimes = acceptedMimes;
        }
    }
}