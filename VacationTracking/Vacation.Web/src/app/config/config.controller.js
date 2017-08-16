(function () {
    'use strict';

    angular.module('app.config')
        .controller('ConfigController', ConfigController);

    /** @ngInject */
    function ConfigController($scope, $state, ConfigService) {
        $scope.loading = {
            change: false
        };
        ConfigService.GetAll().then(function (data) {
            $scope.configs = data;
        });

        $scope.setStatus = function (vacationConfig, index) {
            $scope.loading.change = true;
            ConfigService.Update(vacationConfig).then(function (settedStatus) {
                $scope.loading.change = false;
                $scope.configs[index] = settedStatus;
            });
        };
    }

})();