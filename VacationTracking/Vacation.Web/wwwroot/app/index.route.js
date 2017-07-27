myApp.config(['$routeProvider',
    function ($routeProvider) {
        $routeProvider.
            when('/user', {
                templateUrl: '/app/user/user.html',
                controller: 'UsersController'
            }).          
            otherwise({
                redirectTo: '/'
            })
    }
]);