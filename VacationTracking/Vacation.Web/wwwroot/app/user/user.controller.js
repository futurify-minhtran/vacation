myApp.controller('UsersController', UsersController);

function UsersController($scope, UserService) {

    $scope.users = [];

    UserService.GetUsers().then(function (data) {
        $scope.users = data;
    });

    $scope.addUser = function () {
        var modelUser = {
            FirstName: $scope.firstName,
            LastName: $scope.lastName,
            Position: $scope.position,
            Gender: $scope.gender,
            PhoneNumber: $scope.phoneNumber,
            Email: $scope.email,
            Password: $scope.password
        };

        UserService.Create(modelUser).then(function (data) {
            if (data.error == true) {
                $scope.message = data.message;
            } else {
                $scope.user = data.user;
                $scope.users.unshift(data.user);
            }
            $scope.error = data.error;
        });
    };

    $scope.deleteUser = function (user,index) {
        UserService.Delete(user.id).then(function () {
            $scope.users.splice(index, 1);
        })
    }

    
}

