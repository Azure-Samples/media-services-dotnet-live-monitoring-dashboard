(function () {
    'use strict';

    angular
        .module("mediaApp")
        .controller("originDeleteController", originDeleteController);

    originDeleteController.$inject = ['$scope', '$http', 'APP_CONFIG', 'params'];

    function originDeleteController($scope, $http, APP_CONFIG, params) {
        $scope.account = params.account;
        $scope.origin = params.origin;

        $scope.delete = function () {
            var url = APP_CONFIG.apiUrl + "/Accounts/" + $scope.account.Name + "/Origins/" + $scope.origin.Id;
            $http.delete(url)
                .then(function () {
                    alert("Origin deletion submitted!");
                })
                .catch(function (body) {
                    alert("Failed to delete Origin!" + body.Message)
                });
            $scope.$close();
        }
    }
})();
