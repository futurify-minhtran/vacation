myApp.controller('ForgotPasswordController', ForgotPasswordController);

function ForgotPasswordController($scope, $location, AuthenticationService,$sce) {
    $scope.requestResetPassword = function () {
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