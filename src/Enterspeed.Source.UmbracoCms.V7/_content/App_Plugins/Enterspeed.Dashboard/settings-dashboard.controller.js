﻿function dashboardSettingsController() {
    var vm = this;

    // tabs
    vm.tabs = [
        {
            id: "configurationTab",
            label: "Configuration"
        }
    ];

    function init() {
    }
    
    init();
}

angular.module("umbraco").controller("DashboardSettingsController", dashboardSettingsController);