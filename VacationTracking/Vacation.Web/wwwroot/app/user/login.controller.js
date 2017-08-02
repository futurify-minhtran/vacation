myApp.controller('LoginController', LoginController);

function LoginController($scope, AuthenticationService) {
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

            //if (AuthenticationService.Permissions.indexOf('JOBSEEKER') != -1 && AuthenticationService.Permissions.length == 1) {
            //    ctrl.error = { denied: true };
            //    AuthenticationService.SignOut();
            //} else {
            //    $state.go('app.dashboard')
            //}
        }, function (error) {
            $timeout(function () {
                $scope.loggingIn = false;
                if (error && error.Code) {
                    switch (error.Code) {
                        case 'INCORRECT_LOGIN':
                            $scope.error = { incorrect: true }
                            break;
                        default:
                            $scope.error = { busy: true }
                            break;
                    }
                } else {
                    $scope.error = { busy: true }
                }
            }, 300)
        });
    }
}