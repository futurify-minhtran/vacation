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

            'app.stringcommon'
        ]);
})();