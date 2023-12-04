function enterspeedSeedController(enterspeedDashboardResources, notificationsService, editorService) {
    var vm = this;
    vm.loadingConfiguration = false;
    vm.seedState = "success";
    vm.clearPendingJobsState = "success";
    vm.numberOfPendingJobs = 0;
    vm.seedModes = ['Everything', 'Custom'];
    vm.selectedSeedMode = vm.seedModes[0];
    vm.seedModeSelectOpen = false;
    vm.seedResponse = null;
    vm.contentTree = [];
    vm.mediaTree = [];
    vm.dictionaryTree = [];
    vm.selectedNodesToSeed = [];
    vm.selectedNodesToSeed['content'] = [];
    vm.selectedNodesToSeed['media'] = [];
    vm.selectedNodesToSeed['dictionary'] = [];
    vm.configuration = {};
    let intervalId;

    function init() {
        getConfiguration();
        getNumberOfPendingJobs();

        intervalId = setInterval(getNumberOfPendingJobs, 10 * 1000);

        window.addEventListener(
            "hashchange",
            () => {
                clearInterval(intervalId);
            },
            false
        );
    }

    function getConfiguration() {
        vm.loadingConfiguration = true;
        enterspeedDashboardResources.getEnterspeedConfiguration()
            .then(function (result) {
                if (result.data.isSuccess) {
                    vm.runJobsOnServer = result.data.data.runJobsOnServer;
                    vm.serverRole = result.data.data.serverRole;
                    vm.loadingConfiguration = false;
                }
            });
    };

    vm.seed = function () {
        vm.seedState = "busy";
        enterspeedDashboardResources.seed().then(function (result) {
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

    vm.customSeed = function () {
        vm.seedState = "busy";
        var customSeed = {
            contentNodes: vm.selectedNodesToSeed['content'],
            mediaNodes: vm.selectedNodesToSeed['media'],
            dictionaryNodes: vm.selectedNodesToSeed['dictionary']
        };
        enterspeedDashboardResources.customSeed(customSeed).then(function (result) {
            if (result.data.isSuccess) {
                notificationsService.success("Seed", "Successfully started seeding Enterspeed");
                vm.seedResponse = result.data.data;
                vm.numberOfPendingJobs = result.data.data.numberOfPendingJobs;

                vm.selectedNodesToSeed['content'] = [];
                vm.selectedNodesToSeed['media'] = [];
                vm.selectedNodesToSeed['dictionary'] = [];
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
        enterspeedDashboardResources.clearPendingJobs().then(function (result) {
            if (result.data.isSuccess) {
                notificationsService.success("Clear job queue", "Successfully cleared the queue of pending jobs");
                vm.numberOfPendingJobs = 0;
            }
            vm.clearPendingJobsState = "success";
        }, function (error) {
            notificationsService.error("Clear job queue", error.data.message);
        });
    };

    function getNumberOfPendingJobs() {
        enterspeedDashboardResources.getNumberOfPendingJobs().then(function (result) {
            if (result.data.isSuccess) {
                vm.numberOfPendingJobs = result.data.data.numberOfPendingJobs;
            } else {
                vm.numberOfPendingJobs = 0;
            }
        }, function (error) {
            notificationsService.error("Failed to check queue length", error.data.message);
        });
    };

    vm.openSelectNode = function (section) {
        editorService.open({
            title: "Select " + section + " node",
            subtitle: "Select a node to include in the seed",
            view: "/App_Plugins/Enterspeed.Dashboard/Dashboard.Sub.Views/selectNode.html",
            contentType: section,
            size: "small",
            submit: function (value) {
                if (!value.targets) {
                    return;
                }

                for (var i = 0; i < value.targets.length; i++) {
                    let target = value.targets[i];
                    if (target.id === "-1") {
                        vm.selectedNodesToSeed[section] = [target];
                    } else {
                        var existingNodeIndex = vm.selectedNodesToSeed[section].findIndex(element => element.id === target.id);
                        if (existingNodeIndex >= 0) {
                            vm.selectedNodesToSeed[section][existingNodeIndex] = target;
                        } else {
                            vm.selectedNodesToSeed[section].push(target);
                        }
                    }
                }

            },
            close: function () {
                editorService.close();
            }
        });
    }

    vm.removeSelectNode = function (section, index) {
        vm.selectedNodesToSeed[section].splice(index, 1);
    }

    vm.allowAddingNodes = function (section) {
        return vm.selectedNodesToSeed[section].findIndex(element => element.id === "-1") < 0;
    }

    vm.setSeedMode = function (seedMode) {
        vm.selectedSeedMode = seedMode;
    }

    vm.toggleSeedModeSelect = function () {
        vm.seedModeSelectOpen = !vm.seedModeSelectOpen;
    }

    init();
}

angular.module("umbraco").controller("EnterspeedSeedController", enterspeedSeedController);