(function () {
    'use strict';

    angular
        .module("mediaApp")
        .controller("channelDeleteController", channelDeleteController);

    channelDeleteController.$inject = ['$scope', '$http', 'APP_CONFIG', 'params'];

    function channelDeleteController($scope, $http, APP_CONFIG, params) {
        $scope.account = params.account;
        $scope.channel = params.channel;

        $scope.delete = function () {
            var url = APP_CONFIG.apiUrl + "/Accounts/" + $scope.account.Name + "/Channels/" + $scope.channel.Id;
            $http.delete(url)
                .then(function () {
                    alert("Channel delete submitted!");
                })
                .catch(function (body) {
                    alert("Failed to delete Channel!" + body.Message)
                });
            $scope.$close();
        }
    }
})();
