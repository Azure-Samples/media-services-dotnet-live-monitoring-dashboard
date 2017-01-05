(function () {
    'use strict';

    angular
        .module('mediaApp')
        .controller('slateController', slateController);

    slateController.$inject = ['$scope', '$rootScope', '$http', 'params', 'APP_CONFIG']; 

    function slateController($scope, $rootScope, $http, params, APP_CONFIG) {
        $scope.account = params.account;
        $scope.channel = params.channel;

        function getUrl()
        {
            return APP_CONFIG.apiUrl + "/Accounts/" + $scope.account.Name + "/Channels/" + $scope.channel.Id;
        }

        $scope.generate = function () {
            $scope.cueId = Math.floor(new Date().getTime() / 1000);
        };

        $scope.insertSlate = function () {
            var url = getUrl() + "/Slate";
            var data = {
                Duration: $scope.duration,
                AssetId: $scope.assetId
            };

            $http.post(url, data)
                .success(function(operation) {
                    $rootScope.$emit("operation", operation, "Show Slate");
                    alert("Slate operation submitted!");
                })
                .error(function (body) {
                    alert("Failed to submit slate operation!" + body.Message)
                });
            $scope.$close();

        };

        $scope.insertAd = function (showSlate) {
            var url = getUrl() + "/AdMarker";
            var data = {
                Duration: $scope.duration,
                CueId: $scope.cueId,
                ShowSlate: $scope.showSlate,
                AssetId: $scope.assetId
            };

            $http.post(url, data)
                .success(function (operation) {
                    $rootScope.$emit("operation", operation, "Show Ad Marker");
                    alert("Ad operation submitted!");
                })
                .error(function (body) {
                    alert("Failed to submit  Ad operation!" + body.Message)
                });
            $scope.$close();
        };
    }
})();
