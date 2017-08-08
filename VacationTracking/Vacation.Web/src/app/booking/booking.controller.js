(function () {
    'use strict';

    angular.module('app.booking')
        .controller('BookingController', BookingController);

    /** @ngInject */
    function BookingController($scope, BookingService, $state, $log) {

        BookingService.GetAll().then(function (data) {
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
                UserId: null,
                StartDate: new Date(),
                EndDate: new Date(),
                StartTime: startTime,
                EndTime: endTime,
                Reason: "Reason"
            }
            $scope.BookingForm.$setPristine();
            $scope.BookingForm.$setUntouched();
            $scope.error = null;
            $scope.message = null;
        }
        $scope.addBooking = function () {
            debugger;
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
                debugger;
                if (data.Error) {
                    debugger;
                    $scope.error = data.Error;
                } else {
                    debugger;
                    $scope.booking = data.Booking;
                    $scope.success = "Add booking success";
                    $('#addBookingModal').modal('hide');
                    $scope.booking.unshift(data.Booking);
                }
            });
        }

        $scope.deleteBooking = function (booking, index) {
            if (confirm('Are you sure to delete No.' + (index + 1))) {
                BookingService.Delete(booking.Id).then(function () {
                    $scope.bookings.splice(index, 1);
                })
            }
        }

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


//// today
//$scope.today = function () {
//    $scope.startDate = new Date();
//};

//$scope.today();

//// clear
//$scope.clear = function () {
//    $scope.startDate = null;
//};

//// set date
//$scope.setDate = function (year, month, day) {
//    $scope.startDate = new Date(year, month, day);
//};

//$scope.inlineOptions = {
//    customClass: getDayClass,
//    minDate: new Date(),
//    showWeeks: true
//};

//$scope.toggleMin = function () {
//    $scope.inlineOptions.minDate = $scope.inlineOptions.minDate ? null : new Date();
//    $scope.dateOptions.minDate = $scope.inlineOptions.minDate;
//};

//$scope.toggleMin();

//var tomorrow = new Date();
//tomorrow.setDate(tomorrow.getDate() + 1);

//var afterTomorrow = new Date();
//afterTomorrow.setDate(tomorrow.getDate() + 1);

//$scope.events = [
//    {
//        date: tomorrow,
//        status: 'full'
//    },
//    {
//        date: afterTomorrow,
//        status: 'partially'
//    }
//];

//function getDayClass(data) {
//    var date = data.date,
//        mode = data.mode;
//    if (mode === 'day') {
//        var dayToCheck = new Date(date).setHours(0, 0, 0, 0);

//        for (var i = 0; i < $scope.events.length; i++) {
//            var currentDay = new Date($scope.events[i].date).setHours(0, 0, 0, 0);

//            if (dayToCheck === currentDay) {
//                return $scope.events[i].status;
//            }
//        }
//    }

//    return '';
//}

        

