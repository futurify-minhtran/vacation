(function ()
{
    'use strict';

    angular
        .module('app.admin')
        .controller('UsersController', UsersController);

    /** @ngInject */
    function UsersController($scope, UserService) {

        $scope.users = [];

        $scope.clearMessage = function () {
            $scope.success = '';
        }

        UserService.GetUsers().then(function (data) {
            $scope.users = data;
        });

        $scope.clearForm = function () {
            $scope.user = {
                FirstName: null,
                LastName: null,
                position: null,
                gender: null,
                phoneNumber: null,
                email: null,
                password: null
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
                if (data.Error) {
                    $scope.error = data.Error;
                } else {
                    $scope.user = data.User;
                    $scope.users.unshift(data.User);
                    $scope.success = "Add user success!";
                    $('#addUserModal').modal('hide');
                }
            });
        };

        $scope.deleteUser = function (user, index) {
            UserService.Delete(user.Id).then(function () {
                $scope.users.splice(index, 1);
            })
        }

        $scope.detailUser = function (user) {
            // UserService.Detail(user.Id).then(function (data) {
            $scope.user = user;
            //  })
        }

        $scope.setStatusUser = function (user, status, index) {
            UserService.SetStatus(user.Id, status).then(function (settedSatusUser) {
                $scope.users[index] = settedSatusUser;
            });
        }

        $scope.resetPasswordUser = function () {
            UserService.ResetPassword($scope.user).then();
        }
    }
})();