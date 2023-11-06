function enterspeedConfigurationController(enterspeedDashboardResources, notificationsService, $scope, angularHelper) {
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
                    vm.configuration.baseUrl = result.data.data.baseUrl;
                    vm.configuration.apiKey = result.data.data.apiKey;
                    vm.configuration.mediaDomain = result.data.data.mediaDomain;
                    vm.configuration.previewApiKey = result.data.data.previewApiKey;
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

        enterspeedDashboardResources.saveEnterspeedConfiguration(vm.configuration)
            .then(function (result) {
                if (result.data.success) {
                    notificationsService.success("Configuration saved");
                    vm.setPristine();
                } else {
                    notifyErrors(result.data, "Error saving configuration");
                }
                vm.buttonState = null;
            }, function(error) {
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

        enterspeedDashboardResources.testEnterspeedConfiguration(vm.configuration).then(function (result) {
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
            notificationsService.error(data.message);
        } else if (status === 404) {
            notificationsService.error("Url does not exist");
        } else {
            notificationsService.error(errorMessage);
        }
    }

    vm.setPristine = function () {
        $scope.enterpeedSettingsForm.$setPristine();
        var umbracoForm = document.getElementsByClassName("umb-dashboard")[0];
        var umbracoFormScope = angular.element(umbracoForm);
        umbracoFormScope.removeClass("ng-dirty");
        umbracoFormScope.addClass("ng-pristine");
    };

    init();
}

angular.module("umbraco").controller("EnterspeedConfigurationController", enterspeedConfigurationController);