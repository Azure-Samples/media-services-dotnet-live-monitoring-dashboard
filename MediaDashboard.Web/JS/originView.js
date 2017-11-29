'use strict';

angular.module('mediaApp.originView', [])

.controller('originController', ['$scope', '$http', '$interval', 'APP_CONFIG', 'params',

function ($scope, $http, $interval, APP_CONFIG, params) {
    $scope.origin = params.origin;
    $scope.account = params.account;
    $scope.activeTab = 1;
    $scope.selectTab = function (value) {
        $scope.activeTab = value;
    }

    function refreshView() {
        $scope.refreshViewCount = $scope.refreshViewCount + 1;
        var url = getOriginUrl();
        $http.get(url).then(function (response) {
            $scope.origin = response.data;
            $scope.lastRefreshTime = new Date();
        }).catch(function (response) {
            $scope.error = response.data;
        });
    }

    function getOriginUrl() {
        return APP_CONFIG.apiUrl + "/Accounts/" + $scope.account.Name + "/origins/" + $scope.origin.Id;
    }

}]);