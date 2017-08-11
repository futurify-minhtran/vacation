(function ()
{
    'use strict';

    angular
        .module('app.auth')
        .controller('ForgotPasswordController', ForgotPasswordController);

    /** @ngInject */
    function ForgotPasswordController($scope, AuthenticationService) {
        $scope.loading = {
            request: false
        };

        $scope.requestResetPassword = function () {
            $scope.error = null;
            $scope.success = null;

            $scope.loading.request = true;
            AuthenticationService.RequestResetPassword($scope.email).then(function (data) {
                $scope.loading.request = false;
                if (!data.error) {
                    $scope.success = 'Please check your email';
                    $scope.email = '';
                    //$location.path('/user');
                } else {
                    $scope.error = data.error;
                }
            });
        }
    }
})();

