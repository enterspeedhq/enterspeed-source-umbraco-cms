function enterspeedConfigurationController(enterspeedDashboardResources, notificationsService, $scope, $filter, $timeout) {
    var vm = this;
    vm.loadingConfiguration = false;
    vm.buttonState = null;
    vm.configuration = {};

    function init() {
        vm.getConfiguration();
    }

    vm.getConfiguration = function () {
        vm.loadingConfiguration = true;
        vm.buttonState = "busy";
        enterspeedDashboardResources.getEnterspeedConfiguration()
            .then(function (result) {
                if (result.data.isSuccess) {
                    vm.configuration.baseUrl = result.data.data.configuration.baseUrl;
                    vm.configuration.apiKey = result.data.data.configuration.apiKey;
                    vm.configuration.mediaDomain = result.data.data.configuration.mediaDomain;
                    vm.configuration.previewApiKey = result.data.data.configuration.previewApiKey;
                    vm.configuration.configuredFromSettingsFile = result.data.data.configuration.configuredFromSettingsFile;
                    vm.runJobsOnServer = result.data.data.runJobsOnServer;
                    vm.serverRole = result.data.data.serverRole;
                } else {
                    notificationsService.error("Error loading configuration", messageFrom(result));
                }
            })
            .catch(function (error) {
                notificationsService.error("Error loading configuration", messageFrom(error));
            })
            .finally(function () {
                vm.loadingConfiguration = false;
                vm.buttonState = null;
            });
    };

    vm.saveConfiguration = function () {
        if (!vm.configuration.apiKey || !vm.configuration.baseUrl) {
            notificationsService.error("Cannot save configuration", "Enter an api key and a base url first.");
            return;
        }

        vm.buttonState = "busy";
        enterspeedDashboardResources.saveEnterspeedConfiguration(vm.configuration)
            .then(function (result) {
                if (result.data.success) {
                    // An invalid preview api key does not block the save, so it comes back as a success carrying errors.
                    const errors = result.data.errors;
                    if (errors && Object.keys(errors).length > 0) {
                        notificationsService.warning("Configuration saved", keyStatuses(errors));
                    } else {
                        notificationsService.success("Configuration saved", result.data.message || "The configuration has been saved.");
                    }

                    vm.setPristine();
                } else {
                    notifyFailure(result, "Error saving configuration");
                }
            })
            .catch(function (error) {
                notificationsService.error("Error saving configuration", messageFrom(error));
            })
            .finally(function () {
                vm.buttonState = null;
            });
    };

    vm.testConnection = function () {
        if (!vm.configuration.apiKey || !vm.configuration.baseUrl) {
            notificationsService.error("Cannot test connection", "Enter an api key and a base url first.");
            return;
        }

        vm.buttonState = "busy";
        enterspeedDashboardResources.testEnterspeedConfiguration(vm.configuration)
            .then(function (result) {
                if (result.data.success) {
                    notificationsService.success("Connection successful", result.data.message || "The connection to Enterspeed was successful.");
                } else {
                    // Publishing still works when only the preview key fails, so that is not a failed connection.
                    notifyFailure(result, "Connection failed", "Preview connection failed");
                }
            })
            .catch(function (error) {
                notificationsService.error("Connection failed", messageFrom(error));
            })
            .finally(function () {
                vm.buttonState = null;
            });
    };

    // Reports whatever the server returned rather than branching on the status code. A publish failure is an error,
    // whereas a preview only failure still leaves publishing working and is reported as a warning.
    function notifyFailure(result, headline, warningHeadline) {
        const errors = result?.data?.errors;
        const message = keyStatuses(errors) || messageFrom(result);

        if (!errors || errors.publishApiKey) {
            notificationsService.error(headline, message);
        } else {
            notificationsService.warning(warningHeadline || headline, message);
        }
    }

    // Adds the keys that passed, so a mixed result also confirms which key does work. Returns null when there is
    // nothing to add - either the request never reached validation, or every key failed, in which case the message the
    // server composed is already the better wording.
    function keyStatuses(errors) {
        if (!errors || Object.keys(errors).length === 0) {
            return null;
        }

        const statuses = [];
        if (!errors.publishApiKey) {
            statuses.push("Publish API key is valid");
        }
        if (vm.configuration.previewApiKey && !errors.previewApiKey) {
            statuses.push("Preview API key is valid");
        }

        if (statuses.length === 0) {
            return null;
        }

        if (errors.publishApiKey) {
            statuses.unshift(errors.publishApiKey);
        }
        if (errors.previewApiKey) {
            statuses.push(errors.previewApiKey);
        }

        return statuses.join(". ") + ". Verify your API keys under Settings > Data sources in the Enterspeed app.";
    }

    function messageFrom(source) {
        return source?.data?.message
            || source?.message
            || "An unexpected error occurred";
    }

    vm.setPristine = function () {
        $scope.$$childTail.dashboardForm.$setPristine();
    };

    init();
}

angular.module("umbraco").controller("EnterspeedConfigurationController", enterspeedConfigurationController);
