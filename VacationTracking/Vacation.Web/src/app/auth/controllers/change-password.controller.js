(function () {
    'use strict';

    angular
        .module('app.auth')
        .controller('ChangePasswordController', ChangePasswordController);

    /** @ngInject */
    function ChangePasswordController($scope, AuthenticationService, $state, $rootScope, $timeout) {
        $scope.loading = {
            change: false
        };

        $scope.changePassword = function () {
            $scope.success = null;
            $scope.error = null;
            $scope.loading.change = true;

            AuthenticationService.ChangePasswordAsync($scope.user).then(function (response) {
                $scope.loading.change = false;

                if (!response.error) {
                    $scope.success = "Password was changed! Loging out...";
                    $timeout(function () {
                        $state.go('app.auth_logout');
                    }, 2000);
                }
                else {
                    $scope.error = response.error;

                }
            });
        };
    }
})();