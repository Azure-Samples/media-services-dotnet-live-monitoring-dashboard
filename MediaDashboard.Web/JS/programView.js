'use strict';

angular.module('mediaApp.programView', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/accounts/:account/programs/:id', {
        templateUrl: 'templates/programdetails.html',
        controller: 'programController'
    })
}])

.controller('programController', ['$scope', '$http', '$interval', '$routeParams', 'APP_CONFIG', 'params', function ($scope, $http, $interval, $routeParams, APP_CONFIG, params) {
    $scope.tabactive = 1;
    $scope.selectTab = function (value) {
        $scope.tabactive = value;
    }
    $scope.isActive = function (value) {
        return $scope.tabactive == value;
    }

    $scope.program = params.program;
    $scope.channel = params.channel;
    $scope.account = params.account;
    $scope.origin = params.origin;
    showProgramInfo();

    function showProgramInfo(){
        var url = getProgramUrl();
        var params = {
            params: { originHostName: $scope.channel.OriginHostName }
        };
        $http.get(url, params).then(function (response){
            $scope.program.Urls = response.data;
        }).catch(function (response){
            $scope.prgmError = response.data.Message;
        });
    }

    function getProgramUrl() {
        return APP_CONFIG.apiUrl + "/Accounts/" + $scope.account.Name + "/Programs/" + $scope.program.Id + "/Urls";
    }

    $scope.copy = function($event)
    {
        var text = $($event.currentTarget).closest("div").children(":text")[0]
        text.select();
        document.execCommand("copy");
    }
    
}]);