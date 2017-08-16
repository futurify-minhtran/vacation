(function () {
    'use strict';

    angular
        .module('app.auth')
        .factory('AuthenticationService', AuthenticationService)
        .factory('HttpBearerTokenAuthorizationInterceptor', ['$cookies', function ($cookies) {
            return {
                'request': function (config) {
                    var accessToken = $cookies.get('access_token');
                    if (config.params && config.params.isExternalRequest) {
                        delete(config.params.isExternalRequest);
                    } else if (accessToken) {
                        config.headers.Authorization = 'bearer ' + accessToken;
                    }
                    return config;
                }
            };
        }])
        .config(['$httpProvider', function ($httpProvider) {
            $httpProvider.interceptors.push('HttpBearerTokenAuthorizationInterceptor');
        }]);

    /** @ngInject **/
    function AuthenticationService($http, $q, $cookies, $rootScope, $interval) {

        var baseUrl = 'http://localhost:58283';

        var loadCurrentPromises = [];
        var loadCurrentProfilePromises = [];

        var service = {
            IsAuthenticated: false,
            AccessToken: null,
            Account: null,
            GetCurrentAsync: function () {
                var deferer = $q.defer();
                if (service.Account == null) {
                    loadCurrentPromises.push(deferer);
                } else {
                    deferer.resolve(service.Account);
                }
                return deferer.promise;
            },
            Permissions: [],
            SignInAsync: SignInAsync,
            SignOut: SignOut,
            HasPermissions: HasPermissions,
            ChangePasswordAsync: ChangePasswordAsync,
            GetProfileAsync: function () {
                var deferer = $q.defer;

                if (service.Profile == null) {
                    loadCurrentProfilePromises.push(deferer);
                } else {
                    deferer.resolve(service.Profile);
                }

                return deferer.promise;
            },
            RequestResetPassword: RequestResetPassword,
            ResetPassword: ResetPassword,
            Profile: null
        };

        $rootScope.$authService = service;

       

        function SignInAsync(email, password, remember) {
            var deferer = $q.defer();

            $http.post(
                baseUrl + '/token',
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
                    });
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });
            return deferer.promise;
        }

        function ChangePasswordAsync(model) {
            var deferer = $q.defer();

            $http.put(baseUrl + '/api/account/me/password', model)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function SignOut() {
            $cookies.remove('access_token');
            service.IsAuthenticated = false;
            service.AccessToken = null;
            service.Account = null;
            service.Profile = null;
            service.Permissions = [];
        }

        function RequestResetPassword(email) {
            var deferer = $q.defer();

            $http.get(baseUrl + '/api/account/reset-password?email=' + email)
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

            $http.put(baseUrl + '/api/account/reset-password', model)
                .then(function (response) {
                    deferer.resolve(response.data);
                }, function (responseErrors) {
                    deferer.reject(responseErrors.data);
                });

            return deferer.promise;
        }

        function HasPermissions(permissions) {
            if (!service.IsAuthenticated) {
                return false;
            }
            else if (permissions && permissions.length) {
                if (!service.Permissions.length) {
                    return false;
                }
                for (var i = 0; i < permissions.length; i++) {
                    if (service.Permissions.indexOf(permissions[i]) != -1)
                        return true;
                }
                return false;
            } else {
                return true;
            }
        }

        function _authenticate(signInResponse, remember) {
            var cookieOptions = {
                path: '/'
            };

            if (remember) {
                cookieOptions.expires = new Date(signInResponse.Expires);
            }

            $cookies.put('access_token', signInResponse.AccessToken, cookieOptions);
            service.IsAuthenticated = true;
            service.AccessToken = signInResponse.AccessToken;
            service.Account = signInResponse.Account;
        }

        // check
        var firstCheck = true;
        function _checkAuthentication() {
            service.AccessToken = $cookies.get('access_token');
            service.IsAuthenticated = service.AccessToken && firstCheck ? true : service.IsAuthenticated;
            if (service.AccessToken) {
                $http.get(baseUrl + '/api/account/is-me-authenticated', { hideAjaxLoader: true })
                    .then(function (authenticated) {
                        service.IsAuthenticated = authenticated.data;
                        if (service.IsAuthenticated) {
                            if (!service.Account) {
                                $http.get(baseUrl + '/api/account/me', { hideAjaxLoader: true })
                                    .then(function (account) {
                                        service.Account = account.data;
                                        var deferer = loadCurrentPromises.pop();
                                        while (deferer) {
                                            deferer.resolve(service.Account);
                                            deferer = loadCurrentPromises.pop();
                                        }
                                    });
                                _syncPermissions();
                            }
                        } else if (firstCheck) {
                            service.SignOut();
                            window.location.reload();
                        }
                        firstCheck = false;
                    });
            }
            
        }

        function _syncPermissions() {
            if (service.IsAuthenticated) {
                return $http.get(baseUrl + '/api/account/me/permissions', { hideAjaxLoader: true })
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
                    });
            } else {
                var deferer = $q.defer();
                deferer.reject();
                return deferer.promise;
            }
        }

        $interval(_checkAuthentication, 300000);
        _checkAuthentication();

        return service;
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
    }
})();