'use strict';

angular.module('mediaApp.channelAlertsView', [])

.controller('channelAlertsController', ['$scope', '$http', '$interval', '$uibModalInstance', 'APP_CONFIG', 'params',
    function ($scope, $http, $interval, $uibModalInstance, APP_CONFIG, params) {
        $scope.channel = params.channel;
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
        $scope.origin = true;
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
            var metricTypes = [];
            if ($scope.archive) {
                metricTypes.push("Archive");
            }
            if ($scope.ingest) {
                metricTypes.push("Ingest");
            }
            if ($scope.encoding) {
                metricTypes.push("Encoding");
            }
            if ($scope.origin) {
                metricTypes.push("Origin");
            }

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
                MetricTypes: metricTypes,
                StatusLevels: statusLevels
            };
            return params;
        }

        function refreshView() {
            $scope.refreshViewCount = $scope.refreshViewCount + 1;

            $('#systemLoader').show();
            var url = APP_CONFIG.apiUrl + "/Accounts/" + $scope.account.Name + "/ChannelAlerts/" + $scope.channel.Id + "/" + $scope.channel.OriginId;
            $http({
                url: url,
                method: 'GET',
                params: $scope.queryParams
            }).then(function (response) {
                $scope.alerts = response.data;
                $scope.alertsLastRefreshTime = new Date();
                $('#systemLoader').hide();
            }).catch(function (response) {
                $scope.alertsError = response.data;
                $('#systemLoader').hide();
            });
        }
    }]
);