function enterspeedSelectNodeController($scope) {
    let vm = this;
    vm.includeDescendants = false;
    vm.includeEverything = false;

    function init() {
        vm.contentType = $scope.model.contentType;
        $scope.model.targets = [];
        resetNodeSelection();
    }

    $scope.onTreeInit = function () {
        $scope.dialogTreeApi.callbacks.treeNodeSelect(nodeSelectHandler);
    }

    function resetNodeSelection() {
        $scope.dialogTreeApi = {};
        $scope.model.targets = [];
    }

    function nodeSelectHandler(args) {
        if (args && args.event) {
            args.event.preventDefault();
            args.event.stopPropagation();
        }

        $scope.currentNode = args.node;

        // Check if exists already. If it exists, then we remove
        let existingTargetIndex = $scope.model.targets.findIndex(element => element.id === args.node.id);
        if (existingTargetIndex > -1) {
            $scope.model.targets.splice(existingTargetIndex, 1);
            $scope.currentNode.selected = false;
        } else {
            $scope.model.targets.push({
                id: args.node.id,
                name: args.node.name,
                icon: args.node.icon
            });

            $scope.currentNode.selected = true;
        }
    }

    vm.close = function () {
        if ($scope.model && $scope.model.close) {
            $scope.model.close();
        }
    }

    vm.submit = function () {
        if ($scope.model && $scope.model.submit) {
            if (vm.includeEverything) {
                $scope.model.targets = [];
                $scope.model.targets.push({
                    id: "-1",
                    name: "Everything",
                    icon: "icon-item-arrangement",
                    includeDescendants: true
                });

            } else {
                for (let i = 0; i < $scope.model.targets.length; i++) {

                    $scope.model.targets[i].includeDescendants = vm.includeDescendants;
                }
            }

            $scope.model.submit($scope.model);
        }

        vm.close();
    }

    init();
}

angular.module("umbraco").controller("EnterspeedSelectNodeController", enterspeedSelectNodeController);