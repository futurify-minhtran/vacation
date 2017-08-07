(function () {
    'use strict';
    angular
        .module('vacationTracking', [
            // Core
            'app.core',
     
            // auth
            'app.auth',

            // admin
            'app.admin',

            // booking
            'app.booking',

            'app.stringcommon'
        ]);
})();