function dashboardController() {
    var vm = this;
    vm.changeTab = changeTab;

    vm.loadingFailedJobs = false;
    vm.failedJobs = [];
    vm.failedJobsCurrentPage = [];

    // tabs
    vm.tabs = [
        {
            "alias": "failedJobsTab",
            "label": "Failed Jobs",
            "active": true
        },
        {
            "alias": "seedsTab",
            "label": "Seed"
        }
    ];

    function changeTab(selectedTab) {
        vm.tabs.forEach(function (tab) {
            tab.active = false;
        });
        selectedTab.active = true;
    }

    function init() {
    }
    
    init();
}

angular.module("umbraco").controller("DashboardController", dashboardController);