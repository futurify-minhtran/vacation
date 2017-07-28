myApp.controller('UsersController', UsersController);

function UsersController($scope, UserService) {

    $scope.users = [];

    $scope.clearMessage = function () {
        $scope.message = '';
    }
  
    UserService.GetUsers().then(function (data) {
        $scope.users = data;
    });

    $scope.clearForm = function () {
        $scope.user = {
            FirstName : null,
            LastName : null,
            position : null,
            gender : null,
            phoneNumber : null,
            email : null,
            password : null
        }
        $scope.UserForm.$setPristine();
        $scope.UserForm.$setUntouched();
        $scope.user = null;
        $scope.error = null;
        $scope.message = null;
    }

    $scope.addUser = function () {
        var model = angular.copy($scope.user);

        UserService.Create(model).then(function (data) {
            if (data.Error == true) {
                $scope.message = data.Message;
            } else {
                $scope.user = data.User;
                $scope.users.unshift(data.User);
                $scope.message = 'Success!';
                $('#myModal').modal('hide');
            }
            $scope.error = data.Error;
        });
    };

    $scope.deleteUser = function (user,index) {
        UserService.Delete(user.Id).then(function () {
            $scope.users.splice(index, 1);
        })
    }

    $scope.detailUser = function (user) {
        //debugger;
       // UserService.Detail(user.Id).then(function (data) {
        $scope.user = user;
      //  })
    }
    
}