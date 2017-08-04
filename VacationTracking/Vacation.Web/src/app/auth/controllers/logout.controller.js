(function () {
    'use strict';

    angular
        .module('app.auth')
        .controller('LogoutController', LogoutController);

    /** @ngInject */
    function LogoutController(AuthenticationService, $state) {
        function logout() {
            AuthenticationService.SignOut();
            $state.go('app.auth_login');
        };
        logout();
    }
})();