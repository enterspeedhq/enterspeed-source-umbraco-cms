function enterspeedFailedJobsController(enterspeedDashboardResources, $scope, $filter, $timeout) {
    var vm = this;
    vm.loadingFailedJobs = false;
    vm.failedJobs = [];
    vm.activeException = "";
    vm.deleteModes = ['Everything', 'Selected'];
    vm.selectedDeleteMode = vm.deleteModes[0];
    vm.deleteModeSelectOpen = false;
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

        enterspeedDashboardResources.getFailedJobs().then(function (result) {
            if (result.data.isSuccess) {
                vm.failedJobs = result.data.data;
                vm.pagination.totalPages = Math.ceil(vm.failedJobs.length / vm.pagination.pageSize);
            }

            vm.loadingFailedJobs = false;
        });
    };

    $scope.getData = function () {
        return $filter('filter')(vm.failedJobs, $scope.q);
    };

    vm.setDeleteMode = function (deleteMode) {
        vm.selectedDeleteMode = deleteMode;
    }

    vm.toggleDeleteModeSelect = function () {
        vm.deleteModeSelectOpen = !vm.deleteModeSelectOpen;
    }

    vm.deleteFailedJobs = function () {
        if (!confirm("Are you sure you want to delete the failed job(s)?")) {
            return;
        }
        vm.deletingFailedJobs = true;

        if (vm.selectedDeleteMode === "Selected") {
            let failedJobsToDelete = vm.getSelectedFailedJobs();

            if (failedJobsToDelete.length) {
                let jobIdsToDelete = {
                    Ids: failedJobsToDelete.map(fj => fj.id)
                }

                enterspeedDashboardResources.deleteSelectedFailedJobs(jobIdsToDelete).then(function (result) {
                    if (result.data.isSuccess) {
                        vm.getFailedJobs();
                    }
                });
            }

        } else {
            enterspeedDashboardResources.deleteFailedJobs().then(function (result) {
                if (result.data.isSuccess) {
                    vm.getFailedJobs();
                }
            });
        }

        vm.deletingFailedJobs = false;
    }

    vm.getSelectedFailedJobs = function () {
        return vm.failedJobs.filter(fj => fj.selected === true);
    }

    init();
}

angular.module("umbraco").controller("EnterspeedFailedJobsController", enterspeedFailedJobsController);

angular.module("umbraco").filter('startFrom', function () {
    return function (input, start) {
        start = +start;
        return input.slice(start);
    };
});