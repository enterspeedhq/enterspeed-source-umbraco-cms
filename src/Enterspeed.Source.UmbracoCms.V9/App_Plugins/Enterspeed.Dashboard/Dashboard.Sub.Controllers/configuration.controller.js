function configurationController(dashboardResources, notificationsService, $scope, $filter, $timeout) {
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
        dashboardResources.getEnterspeedConfiguration()
            .then(function(result) {
                if (result.data.isSuccess) {
                    vm.configuration.baseUrl = result.data.data.baseUrl;
                    vm.configuration.apiKey = result.data.data.apiKey;
                    vm.configuration.mediaDomain = result.data.data.mediaDomain;
                    vm.loadingConfiguration = false;
                    vm.buttonState = null;
                }
            });
    };

    vm.saveConfiguration = function () {
        vm.buttonState = "busy";
        if (!vm.configuration.apiKey || !vm.configuration.baseUrl) {
            notificationsService.error("Add api key and base url");
            vm.buttonState = null;
            return;
        }

        dashboardResources.saveEnterspeedConfiguration(vm.configuration)
            .then(function (result) {
                if (result.data.success) {
                    notificationsService.success("Configuration saved");
                    vm.setPristine();
                } else {
                    notifyErrors(result.data, "Error saving configuration");
                }
                vm.buttonState = null;
            })
            .catch(function (error) {
                console.error(error);
                notifyErrors(error.data, `Error saving configuration: ${error.data.message}`);
                vm.buttonState = null;
            });
    };

    vm.testConnection = function () {
        vm.buttonState = "busy";

        if (!vm.configuration.apiKey || !vm.configuration.baseUrl) {
            notificationsService.error("Missing api key or base url");
            vm.buttonState = null;
            return;
        }

        dashboardResources.testEnterspeedConfiguration(vm.configuration).then(function (result) {
            if (result.data.success) {
                notificationsService.success("Connection succeeded");
            } else {
                notifyErrors(result.data);
            }
            vm.buttonState = null;
        });
    };

    function notifyErrors(data, errorMessage) {
        var status = data.statusCode;

        errorMessage = errorMessage || "Something went wrong";

        if (status === 401) {
            notificationsService.error("Api key is invalid");
        } else if (status === 404) {
            notificationsService.error("Url does not exist");
        } else {
            notificationsService.error(errorMessage);
        }
    }

    vm.setPristine = function () {
        $scope.$$childTail.dashboardForm.$setPristine();
    };

    init();
}

angular.module("umbraco").controller("ConfigurationController", configurationController);