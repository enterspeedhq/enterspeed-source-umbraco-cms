# UmbracoContentEntity : [IEnterspeedEntity](https://github.com/enterspeedhq/enterspeed-sdk-dotnet/blob/master/documentation/entities)

## Properties

|Name               | Type                                      |Description |
|:----              | :-----                                    |:-----|
|Id                 | `string`                                  | Unique identifier ie. "1078-en-us"
|Type               | `string`                                  | ContentType alias
|Url                | `string`                                  | The absolute url of the node
|Redirects          | `string[]`                                | Array of redirects for the node
|ParentId           | `string`                                  | Unique identifier of parent ie. "1077-en-us"
|Properties         | `Dictionary<string, IEnterspeedProperty>` | Dictionary of properties, where key is alias of property and value is the converted Enterspeed property

Read more about properties [here](https://github.com/enterspeedhq/enterspeed-sdk-dotnet/blob/master/documentation/entities/properties)

### Meta data properties

Umbraco defines a set of meta data that without futher do
will be dispatched to Enterspeed.

|Key                | Type                       |Description |
|:----              | :-----                     |:-----|
|nodeName           | `StringEnterspeedProperty` | The name of the node |
|sortOrder          | `NumberEnterspeedProperty` | The order of the node in the tree |
|culture            | `StringEnterspeedProperty` | The culture of the entity |
|nodePath           | `ArrayEnterspeedProperty`  | The path in the tree, eg. [1061, 1062, 1063] (site (level 1), blog (level 2), blogPost(level 3)) |
|createDate         | `StringEnterspeedProperty` | The date time the node was created |
|updateDate         | `StringEnterspeedProperty` | The date time the node was updated |
|level              | `StringEnterspeedProperty` | The level in the tree (1, 2, 3, etc.) |

The meta data properties, are available from the _propeties_ object
like the example below.

```js
{
  "properties": {
    "metaData": {
      "culture": {
        "name": "culture",
        "type": "string",
        "value": "en-US"
      },
      "nodeName": {
        "name": "nodeName",
        "type": "string",
        "value": "This is the name of a node"
      },
      "createDate": {
        "name": "createDate",
        "type": "string",
        "value": "09-12-2020T10:49:01:00"
      },
      "updateDate": {
        "name": "updateDate",
        "type": "string",
        "value": "10-12-2020T10:49:01:00"
      },
      "nodePath": {
        "name": "nodePath",
        "type": "array",
        "items": [
          {
            "name": null,
            "type": "number",
            "value": 1061,
            "precision": 0
          },
          {
            "name": null,
            "type": "number",
            "value": 1062,
            "precision": 0
          },
          {
            "name": null,
            "type": "number",
            "value": 1063,
            "precision": 0
          },
        ]
      },
      "sortOrder": {
          "name": "sortOrder",
          "type": "number",
          "value": 1,
          "precision": 0
      },
      "level": {
          "name": "level",
          "type": "number",
          "value": 1,
          "precision": 0
      },
    },
  }
}
```
