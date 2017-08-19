(function ()
{
    'use strict';

    angular
        .module('app.admin')
            .controller('UserProfileController', UserProfileController);

    /** @ngInject */
    function UserProfileController($scope, $rootScope, UserService) {
        $scope.genders = [
            { id: 0, name: 'Undefined' },
            { id: 1, name: 'Male' },
            { id: 2, name: 'Female' }
        ];

        $scope.positions = [
            { id: 0, name: 'Marketing' },
            { id: 1, name: 'Quality' },
            { id: 2, name: 'Development' },
            { id: 3, name: 'Sale' },
            { id: 4, name: 'Business Development' }
        ];

        $scope.departments = [
            { id: 0, name: 'Enterprise' },
            { id: 1, name: 'Staffing' },
            { id: 2, name: 'Outsource' },
            { id: 3, name: 'Product' }
        ];

        $scope.user = $rootScope.$authService.Account;
        $scope.user.DateOfBirth = new Date($scope.user.DateOfBirth);

        $scope.dateOptions = {
            formatYear: 'yy',
            maxDate: new Date(),
            minDate: new Date(1900, 1, 1),
            startingDay: 1
        };

        $scope.dateOfBirthPopup = {
            opened: false
        };

        $scope.openDateOfBirthPopup = function () {
            $scope.dateOfBirthPopup.opened = true;
        }

        $scope.updateUser = function () {
            _removeTime($scope.user.DateOfBirth);
            var model = angular.copy($scope.user);

            UserService.UpdateUser(model).then(function (data) {
                if (data.Error) {
                    $scope.error = data.Error;
                } else {
                    $scope.success = "Update user success!";
                }
            });
        }

        // Remove time in datetime
        var _removeTime = function (datetime) {
            datetime.setHours(0);
            datetime.setMinutes(0);
            datetime.setSeconds(0);
            datetime.setMilliseconds(0);
        };


        $scope.clearMessage = function () {
            $scope.error = '';
            $scope.success = '';
        }
    }
        
})();