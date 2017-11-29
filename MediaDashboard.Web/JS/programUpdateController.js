(function () {
    'use strict';

    angular
        .module("mediaApp")
        .controller("programUpdateController", programUpdateController);

    programUpdateController.$inject = ['$scope', '$http', 'APP_CONFIG', 'params'];

    function programUpdateController($scope, $http, APP_CONFIG, params) {
        $scope.account = params.account;
        $scope.channel = params.channel;
        $scope.program = params.program;
        $scope.description = params.program.Description;
        $scope.archiveWindowLength = params.program.ArchiveWindowLength;

        $scope.update = function () {
            var url = APP_CONFIG.apiUrl + "/Accounts/" + $scope.account.Name + "/Programs/" + $scope.program.Id;
            var data = {
                Description: $scope.description,
                ArchiveWindowLength: $scope.archiveWindowLength
            }
            $http.put(url, data)
                .then(function () {
                    alert("Program updated!");
                })
                .catch(function (body) {
                    alert("Failed to update Program!" + body.Message)
                });
            $scope.$close();
        }
    }
})();
