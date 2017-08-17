(function ()
{
    'use strict';

    angular
        .module('app.report', ['ui.router'])
        .config(config);

    /** @ngInject */
    function config($stateProvider) {
        $stateProvider
            .state('app.report', {
                url: '/report',
                views: {
                    'main@': {
                        templateUrl: 'app/core/layouts/layout.html',
                        controller: 'IndexController as vm'
                    },
                    'content@app.report': {
                        templateUrl: 'app/report/views/report.html',
                        controller: 'ReportController as vm'
                    }
                },
                protect: true,
                permissions: ['ADMIN']
            })
            .state('app.report-remaining-day', {
                url: '/report-remaining-day',
                views: {
                    'main@': {
                        templateUrl: 'app/core/layouts/layout.html',
                        controller: 'IndexController as vm'
                    },
                    'content@app.report-remaining-day': {
                        templateUrl: 'app/report/views/report-remaining-day.html',
                        controller: 'ReportController as vm'
                    }
                },
                protect: true,
                permissions: ['ADMIN']
            });
    }
})();