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
            InitNewUser: InitNewUser,
            GetRemaingDaysOff: GetRemaingDaysOff
        };

        return service;

        function Create(email,model) {
            var deferer = $q.defer();

            $http.post(baseUrl + '/api/vacation/booking/' + email, model)
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

            $http.get(baseUrl + '/api/vacation/booking/user/' + userId)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function Delete(id,email) {
            var deferer = $q.defer();

            $http.delete(baseUrl + '/api/vacation/booking/' + id + '/' + email)
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

            $http.get(baseUrl + '/api/vacation/get-booking/' + userId + '/' + year)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function CheckNewUser(userId) {
            var deferer = $q.defer();

            $http.get(baseUrl + '/api/vacation/check-new-user/' + userId)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function InitNewUser(vacationDay) {
            var deferer = $q.defer();

            $http.post(baseUrl + '/api/vacation/init-new-user', vacationDay)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function GetRemaingDaysOff(userId, year) {
            var deferer = $q.defer();

            $http.get(baseUrl + '/api/vacation/get-remaining-days-off/' + userId + '/' + year)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }
    }
})();