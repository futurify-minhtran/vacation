(function ()
{
    'use strict';

    angular
        .module('app.auth', ['ngCookies', 'ui.router'])
        .config(config);

    /** @ngInject */
    function config($stateProvider)
    {
        // State
        $stateProvider
            .state('app.auth_login', {
                url: '/login',
                views: {
                    'main@': {
                        templateUrl: 'app/auth/views/login.html',
                        controller: 'LoginController as vm'
                    }
                },
                bodyClass: 'login'
            })
            .state('app.auth_forgot-password', {
                url: '/forgot-password',
                views: {
                    'main@': {
                        templateUrl: 'app/auth/views/forgot-password.html',
                        controller: 'ForgotPasswordController as vm'
                    }
                },
                bodyClass: 'forgot-password'
            })
            .state('app.auth_reset-password', {
                url: '/reset-password?email&token',
                views: {
                    'main@': {
                        templateUrl: 'app/auth/views/reset-password.html',
                        controller: 'ResetPasswordController as vm'
                    }
                },
                bodyClass: 'reset-password'
            })
            .state('app.auth_congratulation', {
                url: '/auth-congratulation',
                views: {
                    'main@': {
                        templateUrl: 'app/auth/views/congratulation.html',
                        controller: 'CongratulationController as vm'
                    }
                },
                bodyClass: 'lock'
            })
    }
})();