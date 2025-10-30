using System.Collections.Generic;

namespace DocumentIngestApp
{
    public class IngestResult
    {
        public string DetectedMime { get; }
        public long Size { get; }
        public string Sha256 { get; }
        public bool Ok { get; }
        public List<string> Errors { get; }

        public IngestResult(string detectedMime, long size, string sha256, bool ok, List<string> errors)
        {
            DetectedMime = detectedMime;
            Size = size;
            Sha256 = sha256;
            Ok = ok;
            Errors = errors;
        }
    }

}