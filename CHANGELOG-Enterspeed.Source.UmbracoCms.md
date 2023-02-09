# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
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
