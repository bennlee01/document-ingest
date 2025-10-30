using System;
using System.IO;

namespace DocumentIngestApp
{
    class Program
    {
        static void Main()
        {
            var cfg = new IngestConfig
            {
                MaxContentLength = 10_000_000,
                AcceptedMimes = new HashSet<string>
                {
                    "application/pdf",
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    "image/png"
                }
            };

            var ingestor = new Ingestor();
            var sink = new MockSink();

            // Get the project directory dynamically
            string projectDir = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;
            string resourcesDir = Path.Combine(projectDir, "resources");

            string[] testFiles = { "sample.pdf", "sample.docx", "sample.png" };
            foreach (var file in testFiles)
            {
                string filePath = Path.Combine(resourcesDir, file);

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"File not found: {filePath}");
                    continue;
                }

                var bytes = File.ReadAllBytes(filePath);
                var meta = new UploadMeta
                {
                    Filename = file,
                    ClaimedMime = "application/octet-stream",
                    ContentLength = bytes.Length
                };

                ingestor.Ingest(meta, cfg, new ArrayByteSource(bytes), sink);

                Console.WriteLine($"File: {file}, DetectedMime: {sink.LastResult.DetectedMime}, Size: {sink.LastResult.Size}, SHA256: {sink.LastResult.Sha256}, Ok: {sink.LastResult.Ok}");
            }
        }
    }
}