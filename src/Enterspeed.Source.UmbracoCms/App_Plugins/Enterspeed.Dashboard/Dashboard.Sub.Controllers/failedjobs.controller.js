﻿function failedJobsController(dashboardResources, $scope, $filter, $timeout) {
    var vm = this;
    vm.loadingFailedJobs = false;
    vm.failedJobs = [];
    vm.activeException = "";

    vm.removeModes = ['Everything', 'Selected'];
    vm.selectedRemoveMode = vm.removeModes[0];
    vm.removeModeSelectOpen = false;

    function init() {
        vm.getFailedJobs();
    }

    vm.toggleException = function (index) {
        if (index === vm.activeException) {
            vm.activeException = "";
        } else {
            vm.activeException = index;
        }
    };

    // Pagination
    vm.pagination = {
        pageIndex: 0,
        pageNumber: 1,
        totalPages: 1,
        pageSize: 30
    };
    $scope.q = '';

    vm.nextPage = function () {
        vm.pagination.pageIndex = vm.pagination.pageIndex + 1;
    };

    vm.prevPage = function () {
        vm.pagination.pageIndex = vm.pagination.pageIndex - 1;
    };

    vm.goToPage = function (pageNumber) {
        vm.pagination.pageIndex = pageNumber - 1;
    };

    vm.getFailedJobs = function () {
        vm.loadingFailedJobs = true;

        dashboardResources.getFailedJobs().then(function (result) {
            if (result.data.isSuccess) {
                vm.failedJobs = result.data.data;
                vm.pagination.totalPages = Math.ceil(vm.failedJobs.length / vm.pagination.pageSize);
            }

            vm.loadingFailedJobs = false;
        });
    };

    vm.setRemoveMode = function (removeMode) {
        vm.selectedRemoveMode = removeMode;
    }

    vm.toggleRemoveModeSelect = function () {
        vm.removeModeSelectOpen = !vm.removeModeSelectOpen;
    }

    vm.removeFailedJobs = function () {
        dashboardResources.removeFailedJobs().then(function (result) {
            vm.removingFailedJobs = true;
            if (result.data.isSuccess) {
                vm.getFailedJobs();
            }

            vm.removingFailedJobs = false;
        });
    };

    $scope.getData = function () {
        return $filter('filter')(vm.failedJobs, $scope.q);
    };

    init();
}

angular.module("umbraco").controller("FailedJobsController", failedJobsController);

angular.module("umbraco").filter('startFrom', function () {
    return function (input, start) {
        start = +start;
        return input.slice(start);
    };
});