(function ()
{
    'use strict';

    angular
        .module('vacationTracking')
        .config(routeConfig);

    /** @ngInject */
    function routeConfig($stateProvider, $urlRouterProvider, $locationProvider)
    {
        $locationProvider.html5Mode(true);

        $urlRouterProvider.otherwise('/');

        // Inject $cookies
        var $cookies;
        angular.injector(['ngCookies']).invoke([
            '$cookies', function (_$cookies) {
                $cookies = _$cookies;
            }
        ]);

        // State definitions
        $stateProvider
            .state('app', {
                abstract: true,
                views: {
                    'main@': {
                        templateUrl: 'app/core/layouts/layout.html'
                    },
                    'content@app': {
                        templateUrl: 'app/admin/user.html',
                        controller: 'UsersController as vm'
                    }
                }
            });
    }
})();


//myApp.config(['$routeProvider',
//    function ($routeProvider) {
//        $routeProvider.
//            when('/user', {
//                templateUrl: '/app/user/user.html',
//                controller: 'UsersController'
//            }). 
//            when('/user/login', {
//                templateUrl: '/app/user/login.html',
//                controller: 'LoginController'
//            }).
//            when('/user/forgot-password', {
//                templateUrl: '/app/user/forgot-password.html',
//                controller: 'ForgotPasswordController'
//            }).  
//            when('/user/reset-password', {
//                templateUrl: '/app/user/reset-password.html',
//                controller: 'ResetPasswordController'
//            })

        
//    }
//]);