(function ()
{
    'use strict';

    angular
        .module('vacationTracking')
        .controller('IndexController', IndexController);

    /** @ngInject */
    function IndexController($scope, $rootScope)
    {
        var vm = this;
        $scope.userInfo = $rootScope.$authService.Account;
    }
})();