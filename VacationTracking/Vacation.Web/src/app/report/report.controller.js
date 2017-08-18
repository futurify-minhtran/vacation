(function ()
{
    'use strict';

    angular
        .module('app.report')
        .controller('ReportController', ReportController);

    /** @ngInject */
    function ReportController($scope, $rootScope, $state, ReportService, UserService, BookingService) {
        $scope.bookings = null;
        $scope.totalHours = null;
        $scope.remainingDaysOff = null;

        UserService.GetUsersPaging(1000, 1, '', 'Id', 'asc').then(function (data) {
            $scope.users = data;
        });
        $scope.year = (new Date()).getFullYear();
        $scope.months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

        $scope.hasChange = function () {
            // Get all user
            if (!$scope.user) {
                // Get all user & all month
                if (!$scope.month) {
                    ReportService.GetAll().then(function (data) {
                        $scope.bookings = data.Bookings;
                        $scope.totalHours = data.TotalHours;
                    });
                }
                // Get all user & by month
                else {
                    ReportService.GetAllByMonth($scope.year, $scope.month).then(function (data) {
                        $scope.bookings = data.Bookings;
                        $scope.totalHours = data.TotalHours;
                    });
                }
                $scope.remainingDaysOff = null;
            }
            // Get by userId
            else {
                // Get by userId & all month
                if (!$scope.month) {
                    ReportService.GetAllByUserId($scope.user).then(function (data) {
                        $scope.bookings = data.Bookings;
                        $scope.totalHours = data.TotalHours;
                    });
                }

                // Get by userId & by month
                else {
                    ReportService.GetAllByUserIdWithMonth($scope.user,$scope.year,$scope.month).then(function (data) {
                        $scope.bookings = data.Bookings;
                        $scope.totalHours = data.TotalHours;
                    });
                }
                BookingService.GetRemaingDaysOff($scope.user, $scope.year).then(function (data) {
                    $scope.remainingDaysOff = data * 8;
                });
            }
        }

        $scope.hasChange();
    }

})();