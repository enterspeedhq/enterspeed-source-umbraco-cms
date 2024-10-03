# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [5.0.0 - 2024-10-03]
### Added
- Added Umbraco 14 support.

### Breaking
- Umbraco.Source.Cms namespace has been updated to Umbraco.Source.Cms.Base.
- EnterspeedComposer must now be referenced on `Enterspeed.Source.UmbracoCms.V14Plus.EnterspeedComposer` and `Enterspeed.Source.UmbracoCms.V9Plus.EnterspeedComposer`.

## [4.2.1 - 2024-01-31]
### Fixed
- Repeatable textstrings (Umbraco.MultipleTextstring) courses error when the list is empty

## [4.2.0 - 2024-01-29]
### Added
- Added the option to have the package ingesting a master variant source entity. 
  Nodes are ingested to Enterspeed as one source entity per language, but with this feature you can also have a shared master variant ingested.
  This can be useful if you want to have a schemas triggered when one of the language variants changes or when all varaints are unpublished.  
  Note: the feature is disabled by default and must be enabled in the `appsettings.json` file under the `Enterspeed` setting by setting `EnableMasterContent` to `true`.

### Fixed
- The URL for medias selected in `DefaultMultiNodeTreePickerPropertyValueConverter` does not add custom media domain

## [4.1.0 - 2023-12-14]
### Added
- Added Umbraco 13 support
- Added nodeId to `Content`, `Media` and `Dictionary` entities in the metaData object

## [4.0.1 - 2023-12-05]
### Fixed
- Fixed renaming of `enterspeedDashboardResources` that coursed the Seed and Failed jobs views to fail.

## [4.0.0 - 2023-11-15]
### Breaking
- Upgraded `Enterspeed.Source.Sdk` to 2.0.0 meaning that `properties` on `IEnterspeedEntity` is changes to 
  `object` instead of `<IDictionary<string, IEnterspeedProperty>>`. A generic version of IEnterspeedEntity is added.  
  This is a breaking change if you have created your own types implementing `IEnterspeedEntity`. If that's the case 
  you just need to make it generic of `<IDictionary<string, IEnterspeedProperty>>`.

## [3.2.0 - 2023-11-06]
### Breaking 
- Added `EnterspeedPropertyMetaDataMappers` and `EnterspeedPropertyDataMappers`. These are additions to 
  `MapAdditionalMetaData` and `MapAdditionalProperties` on the `EnterspeedPropertyService` class, but useful if you
  have more complex mapping or different mapping based on different types. (contribution by [Adrian Ochmann](https://github.com/aochmann))  
  This is a breaking changes if you have overridden `EnterspeedPropertyService` as it has two new dependencies that needs to be added.  
  As a breaking changes this change should have been part of the new major.

### Fixed 
- Ingest jobs from `Save` or `Save and Publish` will only be executed on servers configured as `ServerRole.SchedulingPublisher` or `ServerRole.Single` just like Ingest jobs from seed.
- Prefixed JavaScript controllers to avoid conflicts with other packages

## [3.1.0 - 2023-09-21]
### Added 
- Added option to manually register Enterspeed using `.addEnterspeed()` on the Umbraco builder if you are not using Umbracos `.AddComposers()` method.
- Rename of node updates urls on all descendants in Enterspeed sources.

## [3.0.1 - 2023-08-11]
### Added 
- Tooltip added to API key label

### Fixed
- Fixed an issue with relative image paths in the rich text parser.
- Deadlock issue on the jobs table has been fixed

## [3.0.0 - 2023-06-27]
### Breaking
- Virtual source-entities for root dictionary items. This new feaures adds a originParentId to dictionary items in the root, making at easier to group all dictionary items of a specific culture in your Enterspeed schema. If this is a breaking change for you, you can simple disable the new feature by setting `DisableRootDictionaries` to true in the appsettings.json file under the `Enterspeed` section.

### Fixed
- VariationContext is set to the correct culture in the jobs pipeline
- Fixed issue with how pending jobs are fetched. 


## [2.5.0 - 2023-06-12]
### Addded
- Added buttons to clear a single or all failed jobs in the failed jobs list
- Only store last version of a failed job in the failed jobs list
- Logging actual exception instead of just a generic error message on ingest error

## [2.4.0 - 2023-05-08]
### Addded
- Option for users to manually select nodes and do a custom seed. This way users don't have to do full seeds, but can select only a sub-node and it's descendent or eg. only seed all media nodes.
- Option for users to clear the seed job queue. If you misclicked the seed button or just changed your mind, you can now clear the job queue with a click of a button on the seed page

### Updated
- Update dpendency to Enterspeed.Source.Sdk v1.0.2

### Fixed
- Resolve correct url by culture in `DefaultMultiUrlPickerPropertyValueConverter`
- Removed double slash after domain for redirects if domain ends with slash

## [2.3.1 - 2023-04-19]
### Addded
- Added loggin of Umbraco version

### Updated
- Update dpendency to Enterspeed.Source.Sdk v1.0.1

## [2.3.0 - 2023-03-29]
### Addded
- Added option to customize culture logic (contribution by [Mikkel Keller](https://github.com/K3llr))
- Added option to create custom notification handlers

### Fixed
- Fixed missing sorting event for ingest (contribution by [Mikkel Keller](https://github.com/K3llr))

### Changed
- Made methods on `UmbracoRedirectsService` virtual to allow overring logic

## [2.2.0 - 2023-03-02]
### Addded
- Ingesting preview media when doing a seed

### Changed
- Made `GetUrl` on `UrlFactory` virtual to allow overriding

### Fixed
- Unable to create new configuration from Umbraco backoffice because of null reference exception
- Don't ingest trashed media items on seed

## [2.1.0 - 2023-02-20]
### Changed
- Made `CanHandle` and `Handle` on all types of `IEnterspeedJobHandler` virtual to allow overriding these methods if you want to customize the logic for specific handlers.

## [2.0.0 - 2023-02-15]
### Breaking changes
- Fixed configuration stored in database even if you are using settings file. If you are using the settings file, the settings file  will now take priority over potential configuration in the database.
  Also the UI will show a message indicating if the settings are loaded from the settings file and you cant save configuration changes from the UI.

### Added
- Added information note on the Enterspeed settings page, if the Umbraco server is running with `ServerRole.Subscriber` as the Enterspeed jobs is only configured to run on servers configured as `ServerRole.SchedulingPublisher` and `ServerRole.Single`. Also upgraded the logging about this from debug to information.
- Added focal point data for images
- Ingesting medias to Enterspeed for preview

## [1.1.0 - 2023-01-25]
### Added
- Added support for the new Block grid editor in Umbraco 10.4 and 11
- Added property validation to make sure null is not passed as a value

## [1.0.0 - 2023-01-19]
- Merged Umbraco 9+ coded bases to a unified codebase
