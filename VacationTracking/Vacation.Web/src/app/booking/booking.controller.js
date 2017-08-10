(function () {
    'use strict';

    angular.module('app.booking')
        .controller('BookingController', BookingController);

    /** @ngInject */
    function BookingController($scope, BookingService, $state, $rootScope) {
        var userId = $rootScope.$authService.Account.Id;

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
                Reason: "Reason"
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

        // Calendar
        $scope.calendarView = 'month';
        $scope.viewDate = (new Date).getMonth();

        $scope.events = [
            {
                title: 'My event title', // The title of the event
                startsAt: new Date(2013, 5, 1, 1), // A javascript date object for when the event starts
                endsAt: new Date(2014, 8, 26, 15), // Optional - a javascript date object for when the event ends
                color: { // can also be calendarConfig.colorTypes.warning for shortcuts to the deprecated event types
                    primary: '#e3bc08', // the primary event color (should be darker than secondary)
                    secondary: '#fdf1ba' // the secondary event color (should be lighter than primary)
                },
                actions: [{ // an array of actions that will be displayed next to the event title
                    label: '<i class=\'glyphicon glyphicon-pencil\'></i>', // the label of the action
                    cssClass: 'edit-action', // a CSS class that will be added to the action element so you can implement custom styling
                    onClick: function (args) { // the action that occurs when it is clicked. The first argument will be an object containing the parent event
                        console.log('Edit event', args.calendarEvent);
                    }
                }],
                draggable: true, //Allow an event to be dragged and dropped
                resizable: true, //Allow an event to be resizable
                incrementsBadgeTotal: true, //If set to false then will not count towards the badge total amount on the month and year view
                recursOn: 'year', // If set the event will recur on the given period. Valid values are year or month
                cssClass: 'a-css-class-name', //A CSS class (or more, just separate with spaces) that will be added to the event when it is displayed on each view. Useful for marking an event as selected / active etc
                allDay: false // set to true to display the event as an all day event on the day view
            }
        ];
        // Calendar
    }

    angular.module('app.booking')
        .config(['calendarConfig', function (calendarConfig) {

            // View all available config
            console.log(calendarConfig);

            // Change the month view template globally to a custom template
            //calendarConfig.templates.calendarMonthView = 'path/to/custom/template.html';

            // Use either moment or angular to format dates on the calendar. Default angular. Setting this will override any date formats you have already set.
            //calendarConfig.dateFormatter = 'moment';

            // This will configure times on the day view to display in 24 hour format rather than the default of 12 hour
            calendarConfig.allDateFormats.moment.date.hour = 'HH:mm';

            // This will configure the day view title to be shorter
            calendarConfig.allDateFormats.moment.title.day = 'ddd D MMM';

            // This will set the week number hover label on the month view
            calendarConfig.i18nStrings.weekNumber = 'Week {week}';

            // This will display all events on a month view even if they're not in the current month. Default false.
            calendarConfig.displayAllMonthEvents = true;

            // Make the week view more like the day view, ***with the caveat that event end times are ignored***.
            calendarConfig.showTimesOnWeekView = true;

        }]);
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

        

