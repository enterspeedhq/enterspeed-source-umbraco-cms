# Property Value Converters

A property value converter is a class that will convert
the input value from Umbraco into a [IEnterspeedProperty](https://github.com/enterspeedhq/enterspeed-sdk-dotnet/blob/master/documentation/entities/properties).  
To implement your own converter you need to implement the
`IEnterspeedPropertyValueConverter` interface

## IEnterspeedPropertyValueConverter

This interface contains two methods that needs to be implemented

### IsConverter

```csharp
bool IsConverter(IPublishedPropertyType propertyType);
```

This method is called when the
[EnterspeedPropertyService](../../services/property-service/README.md)
tries to find the proper converter for this property.  
An implementation of this method could look like this:

```csharp
public bool IsConverter(IPublishedPropertyType propertyType)
{
    return propertyType.EditorAlias.Equals("Umbraco.TextBox");
}
```

### Convert

```csharp
IEnterspeedProperty Convert(IPublishedProperty property, string culture);
```

This is the method that is converting the Umbraco property
to an IEnterspeed property.  
An implementation of this method could look like this:

```csharp
public IEnterspeedProperty Convert(IPublishedProperty property, string culture)
{
    var value = property.GetValue<string>(culture);
    return new StringEnterspeedProperty(property.Alias, value);
}
```

## Registering a converter

Converters are registered in Umbraco via an IComposer in Umbraco.
Example:

```csharp
[RuntimeLevel(MinLevel = RuntimeLevel.Run)]
public class MyCustomerPropertyValueConverterComposer : IUserComposer
{
    composition.EnterspeedPropertyValueConverters()
                .Append<MyCustomPropertyValueConverter>();
}
```

Note that the [EnterspeedPropertyService](./../../services/property-service/README.md)
will find the converters in the order that they are registered,  
which means that if you want to replace an default converter with your own,
you need to insert your converter like this:

```csharp
[RuntimeLevel(MinLevel = RuntimeLevel.Run)]
public class MyCustomerPropertyValueConverterComposer : IUserComposer
{
    composition.EnterspeedPropertyValueConverters()
    .InsertBefore<DefaultTextboxPropertyValueConverter,MyCustomPropertyValueConverter>();
}
```

## Default converters

Enterspeed ships with
[default property value converters](./defaults/README.md)  
for all the built-in property editors that Umbraco ships with out of the box.
