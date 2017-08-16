(function () {
    'use strict';

    angular
        .module('app.auth')
        .controller('LoginController', LoginController);

    /** @ngInject */
    function LoginController($scope, AuthenticationService, $state, $timeout) {

        if (AuthenticationService.IsAuthenticated) {
            $state.go('app.admin_user');
        }

        $scope.loginForm = {
            Email: null,
            Password: null,
            Remember: false
        };

        $scope.error = null;
        $scope.loggingIn = false;

        $scope.login = function () {
            $scope.error = null;
            $scope.loggingIn = true;
            AuthenticationService.SignInAsync($scope.loginForm.Email, $scope.loginForm.Password, $scope.loginForm.Remember).then(function () {
                $scope.loggingIn = false;
                $state.go('app.admin_user');

            }, function (error) {
                $timeout(function () {
                    $scope.loggingIn = false;
                    if (error && error.Code) {
                        switch (error.Code) {
                            case 'INCORRECT_LOGIN':
                                $scope.error = { incorrect: true };
                                break;
                            case 'ACCOUNT_INACTIVE':
                                $scope.error = { inactive: true };
                                break;
                            default:
                                $scope.error = { busy: true };
                                break;
                        }
                    } else {
                        $scope.error = { busy: true };
                    }
                }, 300);
            });
        };
    }
})();

// success
//if (AuthenticationService.Permissions.indexOf('JOBSEEKER') != -1 && AuthenticationService.Permissions.length == 1) {
//    ctrl.error = { denied: true };
//    AuthenticationService.SignOut();
//} else {
//    $state.go('app.dashboard')
//}