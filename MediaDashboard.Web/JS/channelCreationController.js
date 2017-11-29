(function () {
    'use strict';

    angular
        .module("mediaApp")
        .controller("channelCreationController", channelCreationController);

    channelCreationController.$inject = ['$scope', '$rootScope', '$http', 'APP_CONFIG', 'params']; 

    function channelCreationController($scope, $rootScope, $http, APP_CONFIG, params) {
        $scope.account = params;
        $scope.settings = {
            encodingType: "Standard",
            streamingProtocol: "RTMPMEGTS",
            encodingPreset: "Default720p",
            ingestAllowList: "0.0.0.0/0"
        };
        $scope.streamingProtocols = ["RTMP", "FragmentedMP4", "RTPMPEGTS" ];

        $scope.encodingChanged = function () {
            if ($scope.settings.encodingType === "Standard") {
                $scope.settings.encodingPreset = "Default720p";
                $scope.settings.streamingProtocol = "RTPMPEGTS";
                $scope.streamingProtocols.push("RTPMPEGTS");
            } else if ($scope.settings.encodingType === "Premium") {
                $scope.settings.encodingPreset = "Default1080p";
                $scope.settings.streamingProtocol = "RTPMPEGTS";
                $scope.streamingProtocols.push("RTPMPEGTS");
            } else {
                $scope.settings.encodingPreset = undefined;
                $scope.settings.streamingProtocol = "RTMP";
                $scope.streamingProtocols.pop();
            }            
        }

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

        $scope.create = function () {
            var url = APP_CONFIG.apiUrl + "/Accounts/" + $scope.account.Name + "/Channels"
            var data = {
                Name: $scope.settings.name,
                Description: $scope.settings.description,
                EncodingType: $scope.settings.encodingType,
                EncodingPreset: $scope.settings.encodingPreset,
                StreamingProtocol: $scope.settings.streamingProtocol,
                IngestAllowList: splitIpList($scope.settings.ingestAllowList),
                PreviewAllowList: splitIpList($scope.settings.previewAllowList),
            }
            $http.post(url, data)
                .then(function (response) {
                    $rootScope.$emit("operation", response.data, "Channel start")
                    alert("Channel creation submitted!");
                })
                .catch(function (body) {
                    alert("Failed to create Channel!" + body.Message)
                });
            $scope.$close();
        }
    }
})();
