'use strict';

angular.module('mediaApp.detailsView', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/details/:id', {
        templateUrl: 'templates/details.html',
        controller: 'detailsController',
        requireADLogin: true
    })
}])

.controller('detailsController', ['$scope', '$rootScope', '$http', '$interval', '$routeParams', 'APP_CONFIG', 'adalAuthenticationService', '$uibModal', '$location', '$anchorScroll',
    function ($scope, $rootScope, $http, $interval, $routeParams, APP_CONFIG, adalService, $uibModal, $location, $anchorScroll) {

        // an object to store the state of an entity between refreshes.
        var entityState = {};

        function hasState(id) {
            return entityState.hasOwnProperty(id);
        }

        function getEntityState(id) {
            if (!hasState(id)) {
                entityState[id] = {}
            }
            return entityState[id];
        }

        function clearState(id) {
            delete entityState[id];
        }

        $scope.id = $routeParams.id;
        var role = $scope.userInfo.profile.roles ? $scope.userInfo.profile.roles[0] : 'Viewer';
        $scope.operator = role === "Operator" || role === "Administrator";
        $scope.admin = role === "Administrator";

        refreshView();
        $scope.refreshViewCount = 1;
        $scope.$on("refresh-start", function () {
            refreshView();
        });

        function refreshView() {
            $scope.refreshViewCount = $scope.refreshViewCount + 1;

            // $('#loader').show();
            var url = APP_CONFIG.apiUrl + "/Customers/" + $scope.id;

            $http.get(url).success(function (data) {
                $scope.customer = data;
                updateData($scope.customer);
                $scope.customerLastRefreshTime = new Date();
            }).error(function (data) {
                $scope.customerError = JSON.stringify(data);
            }).finally(function () {
                $('#loader').hide();
                $scope.$emit("refresh-end", { refreshViewCount: $scope.refreshViewCount });
            });
        }

        function updateData(customer) {
            var date = new Date();
            $(document).prop('title', customer.Name);
            customer.Accounts.forEach(function (account) {
                getVodOrigins(account);
                var runningChannels = account.Channels.filter(function (c) {
                    return c.EncodingType !== 'None' && c.State === 'Running';
                }).forEach(function (c) {
                    loadThumbnails(c);
                });
            });
        }

        function loadThumbnails(channel) {
            var state = getEntityState(channel.Id);
            $http.get(channel.ThumbnailUrl, {
                responseType: "arraybuffer"
            }).success(function (response) {
                var blob = new Blob([response], { type: "image/jpeg" });
                state.Thumbnail = URL.createObjectURL(blob);
            }).error(function (data) {
                state.Thumbnail = '/Content/noimage.png';
            });
        }

        function getOrigin(account, id) {
            var origin;
            account.Origins.forEach(function(o) {
                if (o.Id === id) {
                    origin = o;
                }
            });
            return origin;
        }

        function getVodOrigins(account) {
            account.vodOrigins = account.VodOrigins.map(function (id) {
                return getOrigin(account, id);
            });
        }
        $scope.isTrue = function (challenge) {
            if (Boolean(challenge)) {
                return "defaultDiff";
            } else {
                return "";
            }
        }
        $scope.getThumbnail = function (channel) {
            if (channel.EncodingType !== "None" && channel.State === "Running") {
                if (hasState(channel.Id)) {
                    return getEntityState(channel.Id).Thumbnail;
                } else {
                    var date = new Date();
                    return channel.ThumbnailUrl + "?cb=" + date.getTime();
                }
            }
            return "/Content/screen.png";
        }

        $scope.getOrigin = function (account, channel) {
            return getOrigin(account, channel.OriginId);
        };

        $scope.showProgramInfo = function (program, channel, account, originId) {
            var params = {
                channel: channel,
                program: program,
                account: account,
                origin: getOrigin(account, originId)
            };
            showOverlay(params, "templates/ProgramDetails.html", "programController");
        };

        $scope.showChannelDetails = function (account, channel) {
            var params = {
                account: account,
                channel: channel,
                origin: getOrigin(account, channel.OriginId)
            };
            showOverlay(params, "Templates/channelDetails.html", "channelController");
        };

        $scope.popupPreview = function (url) {
            var params = {
                playbackUrl: url,
                type: "application/vnd.ms-sstr+xml"
            };
            showOverlay(params, "templates/player.html", "playerController");
        };

        $scope.showProgramPreview = function (account, channel, program) {
            var params = {
                params: {
                    originHostName: channel.OriginHostName 
                }
            };
            $http.get(getProgramUrl(account, program) + "/Urls", params).success(function (urls) {
                $scope.popupPreview(urls ? urls.SmoothStreamUrl : "No locators present!");
            });
        }

        $scope.hasPreview = function (c) {
            return c.State === "Running"
                && channel.EncodingType !== "None"
                && hasState(c.Id)
                && getEntityState(c.Id).player;
        };

        $scope.startChannelPreview = function (channel, $event) {
            if (channel.State === "Running" && channel.EncodingType !== "None") {
                var state = getEntityState(channel.Id);
                state.player = startPreview(channel.PreviewUrl, "application/vnd.ms-sstr+xml", $event);
            }
        }

        $scope.startProgramPreview = function (account, channel, program, $event) {
            if (program.State === "Running") {
                $http.get(getProgramUrl(account, program) + "/Urls",
                    {
                        params: { originHostName: channel.OriginHostName}
                    } ).success(function (urls) {
                    if (urls && urls.SmoothStreamUrl) {
                        var state = getEntityState(program.Id);
                        state.player = startPreview(urls.SmoothStreamUrl, "application/vnd.ms-sstr+xml", $event);
                    }
                });
            }
        }

        function startPreview(url, type, $event) {
            var parent = $($event.currentTarget).parent();
            $event.stopPropagation();
            var video = $("<video width='100%' height='100%' class='azuremediaplayer amp-default-skin' ng-click='endPreview(c)'></video>");
            parent.append(video);
            var player = amp(
                video.get(0),
                { /* Options */
                    heuristicProfile: "HighQuality",
                    nativeControlsForTouch: false,
                    logo: { enabled: true },
                    autoplay: true,
                    controls: false,
                },
                function () {
                    this.play();
                }
            );
            player.src([{
                src: url,
                type: type,
                disableUrlRewriter: !url.startsWith("https")
            }]);
            return player;
        }

        $scope.endPreview = function (source, $event) {
            var state = getEntityState(source.Id);
            if (state.player) {
                state.player.dispose();
                delete state.player;
            }
            $($event.currentTarget).remove(".azuremediaplayer");
        }

        $scope.scrollTo = function (id) {
            $anchorScroll(id);
        }

        $scope.showIngestMetrics = function (account, channel) {
            var params = {
                account: account,
                channel: channel
            };
            showOverlay(params, "templates/ingestTelemetry.html", "ingestController");
        }

        $scope.showChannelOriginMetrics = function (account, channel) {
            var origin = getOrigin(account, channel.OriginId);
            showOriginMetrics(account, origin);
        }

        $scope.showOriginMetrics = function (account, origin) {
            showOriginMetrics(account, origin);
        }

        function showOriginMetrics(account, origin) {
            var params = {
                account: account,
                origin: origin
            };
            showOverlay(params, "templates/originTelemetry.html", "originTelemetryController");
        }

        $scope.showArchiveMetrics = function (account, channel) {
            var params = {
                account: account,
                channel: channel
            };
            showOverlay(params, "templates/archiveTelemetry.html", "archiveController");
        }

        $scope.showEncodingMetrics = function (account, channel) {
            var params = {
                account: account,
                channel: channel
            };
            showOverlay(params, "templates/ChannelTelemetry.html", "channelTelemetryController");
        }

        $scope.showAlerts = function (account, channel) {
            var params = {
                account: account,
                channel: channel
            };
            showOverlay(params, "templates/ChannelAlerts.html", "channelAlertsController");
        }

        $scope.showOriginAlerts = function (account, origin) {
            var params = {
                account: account,
                origin: origin
            };
            showOverlay(params, "templates/originAlerts.html", "originAlertsController");
        }

        $scope.showOriginInfo = function (origin, account) {
            var params = {
                origin: origin,
                account: account
            }
            showOverlay(params, "templates/originDetails.html", "originController")
        }

        $scope.startChannel = function (account, channel) {
            $http.post(getChannelUrl(account, channel) + "/Start")
            .success(function () {
                alert("Starting channel!");
            }).error(function (data) {
                alert("Failed to start channel!" + data.Message);
            });
        }

        $scope.resetChannel = function (account, channel) {
            $http.post(getChannelUrl(account, channel) + "/Reset")
            .success(function(operation) {
                $rootScope.$emit("operation", operation, "Channel start");
                alert("Resetting channel!");
            }).error(function (data) {
                alert("Failed to reset channel!" + data.Message);
            });
        }

        $scope.stopChannel = function (account, channel) {
            $http.post(getChannelUrl(account, channel) + "/Stop")
            .success(function(operation) {
                $rootScope.$emit("operation", operation, "Channel stop");
                alert("Stopping channel!");
            }).error(function (data) {
                alert("Failed to stop channel!" + data.Message);
            });
        }

        $scope.startProgram = function (account, program) {
            $http.post(getProgramUrl(account, program) + "/Start")
            .success(function(operation) {
                $rootScope.$emit("operation", operation, "Program start");
                alert("Starting Program!");
            }).error(function (data) {
                alert("Failed to start Program!" + data.Message);
            });
        }

        $scope.stopProgram = function (account, program) {
            $http.post(getProgramUrl(account, program) + "/Stop")
            .success(function(operation) {
                $rootScope.$emit("operation", operation, "Program stop");
                alert("Stopping program!");
            }).error(function (data) {
                alert("Failed to stop program!" + data.Message);
            });
        }

        $scope.startOrigin = function (account, origin) {
            $http.post(getOriginUrl(account, origin) + "/Start")
            .success(function(operation) {
                $rootScope.$emit("operation", operation, "Origin start");
                alert("Starting origin!");
            }).error(function (data) {
                alert("Failed to start origin!" + data.Message);
            });
        }

        $scope.stopOrigin = function (account, origin) {
            $http.post(getOriginUrl(account, origin) + "/Stop")
            .success(function(operation) {
                $rootScope.$emit("operation", operation, "Origin stop");
                alert("Stopping origin!");
            }).error(function (data) {
                alert("Failed to stop origin!" + data.Message);
            });
        }

        $scope.startChannelOrigin = function (account, channel) {
            var origin = $scope.getOrigin(account, channel);
            if (!!origin) {
                $scope.startOrigin(account, origin);
            }
        }

        $scope.stopChannelOrigin = function (account, channel) {
            var origin = $scope.getOrigin(account, channel);
            if (!!origin) {
                $scope.stopOrigin(account, origin);
            }
        }

        $scope.insertSlate = function (account, channel) {
            var params = {
                account: account,
                channel: channel
            };
            showOverlay(params, "templates/channelSlate.html", "slateController");
        };

        $scope.createChannel = function (account) {
            showOverlay(account, "templates/channelCreation.html", "channelCreationController");
        };

        $scope.createOrigin = function (account) {
            showOverlay(account, "templates/originCreation.html", "originCreationController");
        }

        $scope.createProgram = function (account, channel) {
            var params = {
                account: account,
                channel: channel
            };
            showOverlay(params, "templates/programCreation.html", "programCreationController");
        }

        $scope.updateChannel = function (account, channel) {
            var params = {
                account: account,
                channel: channel
            };
            showOverlay(params, "templates/channelUpdate.html", "channelUpdateController");
        }

        $scope.updateOrigin = function (account, origin) {
            var params = {
                account: account,
                origin: origin
            };
            showOverlay(params, "templates/originUpdate.html", "originUpdateController");
        }

        $scope.updateProgram = function (account, channel, program) {
            var params = {
                account: account,
                channel: channel,
                program: program
            };
            showOverlay(params, "templates/programUpdate.html", "programUpdateController");
        }

        $scope.deleteChannel = function (account, channel) {
            var params = {
                account: account,
                channel: channel
            };
            showOverlay(params, "templates/channelDelete.html", "channelDeleteController", "sm");
        }

        $scope.deleteOrigin = function (account, origin) {
            var params = {
                account: account,
                origin: origin
            };
            showOverlay(params, "templates/originDelete.html", "originDeleteController", "sm");
        }

        $scope.deleteProgram = function (account, channel, program) {
            var params = {
                account: account,
                channel: channel,
                program: program
            };
            showOverlay(params, "templates/programDelete.html", "programDeleteController", "sm");
        }

        function getChannelUrl(account, channel) {
            return APP_CONFIG.apiUrl + "/Accounts/" + account.Name + "/Channels/" + channel.Id;
        }

        function getOriginUrl(account, origin) {
            return APP_CONFIG.apiUrl + "/Accounts/" + account.Name + "/Origins/" + origin.Id;
        }

        function getProgramUrl(account, program) {
            return APP_CONFIG.apiUrl + "/Accounts/" + account.Name + "/Programs/" + program.Id;
        }

        function showOverlay(params, template, controller, size, windowClass) {
            $uibModal.open({
                templateUrl: template,
                controller: controller,
                backdrop: "static",
                size: size || "lg",
                windowClass: windowClass,
                resolve: {
                    params: function () {
                        return params;
                    }
                }
            });
        }
    }]);
