(function () {
    'use strict';

    angular
        .module("mediaApp")
        .controller("programDeleteController", programDeleteController);

    programDeleteController.$inject = ['$scope', '$http', 'APP_CONFIG', 'params'];

    function programDeleteController($scope, $http, APP_CONFIG, params) {
        $scope.account = params.account;
        $scope.channel = params.channel;
        $scope.program = params.program;

        $scope.delete = function () {
            var url = APP_CONFIG.apiUrl + "/Accounts/" + $scope.account.Name + "/Programs/" + $scope.program.Id;
            $http.delete(url)
                .then(function () {
                    alert("Program deleted!");
                })
                .catch(function (body) {
                    alert("Failed to delete Program!" + body.Message)
                });
            $scope.$close();
        }
    }
})();
