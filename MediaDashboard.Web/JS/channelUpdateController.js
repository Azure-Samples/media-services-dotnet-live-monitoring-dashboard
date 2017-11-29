(function () {
    'use strict';

    angular
        .module("mediaApp")
        .controller("channelUpdateController", channelUpdateController);

    channelUpdateController.$inject = ['$scope', '$http', 'APP_CONFIG', 'params'];

    function channelUpdateController($scope, $http, APP_CONFIG, params) {
        $scope.account = params.account;
        $scope.channel = params.channel
        $scope.ingestAllowList = params.channel.IngestAllowList;
        $scope.previewAllowList = params.channel.PreviewAllowList;
        $scope.description = params.channel.Description;

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
            var url = APP_CONFIG.apiUrl + "/Accounts/" + $scope.account.Name + "/Channels/" + $scope.channel.Id;
            var data = {
                Description: $scope.description,
                IngestAllowList: splitIpList($scope.ingestAllowList),
                PreviewAllowList: splitIpList($scope.previewAllowList)
            }
            $http.put(url, data)
                .then(function () {
                    alert("Channel update submitted!");
                })
                .catch(function (body) {
                    alert("Failed to update Channel!" + body.Message)
                });
            $scope.$close();
        }
    }
})();
