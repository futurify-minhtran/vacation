myApp.controller('ForgotPasswordController', ForgotPasswordController);

function ForgotPasswordController($scope, $location, AuthenticationService,$sce) {
    $scope.requestResetPassword = function () {
        AuthenticationService.RequestResetPassword($scope.email).then(function (data) {
            $scope.emailTemplate = $sce.trustAsHtml(data.EmailTemplate);
            $scope.error = data.Error;
            //$location.path('/user');
        });
    }
}