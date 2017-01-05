(function () {
    'use strict';

    angular
        .module("mediaApp")
        .controller("originCreationController", originCreationController);

    originCreationController.$inject = ['$scope', '$http', 'APP_CONFIG', 'params'];

    function originCreationController($scope, $http, APP_CONFIG, params) {
        $scope.account = params;
        $scope.scaleUnits = 1;

        $scope.create = function () {
            var url = APP_CONFIG.apiUrl + "/Accounts/" + $scope.account.Name + "/Origins";
            var data = {
                Name: $scope.name,
                Description: $scope.description,
                ScaleUnits: $scope.scaleUnits,
            };
            $http.post(url, data)
                .success(function () {
                    alert("Origin created!");
                })
                .error(function (body) {
                    alert("Failed to craeate Origin!" + body.Message)
                });
            $scope.$close();
        }
    }
})();
