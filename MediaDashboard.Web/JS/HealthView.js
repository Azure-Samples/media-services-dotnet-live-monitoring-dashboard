'use strict';

var myApp = angular.module('mediaApp.healthView', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/health', {
        templateUrl: 'Templates/Health.html',
        controller: 'healthController'
    });
}])

.controller('healthController', ['$scope', '$http', '$interval', '$routeParams', 'APP_CONFIG', 'dataService', function ($scope, $http, $interval, $routeParams, APP_CONFIG, dataService) {

    refreshView();
    $scope.refreshViewCount = 1;
    $interval(refreshView, APP_CONFIG.REFRESH_TIME);

    function refreshView() {
        $scope.refreshViewCount = $scope.refreshViewCount + 1;
        loadHealthStatus();
    }

    function loadHealthStatus() {
        $('#healthStatusLoader').show();

        var url = "http://localhost:18248" + "/api/HealthStatus";

        $http.get(url).success(function (data) {
            $scope.healthStatus = data;
            $scope.healthStatusLastRefreshTime = new Date();
            $('#healthStatusLoader').hide();
        }).error(function (data) {
            $scope.healthStatusError = data.Message;
            $('#healthStatusLoader').hide();
        });
    }

    $scope.dataPageSize = 5;
    $scope.setPageSize = function (pageSize) { $scope.dataPageSize = pageSize; }
    
}]);

//myApp.filter("pagingFilter", function () {
//    return function (input, currentPage, pageSize) {
//        return input ? input.slice(currentPage * pageSize, currentPage * (pageSize + 1)) : [];
//    }

//});

//myApp.directive("paging", function () {

//    return {
//        template: '<div><button ng-disabled="!hasPrevious()" ng-click="onPrev()"> Previous </button> {{start()}} - {{end()}} out of {{size()}} <button ng-disabled="!hasNext()" ng-click="onNext()"> Next </button><div ng-transclude=""></div> </div>',
//        restrict: 'AEC',
//        transclude: true,
//        scope: {
//            'currentPage': '=',
//            'pageSize': '=',
//            'data': '&'

//        },
//        link: function ($scope, element, attrs) {

//            $scope.size = function () {

//                return angular.isDefined($scope.data()) ? $scope.data().length : 0;
//            };

//            $scope.end = function () {
//                return $scope.start() + $scope.pageSize;
//            };

//            $scope.start = function () {
//                return $scope.currentPage * $scope.pageSize;
//            };

//            $scope.page = function () {
//                return !!$scope.size() ? ($scope.currentPage + 1) : 0;
//            };

//            $scope.hasNext = function () {
//                return $scope.page() < ($scope.size() / $scope.pageSize);
//            };

//            $scope.onNext = function () {
//                $scope.currentPage = parseInt($scope.currentPage) + 1;
//            };

//            $scope.hasPrevious = function () {
//                return !!$scope.currentPage;
//            };

//            $scope.onPrev = function () {
//                $scope.currentPage = $scope.currentPage - 1;
//            };

//            try {
//                if (typeof ($scope.data) == "undefined") {
//                    $scope.data = [];
//                }
//                if (typeof ($scope.currentPage) == "undefined") {
//                    $scope.currentPage = 0;
//                }
//                if (typeof ($scope.pageSize) == "undefined") {
//                    $scope.pageSize = 10;
//                }
//            } catch (e) { console.log(e); }
//        }

//    }

//})
