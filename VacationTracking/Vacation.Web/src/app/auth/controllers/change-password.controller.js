(function () {
    'use strict';

    angular
        .module('app.auth')
        .controller('ChangePasswordController', ChangePasswordController);

    /** @ngInject */
    function ChangePasswordController($scope, AuthenticationService, $state, $rootScope) {
        $scope.loading = {
            change: false
        }

        $scope.changePassword = {
            userId: $rootScope.$authService.Account.Id,
            oldPassword: $scope.oldPassword,
            newPassword: $scope.newPassword
        }

        $scope.changePassword = function () {
            $scope.loading.change = true;
            AuthenticationService.ChangePasswordAsync($scope.changePassword).then(function (data) {
                $scope.loading.change = false;
                if (data.Error) {
                    $scope.error = data.Error;
                }
                else {
                    $scope.message = "Password was changed! Loging out...";
                    $timeout(function () {
                        $state.go('app.auth_logout');
                    }, 2000);
                }
            });

        }
    }
})();