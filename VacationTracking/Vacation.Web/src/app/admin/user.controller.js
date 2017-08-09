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

        // Paging Users
        $scope.itemsPerPage = 5;
        $scope.totalItems = null;
        $scope.currentPageUsers = 1;

        UserService.GetUsers().then(function (data) {
            $scope.totalItems = data.length;
        });

        $scope.getUsersPaging = function (pageSize, page) {
            UserService.GetUsersPaging(pageSize, page).then(function (data) {
                $scope.usersPaging = data;
            });
        }

        $scope.getUsersPaging($scope.itemsPerPage, 1);
        // Paging Users

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
                    $scope.usersPaging.unshift(data.User);
                    $scope.totalItems++;
                    $scope.success = "Add user success!";
                    $('#addUserModal').modal('hide');
                }
            });
        };

        $scope.deleteUser = function (user, index) {
            UserService.Delete(user.Id).then(function () {
                $scope.usersPaging.splice(index, 1);
                $scope.totalItems--;
            })
        }

        $scope.detailUser = function (user) {
            // UserService.Detail(user.Id).then(function (data) {
            $scope.user = user;
            //  })
        }

        $scope.editUser = function (user) {
            $scope.user = user;
        }

        $scope.setStatusUser = function (user, status, index) {
            UserService.SetStatus(user.Id, status).then(function (settedSatusUser) {
                $scope.usersPaging[index] = settedSatusUser;
            });
        }

        $scope.resetPasswordUser = function () {
            UserService.ResetPassword($scope.user).then();
        }
    }
})();