function enterspeedSeedController(enterspeedDashboardResources, $scope, $filter, $timeout) {
    var vm = this;
    vm.seedState = "success";
    vm.seedResponse = null;

    function init() {
    }

    vm.seed = function () {
        vm.seedState = "busy";
        enterspeedDashboardResources.seed().then(function (result) {
            if (result.data.isSuccess) {
                vm.seedResponse = result.data.data;
            } else {
                vm.seedResponse = null;
            }
            vm.seedState = "success";
        });
    };

    init();
}

angular.module("umbraco").controller("EnterspeedSeedController", enterspeedSeedController);