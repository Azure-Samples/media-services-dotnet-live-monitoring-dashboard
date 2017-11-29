(function () {
    'use strict';

    angular
        .module("mediaApp")
        .controller("originUpdateController", originUpdateController);

    originUpdateController.$inject = ['$scope', '$http', 'APP_CONFIG', 'params'];

    function originUpdateController($scope, $http, APP_CONFIG, params) {
        $scope.account = params.account;
        $scope.origin = params.origin
        $scope.description = params.origin.Description;
        $scope.allowList = params.origin.IPAllowList;

        function splitIpList(ipList) {
            if (ipList && ipList.length > 0) {
                return ipList.split(";").map(function (value) {
                    var parts = value.split("/");
                    return {
                        Address: parts[0],
                        SubnetPrefixLength: parts[1]
                    };
                });
            }
        }

        $scope.update = function () {
            var url = APP_CONFIG.apiUrl + "/Accounts/" + $scope.account.Name + "/Origins/" + $scope.origin.Id;
            var data = {
                Description: $scope.description,
                AllowList: splitIpList($scope.allowList),
            }
            $http.put(url, data)
                .then(function () {
                    alert("Origin updated!");
                })
                .catch(function (body) {
                    alert("Failed to update Origin!" + body.Message)
                });
            $scope.$close();
        }
    }
})();
