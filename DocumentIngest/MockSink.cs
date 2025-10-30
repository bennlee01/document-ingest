using System;

namespace DocumentIngestApp
{
    public class MockSink : IngestSink
    {
        public long BytesReceived { get; private set; } = 0;
        public IngestResult LastResult { get; private set; } = null!;

        public void Persist(UploadMeta meta, IngestResult result, ByteSource data)
        {
            LastResult = result;
            BytesReceived = 0;

            byte[] chunk;
            while ((chunk = data.NextChunk()).Length > 0)
            {
                BytesReceived += chunk.Length;
            }
        }
    }
}