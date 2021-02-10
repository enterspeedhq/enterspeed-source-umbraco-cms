# Known limitations

This is a list of known limitations in the Enterspeed Umbraco integration.

## Multiple domains per culture

For the moment Enterspeed can only handle one domain per Culture  
in _Culture and hostnames_.

### Non-working Culture and Hostnames setup

This setup would **not** work as expected:

|Domain                   | Language |
|:---                     | :---     |
|https://enterspeed.com   | en-US    |
|https://enterspeed.dk    | en-US    |

It is expected that both URLs would serve the same content from Umbraco.  
In reality only one of them would work in Enterspeed.

This is because Umbraco defaults to one of the domains when no _current uri_ is available,  
and as Enterspeed must have a single URL. As the integration just uses  
Umbracos built in `IPublishedContent.Url()` method.  
This method chooses the best suitable URL by culture.  

### Working Culture and Hostnames setup

This setup would work as expected:

|Domain                   | Language |
|:---                     | :---     |
|https://enterspeed.com   | en-US    |
|https://enterspeed.dk    | da-DK    |

This setup would give english content the `https://enterspeed.com` domain  
and the danish content the `https://enterspeed.dk`.

If you do require to have the same content and same language on multiple domains,  
you have to implement some logic on your frontend that handles that.

## Changing _Culture and hostnames_

When changing a hostname in the _Culture and hostnames_ list,  
preferrable the content impacted would be reprocessed in Enterspeed,  
but as for now, this is not possible.  

So if you are to change _Culture and hostnames_ you should manually  
go to Content -> Enterspeed Content -> Seed and press on _Seed_.  
This will queue all content for reprocessing in Enterspeed.
