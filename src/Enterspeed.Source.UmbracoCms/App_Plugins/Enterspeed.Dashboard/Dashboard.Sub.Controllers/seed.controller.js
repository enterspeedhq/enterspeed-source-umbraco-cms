function seedController(dashboardResources, notificationsService) {
    var vm = this;
    vm.seedState = "success";
    vm.clearPendingJobsState = "success";
    vm.numberOfPendingJobs = 0;
    vm.seedResponse = null;

    function init() {
        getNumberOfPendingJobs();

        setInterval(getNumberOfPendingJobs, 10 * 1000);
    }

    vm.seed = function () {
        vm.seedState = "busy";
        dashboardResources.seed().then(function (result) {
            if (result.data.isSuccess) {
                notificationsService.success("Seed", "Successfully started seeding to Enterspeed");
                vm.seedResponse = result.data.data;
                vm.numberOfPendingJobs = result.data.data.numberOfPendingJobs;
            } else {
                vm.seedResponse = null;
            }
            vm.seedState = "success";
        }, function (error) {
            notificationsService.error("Seed", error.data.message);
        });
    };

    vm.clearPendingJobs = function () {
        vm.clearPendingJobsState = "busy";
        dashboardResources.clearPendingJobs().then(function (result) {
            if (result.data.isSuccess) {
                notificationsService.success("Clear job queue", "Successfully cleared the queue of pending jobs");
                vm.numberOfPendingJobs = 0;
            }
            vm.clearPendingJobsState = "success";
        }, function (error) {
            notificationsService.error("Clear job queue", error.data.message);
        });
    };

    function getNumberOfPendingJobs () {
        dashboardResources.getNumberOfPendingJobs().then(function (result) {
            if (result.data.isSuccess) {
                vm.numberOfPendingJobs = result.data.data.numberOfPendingJobs;
            } else {
                vm.numberOfPendingJobs = 0;
            }
        }, function (error) {
            notificationsService.error("Failed to check queue length", error.data.message);
        });
    };

    init();
}

angular.module("umbraco").controller("SeedController", seedController);