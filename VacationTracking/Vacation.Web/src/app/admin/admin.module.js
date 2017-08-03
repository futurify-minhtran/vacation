(function ()
{
    'use strict';

    angular
        .module('app.admin', ['ngCookies', 'ui.router'])
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
                        templateUrl: 'app/admin/user.html',
                        controller: 'UsersController as vm'
                    }
                },
                bodyClass: 'admin'
            })
    }
})();