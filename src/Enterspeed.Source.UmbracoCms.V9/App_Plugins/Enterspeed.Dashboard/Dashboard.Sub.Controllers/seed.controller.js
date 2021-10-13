function seedController(dashboardResources, notificationsService) {
    var vm = this;
    vm.seedState = "success";
    vm.seedResponse = null;

    function init() {
    }

    vm.seed = function () {
        vm.seedState = "busy";
        dashboardResources.seed().then(function (result) {
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

angular.module("umbraco").controller("SeedController", seedController);