# EnterspeedPropertyService : IEnterspeedPropertyService

This service is used for converting an Umbraco property to an IEnterspeedProperty.

## Methods

```csharp
IDictionary<string, IEnterspeedProperty> GetProperties
(IPublishedContent content, string culture = null);
```

```csharp
IDictionary<string, IEnterspeedProperty> ConvertProperties
(IEnumerable<IPublishedProperty> properties, string culture = null);
```

Both methods will find the correct registered [Property Value Converter](./../../enterspeed-value-converters/property-value-converters/README.md)  
and convert the value to an IEnterspeedProperty.
