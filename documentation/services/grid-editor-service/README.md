# EnterspeedGridEditorService : IEnterspeedGridEditorService

This service is used for converting an Umbraco grid editor value to an IEnterspeedProperty.

## Methods

```csharp
IEnterspeedProperty ConvertGridEditor(GridControl control, string culture = null)
```

This will find the correct registered [Grid Editor Value Converter](./../../enterspeed-value-converters/grid-editor-value-converters/README.md),  
and convert the value to an IEnterspeedProperty
