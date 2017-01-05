'use strict';

angular.module('mediaApp.navbarView', [])
    .controller('navbarController', ['$scope', '$rootScope', '$interval', 'APP_CONFIG', 'adalAuthenticationService',
    function ($scope, $rootScope, $interval, APP_CONFIG, adalService) {

        var timer;
        $scope.refreshViewCount = 0;
        $scope.refreshDelay = APP_CONFIG.REFRESH_TIME / 1000;
        $scope.operations = [];

        $scope.logout = function () {
            adalService.logOut();
        }

        $scope.login = function () {
            adalService.login();
        }

        $rootScope.$on("$routeChangeSuccess", function () {
            if (timer) {
                // cancel any previous timer.
                $interval.cancel(timer);
            }
            $scope.refreshing = true;
            $scope.refreshDelay = APP_CONFIG.REFRESH_TIME / 1000;
        });

        $rootScope.$on("refresh-end", function (event, args) {
            $scope.refreshViewCount = args.refreshViewCount;
            $scope.refreshTime = Date.now();
            $scope.refreshing = false;
            timer = $interval(refreshView, 1000);
            $scope.refreshDelay = APP_CONFIG.REFRESH_TIME / 1000;
        });

        $rootScope.$on("operation", function (event, operation, message) {
            operation.Message = message;
            $scope.operations.push(operation);
            if (scope.operation.length > 10) {
                $scope.operations.shift();
            }
        });

        function refreshView() {
            if (!$scope.refreshing) {
                --$scope.refreshDelay;
                if ($scope.refreshDelay == 0) {
                    $interval.cancel(timer);
                    $scope.refreshing = true;
                    $rootScope.$broadcast("refresh-start");
                }
            }
        }

    }]);