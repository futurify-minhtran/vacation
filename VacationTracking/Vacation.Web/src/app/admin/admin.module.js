(function ()
{
    'use strict';

    angular
        .module('app.admin', ['ngCookies', 'ui.router', 'ngAnimate', 'ngSanitize', 'ui.bootstrap'])
        .config(config)

    /** @ngInject */
    function config($stateProvider)
    {
        // State
        $stateProvider
            .state('app.admin_user', {
                url: '/admin',
                views: {
                    'main@': {
                        templateUrl: 'app/core/layouts/layout.html',
                        controller: 'IndexController as vm'
                    },
                    'content@app.admin_user': {
                        templateUrl: 'app/admin/user.html',
                        controller: 'UsersController as vm'
                    }
                },
                protect: true,
                permissions: ['USER']
            })
    }
})();
//bodyClass: 'admin',