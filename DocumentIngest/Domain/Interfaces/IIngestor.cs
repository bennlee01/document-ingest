using System;

namespace DocumentIngestApp.Domain.Interfaces
{
    /// <summary>
    /// Domain service that performs ingestion of uploaded content:
    /// validates, computes SHA256, detects MIME, and forwards to a sink.
    /// </summary>
    public interface IIngestor
    {
        /// <summary>
        /// Ingests a document from the given byte source, validates it
        /// against the provided configuration and metadata, and forwards
        /// the validated bytes and result to the given sink.
        /// </summary>
        /// <param name="meta">Metadata describing the upload.</param>
        /// <param name="cfg">Validation and configuration settings.</param>
        /// <param name="src">Finite, read-once source of bytes representing the upload.</param>
        /// <param name="sink">Sink that receives the validated result and byte stream.</param>
        void Ingest(UploadMeta meta, IngestConfig cfg, IByteSource src, IIngestSink sink);
    }
}
