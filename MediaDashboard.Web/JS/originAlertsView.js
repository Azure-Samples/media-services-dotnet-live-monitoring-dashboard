'use strict';

angular.module('mediaApp.originAlertsView', [])

.controller('originAlertsController', ['$scope', '$http', '$interval', '$uibModalInstance', 'APP_CONFIG', 'params',
    function ($scope, $http, $interval, $uibModalInstance, APP_CONFIG, params) {
        $scope.origin = params.origin;
        $scope.account = params.account;
        $scope.query = function () {
            $scope.queryParams = buildQueryParams();
            refreshView();
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
        $scope.refreshViewCount = 1;
        $scope.endTime = new Date();
        $scope.startTime = new Date($scope.endTime.getTime() - 3600 * 1000);
        $scope.ingest = true;
        $scope.encoding = true;
        $scope.archive = true;
        $scope.warning = true;
        $scope.critical = true;
        refreshView();

        $uibModalInstance.rendered.then(function () {
            $(function () {
                $("#startdate").datetimepicker( {
                    defaultDate: $scope.startTime
                });
                $("#enddate").datetimepicker({
                    defaultDate: $scope.endTime
                });
            });
        });

        function buildQueryParams()
        {
            var statusLevels = [];
            if ($scope.warning) {
                statusLevels.push("Warning");
            }
            if ($scope.critical) {
                statusLevels.push("Critical");
            }


            var params = {
                StartTime: $("#startdate").data("DateTimePicker").date().format(),
                EndTime: $("#enddate").data("DateTimePicker").date().format(),
                StatusLevels: statusLevels
            };
            return params;
        }

        function refreshView() {
            $scope.refreshViewCount = $scope.refreshViewCount + 1;

            $('#systemLoader').show();
            var url = APP_CONFIG.apiUrl + "/Accounts/" + $scope.account.Name + "/OriginAlerts/" + $scope.origin.Id;
            $http({
                url: url,
                method: 'GET',
                params: $scope.queryParams
            }).success(function (data) {
                $scope.alerts = data;
                $scope.alertsLastRefreshTime = new Date();
                $('#systemLoader').hide();
            }).error(function (data) {
                $scope.alertsError = data;
                $('#systemLoader').hide();
            });
        }
    }]
);