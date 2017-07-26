(function () {
    'use strict';

    angular.module('auth', [
        // Angular modules 
        'ngRoute'

        // Custom modules 

        // 3rd Party Modules

    ]);

    /** @ngInject */
    function config($stateProvider, $translatePartialLoaderProvider, msApiProvider, msNavigationServiceProvider) {
        // State
        $stateProvider
            .state('app.auth_login', {
                url: '/login',
                views: {
                    'main@': {
                        templateUrl: 'app/core/layouts/content-only.html',
                        controller: 'MainController as vm'
                    },

                    'content@app.auth_login': {
                        templateUrl: 'app/main/auth/views/login.html',
                        controller: 'LoginController as vm'
                    }
                },
                bodyClass: 'login'
                //resolve: {
                //    SampleData: function (msApi) {
                //        return msApi.resolve('sample@get');
                //    }
                //}
            });

        // Translation
        $translatePartialLoaderProvider.addPart('app/main/auth');

        //// Api
        //msApiProvider.register('sample', ['app/data/sample/sample.json']);

        //// Navigation
        //msNavigationServiceProvider.saveItem('fuse', {
        //    title: 'SAMPLE',
        //    group: true,
        //    weight: 1
        //});

        //msNavigationServiceProvider.saveItem('fuse.sample', {
        //    title: 'Sample',
        //    icon: 'icon-tile-four',
        //    state: 'app.sample',
        //    /*stateParams: {
        //        'param1': 'page'
        //     },*/
        //    translate: 'SAMPLE.SAMPLE_NAV',
        //    weight: 1
        //});
    }
})();