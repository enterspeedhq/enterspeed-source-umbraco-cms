function dashboardSettingsController() {
    var vm = this;
    vm.changeTab = changeTab;

    // tabs
    vm.tabs = [
        {
            "alias": "configurationTab",
            "label": "Configuration",
            "active": true
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

angular.module("umbraco").controller("DashboardSettingsController", dashboardSettingsController);