using System;
using System.IO;
using System.Collections.Generic;

namespace DocumentIngestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Config
            var acceptedMimes = new HashSet<string>
            {
                "application/pdf",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "image/png"
            };
            var config = new IngestConfig(maxContentLength: 10_000_000, acceptedMimes: acceptedMimes);

            // Files to ingest
            string resourcesDir = Path.Combine(Directory.GetCurrentDirectory(), "../../test/resources");
            if (!Directory.Exists(resourcesDir))
            {
                Console.WriteLine($"Resources folder not found: {resourcesDir}");
                return;
            }

            var ingestor = new Ingestor();

            foreach (var filePath in Directory.GetFiles(resourcesDir))
            {
                var fileName = Path.GetFileName(filePath);
                var claimedMime = fileName.EndsWith(".pdf") ? "application/pdf" :
                                  fileName.EndsWith(".docx") ? "application/vnd.openxmlformats-officedocument.wordprocessingml.document" :
                                  fileName.EndsWith(".png") ? "image/png" :
                                  "application/octet-stream";

                var meta = new UploadMeta(fileName, claimedMime, contentLength: null);
                var byteSource = new FileByteSource(filePath);

                var sink = new ConsoleSink();

                ingestor.Ingest(meta, config, byteSource, sink);
            }
        }
    }

    // ByteSource from a file
    public class FileByteSource : ByteSource
    {
        private readonly FileStream _stream;

        public FileByteSource(string path)
        {
            _stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        }

        public byte[] NextChunk()
        {
            var buffer = new byte[8192];
            int read = _stream.Read(buffer, 0, buffer.Length);
            if (read == 0) return Array.Empty<byte>();
            if (read < buffer.Length)
            {
                var partial = new byte[read];
                Array.Copy(buffer, partial, read);
                return partial;
            }
            return buffer;
        }
    }

    // Simple sink that prints results
    public class ConsoleSink : IngestSink
    {
        public void Persist(UploadMeta meta, IngestResult result, ByteSource data)
        {
            Console.WriteLine($"{meta.Filename} -> OK: {result.Ok}, MIME: {result.DetectedMime}, Size: {result.Size}, SHA256: {result.Sha256}");
            if (result.Errors.Count > 0)
            {
                Console.WriteLine("Errors: " + string.Join(", ", result.Errors));
            }
        }
    }
}
