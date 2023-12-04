function enterspeedSeedController(enterspeedDashboardResources, notificationsService) {
    var vm = this;
    vm.loadingConfiguration = false;
    vm.seedState = "success";
    vm.seedResponse = null;
    vm.configuration = {};

    function init() {
        getConfiguration();
    }

    function getConfiguration() {
        vm.loadingConfiguration = true;
        enterspeedDashboardResources.getEnterspeedConfiguration()
            .then(function (result) {
                if (result.data.isSuccess) {
                    vm.runJobsOnServer = result.data.data.runJobsOnServer;
                    vm.serverRole = result.data.data.serverRole;
                    vm.loadingConfiguration = false;
                }
            });
    };

    vm.seed = function () {
        vm.seedState = "busy";
        enterspeedDashboardResources.seed().then(function (result) {
            if (result.data.isSuccess) {
                notificationsService.success("Seed", "Successfully started seeding Enterspeed")
                vm.seedResponse = result.data.data;
            } else {
                vm.seedResponse = null;
            }
            vm.seedState = "success";
        }, function (error) {
            notificationsService.error("Seed", error.data.message);
        });
    };

    init();
}

angular.module("umbraco").controller("EnterspeedSeedController", enterspeedSeedController);