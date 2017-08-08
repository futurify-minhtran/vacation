(function ()
{
    'use strict';

    angular
        .module('vacationTracking')
        .run(run);

    /** @ngInject */
    function run($rootScope, $timeout, $state)
    {
        $state.go('app.admin_user');
        //// Activate loading indicator
        //var stateChangeStartEvent = $rootScope.$on('$stateChangeStart', function ()
        //{
        //    $rootScope.loadingProgress = true;
        //});

        //// De-activate loading indicator
        //var stateChangeSuccessEvent = $rootScope.$on('$stateChangeSuccess', function ()
        //{
        //    $timeout(function ()
        //    {
        //        $rootScope.loadingProgress = false;
        //    });
        //});

        //// Store state in the root scope for easy access
        //$rootScope.state = $state;

        //// Cleanup
        //$rootScope.$on('$destroy', function ()
        //{
        //    stateChangeStartEvent();
        //    stateChangeSuccessEvent();
        //});


        ////////////////////////////////////////////////////////
        //function run(SVCS, $http, AuthenticationService, $stateParams, $rootScope, $state, ProfileService) {
        //// ping all server to prevent them idle

        //$http.get(SVCS.Auth + '/ping').then(function () { }, function () { })
        //$http.get(SVCS.Profile + '/ping').then(function () { }, function () { })
        //$http.get(SVCS.RVListing + '/ping').then(function () { }, function () { })
        //$http.get(SVCS.Booking + '/ping').then(function () { }, function () { })
        //$http.get(SVCS.Notification + '/ping').then(function () { }, function () { })
        //$http.get(SVCS.Payment + '/ping').then(function () { }, function () { })
        //$http.get(SVCS.Calendar + '/ping').then(function () { }, function () { })
        //$http.get(SVCS.Scheduler + '/ping').then(function () { }, function () { })

        //if (AuthenticationService.IsAuthenticated) {
        //    if (window.location.pathname === '/') {
        //        AuthenticationService.GetProfileAsync().then(function (profile) {
        //            var lang = 'en';
        //            switch (profile.Language) {
        //                case 0:
        //                    lang = 'en';
        //                    break;
        //                case 1:
        //                    lang = 'fr';
        //                    break;
        //                default:
        //                    break;
        //            }

        //            if (lang != $stateParams.lang && !$state.current.abstract) {
        //                $state.go('.', { lang: lang });
        //            }
        //        })
        //    }
        //}

        //$rootScope.$on('AUTHENTICATED', function () {
        //    ProfileService.setTimeLogin().then(function (res) {
        //    }, function (error) {
        //    });
        //})
    }
})();