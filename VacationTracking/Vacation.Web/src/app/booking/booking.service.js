(function () {
    'use strict';

    angular.module('app.booking')
        .factory('BookingService', BookingService);

    /** @ngInject */
    function BookingService($http,$q) {

        var baseUrl = 'http://localhost:49479';

        var service = {
            Create: Create,
            GetAll: GetAll,
            Delete: Delete
        }

        return service;

        function Create(model) {
            var deferer = $q.defer();

            $http.post(baseUrl + '/api/vacation/booking', model)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (reponseErrors) {
                    deferer.reject(reponseErrors.data);
                });

            return deferer.promise;
        }

        function GetAll() {
            var deferer = $q.defer();

            $http.get(baseUrl + '/api/vacation/booking')
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function Delete(id) {
            var deferer = $q.defer();

            $http.delete(baseUrl + '/api/vacation/booking/' + id)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }
    }
})();