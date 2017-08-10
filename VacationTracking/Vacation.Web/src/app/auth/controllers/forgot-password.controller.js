(function ()
{
    'use strict';

    angular
        .module('app.auth')
        .controller('ForgotPasswordController', ForgotPasswordController);

    /** @ngInject */
    function ForgotPasswordController($scope, $location, AuthenticationService, $sce) {
        $scope.requestResetPassword = function () {
            $scope.error = null;
            $scope.success = null;

            AuthenticationService.RequestResetPassword($scope.email).then(function (data) {
                if (data.Error) {
                    $scope.error = data.Error;
                } else {
                    $scope.success = 'Success';
                }
                //$location.path('/user');
            });
        }
    }
})();

