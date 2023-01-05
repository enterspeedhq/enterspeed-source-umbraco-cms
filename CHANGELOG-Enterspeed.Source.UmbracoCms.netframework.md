# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
