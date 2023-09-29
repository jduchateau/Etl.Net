using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Paillave.Etl.Core;

namespace Paillave.AzureDocumentIntelligence;

public class AzureDocumentValuesProvider : ValuesProviderBase<IFileValue, AzureDocument>
{
    private readonly AzureDocumentValuesProviderArgs _args;
    public AzureDocumentValuesProvider(AzureDocumentValuesProviderArgs args) => _args = args;

    public override void PushValues(IFileValue input, Action<AzureDocument> push,
        CancellationToken cancellationToken,
        IExecutionContext context)
    {
        using var stream = input.Get(_args.UseStreamCopy);
        var client = new DocumentAnalysisClient(_args.Endpoint, _args.Credential);
        var operation = client.AnalyzeDocument(WaitUntil.Completed, _args.ModelId, stream, _args.DocumentOptions,
            cancellationToken);
        push(new AzureDocument
        {
            Operation = operation,
            File = input,
        });
    }

    public override ProcessImpact PerformanceImpact => ProcessImpact.Heavy;
    public override ProcessImpact MemoryFootPrint => ProcessImpact.Heavy;
}