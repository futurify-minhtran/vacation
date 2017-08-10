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
            GetAllByUserId: GetAllByUserId,
            Delete: Delete,
            GetVacationDay: GetVacationDay,
            GetBookingVacationDay: GetBookingVacationDay,
            CheckNewUser: CheckNewUser,
            InitNewUser: InitNewUser
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

        function Update(model) {
            var deferer = $q.defer();

            $http.put(baseUrl + '/api/vacation/booking', model)
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
        
        function GetAllByUserId(userId) {
            var deferer = $q.defer();

            $http.get(baseUrl + '/api/vacation/booking/userId/' + userId)
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

        function GetVacationDay(userId, year) {
            var deferer = $q.defer();

            $http.get(baseUrl + '/api/vacation/' + userId + '/' + year)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function GetBookingVacationDay(userId, year) {
            var deferer = $q.defer();

            $http.get(baseUrl + '/api/vacation/getbooking/' + userId + '/' + year)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function CheckNewUser(userId) {
            var deferer = $q.defer();

            $http.get(baseUrl + '/api/vacation/checknewuser/' + userId)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function InitNewUser(vacationDay) {
            var deferer = $q.defer();

            $http.post(baseUrl + '/api/vacation/initnewuser', vacationDay)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }
        
    }
})();