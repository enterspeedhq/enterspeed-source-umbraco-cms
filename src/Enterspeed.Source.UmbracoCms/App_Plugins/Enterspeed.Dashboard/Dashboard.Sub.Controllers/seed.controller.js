function seedController(dashboardResources, notificationsService, editorService) {
    var vm = this;
    vm.seedState = "success";
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

    vm.seed = function () {
        vm.seedState = "busy";
        dashboardResources.seed().then(function (result) {
            if (result.data.isSuccess) {
                notificationsService.success("Seed", "Successfully started seeding Enterspeed");
                vm.seedResponse = result.data.data;
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
        dashboardResources.customSeed(customSeed).then(function (result) {
            if (result.data.isSuccess) {
                notificationsService.success("Seed", "Successfully started seeding Enterspeed");
                vm.seedResponse = result.data.data;

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

    vm.openSelectNode = function(section) {
        editorService.open({
            title: "Select " + section + " node",
            subtitle: "Select a node to include in the seed",
            view: "/App_Plugins/Enterspeed.Dashboard/Dashboard.Sub.Views/selectNode.html",
            contentType: section,
            size: "small",
            submit: function(value) {
                if (!value.target || !value.target.id) {
                    return;
                }

                if (value.target.id === "-1") {
                    vm.selectedNodesToSeed[section] = [value.target];
                }
                else {
                    var existingNodeIndex = vm.selectedNodesToSeed[section].findIndex(element => element.id === value.target.id);
                    if (existingNodeIndex >= 0) {
                        vm.selectedNodesToSeed[section][existingNodeIndex] = value.target;
                    } else {
                        vm.selectedNodesToSeed[section].push(value.target);
                    }
                }

            },
            close: function() {
                editorService.close();
            }
        });
    }

    vm.removeSelectNode = function(section, index) {
        vm.selectedNodesToSeed[section].splice(index, 1);
    }

    vm.allowAddingNodes = function(section) {
        return vm.selectedNodesToSeed[section].findIndex(element => element.id === "-1") < 0;
    }

    vm.setSeedMode = function (seedMode) {
        vm.selectedSeedMode = seedMode;
    }

    vm.toggleSeedModeSelect = function () {
        vm.seedModeSelectOpen = !vm.seedModeSelectOpen;
    }
}

angular.module("umbraco").controller("SeedController", seedController);