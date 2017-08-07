(function ()
{
    'use strict';

    angular
        .module('app.booking', ['ngCookies', 'ui.router', 'ui.bootstrap'])
        .config(config);

    /** @ngInject */
    function config($stateProvider) {

        // State
        $stateProvider
            .state('app.booking', {
                url: '/booking',
                views: {
                    'main@': {
                        templateUrl: 'app/booking/views/booking.html',
                        controller: 'BookingController as vm'
                    }
                },
                protect: true,
                permissions: ['USER']
            });
            //.state('app.booking_', {
            //    url: '/booking/',
            //    views: {
            //        'main@': {
            //            templateUrl: '',
            //            controller: 'Controller as vm'
            //        }
            //    }
            //})
    }

})();