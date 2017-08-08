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