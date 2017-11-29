'use strict';

// logging for ADAL.
Logging = {
    level: 1,
    log: function (message) {
        console.log(message);
    }
};

/* App Module */

var myApp = angular.module('mediaApp', [
    'ngRoute',
    'AdalAngular',
    'ui.bootstrap',
    'mediaApp.navbarView',
    'mediaApp.homeView',
    'mediaApp.detailsView',
    'mediaApp.channelTelemetryView',
    'mediaApp.channelAlertsView',
    'mediaApp.playerView',
    'mediaApp.programView',
    'mediaApp.ingestTelemetryView',
    'mediaApp.originView',
    'mediaApp.originTelemetryView',
    'mediaApp.archiveTelemetryView',
    'mediaApp.channelView',
    'mediaApp.originAlertsView'
]);

myApp.config(['$locationProvider', function ($locationProvider) {
    $locationProvider.html5Mode(false).hashPrefix('');
}]);

myApp.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.otherwise({
        redirectTo: '/home'
    });
}]);

myApp.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.useApplyAsync(true);
}]);

myApp.config(['$compileProvider', function ($compileProvider) {
    $compileProvider.debugInfoEnabled(false);
}]);

myApp.constant("APP_CONFIG", {
    "REFRESH_TIME": 30000,
    apiUrl: "/api"
    });

myApp.filter("pagingFilter", function () {
    return function (input, currentPage, pageSize) {
        return input ? input.slice(currentPage * pageSize, currentPage * (pageSize + 1)) : [];
    }
});

myApp.directive("paging", function () {

    return {
        templateUrl: 'templates/paging.html',
        restrict: 'AEC',
        transclude: true,
        scope: {
            'currentPage': '=',
            'pageSize': '=',
            'data': '&'

        },
        link: function ($scope, element, attrs) {

            $scope.size = function () {

                return angular.isDefined($scope.data()) ? $scope.data().length : 0;
            };

            $scope.end = function () {
                return $scope.start() + $scope.pageSize;
            };

            $scope.start = function () {
                return $scope.currentPage * $scope.pageSize;
            };

            $scope.page = function () {
                return $scope.size() ? ($scope.currentPage + 1) : 0;
            };

            $scope.hasNext = function () {
                return $scope.page() < ($scope.size() / $scope.pageSize);
            };

            $scope.onNext = function () {
                $scope.currentPage = parseInt($scope.currentPage) + 1;
            };

            $scope.hasPrevious = function () {
                return !!$scope.currentPage;
            };

            $scope.onPrev = function () {
                $scope.currentPage = $scope.currentPage - 1;
            };

            try {
                if (typeof ($scope.data) === "undefined") {
                    $scope.data = [];
                }
                if (typeof ($scope.currentPage) === "undefined") {
                    $scope.currentPage = 0;
                }
                if (typeof ($scope.pageSize) === "undefined") {
                    $scope.pageSize = 10;
                }
            } catch (e) { console.log(e); }
        }
    }
})