(function () {
    'use strict';

    angular
        .module('app.auth')
        .directive('isUserNameAvailable', ['$q', '$http', '$timeout', 'SVCS', function ($q, $http, $timeout, SVCS) {
            return {
                require: 'ngModel',
                link: function (scope, elm, attrs, ctrl) {
                    var checkingTask = null;

                    var baseUrl = SVCS.Auth;

                    ctrl.$asyncValidators.isUserNameAvailable = function (modelValue) {

                        if (ctrl.$isEmpty(modelValue)) {
                            return $q.when();
                        }

                        var def = $q.defer();

                        if (checkingTask != null) {
                            $timeout.cancel(checkingTask);
                            checkingTask = null;
                        }
                        checkingTask = $timeout(function () {
                            $http.get(baseUrl + '/api/account/is-user-name-available', { params: { UserName: modelValue } }).then(function (response) {
                                if (response.data) {
                                    def.resolve();
                                } else {
                                    def.reject();
                                }
                            }, function (responseError) {
                                def.reject();
                            });
                        }, 500);

                        return def.promise;
                    };
                }
            };
        }])
        .directive('isEmailAvailable', ['$q', '$http', '$timeout', 'SVCS', function ($q, $http, $timeout, SVCS) {
            return {
                require: 'ngModel',
                link: function (scope, elm, attrs, ctrl) {
                    var checkingTask = null;

                    var baseUrl = SVCS.Auth;

                    ctrl.$asyncValidators.isEmailAvailable = function (modelValue) {

                        if (ctrl.$isEmpty(modelValue)) {
                            return $q.when();
                        }

                        var def = $q.defer();

                        if (checkingTask != null) {
                            $timeout.cancel(checkingTask);
                            checkingTask = null;
                        }
                        checkingTask = $timeout(function () {
                            $http.get(baseUrl + '/api/account/is-email-available', { params: { email: modelValue } }).then(function (response) {
                                if (response.data) {
                                    def.resolve();
                                } else {
                                    def.reject();
                                }
                            }, function (responseError) {
                                def.reject();
                            });
                        }, 500);

                        return def.promise;
                    };
                }
            };
        }])
        .directive('isValidPhoneNumber', ['$q', '$http', '$timeout', 'SVCS', function ($q, $http, $timeout, SVCS) {
            return {
                require: 'ngModel',
                link: function (scope, elm, attrs, ctrl) {
                    var checkingTask = null;

                    var baseUrl = SVCS.Auth;

                    ctrl.$asyncValidators.isValidPhoneNumber = function (modelValue) {

                        if (ctrl.$isEmpty(modelValue)) {
                            return $q.when();
                        }

                        var def = $q.defer();

                        if (checkingTask != null) {
                            $timeout.cancel(checkingTask);
                            checkingTask = null;
                        }
                        checkingTask = $timeout(function () {
                            $http.get(baseUrl + '/api/utils/is-valid-phone-number', { params: { PhoneNumber: modelValue } }).then(function (response) {
                                if (response.data) {
                                    def.resolve();
                                } else {
                                    def.reject();
                                }
                            }, function (responseError) {
                                def.reject();
                            });
                        }, 500);

                        return def.promise;
                    };
                }
            };
        }])
        .directive('passwordVerify', function passwordVerify() {
            return {
                restrict: 'A', // only activate on element attribute
                require: '?ngModel', // get a hold of NgModelController
                link: function (scope, elem, attrs, ngModel) {
                    if (!ngModel) return; // do nothing if no ng-model

                    // watch own value and re-validate on change
                    scope.$watch(attrs.ngModel, function () {
                        validate();
                    });

                    // observe the other value and re-validate on change
                    attrs.$observe('passwordVerify', function (val) {
                        validate();
                    });

                    var validate = function () {
                        // values
                        var val1 = ngModel.$viewValue;
                        var val2 = attrs.passwordVerify;

                        // set validity
                        ngModel.$setValidity('passwordVerify', val1 === val2);
                    };
                }
            };
        });
})();