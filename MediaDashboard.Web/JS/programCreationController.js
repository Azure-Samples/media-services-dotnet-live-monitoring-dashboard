(function () {
    'use strict';

    angular
        .module("mediaApp")
        .controller("programCreationController", programCreationController);

    programCreationController.$inject = ['$scope', '$http', 'APP_CONFIG', 'params'];

    function programCreationController($scope, $http, APP_CONFIG, params) {
        $scope.account = params.account;
        $scope.channel = params.channel;
        $scope.archiveWindowLength = "04:00:00";
        $scope.autoCreateAsset = true;

        $scope.create = function () {
            var url = APP_CONFIG.apiUrl + "/Accounts/" + $scope.account.Name + "/Channels/" + $scope.channel.Id + "/Programs";
            var data = {
                Name: $scope.name,
                Description: $scope.description,
                ArchiveWindowLength: $scope.archiveWindowLength,
                AssetId: $scope.assetId,
                ManifestName: $scope.manifestName,
            };
            $http.post(url, data)
                .then(function() {
                    alert("Program creation submitted!");
                })
                .catch(function (body) {
                    alert("Failed to craeate Program!" + body.Message)
                });
            $scope.$close();
        }
    }
})();
