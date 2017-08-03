(function () {
    'use strict';

    angular
        .module('app.auth')
        .factory('AuthenticationService', AuthenticationService)

    /** @ngInject **/
    function AuthenticationService($http, $q, $cookies, $rootScope) {

        var authServer = 'http://localhost:58283';

        var service = {
            IsAuthenticated: false,
            AccessToken: null,
            Account: null,
            SignInAsync: SignInAsync,
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

        function SignInAsync(email, password, remember) {
            var deferer = $q.defer();

            $http.post(
                authServer + '/token',
                { Email: email, Password: password },
                {
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded'
                    },
                    noBigBox: true,
                    transformRequest: function (obj) {
                        var str = [];
                        for (var p in obj)
                            str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
                        return str.join("&");
                    }
                }).then(function (response) {
                    _authenticate(response.data, remember);
                    _syncPermissions().then(function () {
                        deferer.resolve();
                    })
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                })
            return deferer.promise;
        }

        function _authenticate(signInResponse, remember) {
            var cookieOptions = {
                path: '/'
            };

            if (remember) {
                cookieOptions.expires = new Date(signInResponse.Expires);
            }

            $cookies.put('access_token', signInResponse.AccessToken, cookieOptions)
            service.IsAuthenticated = true;
            service.AccessToken = signInResponse.AccessToken;
            service.Account = signInResponse.Account;
        }

        function _syncPermissions() {
            if (service.IsAuthenticated) {
                return $http.get(authServer + '/api/account/me/permissions', { hideAjaxLoader: true })
                    .then(function (permissions) {
                        service.Permissions = permissions.data;
                        $rootScope.$emit('PERMISSIONS_LOADED');

                        //if (service.Permissions.indexOf('RECRUITER') != -1) {
                        //    $http.get(SVCS.Profile + '/api/recruiters/me').then(function (profile) {
                        //        service.Profile = profile.data;
                        //        var deferer = loadCurrentProfilePromises.pop();
                        //        while (deferer) {
                        //            deferer.resolve(service.Profile);
                        //            deferer = loadCurrentProfilePromises.pop();
                        //        }
                        //    })
                        //}
                    })
            } else {
                var deferer = $q.defer();
                deferer.reject();
                return deferer.promise;
            }
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
})();