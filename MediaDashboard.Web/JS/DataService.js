'use strict';

angular.module('mediaApp.dataService', [])
    .factory('dataService', ['$http', function ($http) {

        var service = {
            getContentProviders: getContentProvidersAPI,
            getMessages: getMessagesAPI
        };

        return service;

        function getContentProvidersAPI() {
            $http.get('Data/Providers.json').success(function (data) {
                return data;
            });
        }

        function getMessagesAPI() {
            $http.get('Data/Messages.json').success(function (data) {
                return data;
            });
        }
    }]);