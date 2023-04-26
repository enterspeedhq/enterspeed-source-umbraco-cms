angular.module('umbraco.resources')
    .factory('dashboardResources',
        function ($http) {
            return {
                getFailedJobs: function () {
                    return $http.get("/umbraco/backoffice/api/DashboardApi/GetFailedJobs");
                },
                seed: function () {
                    return $http.get("/umbraco/backoffice/api/DashboardApi/Seed");
                },
                clearPendingJobs: function () {
                    return $http.post("/umbraco/backoffice/api/DashboardApi/ClearPendingJobs");
                },
                getNumberOfPendingJobs: function () {
                    return $http.get("/umbraco/backoffice/api/DashboardApi/GetNumberOfPendingJobs");
                },
                getEnterspeedConfiguration: function () {
                    return $http.get("/umbraco/backoffice/api/DashboardApi/GetEnterspeedConfiguration");
                },
                saveEnterspeedConfiguration: function (configuration) {
                    return $http.post("/umbraco/backoffice/api/DashboardApi/SaveConfiguration", configuration);
                },
                testEnterspeedConfiguration: function (configuration) {
                    return $http.post("/umbraco/backoffice/api/DashboardApi/TestConfigurationConnection", configuration);
                }
            };
        });