myApp.factory('AuthenticationService', AuthenticationService);

function AuthenticationService($http, $q) {

    var authServer = 'http://localhost:58283';

    var service = {
        RequestResetPassword: RequestResetPassword,
        ResetPassword: ResetPassword
    };

    return service;

    function RequestResetPassword(email) {
        var deferer = $q.defer();

        $http.get(authServer + '/api/account/reset-password?email=' + email)
            .then(function (response) {
                deferer.resolve(response.data);
            }, function (responseErrors) {
                deferer.reject(responseErrors.data);
            });

        return deferer.promise;
    }

    function ResetPassword(email, token, newPassword) {
        var deferer = $q.defer();

        var model = {
            Email: email,
            Token: token,
            NewPassword: newPassword
        };

        $http.put(authServer + '/api/account/reset-password', model)
            .then(function (response) {
                deferer.resolve(response.data);
            }, function (responseErrors) {
                deferer.reject(responseErrors.data);
            });

        return deferer.promise;
    }


    //function GetUsers() {
    //    var deferer = $q.defer();

    //    $http.get(authServer + '/api/account/me')
    //        .then(function (response) {
    //            deferer.resolve(response.data)
    //        }, function (responseErrors) {
    //            deferer.reject(responseErrors.data)
    //        });

    //    return deferer.promise;
    //}

    //function Create(model) {
    //    var deferer = $q.defer();

    //    $http.post(authServer + '/api/account/register', model)
    //        .then(function (response) {
    //            deferer.resolve(response.data);
    //        }, function (responseErrors) {
    //            deferer.reject(responseErrors.data);
    //        });

    //    return deferer.promise;
    //}

    //function Update(id,model) {
    //    var derferer = $q.defer();

    //    $http.put(authServer + 'api/account/' + id)
    //        .then(function (response) {
    //            deferer.resolve(response.data);
    //        }, function (responseErrors) {
    //            deferer.reject(responseErrors.data);
    //        });
    //} 

    //function Delete(id) {
    //    var deferer = $q.defer();

    //    $http.delete(authServer + '/api/account/' + id)
    //        .then(function (response) {
    //            deferer.resolve(response.data);
    //        }, function (responseErrors) {
    //            deferer.reject(responseErrors.data);
    //        });

    //    return deferer.promise;
    //}

    //function Detail(id) {
    //    var deferer = $q.defer();

    //    $http.get(authServer + '/api/account/' + id)
    //        .then(function (response) {
    //            deferer.resolve(response.data);
    //        }, function (responseErrors) {
    //            deferer.reject(responseErrors.data);
    //        });

    //    return deferer.promise;
    //}

    //function SetStatus(id, status) {
    //    var deferer = $q.defer();

    //    $http.get(authServer + '/api/account/' + id + '/' + status)
    //        .then(function (response) {
    //            deferer.resolve(response.data);
    //        }, function (responseErrors) {
    //            deferer.reject(responseErrors.data);
    //        });

    //    return deferer.promise;
    //}
};