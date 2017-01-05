'use strict';

angular.module('mediaApp.channelView', ['ngRoute'])

.controller('channelController', ['$scope', '$http', '$interval', 'APP_CONFIG', 'params', function ($scope, $http, $interval, APP_CONFIG, params) {

        $scope.channel = params.channel;
        $scope.account = params.account;
        $scope.origin = params.origin;
        $scope.tabactive = 1;
        $scope.selectTab = function (value) {
            $scope.tabactive = value;
        }
        $scope.isActive = function (value) {
            return $scope.tabactive == value;
        }

        $scope.copy = function ($event) {
            var text = $($event.currentTarget).parent().children(":text")[0]
            text.select();
            document.execCommand("copy");
        }

        function getChannelUrl() {
            return APP_CONFIG.apiUrl + "/Accounts/" + $scope.account.Name + "/channels/" + $scope.channel.Id;
        }
        
        // showData();
        
        function showData() {
            var url = getChannelUrl();
           
            $http.get(url).success(function (data) {
                var results =data;
                $scope.details = results;
                $scope.LastRefreshTime = new Date();
                
            }).error(function (data) {
                $scope.error = data.Message;
                
            });
        }
    }]);



