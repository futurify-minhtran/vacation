myApp.config(['$routeProvider',
    function ($routeProvider) {
        $routeProvider.
            when('/user', {
                templateUrl: '/app/user/user.html',
                controller: 'UsersController'
            }).  
            when('/user/add', {
                templateUrl: '/app/user/user-form.html',
                controller: 'UsersController'
            }).    
            when('/user/detail', {
                templateUrl: '/app/user/user-detail.html',
                controller: 'UsersController'
            }).    
            otherwise({
                redirectTo: '/user'
            })
    }
]);