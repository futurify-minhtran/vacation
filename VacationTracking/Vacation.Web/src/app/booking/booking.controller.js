(function () {
    'use strict';

    angular.module('app.booking')
        .controller('BookingController', BookingController);

    /** @ngInject */
    function BookingController($scope, BookingService, $state, $rootScope) {
        var userId = $rootScope.$authService.Account.Id;
        var email = $rootScope.$authService.Account.Email;
        var year = (new Date()).getFullYear();

        $scope.loading = {
            create: false,
            delete: false,
            update: false
        };

        // Init vacation day default is 12
        BookingService.CheckNewUser(userId).then(function (data) {
            // New user, must be create a vacation day
            if (data == true) {
                var vacationDay = {
                    "UserId": userId,
                    "Year": (new Date()).getFullYear(),
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
            $scope.error = '';
            $scope.success = '';
        };

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
            };
            $scope.BookingForm.$setPristine();
            $scope.BookingForm.$setUntouched();
            $scope.error = null;
            $scope.message = null;
        };

        // add timepicker to datepicker
        var _addTime = function (datepicker, timepicker) {
            datepicker.setHours(timepicker.getHours());
            datepicker.setMinutes(timepicker.getMinutes());
            datepicker.setSeconds(0);
            datepicker.setMilliseconds(0);
        };

        $scope.addBooking = function () {
            $scope.loading.create = true;
            var model = angular.copy($scope.booking);

            _addTime(model.StartDate,model.StartTime);
            _addTime(model.EndDate,model.EndTime);

            BookingService.Create(email,model).then(function (data) {
                $scope.loading.create = false;
                if (data.Error) {
                    $scope.error = data.Error;
                } else {
                    //$scope.booking = data.Booking;
                    $scope.success = "Add booking success";
                    $('#addBookingModal').modal('hide');
                    $scope.bookings.unshift(data.Booking);

                    checkBookingVacationDay();

                    // update Remainng Days Off
                    BookingService.GetRemaingDaysOff(userId, year).then(function (data) {
                        $rootScope.$authService.UpdateRemainingDaysOff(userId, data);
                    });
                }
            });
        };

        $scope.deleteBooking = function (booking, index) {
            if (confirm('Are you sure to delete No.' + (index + 1))) {
                $scope.loading.delete = true;
                BookingService.Delete(booking.Id,email).then(function () {
                    $scope.loading.delete = false;
                    $scope.bookings.splice(index, 1);
                    checkBookingVacationDay();

                    // update Remainng Days Off
                    BookingService.GetRemaingDaysOff(userId, year).then(function (data) {
                        $rootScope.$authService.UpdateRemainingDaysOff(userId, data);
                    });
                });
            }
        };

        // Remaining vacation days
        $scope.totalVacationDay = null;
        $scope.totalBookingVacationDay = null;
        var checkBookingVacationDay = function () {
            BookingService.GetVacationDay(userId, $scope.year).then(function (data) {
                $scope.totalVacationDay = data;
            });
            BookingService.GetBookingVacationDay(userId, $scope.year).then(function (data) {
                $scope.totalBookingVacationDay = data;
            });
        };
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

        // update booking
        $scope.editBooking = function (booking, index) {
            $scope.clearMessage();
            $scope.booking = angular.copy(booking);
            $scope.editIndex = index;
            $scope.booking.StartDate = new Date(booking.StartDate);
            $scope.booking.EndDate = new Date(booking.EndDate);
            $scope.booking.StartTime = new Date(booking.StartDate);
            $scope.booking.EndTime = new Date(booking.EndDate);
        };

        $scope.updateBooking = function () {
            $scope.loading.update = true;
            var model = angular.copy($scope.booking);

            _addTime(model.StartDate, model.StartTime);
            _addTime(model.EndDate, model.EndTime);
           
            BookingService.Update(email, model).then(function (data) {
                $scope.loading.update = false;
                if (data.Error) {
                    $scope.error = data.Error;
                } else {
                    //$scope.booking = data.Booking;
                    $scope.success = "Update booking success";
                    $('#editBookingModal').modal('hide');
                    $scope.bookings[$scope.editIndex] = data.Booking;

                    checkBookingVacationDay();

                    // update Remainng Days Off
                    BookingService.GetRemaingDaysOff(userId, year).then(function (data) {
                        $rootScope.$authService.UpdateRemainingDaysOff(userId, data);
                    });
                }
            });
        };
    }
})();