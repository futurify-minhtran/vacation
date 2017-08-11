(function () {
    'use strict';

    angular.module('app.booking')
        .controller('BookingController', BookingController);

    /** @ngInject */
    function BookingController($scope, BookingService, $state, $rootScope) {
        var userId = $rootScope.$authService.Account.Id;

        // Init vacation day default is 12
        BookingService.CheckNewUser(userId).then(function (data) {
            // New user, must be create a vacation day
            if (data == true) {
                var vacationDay = {
                    "UserId" : userId,
                    "Year" : 2017,
                    "TotalMonth" : 12
                };
                BookingService.InitNewUser(vacationDay);
            }
        });

        // End - Init vacation day default is 12

        $scope.year = 2017;

        BookingService.GetAllByUserId(userId).then(function (data) {
            $scope.bookings = data;
        });

        $scope.clearMessage = function () {
            $scope.error = ''
            $scope.success = '';
        }

        $scope.clearForm = function () {
            var startTime = new Date();
            startTime.setHours(8);
            startTime.setMinutes(0);

            var endTime = new Date();
            endTime.setHours(18);
            endTime.setMinutes(0);

            $scope.booking = {
                UserId: $rootScope.$authService.Account.Id,
                StartDate: new Date(),
                EndDate: new Date(),
                StartTime: startTime,
                EndTime: endTime,
                Reason: "",
                AllDay: false
            }
            $scope.BookingForm.$setPristine();
            $scope.BookingForm.$setUntouched();
            $scope.error = null;
            $scope.message = null;
        }
        $scope.addBooking = function () {
            var model = angular.copy($scope.booking);

            model.StartDate.setHours(model.StartTime.getHours());
            model.StartDate.setMinutes(model.StartTime.getMinutes());
            model.StartDate.setSeconds(0);
            model.StartDate.setMilliseconds(0);

            model.EndDate.setHours(model.EndTime.getHours());
            model.EndDate.setMinutes(model.EndTime.getMinutes());
            model.EndDate.setSeconds(0);
            model.EndDate.setMilliseconds(0);

            BookingService.Create(model).then(function (data) {
                if (data.Error) {
                    $scope.error = data.Error;
                } else {
                    //$scope.booking = data.Booking;
                    $scope.success = "Add booking success";
                    $('#addBookingModal').modal('hide');
                    $scope.bookings.unshift(data.Booking);

                    checkBookingVacationDay();
                }
            });
        }

        $scope.deleteBooking = function (booking, index) {
            if (confirm('Are you sure to delete No.' + (index + 1))) {
                BookingService.Delete(booking.Id).then(function () {
                    $scope.bookings.splice(index, 1);
                    checkBookingVacationDay();
                })
            }
        }

        // Remaining vacation days
        var checkBookingVacationDay = function () {
            BookingService.GetVacationDay(userId, $scope.year).then(function (data) {
                $scope.totalVacationDay = data;
            });

            BookingService.GetBookingVacationDay(userId, $scope.year).then(function (data) {
                $scope.totalBookingVacationDay = data;
            });
        }
        checkBookingVacationDay();
        // Remaining vacation days

        // open popup
        $scope.startDatePopup = {
            opened: false
        };
        $scope.openStartDatePopup = function () {
            $scope.startDatePopup.opened = true;
        };

        // open popup
        $scope.endDatePopup = {
            opened: false
        };
        $scope.openEndDatePopup = function () {
            $scope.endDatePopup.opened = true;
        };

        // dateOptions
        $scope.dateOptions = {
            dateDisabled: disabled,
            formatYear: 'yy',
            maxDate: new Date(2020, 5, 22),
            minDate: new Date(),
            startingDay: 1
        };

        // Disable weekend selection
        function disabled(data) {
            var date = data.date,
                mode = data.mode;
            return mode === 'day' && (date.getDay() === 0 || date.getDay() === 6);
        }

    }
})();