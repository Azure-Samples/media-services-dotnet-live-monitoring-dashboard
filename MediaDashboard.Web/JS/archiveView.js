'use strict';

angular.module('mediaApp.archiveTelemetryView', [])

.controller('archiveController', ['$scope', '$http', '$interval', 'APP_CONFIG', 'params',
    function ($scope, $http, $interval, APP_CONFIG, params) {
        $scope.account = params.account;
        $scope.channel = params.channel;
        refreshView();
        $scope.refreshViewCount = 1;
        var timer = $interval(refreshView, APP_CONFIG.REFRESH_TIME);
        $scope.$on("modal.closing", function () {
            $interval.cancel(timer);
        });

        function refreshView() {
            $scope.refreshViewCount = $scope.refreshViewCount + 1;
            var url = getArchiveUrl();
            $http.get(url).then(function (response) {
                $scope.metricGroups = response.data;
                $scope.lastRefreshTime = new Date();
            }).catch(function (response) {
                $scope.error = response.data;
            });
        }

        function getArchiveUrl() {
            return APP_CONFIG.apiUrl + "/Accounts/" + $scope.account.Name + "/archiveTelemetry/" + $scope.channel.Id;
        }

    }]);