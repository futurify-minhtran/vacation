(function ()
{
    'use strict';

    angular
        .module('app.report')
        .controller('ReportController', ReportController);

    /** @ngInject */
    function ReportController($scope, $rootScope, $state, ReportService, UserService, BookingService) {
        UserService.GetUsersPaging(1000, 1, '', 'Id', 'asc').then(function (data) {
            $scope.users = data;
        });
        $scope.year = (new Date()).getFullYear();
        $scope.months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

        $scope.hasChange = function () {
            if (!$scope.user) {
                ReportService.GetAll().then(function (data) {
                    $scope.bookings = data;
                });
            }
            else {
                if (!$scope.month) {
                    ReportService.GetAllByUserId($scope.user).then(function (data) {
                        $scope.bookings = data;
                    });
                }
                else {
                    ReportService.GetAllByUserIdWithMonth($scope.user,$scope.year,$scope.month).then(function (data) {
                        $scope.bookings = data;
                    });
                }
                BookingService.GetRemaingDaysOff($scope.user, $scope.year).then(function (data) {
                    $scope.remainingDaysOff = data;
                });
            }
            
        }

        $scope.hasChange();
    }

})();