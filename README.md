# ASP.NET Core extensions for Kontent.ai apps

[![NuGet](https://img.shields.io/nuget/vpre/Kontent.Ai.AspNetCore.svg?style=for-the-badge)](https://www.nuget.org/packages/Kontent.Ai.AspNetCore/)
[![Downloads](https://img.shields.io/nuget/dt/Kontent.Ai.AspNetCore.svg?style=for-the-badge)](https://www.nuget.org/packages/Kontent.Ai.AspNetCore/)
[![Stack Overflow](https://img.shields.io/badge/Stack%20Overflow-ASK%20NOW-FE7A16.svg?logo=stackoverflow&logoColor=white&style=for-the-badge)](https://stackoverflow.com/tags/kontent-ai)
[![Discord](https://img.shields.io/discord/821885171984891914?color=%237289DA&label=Kontent.ai%20Discord&logo=discord&style=for-the-badge)](https://discord.gg/SKCxwPtevJ)

Companion package to the [Kontent.ai Delivery SDK](https://github.com/kontent-ai/delivery-sdk-net) that provides ASP.NET Core–specific helpers: responsive image tag helpers and webhook signature validation middleware.

## Installation

```bash
dotnet add package Kontent.Ai.AspNetCore
```

The package targets `net8.0` and aligns version-wise with the Delivery SDK (`19.x`).

## Tag Helpers

### `img-asset` tag helper

Useful for rendering responsive images. Accepts any `IAsset` returned by the Delivery SDK (rich-text asset elements, asset element values, etc.).

`appsettings.json`:

```json
"ImageTransformationOptions": {
  "ResponsiveWidths": [ 200, 300, 400, 600, 800, 1000, 1200, 1400, 1600, 2000 ]
}
```

`Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ImageTransformationOptions>(
    builder.Configuration.GetSection(nameof(ImageTransformationOptions)));

var app = builder.Build();
```

`_ViewImports.cshtml`:

```razor
@addTagHelper *, Kontent.Ai.AspNetCore
```

`View.cshtml`:

```razor
<img-asset asset="@Model.TeaserImage.First()" class="img-responsive" default-width="300">
  <media-condition min-width="769" image-width="300" />
  <media-condition min-width="330" max-width="768" image-width="689" />
</img-asset>
```

#### Output

```html
<img
  class="img-responsive"
  alt="Coffee Brewing Techniques"
  sizes="(min-width: 769px) 300px, (max-width: 768px) and (min-width: 330px) 689px, 300px"
  src="https://assets-us-01.kc-usercontent.com/975bf280-fd91-488c-994c-2f04416e5ee3/fcbb12e6-66a3-4672-85d9-d502d16b8d9c/which-brewing-fits-you-1080px.jpg?w=2000"
  srcset="
    https://assets-us-01.kc-usercontent.com/975bf280-fd91-488c-994c-2f04416e5ee3/fcbb12e6-66a3-4672-85d9-d502d16b8d9c/which-brewing-fits-you-1080px.jpg?w=200   200w,
    https://assets-us-01.kc-usercontent.com/975bf280-fd91-488c-994c-2f04416e5ee3/fcbb12e6-66a3-4672-85d9-d502d16b8d9c/which-brewing-fits-you-1080px.jpg?w=300   300w,
    https://assets-us-01.kc-usercontent.com/975bf280-fd91-488c-994c-2f04416e5ee3/fcbb12e6-66a3-4672-85d9-d502d16b8d9c/which-brewing-fits-you-1080px.jpg?w=400   400w,
    https://assets-us-01.kc-usercontent.com/975bf280-fd91-488c-994c-2f04416e5ee3/fcbb12e6-66a3-4672-85d9-d502d16b8d9c/which-brewing-fits-you-1080px.jpg?w=600   600w,
    https://assets-us-01.kc-usercontent.com/975bf280-fd91-488c-994c-2f04416e5ee3/fcbb12e6-66a3-4672-85d9-d502d16b8d9c/which-brewing-fits-you-1080px.jpg?w=800   800w,
    https://assets-us-01.kc-usercontent.com/975bf280-fd91-488c-994c-2f04416e5ee3/fcbb12e6-66a3-4672-85d9-d502d16b8d9c/which-brewing-fits-you-1080px.jpg?w=1000 1000w,
    https://assets-us-01.kc-usercontent.com/975bf280-fd91-488c-994c-2f04416e5ee3/fcbb12e6-66a3-4672-85d9-d502d16b8d9c/which-brewing-fits-you-1080px.jpg?w=1200 1200w,
    https://assets-us-01.kc-usercontent.com/975bf280-fd91-488c-994c-2f04416e5ee3/fcbb12e6-66a3-4672-85d9-d502d16b8d9c/which-brewing-fits-you-1080px.jpg?w=1400 1400w,
    https://assets-us-01.kc-usercontent.com/975bf280-fd91-488c-994c-2f04416e5ee3/fcbb12e6-66a3-4672-85d9-d502d16b8d9c/which-brewing-fits-you-1080px.jpg?w=1600 1600w,
    https://assets-us-01.kc-usercontent.com/975bf280-fd91-488c-994c-2f04416e5ee3/fcbb12e6-66a3-4672-85d9-d502d16b8d9c/which-brewing-fits-you-1080px.jpg?w=2000 2000w
  "
  title="Coffee Brewing Techniques"
/>
```

## Webhooks

Package provides a model for webhook deserialization: `WebhookNotification`.

## Middlewares

### Webhook signature verification middleware

This middleware verifies the `X-Kontent-ai-Signature` header (and the legacy `X-KC-Signature` header). Returns `401 Unauthorized` when the signature is missing or invalid.

`appsettings.json`:

```json
"WebhookOptions": {
  "Secret": "<your_secret>"
}
```

`Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWebhookSignatureValidator(
    context => context.Request.Path.StartsWithSegments("/webhooks", StringComparison.OrdinalIgnoreCase),
    builder.Configuration.GetSection(nameof(WebhookOptions)));
```

## Upgrading to v19

Version `19.x` aligns with `Kontent.Ai.Delivery 19.0`, which removed the `IImage` interface. The `img-asset` tag helper now accepts `IAsset` directly:

```diff
- public IImage Asset { get; set; }
+ public IAsset? Asset { get; set; }
```

No changes are needed in your Razor views as long as you pass assets returned by the Delivery SDK (`IAsset` values from element accessors and rich-text assets).
