using Paillave.Etl.Core;
using Paillave.Etl.ExecutionToolkit;
using Paillave.Etl.FileSystem;

namespace Paillave.AzureDocumentIntelligence.Test;

public class Tests
{
    private string? _azureKey;
    private string? _azureEndpoint;

    [SetUp]
    public void Setup()
    {
        _azureKey = Environment.GetEnvironmentVariable("AZURE_KEY");
        _azureEndpoint = Environment.GetEnvironmentVariable("AZURE_ENDPOINT");
        Assert.Multiple(() =>
        {
            Assert.That(_azureKey, Is.Not.Null);
            Assert.That(_azureEndpoint, Is.Not.Null);
        });
    }

    [Test]
    public async Task TestReadPdf()
    {
        var documentsText = new List<string>();

        var testFilesFolder = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../../TestFiles"));
        string[] pdfFiles = Directory.GetFiles(testFilesFolder, "*.pdf");

        var processRunner = StreamProcessRunner.Create<string>(ProcessDefinition);
        ITraceReporter traceReporter = new AdvancedConsoleExecutionDisplay();
        Console.WriteLine($"Test files folder: {testFilesFolder}");

        var executionOptions = new ExecutionOptions<string>
        {
            Connectors = new FileValueConnectors()
                .Register(new FileSystemFileValueProvider("PDF", "PDF",
                    testFilesFolder, "*.pdf")),
            TraceProcessDefinition = traceReporter.TraceProcessDefinition,
        };
        processRunner.DebugNodeStream += (sender, e) => { };

        var res = await processRunner.ExecuteAsync(Environment.CurrentDirectory, executionOptions);
        Assert.False(res.Failed, $"Execution failed {res.ErrorTraceEvent}");

        Assert.That(documentsText, Has.Count.EqualTo(pdfFiles.Length));

        Console.WriteLine(documentsText[0]);
        Assert.That(documentsText[0], Contains.Substring("Attijariwafa"));

        void ProcessDefinition(IStream<string> contextStream)
        {
            var pdfFileStream = contextStream
                .FromConnector("Get pdf files", "PDF")
                .Where("Keep one file", file => file.Name.Contains("Attijariwafa"))
                .CrossApplyAzureContent("OCR with Azure",
                    args => args
                        .SetKey(_azureKey!)
                        .SetEndpoint(_azureEndpoint!)
                )
                .Do("Print ocr result",
                    value =>
                    {
                        documentsText.Add(String.Join("\n\n",
                            value.Result.Pages.SelectMany(page => page.Lines.Select(line => line.Content))
                            // value.Result.Pages.SelectMany(page => page.Words)
                            //     .GroupBy(word =>
                            //     {
                            //         var pos = word.BoundingPolygon[0].Y;
                            //         return pos;
                            //         return (int)Math.Round(pos * 6);
                            //     })
                            //     .OrderBy(i => i.Key)
                            //     .Select(line
                            //         => String.Join("  ", line
                            //             .OrderBy(word => word.BoundingPolygon[0].X)
                            //             .Select(word => word.Content)
                            //         )
                            //     ))
                        ));
                    });

            contextStream.WaitWhenDone("Wait ocr", pdfFileStream);
        }
    }
}