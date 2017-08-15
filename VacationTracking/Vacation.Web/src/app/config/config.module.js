(function ()
{
    'use strict';

    angular
        .module('app.config', ['ngCookies', 'ui.router'])
        .config(config);

    /** @ngInject */
    function config($stateProvider) {
        $stateProvider
            .state('app.config', {
                url: '/config',
                views: {
                    'main@': {
                        templateUrl: 'app/core/layouts/layout.html',
                        controller: 'IndexController as vm'
                    },
                    'content@app.config': {
                        templateUrl: 'app/config/views/config.html',
                        controller: 'ConfigController as vm'
                    }
                },
                protect: true,
                permissions: ['USER']
            });
    }

})();