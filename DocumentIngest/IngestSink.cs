namespace DocumentIngestApp
{
    public interface IngestSink
    {
        void Persist(UploadMeta meta, IngestResult result, ByteSource data);
    }
}