(function ()
{
    'use strict';

    angular
        .module('app.report')
        .factory('ReportService', ReportService);

    /** @ngInject */
    function ReportService($http, $q) {
        var baseUrl = 'http://localhost:49479';

        var service = {
            GetAll: GetAll,
            GetAllByUserId: GetAllByUserId,
            GetAllByMonth: GetAllByMonth,
            GetAllByUserIdWithMonth: GetAllByUserIdWithMonth
        }

        return service;

        function GetAll() {
            var deferer = $q.defer();

            $http.get(baseUrl + '/api/report')
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function GetAllByUserId(userId) {
            var deferer = $q.defer();

            $http.get(baseUrl + '/api/report/' + userId)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function GetAllByMonth(year,month) {
            var deferer = $q.defer();

            $http.get(baseUrl + '/api/report/' + year + '/' + month)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function GetAllByUserIdWithMonth(userId,year,month) {
            var deferer = $q.defer();

            $http.get(baseUrl + '/api/report/' + userId + '/' + year + '/' + month)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }
    }

})();