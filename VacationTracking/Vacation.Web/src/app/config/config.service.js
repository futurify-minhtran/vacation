(function(){
    'use strict';

    angular.module('app.config')
        .factory('ConfigService', ConfigService);

    /** @ngInject */
    function ConfigService($http, $q) {
        var baseUrl = 'http://localhost:49479';

        var service = {
            Update: Update,
            Get: Get,
            GetAll: GetAll,
            SetStatus: SetStatus
        }

        return service;

        function Update(model) {
            var deferer = $q.defer();

            $http.put(baseUrl + "/api/vacation/update-vacation-config", model)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function Get(name) {
            var deferer = $q.defer();

            $http.get(baseUrl + "/api/vacation/get-vacation-config/" + name)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function GetAll() {
            var deferer = $q.defer();

            $http.get(baseUrl + "/api/vacation/get-vacation-config")
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function SetStatus(model) {
            var deferer = $q.defer();

            $http.put(baseUrl + '/api/vacation/vacation-config', model)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }
    }

})();