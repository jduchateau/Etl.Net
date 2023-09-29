using Paillave.Etl.Core;

namespace Paillave.AzureDocumentIntelligence;

public static class AzureDiFileEx
{
    public static IStream<AzureDocument> CrossApplyAzureContent(
        this IStream<IFileValue> stream,
        string name,
        Func<AzureDocumentValuesProviderArgs, AzureDocumentValuesProviderArgs> argsBuilder,
        bool noParallelisation = false)
    {
        return stream.CrossApply(name,
            new AzureDocumentValuesProvider(argsBuilder(new AzureDocumentValuesProviderArgs())),
            noParallelisation);
    }
}