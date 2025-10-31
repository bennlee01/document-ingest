namespace DocumentIngestApp
{
    public class UploadMeta
    {
        public string Filename { get; set; } = "";
        public string ClaimedMime { get; set; } = "";
        public long? ContentLength { get; set; } = null;
    }
}