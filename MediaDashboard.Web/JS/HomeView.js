'use strict';

angular.module('mediaApp.homeView', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/home', {
        templateUrl: 'Templates/Home.html',
        controller: 'homeController',
        requireADLogin: true
    });
}])

.controller('homeController', ['$scope', '$http', '$interval', '$routeParams', 'APP_CONFIG', function ($scope, $http, $interval, $routeParams, APP_CONFIG, dataService) {

    refreshView();
    $scope.refreshViewCount = 1;
    $scope.dataPageSize = 5;
    $scope.setPageSize = function (pageSize) { $scope.dataPageSize = pageSize; }

    function refreshView() {
        $scope.refreshViewCount = $scope.refreshViewCount + 1;
        loadCustomerGroups();
        loadDashboardAlerts();
    }

    $scope.$on("refresh-start", function () {
        refreshView();
    });

    function loadCustomerGroups() {
        $('.customersLoader').show();
        var url = APP_CONFIG.apiUrl + "/CustomerGroups";
        $http.get(url).success(function (data) {
            $scope.customerGroups = data;
            $scope.customerGroupsLastRefreshTime = new Date();
        }).error(function (data) {
            $scope.customerGroupsError = data.Message;
        }).finally(function () {
            $('.customersLoader').hide();
            $scope.$emit("refresh-end", {
                refreshViewCount: $scope.refreshViewCount
            });
        });
    }

    function loadDashboardAlerts() {
        $('.alertsLoader').show();

        var url = APP_CONFIG.apiUrl + "/DashboardAlerts";

        $http.get(url).success(function (data) {
            $scope.alerts = data;
            $scope.alertsLoaderLastRefreshTime = new Date();
            $('.alertsLoader').hide();
        }).error(function (data) {
            $scope.alertsError = data.Message;
            $('.alertsLoader').hide();
        });
    }

    $scope.showAlerts = function () {
        $(".alerts").toggleClass("open");
    }

    $scope.getHealthLevel = function (healthLevel) {
        switch (healthLevel) {
            case "Critical":
            case "Error":
                return 3;
            case "Warning":
                return 2;
            case "Healthy":
            case "Normal":
                return 1;
            default:
                return 0;
        }
    }
}]);
