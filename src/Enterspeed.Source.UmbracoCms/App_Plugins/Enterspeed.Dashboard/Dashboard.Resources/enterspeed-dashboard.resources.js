angular.module('umbraco.resources')
    .factory('enterspeedDashboardResources',
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
                customSeed: function (customSeed) {
                    return $http.post("/umbraco/backoffice/api/DashboardApi/CustomSeed", customSeed);
                },
                getEnterspeedConfiguration: function () {
                    return $http.get("/umbraco/backoffice/api/DashboardApi/GetEnterspeedConfiguration");
                },
                saveEnterspeedConfiguration: function (configuration) {
                    return $http.post("/umbraco/backoffice/api/DashboardApi/SaveConfiguration", configuration);
                },
                testEnterspeedConfiguration: function (configuration) {
                    return $http.post("/umbraco/backoffice/api/DashboardApi/TestConfigurationConnection", configuration);
                },
                deleteFailedJobs: function () {
                    return $http.post("/umbraco/backoffice/api/DashboardApi/DeleteFailedJobs")
                },
                deleteSelectedFailedJobs: function (ids) {
                    return $http.post("/umbraco/backoffice/api/DashboardApi/DeleteJobs", ids)
                }
            };
        });