(function ()
{
    'use strict';

    angular
        .module('app.admin')
        .controller('UsersController', UsersController);

    /** @ngInject */
    function UsersController($scope, UserService, $timeout) {

        $scope.loading = {
            create: false
        };

        $scope.editIndex = null;

        $scope.gender = [
            "Undefined",
            "Male",
            "Female"
        ];

        $scope.genders = [
            { id: 0, name: 'Undefined' },
            { id: 1, name: 'Male' },
            { id: 2, name: 'Female' }
        ];

        $scope.position = [
            "Staff"
        ];

        $scope.positions = [
            { id: 0, name: 'Staff' },
        ];

        $scope.users = [];

        $scope.clearMessage = function () {
            $scope.success = '';
        }

        // Paging & sort Users
        $scope.itemsPerPage = 10;
        $scope.totalItems = null;
        $scope.currentPageUsers = 1;
        $scope.filter = "";
        $scope.sort = "Id";
        $scope.sortType = "asc";

        $scope.getTotalItems = function () {
            UserService.CountAll($scope.filter).then(function (data) {
                $scope.totalItems = data;
            });
        }
        //$scope.getTotalItems();

        $scope.getUsersPaging = function (pageSize, page, filter, sort, sortType) {
            UserService.GetUsersPaging(pageSize, page, filter, sort, sortType).then(function (data) {
                $scope.usersPaging = data;
            });
        }

        $scope.clearFilter = function () {
            $scope.filter = "";
        }

        $scope.hasChange = function () {
            $scope.getUsersPaging($scope.itemsPerPage, $scope.currentPageUsers, $scope.filter, $scope.sort, $scope.sortType);
        }

        // delay search
        $scope.$watch('filter', function (tmpStr) {
            $timeout(function () {
                if (tmpStr === $scope.filter) {
                    $scope.getTotalItems();
                    $scope.hasChange();
                }
            }, 1000);
        });

        $scope.sortColumn = function (column) {
            if ($scope.sort == column) {
                $scope.sortType = $scope.sortType == 'desc' ? 'asc' : 'desc';
            }
            else {
                $scope.sortType = 'asc';
            }

            $scope.sort = column;

            $scope.hasChange();
        }

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
        }

        $scope.addUser = function () {
            $scope.loading.create = true;
            var model = angular.copy($scope.user);
            UserService.Create(model).then(function (data) {
                $scope.loading.create = false;
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
            if (confirm('Are you sure to delete No.' + (index + 1))) {
                UserService.Delete(user.Id).then(function () {
                    $scope.usersPaging.splice(index, 1);
                    $scope.totalItems--;
                })
            }
        }

        $scope.detailUser = function (user) {
            // UserService.Detail(user.Id).then(function (data) {
            $scope.user = user;
            //  })
        }

        $scope.editUser = function (user,index) {
            $scope.user = user;
            $scope.editIndex = index;
        }

        $scope.updateUser = function () {
            var model = angular.copy($scope.user);

            UserService.Update(model).then(function (data) {
                if (data.Error) {
                    $scope.error = data.Error;
                } else {
                    $scope.usersPaging[$scope.editIndex] = data.User
                    $scope.success = "Update user success!";
                    $('#editUserModal').modal('hide');
                }
            });
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