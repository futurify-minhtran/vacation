(function () {
    'use strict';

    angular
        .module('app.auth')
        .controller('ResetPasswordController', ResetPasswordController);

    /** @ngInject */
    function ResetPasswordController($scope, AuthenticationService, $stateParams, $timeout, $state) {

        var email = $stateParams.email;
        var token = $stateParams.token;

        $scope.resetPassword = function () {
            AuthenticationService.ResetPassword(email, token, $scope.newPassword).then(function (data) {
                if (data.Error) {
                    $scope.error = data.Error;
                    $scope.message = "You must request a new reset password! Redirectly to reset password page...";
                    $timeout(function () {
                        $state.go('app.auth_forgot-password');
                    }, 3000);
                }
                else {
                    $scope.message = "Password reset successfully! Redirectly to login page...";
                    $timeout(function () {
                        $state.go('app.auth_login');
                    }, 2000);
                }
            });

        };
    }
})();