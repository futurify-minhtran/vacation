﻿(function ()
{
    'use strict';

    angular
        .module('app.admin')
        .factory('UserService', UserService);

    /** @ngInject */
    function UserService($http, $q) {

        var authServer = 'http://localhost:58283';

        var service = {
            CountAll: CountAll,
            GetUsersPaging: GetUsersPaging,
            Create: Create,
            Update: Update,
            UpdateUser: UpdateUser,
            Delete: Delete,
            Detail: Detail,
            SetStatus: SetStatus
        };

        return service;

        function CountAll(filter) {
            var deferer = $q.defer();

            $http.get(authServer + '/api/account/count-all?filter=' + filter)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function GetUsersPaging(pageSize,page, filter, sort, sortType) {
            var deferer = $q.defer();

            $http.get(authServer + '/api/account/get-all/paging/' + pageSize + '/' + page + '?filter=' + filter + '&sort=' + sort + '&sortType=' + sortType)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function Create(model) {
            var deferer = $q.defer();

            $http.post(authServer + '/api/account/register', model)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function Update(model) {
            var deferer = $q.defer();

            $http.put(authServer + '/api/account/update-account', model)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function UpdateUser(model) {
            var deferer = $q.defer();

            $http.put(authServer + '/api/account/update-user', model)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function Delete(id) {
            var deferer = $q.defer();

            $http.delete(authServer + '/api/account/' + id)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function Detail(id) {
            var deferer = $q.defer();

            $http.get(authServer + '/api/account/' + id)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function SetStatus(id, status) {
            var deferer = $q.defer();

            $http.get(authServer + '/api/account/' + id + '/' + status)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }
    }
})();