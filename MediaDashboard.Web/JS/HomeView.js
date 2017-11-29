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
        $http.get(url).then(function (response) {
            $scope.customerGroups = response.data;
            $scope.customerGroupsLastRefreshTime = new Date();
        }).catch(function (response) {
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

        $http.get(url).then(function (response) {
            $scope.alerts = response.data;
            $scope.alertsLoaderLastRefreshTime = new Date();
            $('.alertsLoader').hide();
        }).catch(function (response) {
            $scope.alertsError = response.data.Message;
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
