# Changelog

All notable changes to this project will be documented in this file.

The format is based on Keep a Changelog, and this project adheres to Semantic Versioning.

## [0.7.2 - 2021-08-11]

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
