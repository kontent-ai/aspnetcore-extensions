using Kontent.Ai.AspNetCore.Webhooks.Models;
using Xunit;

namespace Kontent.Ai.AspNetCore.Tests;

public class ReferenceTests
{
    [Fact]
    public void ById_SetsIdOnly()
    {
        var id = Guid.NewGuid();

        var reference = Reference.ById(id);

        Assert.Equal(id, reference.Id);
        Assert.Null(reference.Codename);
        Assert.Null(reference.ExternalId);
    }

    [Fact]
    public void ByCodename_SetsCodenameOnly()
    {
        var reference = Reference.ByCodename("my_item");

        Assert.Equal("my_item", reference.Codename);
        Assert.Null(reference.Id);
        Assert.Null(reference.ExternalId);
    }

    [Fact]
    public void ByExternalId_SetsExternalIdOnly()
    {
        var reference = Reference.ByExternalId("ext-42");

        Assert.Equal("ext-42", reference.ExternalId);
        Assert.Null(reference.Id);
        Assert.Null(reference.Codename);
    }
}
