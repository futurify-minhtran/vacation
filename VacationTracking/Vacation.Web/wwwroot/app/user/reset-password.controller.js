myApp.controller('ResetPasswordController', ResetPasswordController);

function ResetPasswordController($scope, AuthenticationService, $routeParams) {

    var email = $routeParams.email;
    var token = $routeParams.token;

    $scope.resetPassword = function () {
        AuthenticationService.ResetPassword(email, token, $scope.newPassword);

        $scope.message = "Password reset successfully!";
    }
}