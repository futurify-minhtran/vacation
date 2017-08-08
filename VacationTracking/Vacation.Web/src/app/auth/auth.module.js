(function ()
{
    'use strict';

    angular
        .module('app.auth', ['ngCookies', 'ui.router'])
        .config(config)
        .run(['AuthenticationService', '$rootScope', '$state', '$stateParams', function (AuthenticationService, $rootScope, $state, $stateParams) {
            var permissionsLoaded = false;

            $rootScope.$on('PERMISSIONS_LOADED', function () {
                permissionsLoaded = true;
            })  

            $rootScope.$on("$stateChangeStart", function (event, toState, toParams, fromState, fromParams) {
                if (toState.protect === true) {
                    if (!AuthenticationService.IsAuthenticated) {
                        event.preventDefault();
                        $state.go('app.auth_login');
                    } else if (toState.permissions && toState.permissions.length) {
                        if (!AuthenticationService.HasPermissions(toState.permissions)) {
                            if (permissionsLoaded) {
                                event.preventDefault();
                                AuthenticationService.SignOut();
                                $state.go('app.auth_login');
                            } else {
                                event.preventDefault();
                                $rootScope.$on('PERMISSIONS_LOADED', function () {
                                    if (!AuthenticationService.HasPermissions(toState.permissions)) {
                                        AuthenticationService.SignOut();
                                        $state.go('app.auth_login');
                                    } else {
                                        var params = angular.copy(toParams);
                                        $state.go(toState.name, params);
                                    }
                                })
                            }
                        }
                    }
                }
            });
        }])
        .directive('showRoute', ['AuthenticationService', '$rootScope', '$state', function (AuthenticationService, $rootScope, $state) {
            return {
                restrict: 'A',
                link: function (scope, element, attrs) {
                    handler();
                    $rootScope.$on('PERMISSIONS_LOADED', function () {
                        handler();
                    })
                    function handler() {
                        if (!attrs.uiSref && !attrs.showForRoute) {
                            element.hide();
                        } else {
                            var permissions = [];
                            if (attrs.uiSref) {
                                var uiSrefState = $state.get(attrs.uiSref);
                                if (uiSrefState) {
                                    if (uiSrefState.permissions) {
                                        permissions = uiSrefState.permissions;
                                    }
                                }
                            }
                            if (attrs.showForRoute) {
                                var state = $state.get(attrs.showForRoute);
                                if (state) {
                                    if (state.permissions) {
                                        permissions = permissions.concat(state.permissions);
                                    }
                                }
                            }

                            if (permissions.length) {
                                if (!AuthenticationService.HasPermissions(permissions)) {
                                    element.hide();
                                } else {
                                    element.show();
                                }
                            } else {
                                element.show();
                            }
                        }
                    }
                }
            }
        }])
        .directive('showPermissions', ['AuthenticationService', '$rootScope', '$state', function (AuthenticationService, $rootScope, $state) {
            return {
                restrict: 'A',
                scope: {
                    $$permissions: '=showPermissions'
                },
                link: function (scope, element, attrs) {
                    handler();
                    $rootScope.$on('PERMISSIONS_LOADED', function () {
                        handler();
                    })
                    function handler() {
                        if (scope.$$permissions.length) {
                            if (!AuthenticationService.HasPermissions(scope.$$permissions)) {
                                element.hide();
                            } else {
                                element.show();
                            }
                        } else {
                            element.show();
                        }
                    }
                }
            }
        }]);

    /** @ngInject */
    function config($stateProvider)
    {
        // State
        $stateProvider
            .state('app.auth_login', {
                url: '/login',
                views: {
                    'main@': {
                        templateUrl: 'app/core/layouts/loginLayout.html'
                    },
                    'content@app.auth_login': {
                        templateUrl: 'app/auth/views/login.html',
                        controller: 'LoginController as vm'
                    }
                }
            })
            .state('app.auth_logout', {
                url: '/logout',
                views: {
                    'main@': {
                        controller: 'LogoutController as vm'
                    }
                }
            })
            .state('app.auth_forgot-password', {
                url: '/forgot-password',
                views: {
                    'main@': {
                        templateUrl: 'app/core/layouts/loginLayout.html'
                    },
                    'content@app.auth_forgot-password': {
                        templateUrl: 'app/auth/views/forgot-password.html',
                        controller: 'ForgotPasswordController as vm'
                    }
                }
            })
            .state('app.auth_reset-password', {
                url: '/reset-password?email&token',
                views: {
                    'main@': {
                        templateUrl: 'app/core/layouts/loginLayout.html'
                    },
                    'content@app.auth_reset-password': {
                        templateUrl: 'app/auth/views/reset-password.html',
                        controller: 'ResetPasswordController as vm'
                    }
                }
            })
    }
})();