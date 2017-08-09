(function ()
{
    'use strict';

    angular
        .module('app.admin')
        .factory('UserService', UserService);

    /** @ngInject */
    function UserService($http, $q) {

        var authServer = 'http://localhost:58283';

        var service = {
            GetUsers: GetUsers,
            GetUsersPaging: GetUsersPaging,
            Create: Create,
            Update: Update,
            Delete: Delete,
            Detail: Detail,
            SetStatus: SetStatus
        };

        return service;

        function GetUsers(filter) {
            var deferer = $q.defer();

            $http.get(authServer + '/api/account/getAll?filter=' + filter)
                .then(function (response) {
                    deferer.resolve(response.data)
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data)
                });

            return deferer.promise;
        }

        function GetUsersPaging(pageSize,page, filter, sort, sortType) {
            var deferer = $q.defer();

            $http.get(authServer + '/api/account/getAll/paging/' + pageSize + '/' + page + '?filter=' + filter + '&sort=' + sort + '&sortType=' + sortType)
                .then(function (response) {
                    deferer.resolve(response.data)
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data)
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

        function Update(id, model) {
            var derferer = $q.defer();

            $http.put(authServer + 'api/account/' + id)
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