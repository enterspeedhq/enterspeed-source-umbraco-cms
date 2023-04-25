﻿function selectNodeController($scope) {
    var vm = this;
    vm.includeDescendents = false;

    function init() {
        vm.contentType = $scope.model.contentType;
        resetNodeSelection();
    }

    $scope.onTreeInit = function () {
        $scope.dialogTreeApi.callbacks.treeNodeSelect(nodeSelectHandler);
    }

    function resetNodeSelection() {
        $scope.dialogTreeApi = {};
        $scope.model.target = {};
    }

    function nodeSelectHandler(args) {
        if (args && args.event) {
            args.event.preventDefault();
            args.event.stopPropagation();
        }

        if ($scope.currentNode) {
            //un-select if there's a current one selected
            $scope.currentNode.selected = false;
        }

        $scope.currentNode = args.node;
        $scope.currentNode.selected = true;
        $scope.model.target.id = args.node.id;
        $scope.model.target.udi = args.node.udi;
        $scope.model.target.name = args.node.name;
        $scope.model.target.icon = args.node.icon;
    }

    vm.toggleIncludeDescendents = function () {
        vm.includeDescendents = !vm.includeDescendents;
    }

    vm.close = function () {
        if ($scope.model && $scope.model.close) {
            $scope.model.close();
        }
    }

    vm.submit = function () {
        if ($scope.model && $scope.model.submit) {
            $scope.model.target.includeDescendents = vm.includeDescendents;
            $scope.model.submit($scope.model);
        }

        vm.close();
    }

    init();
}

angular.module("umbraco").controller("SelectNodeController", selectNodeController);