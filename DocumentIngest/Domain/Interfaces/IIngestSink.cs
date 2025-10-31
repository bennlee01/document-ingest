using System;

namespace DocumentIngestApp.Domain.Interfaces
{
    /// <summary>
    /// Receives the validated upload and associated metadata.
    /// Acts as the downstream persistence interface.
    /// </summary>
    public interface IIngestSink
    {
        /// <summary>
        /// Persists the validated upload result along with the byte source.
        /// </summary>
        /// <param name="meta">Metadata describing the upload.</param>
        /// <param name="result">Result of validation and processing.</param>
        /// <param name="data">Byte source representing the same bytes validated.</param>
        void Persist(UploadMeta meta, IngestResult result, IByteSource data);
    }
}