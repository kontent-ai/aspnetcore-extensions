# ASP.NET Core extensions for Kontent.ai apps

[![NuGet](https://img.shields.io/nuget/vpre/Kontent.Ai.AspNetCore.svg?style=for-the-badge)](https://www.nuget.org/packages/Kontent.Ai.AspNetCore/)
[![Downloads](https://img.shields.io/nuget/dt/Kontent.Ai.AspNetCore.svg?style=for-the-badge)](https://www.nuget.org/packages/Kontent.Ai.AspNetCore/)
[![Stack Overflow](https://img.shields.io/badge/Stack%20Overflow-ASK%20NOW-FE7A16.svg?logo=stackoverflow&logoColor=white&style=for-the-badge)](https://stackoverflow.com/tags/kontent-ai)
[![Discord](https://img.shields.io/discord/821885171984891914?color=%237289DA&label=Kontent.ai%20Discord&logo=discord&style=for-the-badge)](https://discord.gg/SKCxwPtevJ)

Companion package to the [Kontent.ai Delivery SDK](https://github.com/kontent-ai/delivery-sdk-net) that provides ASP.NET Core–specific helpers: responsive image tag helpers, a rich-text tag helper that renders structured content via `IHtmlResolver`, and webhook signature validation middleware.

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

Renders as:

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

#### Supported attributes

| Attribute | Purpose |
|---|---|
| `asset` | `IAsset` to render (required). |
| `title` | Overrides the `alt`/`title` attributes (defaults to `asset.Description`). |
| `default-width` | Width used as the last entry of the generated `sizes` attribute (default `300`). |
| `responsive-widths` | Per-tag override for the widths used to build `srcset` (falls back to `ImageTransformationOptions.ResponsiveWidths`). |
| `rendition` | Name of an asset rendition to apply. Today Kontent.ai supports only `default`. See [Renditions](#renditions) below. |
| `format` | Target image format (`jpg`, `png`, `png8`, `pjpg`, `gif`, `webp`). |
| `quality` | Compression quality for lossy formats (`1`–`100`). |
| `fit` | Fit transformation (`clip`, `scale`, `crop`). |
| `auto-format` | Enables WebP delivery when the browser advertises support. |
| `compression` | WebP compression (`lossless` / `lossy`); only meaningful when WebP is delivered. |

Standard HTML `width` and `height` attributes on the `<img-asset>` are honored and translate to `w=`/`h=` query parameters. Setting either one disables `srcset`/`sizes` generation.

#### Renditions

When `rendition="default"` is set and the asset exposes that rendition, its query string is appended to the asset URL to produce `src`, `srcset`/`sizes` are **not** generated (a rendition represents a single chosen crop), and only encoding-level transforms (`format`, `quality`, `auto-format`, `compression`) layer on top. The tag helper's `width`, `height`, and `fit` attributes are ignored because the rendition already defines those.

If the named rendition is not present on the asset, the tag helper silently falls back to the non-rendition path.

#### Attribute interaction matrix

| Configuration | `width` / `height` attrs | `ResponsiveWidths` → srcset | `fit` | Encoding (`format`, `quality`, `auto-format`, `compression`) |
|---|---|---|---|---|
| No `rendition` | Applied | Generates `srcset` + `sizes` | Applied | Applied to every generated URL |
| `rendition="default"` (found) | Ignored | Skipped | Ignored | Applied on top of the rendition query |
| `rendition="…"` (not found) | Applied | Generates `srcset` + `sizes` | Applied | Applied to every generated URL |

#### Custom asset domain

The Delivery SDK handles custom asset domains at mapping time — configure it with `WithCustomAssetDomain("https://cdn.example.com")` when building the `DeliveryClient`. By the time assets reach the tag helper, their `Url` already points at the custom domain, so the `<img>` element emitted by `<img-asset>` uses that domain without any extra configuration in this package.

### `rich-text` tag helper

Renders Kontent.ai structured rich-text content as HTML in Razor views. Integrates with the Delivery SDK's `IHtmlResolver` for customizing how embedded content, content item links, inline images, and HTML nodes are rendered.

`Program.cs` (optional DI registration):

```csharp
using Kontent.Ai.AspNetCore.RichText;

builder.Services.AddKontentRichText(resolverBuilder => resolverBuilder
    .WithContentResolver<Article>(a =>
        $"<div class='article'><h2>{a.Elements.Title}</h2></div>")
    .WithContentItemLinkResolver("article", (link, _) =>
        ValueTask.FromResult($"<a href=\"/articles/{link.ItemId}\">link</a>")));
```

`View.cshtml`:

```razor
@* Uses the IHtmlResolver registered in DI (or SDK defaults if none registered). *@
<rich-text content="@Model.Body" />

@* Per-view resolver override. *@
<rich-text content="@Model.Body" resolver="@myCustomResolver" />
```

The tag helper does not emit a `<rich-text>` wrapper — the resolver's HTML is rendered in place of the element.

#### Extension method alternative

For partial views, view components, or scenarios that benefit from an explicit `CancellationToken`:

```razor
@await Model.Body.ToHtmlContentAsync()
@await Model.Body.ToHtmlContentAsync(myResolver, ViewContext.HttpContext.RequestAborted)
```

#### Without DI registration

Both the tag helper and the extension method fall back to `new HtmlResolverBuilder().Build()` when no resolver is provided and none is registered in DI. This uses the SDK's built-in defaults: HTML-encoded text nodes, default inline-image rendering, and diagnostic HTML comments for missing embedded-content and content-item-link resolvers. See the [Delivery SDK documentation](https://github.com/kontent-ai/delivery-sdk-net) for the full `IHtmlResolverBuilder` API.

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
