myApp.config(['$routeProvider',
    function ($routeProvider) {
        $routeProvider.
            when('/user', {
                templateUrl: '/app/user/user.html',
                controller: 'UsersController'
            }). 
            when('/user/login', {
                templateUrl: '/app/user/login.html',
                controller: 'LoginController'
            }).
            when('/user/forgot-password', {
                templateUrl: '/app/user/forgot-password.html',
                controller: 'ForgotPasswordController'
            }).  
            when('/user/reset-password', {
                templateUrl: '/app/user/reset-password.html',
                controller: 'ResetPasswordController'
            })

        // Inject $cookies
        var $cookies;

        angular.injector(['ngCookies']).invoke([
            '$cookies', function (_$cookies) {
                $cookies = _$cookies;
            }
        ]);
    }
]);