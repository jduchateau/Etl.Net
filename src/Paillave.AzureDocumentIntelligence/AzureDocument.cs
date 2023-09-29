using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Paillave.Etl.Core;

namespace Paillave.AzureDocumentIntelligence;

public class AzureDocument
{
    public required AnalyzeDocumentOperation Operation { get; init; }
    public AnalyzeResult Result => Operation.Value;
    public required IFileValue File { get; init; }
}