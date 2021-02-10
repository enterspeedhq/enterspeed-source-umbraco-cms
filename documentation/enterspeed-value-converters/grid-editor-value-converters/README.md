# Grid Editor Value Converters

A grideditor value converter is a class that will convert
the input value from an Umbraco grid editor into a [IEnterspeedProperty](https://github.com/enterspeedhq/enterspeed-sdk-dotnet/blob/master/documentation/entities/properties).  
To implement your own converter you need to implement
the `IEnterspeedGridEditorValueConverter` interface

If no converter is registered for the grid editor,
the DefaultGridLayoutPropertyValueConverter will try  
to convert it into an IEnterspeedProprety automatically,
by looking at the propertytypes.

## IEnterspeedGridEditorValueConverter

This interface contains two methods that needs to be implemented

### IsConverter

```csharp
bool IsConverter(string alias);
```

This method is called when the
[EnterspeedGridEditorService](./../../services/grid-editor-service/README.md) tries
to find the proper converter for this grid editor.  
An implementation of this method could look like this:

```csharp
public bool IsConverter(string alias)
{
    return alias.InvariantEquals("rte");
}
```

### Convert

```csharp
IEnterspeedProperty Convert(GridControl editor, string culture)
```

This is the method that is converting the Umbraco grid editor
to an IEnterspeed property.  
An implementation of this method could look like this:

```csharp
public IEnterspeedProperty Convert(GridControl editor, string culture)
{
    return new StringEnterspeedProperty(editor.Value.ToString());
}
```

## Registering a converter

Converters are registered in Umbraco via an IComposer in Umbraco.  
Example:

```csharp
[RuntimeLevel(MinLevel = RuntimeLevel.Run)]
public class MyCustomerGridEditorValueConverterComposer : IUserComposer
{
    composition.EnterspeedGridEditorValueConverters()
                .Append<MyCustomGridEditorValueConverter>();
}
```

Note that the [EnterspeedGridEditorService](./../../services/grid-editor-service/README.md)
will find the converters  
in the order that they are registered, which means that,
if you want to replace a default converter with your own,  
you need to insert your converter like this:

```csharp
[RuntimeLevel(MinLevel = RuntimeLevel.Run)]
public class MyCustomerPropertyValueConverterComposer : IUserComposer
{
    composition.EnterspeedGridEditorValueConverters()
    .InsertBefore<DefaultRichTextEditorGridEditorValueConverter,MyCustomGridEditorValueConverter>();
}
```

## Default converters

Enterspeed ships with [default grid editor value converters](./defaults/README.md)
for some of the built-in grid editors that Umbraco ships with out of the box.
