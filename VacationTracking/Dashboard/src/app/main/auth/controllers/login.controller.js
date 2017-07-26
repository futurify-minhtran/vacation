(function () {
    'use strict';

    angular
        .module('app.auth')
        .controller('LoginController', LoginController);

    function LoginController($state, AuthenticationService, $timeout) {
        var ctrl = this;
        ctrl.form = {
            Email: null,
            Password: null,
            Remember: false
        };

        ctrl.error = null;
        ctrl.loggingIn = false;
        ctrl.login = function () {
            ctrl.error = null;
            ctrl.loggingIn = true;
            AuthenticationService.SignInAsync(ctrl.form.Email, ctrl.form.Password, ctrl.form.Remember).then(function () {
                ctrl.loggingIn = false;

            });
        };

        var vm = this;
        vm.title = 'login';

        activate();

        function activate() { }
    }
})();
