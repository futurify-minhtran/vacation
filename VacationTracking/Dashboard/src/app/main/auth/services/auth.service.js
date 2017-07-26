(function () {
    'use strict';

    angular
        .module('app.auth')
        .factory('AuthenticationService', AuthenticationService)
        .factory('HttpBearerTokenAuthorizationInterceptor', ['$cookie', function ($cookie) {
            return {
                'request': function (config) {
                    var accessToken = $cookies.get('access_token');
                    if (config.params && config.params.isExternalRequest) {
                        delete (config.params.isExternalRequest)
                    } else if (accessToken) {
                        config.headers.Authorization = 'bearer ' + accessToken;
                    }
                    return config;
                }
            }
        }])
        .config(['$httpProvider', function ($httpProvider) {
            $httpProvider.interceptors.push('HttpBearerTokenAuthorizationInterceptor');
        }]);


    auth.$inject = ['$http'];

    function auth($http) {
        var service = {
            getData: getData
        };

        return service;

        function getData() { }
    }
})();