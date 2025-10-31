using System;
using DocumentIngestApp.Domain.Interfaces;


namespace DocumentIngestApp
{
    public class MockSink : IIngestSink
    {
        public long BytesReceived { get; private set; } = 0;
        public IngestResult LastResult { get; private set; } = null!;

        public void Persist(UploadMeta meta, IngestResult result, IByteSource data)
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