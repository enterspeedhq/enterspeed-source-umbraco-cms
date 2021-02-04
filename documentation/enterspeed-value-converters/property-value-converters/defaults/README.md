# Default Property Value Converters

## DefaultBlockListPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.BlockList`

Returns `ArrayEnterspeedProperty`

### DefaultBlockListPropertyValueConverter Example

```json
{
    "Name": "blockList",
    "Type": "array",
    "Items": [{
        "Name": null,
        "Type": "object",
        "Properties": {
            "Content": {
                "Name": null,
                "Type": "object",
                "Properties": {
                    "stringProperty": {
                        "Name": "stringProperty",
                        "Type": "string",
                        "Value": "Hello world"
                    }
                }
            },
            "Settings": {
                "Name": null,
                "Type": "object",
                "Properties": {
                    "hideInSitemap": {
                        "Name": "hideInSitemap",
                        "Type": "boolean",
                        "Value": true
                    }
                }
            },
            "ContentType": {
                "Name": null,
                "Type": "string",
                "Value": "testBlock"
            }
        }
    }]
}
```

## DefaultCheckboxListPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.CheckBoxList`

Returns `ArrayEnterspeedProperty`

### DefaultCheckboxListPropertyValueConverter Example

```json
{
    "name": "checkboxList",
    "type": "array",
    "items": [{
        "name": null,
        "type": "string",
        "value": "Answer A"
    }, {
        "name": null,
        "type": "string",
        "value": "Answer B"
    }]
}
```

## DefaultCheckboxPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.TrueFalse`

Returns `BooleanEnterspeedProperty`

### DefaultCheckboxPropertyValueConverter Example

```json
{
    "name": "checkBox",
    "type": "boolean",
    "value": true
}
```

## DefaultColorPickerPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.ColorPicker`

Returns `ObjectEnterspeedProperty`

### DefaultColorPickerPropertyValueConverter Example

```json
{
    "name": "colorPicker",
    "type": "object",
    "properties": {
        "color": {
            "name": "Color",
            "type": "string",
            "value": "4c3d3d"
        },
        "label": {
            "name": "Label",
            "type": "string",
            "value": "4c3d3d"
        }
    }
}
```

## DefaultContentPickerPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.ContentPicker`

Returns `StringEnterspeedProperty`

### DefaultContentPickerPropertyValueConverter Example

```json
{
    "name": "contentPicker",
    "type": "string",
    "value": "1075-en-us"
}
```

Note that this uses the IEntityIdentityService to get
a culture specific content id

## DefaultDateTimePropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.DateTime`

Returns `StringEnterspeedProperty`

### DefaultDateTimePropertyValueConverter Example

```json
{
    "name": "datetimeDefault",
    "type": "string",
    "value": "11/13/2020 12:00:00"
}
```

## DefaultDecimalPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.Decimal`

Returns `NumberEnterspeedProperty`

### DefaultDecimalPropertyValueConverter Example

```json
{
    "name": "decimalDefault",
    "type": "number",
    "value": 2132.12222,
    "precision": 0
}
```

## DefaultDropdownPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.DropDown.Flexible`

Returns `ArrayEnterspeedProperty`

### DefaultDropdownPropertyValueConverter Example

```json
{
    "name": "dropdownDefault",
    "type": "array",
    "items": [{
        "name": null,
        "type": "string",
        "value": "Answer A"
    }, {
        "name": null,
        "type": "string",
        "value": "Answer C"
    }]
}
```

## DefaultEmailAddressPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.EmailAddress`

Returns `StringEnterspeedProperty`

### DefaultEmailAddressPropertyValueConverter Example

```json
{
    "name": "emailAddressDefault",
    "type": "string",
    "value": "test@test.com"
}
```

## DefaultFileUploadPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.UploadField`

Returns `StringEnterspeedProperty`

### DefaultFileUploadPropertyValueConverter Example

```json
{
    "name": "fileUploadDefault",
    "type": "string",
    "value": "/media/n0gcjqvs/media.jpg"
}
```

## DefaultGridLayoutPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.Grid`

Returns `ObjectEnterspeedProperty`

### DefaultGridLayoutPropertyValueConverter Example

```json
{
    "name": "gridDefault",
    "type": "object",
    "properties": {
        "name": {
            "name": "name",
            "type": "string",
            "value": "1 column layout"
        },
        "sections": {
            "name": "sections",
            "type": "array",
            "items": [{
                "name": null,
                "type": "object",
                "properties": {
                    "grid": {
                        "name": "grid",
                        "type": "string",
                        "value": "12"
                    },
                    "rows": {
                        "name": "rows",
                        "type": "array",
                        "items": [{
                            "name": null,
                            "type": "object",
                            "properties": {
                                "name": {
                                    "name": "name",
                                    "type": "string",
                                    "value": "Headline"
                                },
                                "id": {
                                    "name": "id",
                                    "type": "string",
                                    "value": "e4aa48dc-5aac-49e0-53d8-3e74cc773edd"
                                },
                                "areas": {
                                    "name": "areas",
                                    "type": "array",
                                    "items": [{
                                        "name": null,
                                        "type": "object",
                                        "properties": {
                                            "grid": {
                                                "name": "grid",
                                                "type": "string",
                                                "value": "12"
                                            },
                                            "controls": {
                                                "name": "controls",
                                                "type": "array",
                                                "items": [{
                                                    "name": null,
                                                    "type": "object",
                                                    "properties": {
                                                        "value": {
                                                            "name": null,
                                                            "type": "string",
                                                            "value": "<p>Rte content</p>"
                                                        },
                                                        "editor": {
                                                            "name": "editor",
                                                            "type": "object",
                                                            "properties": {
                                                                "name": {
                                                                    "name": "name",
                                                                    "type": "string",
            "value": "Richtexteditor"
                                                                },
                                                                "alias": {
                                                                    "name": "alias",
                                                                    "type": "string",
                                                                    "value": "rte"
                                                                },
                                                                "view": {
                                                                    "name": "view",
                                                                    "type": "string",
                                                                    "value": "rte"
                                                                },
                                                                "render": {
                                                                    "name": "render",
                                                                    "type": "string",
                                                                    "value": null
                                                                },
                                                                "icon": {
                                                                    "name": "icon",
                                                                    "type": "string",
                                                                    "value": "icon-article"
                                                                }
                                                            }
                                                        }
                                                    }
                                                }]
                                            }
                                        }
                                    }]
                                }
                            }
                        }]
                    }
                }
            }]
        }
    }
}
```

Note that this will try to find a registered
[GridEditorValueConverter](../../grideditorvalueconverters/README.md).
If no converter was found, it will create a IEnterspeedProperty
by looking at the grid editor propertytypes

## DefaultImageCropperPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.ImageCropper`

Returns `ObjectEnterspeedProperty`

### DefaultImageCropperPropertyValueConverter Example

```json
{
    "name": "imageCropperDefault",
    "type": "object",
    "properties": {
        "crops": {
            "name": "Crops",
            "type": "array",
            "items": [{
                "name": null,
                "type": "object",
                "properties": {
                    "alias": {
                        "name": null,
                        "type": "string",
                        "value": "Hero"
                    },
                    "height": {
                        "name": null,
                        "type": "number",
                        "value": 200.0,
                        "precision": 0
                    },
                    "width": {
                        "name": null,
                        "type": "number",
                        "value": 500.0,
                        "precision": 0
                    },
                    "coordinates": null
                }
            }, {
                "name": null,
                "type": "object",
                "properties": {
                    "alias": {
                        "name": null,
                        "type": "string",
                        "value": "Open Graph"
                    },
                    "height": {
                        "name": null,
                        "type": "number",
                        "value": 200.0,
                        "precision": 0
                    },
                    "width": {
                        "name": null,
                        "type": "number",
                        "value": 200.0,
                        "precision": 0
                    },
                    "coordinates": {
                        "name": null,
                        "type": "object",
                        "properties": {
                            "x1": {
                                "name": null,
                                "type": "number",
                                "value": 0.0090395480225988721,
                                "precision": 0
                            },
                            "y1": {
                                "name": null,
                                "type": "number",
                                "value": 0.0,
                                "precision": 0
                            },
                            "x2": {
                                "name": null,
                                "type": "number",
                                "value": 0.90056497175141248,
                                "precision": 0
                            },
                            "y2": {
                                "name": null,
                                "type": "number",
                                "value": 0.80629539951573848,
                                "precision": 0
                            }
                        }
                    }
                }
            }]
        },
        "src": {
            "name": null,
            "type": "string",
            "value": "/media/5kaamp4e/media.jpg"
        },
        "focalPoint": {
            "name": null,
            "type": "object",
            "properties": {
                "left": {
                    "name": null,
                    "type": "number",
                    "value": 0.685,
                    "precision": 0
                },
                "top": {
                    "name": null,
                    "type": "number",
                    "value": 0.28928571428571431,
                    "precision": 0
                }
            }
        }
    }
}
```

## DefaultMarkdownEditorPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.MarkdownEditor`

Returns `StringEnterspeedProperty`

### DefaultMarkdownEditorPropertyValueConverter Example

```json
{
    "name": "markdownEditorDefault",
    "type": "string",
    "value": "<p>Markdown editor is <strong>awesome!</strong></p>"
}
```

## DefaultMediaPickerPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.MediaPicker`

Returns `ArrayEnterspeedProperty`

### DefaultMediaPickerPropertyValueConverter Example

```json
{
    "name": "mediaPickerDefault",
    "type": "array",
    "items": [{
        "name": null,
        "type": "object",
        "properties": {
            "id": {
                "name": null,
                "type": "number",
                "value": 1100.0,
                "precision": 0
            },
            "url": {
                "name": null,
                "type": "string",
                "value": "http://my-website.localhost/media/quwnwb1v/media.jpg"
            }
        }
    }]
}
```

## DefaultMemberGroupPickerPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.MemberGroupPicker`

Returns `ArrayEnterspeedProperty`

### DefaultMemberGroupPickerPropertyValueConverter Example

```json
{
    "name": "memberGroupPickerDefault",
    "type": "array",
    "items": [{
        "name": null,
        "type": "object",
        "properties": {
            "id": {
                "name": null,
                "type": "number",
                "value": 1103.0,
                "precision": 0
            },
            "name": {
                "name": null,
                "type": "string",
                "value": "Editors"
            }
        }
    }]
}
```

## DefaultMemberPickerPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.MemberPicker`

Returns `ObjectEnterspeedProperty`

### DefaultMemberPickerPropertyValueConverter Example

```json
{
    "name": "memberPickerDefault",
    "type": "object",
    "properties": {
        "id": {
            "name": null,
            "type": "number",
            "value": 1106.0,
            "precision": 0
        },
        "name": {
            "name": null,
            "type": "string",
            "value": "Enterspeed Member"
        },
        "memberType": {
            "name": null,
            "type": "string",
            "value": "Member"
        }
    }
}
```

## DefaultMultiNodeTreePickerPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.MultiNodeTreePicker`

Returns `ArrayEnterspeedProperty`

### DefaultMultiNodeTreePickerPropertyValueConverter Example

```json
{
    "name": "multinodeTreepickerDefault",
    "type": "array",
    "items": [{
        "name": null,
        "type": "object",
        "properties": {
            "id": {
                "name": null,
                "type": "string",
                "value": "1073-en-us"
            },
            "name": {
                "name": null,
                "type": "string",
                "value": "FrontPage"
            },
            "url": {
                "name": null,
                "type": "string",
                "value": "http://localhost/"
            }
        }
    }, {
        "name": null,
        "type": "object",
        "properties": {
            "id": {
                "name": null,
                "type": "string",
                "value": "1075-en-us"
            },
            "name": {
                "name": null,
                "type": "string",
                "value": "TextPage"
            },
            "url": {
                "name": null,
                "type": "string",
                "value": "http://localhost/textpage"
            }
        }
    }]
}
```

## DefaultMultiUrlPickerPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.MultiUrlPicker`

Returns `ArrayEnterspeedProperty`

### DefaultMultiUrlPickerPropertyValueConverter Example

```json
{
    "name": "multiUrlPickerDefault",
    "type": "array",
    "items": [{
        "name": null,
        "type": "object",
        "properties": {
            "name": {
                "name": null,
                "type": "string",
                "value": "Google.com"
            },
            "target": {
                "name": null,
                "type": "string",
                "value": null
            },
            "type": {
                "name": null,
                "type": "string",
                "value": "External"
            },
            "udi": null,
            "url": {
                "name": null,
                "type": "string",
                "value": "https://google.com"
            }
        }
    }, {
        "name": null,
        "type": "object",
        "properties": {
            "name": {
                "name": null,
                "type": "string",
                "value": "Text page"
            },
            "target": {
                "name": null,
                "type": "string",
                "value": "_blank"
            },
            "type": {
                "name": null,
                "type": "string",
                "value": "Content"
            },
            "udi": {
                "name": null,
                "type": "string",
                "value": "umb://document/e395592754654b8fb55b1cc25dbafecd"
            },
            "url": {
                "name": null,
                "type": "string",
                "value": "/"
            }
        }
    }, {
        "name": null,
        "type": "object",
        "properties": {
            "name": {
                "name": null,
                "type": "string",
                "value": "Media item"
            },
            "target": {
                "name": null,
                "type": "string",
                "value": null
            },
            "type": {
                "name": null,
                "type": "string",
                "value": "Media"
            },
            "udi": {
                "name": null,
                "type": "string",
                "value": "umb://media/07cb2d83e9754203b9c782e356046f22"
            },
            "url": {
                "name": null,
                "type": "string",
                "value": "/media/quwnwb1v/media-item.jpg"
            }
        }
    }]
}
```

## DefaultNestedContentPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.NestedContent`

Returns `ArrayEnterspeedProperty`

### DefaultNestedContentPropertyValueConverter Example

```json
{
    "name": "nestedContentDefault",
    "type": "array",
    "items": [{
        "name": null,
        "type": "object",
        "properties": {
            "stringProperty": {
                "name": "stringProperty",
                "type": "string",
                "value": "Item 1 string property"
            },
            "contentType": {
                "name": null,
                "type": "string",
                "value": "testBlock"
            }
        }
    }]
}
```

Note that this internally calls the
[EnterspeedPropertyService](../../../services/propertyservice/README.md)
to convert the properties from the nested content item.

## DefaultNumericPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.Integer`

Returns `NumberEnterspeedProperty`

### DefaultNumericPropertyValueConverter Example

```json
{
    "name": "numericDefault",
    "type": "number",
    "value": 20.0,
    "precision": 0
}
```

## DefaultRadioButtonListPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.RadioButtonList`

Returns `StringEnterspeedProperty`

### DefaultRadioButtonListPropertyValueConverter Example

```json
{
    "name": "radioButtonListDefault",
    "type": "string",
    "value": "Answer B"
}
```

## DefaultRepeatableTextStringPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.MultipleTextstring`

Returns `ArrayEnterspeedProperty`

### DefaultRepeatableTextStringPropertyValueConverter Example

```json
{
    "name": "repeatableTextstringsDefault",
    "type": "array",
    "items": [{
        "name": null,
        "type": "string",
        "value": "a"
    }, {
        "name": null,
        "type": "string",
        "value": "c"
    }]
}
```

## DefaultRichTextEditorPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.TinyMCE`

Returns `StringEnterspeedProperty`

### DefaultRichTextEditorPropertyValueConverter Example

```json
{
    "name": "richTextEditorDefault",
    "type": "string",
    "value": "<p>Here is some <strong>rte</strong> text</p>\n<p>
    And a <a href=\"https://google.com/\" title=\"Google.com\">link</a></p>
    \n<p><img src=\"/media/quwnwb1v/media.jpg?width=500&amp;height=233.33333333333331\"
    alt=\"\" width=\"500\" height=\"233.33333333333331\"></p>"
}
```

## DefaultSliderPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.Slider`

Returns `ObjectEnterspeedProperty` if range is enabled. Otherwise returns `NumberEnterspeedProperty`

### DefaultSliderPropertyValueConverter Examples

#### Range enabled

```json
{
    "name": "sliderDefault",
    "type": "object",
    "properties": {
        "minimum": {
            "name": null,
            "type": "number",
            "value": 13.0,
            "precision": 0
        },
        "maximum": {
            "name": null,
            "type": "number",
            "value": 37.0,
            "precision": 0
        }
    }
}
```

#### Range disabled

```json
{
    "name": "sliderDefault",
    "type": "number",
    "value": 24.0,
    "precision": 0
}
```

## DefaultTagsPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.Tags`

Returns `ArrayEnterspeedProperty`

### DefaultTagsPropertyValueConverter Example

```json
{
    "name": "tagsDefault",
    "type": "array",
    "items": [{
        "name": null,
        "type": "string",
        "value": "TagA"
    }, {
        "name": null,
        "type": "string",
        "value": "TagB"
    }]
}
```

## DefaultTextAreaPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.TextArea`

Returns `StringEnterspeedProperty`

### DefaultTextAreaPropertyValueConverter Example

```json
{
    "name": "textAreaDefault",
    "type": "string",
    "value": "Hello world!"
}
```

## DefaultTextboxPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.TextBox`

Returns `StringEnterspeedProperty`

### DefaultTextboxPropertyValueConverter Example

```json
{
    "name": "textBoxDefault",
    "type": "string",
    "value": "Hello world!"
}
```

## DefaultUserPickerPropertyValueConverter

Umbraco propertyeditor alias: `Umbraco.UserPicker`

Returns `ObjectEnterspeedProperty`

### DefaultUserPickerPropertyValueConverter Example

```json
{
    "name": "userPickerDefault",
    "type": "object",
    "properties": {
        "id": {
            "name": null,
            "type": "number",
            "value": -1.0,
            "precision": 0
        },
        "name": {
            "name": null,
            "type": "string",
            "value": "Default User"
        }
    }
}
```
