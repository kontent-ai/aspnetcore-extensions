using System.Collections;
using Kontent.Ai.Delivery.Abstractions;

namespace Kontent.Ai.AspNetCore.Tests;

internal sealed class EmptyRichTextContent : IRichTextContent
{
    public int Count => 0;
    public IRichTextBlock this[int index] => throw new IndexOutOfRangeException();
    public IEnumerator<IRichTextBlock> GetEnumerator() => Enumerable.Empty<IRichTextBlock>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal sealed class RecordingHtmlResolver : IHtmlResolver
{
    public int CallCount { get; private set; }
    public IRichTextContent? LastContent { get; private set; }
    public CancellationToken LastCancellationToken { get; private set; }
    public string ReturnValue { get; init; } = string.Empty;

    public ValueTask<string> ResolveAsync(IRichTextContent richText, CancellationToken cancellationToken = default)
    {
        CallCount++;
        LastContent = richText;
        LastCancellationToken = cancellationToken;
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(ReturnValue);
    }
}
