'use strict';

angular.module('mediaApp.channelTelemetryView', [])

.controller('channelTelemetryController', ['$scope', '$http', '$interval', 'APP_CONFIG', 'params', function ($scope, $http, $interval, APP_CONFIG, params) {
    $scope.channel = params.channel;
    $scope.account = params.account;

    $scope.getHealthLevel = function(healthLevel) {
        switch(healthLevel)
        {
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
    $scope.getLossPecentage = function (totalBytesLost, totalBytesReceived) {
        var totalLossPct = 0;
        if(totalBytesReceived>0){
            totalLossPct = (totalBytesLost / totalBytesReceived) * 100;
        }
        return totalLossPct;
    }
    function getTelemetryUrl()
    {
        return APP_CONFIG.apiUrl + "/Accounts/" + $scope.account.Name + "/ChannelTelemetry/" + $scope.channel.Id;
    }

    refreshView();
    $scope.refreshViewCount = 1;
    var timer = $interval(refreshView, APP_CONFIG.REFRESH_TIME);
    $scope.$on("modal.closing", function () {
        $interval.cancel(timer);
    });

    function refreshView() {
        $scope.refreshViewCount = $scope.refreshViewCount + 1;

        $('#systemLoader').show();
        var url = getTelemetryUrl();

        $http.get(url).success(function (data) {
            if ($scope.telemetry) {
                angular.merge($scope.telemetry, data);
            } else {
                $scope.telemetry = data;
            }
            $scope.telemetryLastRefreshTime = new Date();
            $('#systemLoader').hide();
        }).error(function (data) {
            $scope.telemetryError = data.Message;
            $('#systemLoader').hide();
        });
    }

}]);
