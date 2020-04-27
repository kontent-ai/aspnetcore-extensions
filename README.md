[![NuGet](https://img.shields.io/nuget/v/Kentico.Kontent.AspNetCore.svg)](https://www.nuget.org/packages/Kentico.Kontent.AspNetCore/)
[![AppVeyor](https://ci.appveyor.com/api/projects/status/5gm4u8267mabf8af/branch/master?svg=true)](https://ci.appveyor.com/project/kentico/kontent-aspnetcore)
[![Stack Overflow](https://img.shields.io/badge/Stack%20Overflow-ASK%20NOW-FE7A16.svg?logo=stackoverflow&logoColor=white)](https://stackoverflow.com/tags/kentico-kontent)

# ASP.NET Core extensions for Kentico Kontent apps.

## Tag Helpers
### `img-asset` tag helper
Useful for rendering responsive images. Supports Assets and Inline images in rich-text elements.


**appsettings.json**
```json
...
"ImageTransformationOptions": {
    "ResponsiveWidths": [ 200, 300, 400, 600, 800, 1000, 1200, 1400, 1600, 2000 ]
  }
...
```

**Startup.cs**
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Adds services required for using options.
    services.AddOptions();

    // Register the ImageTransformationOptions required by Kentico Kontent tag helpers
    services.Configure<ImageTransformationOptions>(Configuration.GetSection(nameof(ImageTransformationOptions)));
}
```

**View.cshtml**
```html
<img-asset asset="@Model.TeaserImage.First()" class="img-responsive" default-width="300">
  <media-condition min-width="769" image-width="300" />
  <media-condition min-width="330" max-width="768" image-width="689" />
</img-asset>
```

## Middlewares
### Webhook signature verification middleware
This middleware verifies the `X-KC-Signature` header. Returns 401 response if the signature is invalid.

**appsettings.json**
```json
...
"WebhookOptions": {
    "Secret": "<your_secret>"
  },
...
```

**Startup.cs**
```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
  // Register the validation middleware for any number of controllers that serve for processing webhooks
  app.UseWebhookSignatureValidator(context => context.Request.Path.StartsWithSegments("/webhooks/webhooks", StringComparison.OrdinalIgnoreCase), Configuration.GetSection(nameof(WebhookOptions)));
}
```

![Analytics](https://kentico-ga-beacon.azurewebsites.net/api/UA-69014260-4/Kentico/kontent-aspnetcore?pixel)
