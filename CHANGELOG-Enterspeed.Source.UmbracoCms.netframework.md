# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [4.2.0 - 2024-01-29]
### Fixed
- The URL for medias selected in `DefaultMultiNodeTreePickerPropertyValueConverter` does not add custom media domain (Umbraco 8)

## [4.1.0 - 2023-12-14]
### Added
- Added nodeId to `Content`, `Media` and `Dictionary` entities in the metaData object. (Umbraco 7 & 8)

## [4.0.1 - 2023-12-05]
### Fixed
- Fixed renaming of `enterspeedDashboardResources` that coursed the Seed and Failed jobs views to fail. (Umbraco 8)

## [4.0.0 - 2023-11-15]
### Breaking
- Upgraded `Enterspeed.Source.Sdk` to 2.0.0 meaning that `properties` on `IEnterspeedEntity` is changes to 
  `object` instead of `<IDictionary<string, IEnterspeedProperty>>`. A generic version of IEnterspeedEntity is added. (Umbraco 8)  
  This is a breaking change if you have created your own types implementing `IEnterspeedEntity`. If that's the case 
  you just need to make it generic of `<IDictionary<string, IEnterspeedProperty>>`.

## [3.11.0 - 2023-11-06]
### Breaking 
- Added `EnterspeedPropertyMetaDataMappers` and `EnterspeedPropertyDataMappers`. These are additions to 
  `MapAdditionalMetaData` and `MapAdditionalProperties` on the `EnterspeedPropertyService` class, but useful if you
  have more complex mapping or different mapping based on different types. (contribution by [Adrian Ochmann](https://github.com/aochmann)) (Umbraco 8)  
  This is a breaking changes if you have overridden `EnterspeedPropertyService` as it has two new dependencies that needs to be added.  
  As a breaking changes this change should have been part of the new major.

### Fixed 
- Ingest jobs from `Save` or `Save and Publish` will only be executed on servers configured as `ServerRole.Master` or `ServerRole.Single` just like Ingest jobs from seed. (Umbraco 8)
- Prefixed JavaScript controllers to avoid conflicts with other packages (Umbraco 7 & 8)

## [3.10.0 - 2023-09-21]
### Added 
- Rename of node updates urls on all descendants in Enterspeed sources. (Umbraco 8)

## [3.9.0 - 2023-08-11]
### Added 
- Tooltip added to API key label (Umbraco 7 & 8)

### Fixed
- Fixed an issue with missing seed information (Umbraco 7)
- Fixed an issue with relative image paths in the rich text parser. (Umbraco 8)
- Ensuring that scope is completed correctly in repositories (Umbraco 8)

### Added 
- Added support for Umbraco.MultiNodeTreePicker2 (Umbraco 7)

### Fixed 
- Added areas to StartupDashboardSection in config/Dashboard.config during package installation if missing (Umbraco 7)

## [3.8.0 - 2023-06-27]
### Added 
- Added buttons to clear a single or all failed jobs in the failed jobs list (Umbraco 7 & 8)

### Updated
- Update dependency to Enterspeed.Source.Sdk v1.0.2 (Umbraco 7 & 8)

### Fixed
- VariationContext is set to the correct culture in the jobs pipeline (Umbraco 8)
- Removed double slash after domain for redirects if domain ends with slash (Umbraco 8)
- Fixed mapping of Umbraco.Tag data type (Umbraco 7)

## [3.7.1 - 2023-04-19]
### Addded
- Added loggin of Umbraco version

### Updated
- Update dpendency to Enterspeed.Source.Sdk v1.0.1

## [3.7.0 - 2023-03-29]
### Addded
- Added option to customize culture logic (contribution by [Mikkel Keller](https://github.com/K3llr))
- Added sort event to ingest logic (contribution by [Mikkel Keller](https://github.com/K3llr))

### Changed
- Made methods on `UmbracoRedirectsService` virtual to allow overring logic

## [3.6.0 - 2023-03-02]
### Addded
- Ingesting preview media when doing a seed (Umbraco 7 & 8)

### Changed
- Made `GetUrl` on `UrlFactory` virtual to allow overriding. (Umbraco 7 & 8)

### Fixed
- Don't ingest trashed media items on seed (Umbraco 7 & 8)

## [3.5.0 - 2023-02-20]
### Changed
- Made `CanHandle` and `Handle` on all types of `IEnterspeedJobHandler` virtual to allow overriding these methods if you want to customize the logic for specific handlers. (Umbraco 8)

## [3.4.0 - 2023-02-15]
### Added
- Added information note on the Enterspeed settings page, if the Umbraco server is running with `ServerRole.Replica` as the Enterspeed jobs is only configured to run on servers configured as `ServerRole.Master` and `ServerRole.Single`. Also upgraded the logging about this from debug to information.
- Added focal point data for images
- Added support for Umbraco.ContentPicker2 (Umbraco 7)
- Ingesting medias to Enterspeed for preview (Umbraco 7 & 8)

## [3.3.0 - 2023-01-25]
### Added
  - Added property validation to make sure null is not passed as a value

## [3.2.0 - 2022-12-13]
### Added
  - Added virtual method to extend property data (Umbraco 8, 9, 10)
  - Added media domain to other media types, not just images (Umbraco 7, 8, 9 & 10)

## [3.1.0 - 2022-11-22]
### Added
  - Added support for simple types (string, number and boolean) in grid editor array (Umbraco 7, 8, 9 & 10) (contribution by [Adrian Ochmann](https://github.com/aochmann))
  - Extended logging information when ingest throws an error (Umbraco 7, 8 & 9)

### Fixed
  - Handle Single element, list or null value for nested content (Umbraco 7, 8, 9 & 10)
  - DefaultMultiNodeTreePickerPropertyValueConverter throws exception for members as they have no url (Umbraco 8, 9 & 10)


## [3.0.1 - 2022-11-07]
### Added
  - Extended logging information when ingest throws an error (Umbraco 10)

## [3.0.0 - 2022-10-31]
### Breaking changes
  - Added option to differentiate between extending meta-data on media and/or content. This was handled in one method before which extended both types of source-entites. 

### Fixed
  - Fixed Umbraco install error when Enterspeed plugin is installed (Umbraco 9 & 10)

## [2.0.1 - 2022-10-26]
### Added
  - Logging errors instead of responsemessage when ingest throws an error (Umbraco 7, 8, 9 & 10)

## [2.0.0 - 2022-10-21]
### Added
- Added support for Nested Content (Umbraco 7)
- Added support for Umbraco.MediaPicker2 (Umbraco 7)

### Fixed
- Issue with loading PreviewApikey from appsettings (contribution by [Adrian Ochmann](https://github.com/aochmann))
- Color Picker Value reference null exception (contribution by [Adrian Ochmann](https://github.com/aochmann))

### Breaking changes
- Added missing default grid editor value converters for the out of the box grid editors in Umbraco (embed, headline, image and quote) (Umbraco 8, 9 & 10)
  

## [1.0.0 - 2022-10-12]
- Changed output of `DefaultDateTimePropertyValueConverter` from `date.ToString(CultureInfo.InvariantCulture))` to `date.ToString("yyyy-MM-ddTHH:mm:ss"))` (Umbraco 10)
  
  This is done to make the default version sortable and to match the format of other date fields like `createDate` and `updateDate`
  
  If you are using `DefaultDateTimePropertyValueConverter` and this change will break you project, simply override it with your own implementation.

## [0.16.0 - 2022-10-10]
### Fixed
- Fixed missing parsing of internal links and media references from rich text editor in Grid layout (Umbraco 8, 9 & 10)
- Media folders are now also pushed to Enterspeed as data sources (Umbraco 7, 8, 9 & 10)

## [0.15.4 - 2022-09-30]
- Changed structure on media metadata properties

## [0.15.3 - 2022-09-23]
- Added virtual method to extend metadata properties (Umbraco 8, 9 & 10)

## [0.15.2 - 2022-09-21]
- Added missing properties on media items (Umbraco 7 & 8)

## [0.15.1 - 2022-09-12]
- Convert properties made virtual for extendability on property value converters (Umbraco 10)

## [0.15.0 - 2022-09-07]
- Support for media types ingestion in Umbraco 9 & 10

## [0.14.2 - 2022-09-01]
- Fix issue with nested content

## [0.14.1 - 2022-08-30]
- Fix issue with installation of Umbraco V10 package

## [0.14.0-  2022-08-29]
- Umbraco V10 Project added

## [0.13.1 - 2022-07-28]
- Fix node path issue for media types ingestion in Umbraco 7 & 8.

## [0.13.0 - 2022-07-28]
- Add support for media types ingestion in Umbraco 7 & 8.
- Fallback to default Umbraco culture, when none is specified in Umbraco 8.

## [0.12.5 - 2022-05-23]

- Fix casting exception for Nested Content in Umbraco 8.

## [0.12.4 - 2022-04-07]

- Fix bug of Multi Url Picker always resolving url for english culture in Umbraco 8.

## [0.12.3 - 2022-03-31]

- Fix bug of Enterspeed connections configuration being stale for Umbraco 8 & Umbraco 9.

## [0.12.1/0.12.2 - 2022-03-30]

- Content picker to support selection of unpublished content for Umbraco 9.

## [0.12.0 - 2022-03-28]

- Job and event handling extendability improvements for Umbraco 8.
- Preview functionality for Umbraco 7, 8, and 9.

## [0.11.0 - 2022-03-23]

- Job and notification handling extendability improvements for Umbraco 9.

## [0.10.0 - 2022-03-07]

- Media picker 3 support for Umbraco 9.

## [0.9.2 - 2021-11-26]

- Fix media url provider bug, where media domain url is not correctly combined with media path.

## [0.9.1 - 2021-11-25]

- Fix retrieval of culture specific content url for Umbraco 9.1
- Split configuration keys for Umbraco 8 & Umbraco 9

## [0.9.0 - 2021-11-08]

- Umbraco 8 & Umbraco 9 to support ingesting guards.

## [0.8.1 - 2021-10-14]

- Fix Umbraco 9 plugin resources access paths.

## [0.8.0 - 2021-10-13]

- Add integration to Umbraco 9, min. version 9.0.0

## [0.7.1 - 2021-08-11]

- Added a published request when processing property values.

## [0.7.0 - 2021-07-19]

- Add support for ingesting dictionary items into Enterspeed.
  All dictionary items will now automatically be ingested.

## [0.6.2 - 2021-06-29]

-Fix null Enterspeed properties resulting in bad ingestions.

## [0.6.1 - 2021-06-03]

-Fix testing Ingest API endpoint whren it returns 422

## [0.6.0 - 2021-05-26]

- Add integration to Umbraco 7, min. version 7.4.0
- Fix bug where Umbraco 8 throws an exception if Enterspeed is not configured

## [0.5.0 - 2021-05-20]

- Update Enterspeed.Source.Sdk dependency version to lowest fuunctional version
- Rename Udi to Id in DefaultMultiUrlPickerPropertyValueConverter

## [0.4.1 - 2021-03-18]

- Fix naming of property editors variables

## [0.4.0 - 2021-03-17]

- Update Enterspeed.Source.Sdk to v0.4.0
- Add SystemTextJsonSerializer to DI Container

## [0.3.0 - 2021-02-25]

- Fix redirects returning null when no culture and hostname were set

## [0.2.0 - 2021-02-10]

- Update Enterspeed.Source.Sdk to 0.3.0
- Add IEnterspeedConfigurationService + implementation
- Add EnterspeedUmbracoConfigurationProvider + InMemoryEnterspeedUmbracoConfigurationProvider
- Add EnterspeedUmbracoConfiguration with MediaDomain
