using System.Collections.Generic;

namespace DocumentIngestApp
{
    public class IngestResult
    {
        public string DetectedMime { get; set; } = "";
        public long Size { get; set; } = 0;
        public string Sha256 { get; set; } = "";
        public bool Ok => Errors.Count == 0;
        public List<string> Errors { get; set; } = new List<string>();
    }
}