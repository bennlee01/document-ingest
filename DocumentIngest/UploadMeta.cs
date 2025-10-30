namespace DocumentIngestApp
{
    public class UploadMeta
    {
        public string Filename { get; }
        public string ClaimedMime { get; }
        public long? ContentLength { get; }

        public UploadMeta(string filename, string claimedMime, long? contentLength = null)
        {
            Filename = filename;
            ClaimedMime = claimedMime;
            ContentLength = contentLength;
        }
    }
}