'use strict';

angular.module('mediaApp.originTelemetryView', [])

.controller('originTelemetryController', ['$scope', '$http', '$interval', 'APP_CONFIG', 'params',

function ($scope, $http, $interval, APP_CONFIG, params) {
    $scope.origin = params.origin;
    $scope.account = params.account;
    refreshView();
    $scope.refreshViewCount = 1;
    var timer = $interval(refreshView, APP_CONFIG.REFRESH_TIME);
    $scope.$on("modal.closing", function () {
        $interval.cancel(timer);
    });

    function refreshView() {
        $scope.refreshViewCount = $scope.refreshViewCount + 1;
        var url = getOriginUrl();
        $http.get(url).success(function (data){
            $scope.metricGroups = data;
            $scope.lastRefreshTime = new Date();
        }).error(function (data) {
            $scope.error = data;
        });
    }

    function getOriginUrl() {
        return APP_CONFIG.apiUrl + "/Accounts/" + $scope.account.Name + "/originTelemetry/" + $scope.origin.Id;
    }
    
}]);
