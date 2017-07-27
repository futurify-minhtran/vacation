myApp.factory('UserService', UserService);

function UserService($http, $q) {

    var authServer = 'http://localhost:58283';

    var service = {
        GetUsers: GetUsers,
        Create: Create,
        Delete: Delete
    };

    return service;

    function GetUsers() {
        var deferer = $q.defer();

        $http.get(authServer + '/api/account/me')
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
};