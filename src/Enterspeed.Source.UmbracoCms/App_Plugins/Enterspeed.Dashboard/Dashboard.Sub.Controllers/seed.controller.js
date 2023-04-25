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
    vm.selectedNodesToSeed['Content'] = [];
    vm.selectedNodesToSeed['Media'] = [];
    vm.selectedNodesToSeed['Dictionary'] = [];

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

    vm.selectNode = function() {
        editorService.open({
            title: "Select node",
            subtitle: "Select a node to include in the seed",
            view: "/App_Plugins/Enterspeed.Dashboard/Dashboard.Sub.Views/selectNode.html",
            size: "small",
            submit: function(value) {
                if (!value.target || !value.target.id) {
                    return;
                }

                var existingNodeIndex = vm.selectedNodesToSeed[value.target.contentType].findIndex(element => element.id === value.target.id);
                if (existingNodeIndex >= 0) {
                    vm.selectedNodesToSeed[value.target.contentType][existingNodeIndex] = value.target;
                } else {
                    vm.selectedNodesToSeed[value.target.contentType].push(value.target);
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

    vm.setSeedMode = function (seedMode) {
        vm.selectedSeedMode = seedMode;
    }

    vm.toggleSeedModeSelect = function () {
        vm.seedModeSelectOpen = !vm.seedModeSelectOpen;
    }
}

angular.module("umbraco").controller("SeedController", seedController);