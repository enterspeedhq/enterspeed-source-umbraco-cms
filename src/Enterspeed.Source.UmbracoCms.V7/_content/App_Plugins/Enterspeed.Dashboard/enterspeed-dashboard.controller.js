function enterspeedDashboardController() {
    var vm = this;

    vm.loadingFailedJobs = false;
    vm.failedJobs = [];
    vm.failedJobsCurrentPage = [];

    // tabs
    vm.tabs = [
        {
            id: "failedJobsTab",
            label: "Failed Jobs",
            active: true
        },
        {
            id: "seedsTab",
            label: "Seed"
        }
    ];

    function init() {
    }
    
    init();
}

angular.module("umbraco").controller("EnterspeedDashboardController", enterspeedDashboardController);