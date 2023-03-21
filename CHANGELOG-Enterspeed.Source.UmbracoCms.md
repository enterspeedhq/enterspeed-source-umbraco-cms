# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Addded
- Added option to customize culture logic

## [2.2.0 - 2023.03.02]
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
