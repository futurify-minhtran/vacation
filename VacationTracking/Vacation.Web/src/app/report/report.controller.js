(function ()
{
    'use strict';

    angular
        .module('app.report')
        .controller('ReportController', ReportController);

    /** @ngInject */
    function ReportController($scope, $rootScope, $state, ReportService, UserService, BookingService) {
        $scope.bookings = null;
        $scope.remainingDaysOffs = [];
        $scope.emails = [];

        UserService.GetUsersPaging(1000, 1, '', 'Id', 'asc').then(function (data) {
            $scope.users = data;

            for (var i = 0; i < data.length; i++) {
                var user = data[i];
                $scope.emails[user.Id] = user.Email;
                $scope.remainingDaysOffs[user.Id] = user.RemainingDaysOff;
            }

        });
        $scope.year = (new Date()).getFullYear();
        $scope.months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

        $scope.hasChange = function () {
            // Get all user
            if (!$scope.user) {
                // Get all user & all month
                if (!$scope.month) {
                    ReportService.GetAll().then(function (data) {
                        $scope.bookings = data;
                    });
                }
                // Get all user & by month
                else {
                    ReportService.GetAllByMonth($scope.year, $scope.month).then(function (data) {
                        $scope.bookings = data;
                    });
                }
            }
            // Get by userId
            else {
                // Get by userId & all month
                if (!$scope.month) {
                    ReportService.GetAllByUserId($scope.user).then(function (data) {
                        $scope.bookings = { data };
                    });
                }

                // Get by userId & by month
                else {
                    ReportService.GetAllByUserIdWithMonth($scope.user, $scope.year, $scope.month).then(function (data) {
                        $scope.bookings = { data };
                    });
                }
                BookingService.GetRemaingDaysOff($scope.user, $scope.year).then(function (data) {
                    $scope.remainingDaysOff = data * 8;
                });
            }
        };


        $scope.hasChange();
    }

})();