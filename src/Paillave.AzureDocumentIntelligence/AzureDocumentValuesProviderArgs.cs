using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace Paillave.AzureDocumentIntelligence;

public class AzureDocumentValuesProviderArgs
{
    public AzureKeyCredential? Credential { get; set; }
    public Uri? Endpoint { get; set; }

    public AnalyzeDocumentOptions DocumentOptions { get; set; } = new AnalyzeDocumentOptions();

    public string ModelId { get; set; } = "prebuilt-read";

    public AzureDocumentValuesProviderArgs SetKey(string key)
    {
        Credential = new AzureKeyCredential(key);
        return this;
    }

    public AzureDocumentValuesProviderArgs SetEndpoint(string endpoint)
    {
        Endpoint = new Uri(endpoint);
        return this;
    }

    public bool UseStreamCopy { get; set; } = true;
}