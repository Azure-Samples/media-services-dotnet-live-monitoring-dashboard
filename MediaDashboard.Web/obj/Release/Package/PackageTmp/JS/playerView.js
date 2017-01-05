'use strict';

angular.module("mediaApp.playerView", [])
.controller('playerController', ['$scope', '$uibModalInstance', 'params', function ($scope, $uibModalInstance, params) {
    $scope.playbackUrl = params.playbackUrl;
    var player;
    $scope.close = function () {
        if (player) 
        {
            player.dispose();
        }
        $scope.$close();
    }
    $uibModalInstance.rendered.then(function () {
        var playerOptions = {
            heuristicProfile: "HighQuality",
            nativeControlsForTouch: false,
            logo: { enabled: false },
            autoplay: true,
            controls: true,
            width: 800,
            height: 600
        };
        player = amp('popupplayer', playerOptions);
        player.src([{
            src: params.playbackUrl,
            type: params.type,
            disableUrlRewriter: !params.playbackUrl.startsWith("https")
        }]);
    });

    $scope.copy = function ($event) {
        var text = $($event.currentTarget).parent().children(":text")[0]
        text.select();
        document.execCommand("copy");
    }
}]);