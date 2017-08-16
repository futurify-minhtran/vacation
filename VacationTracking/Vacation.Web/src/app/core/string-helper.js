(function ()
{
    'use strict';

    angular
        .module('app.stringcommon', [])
        .factory('stringhelper', stringhelper);

    /** @ngInject */
    function stringhelper($state, SVCS, $stateParams)
    {
        var convertToAliasName = function (name) {
            var result = "";
            if (name) {
                var result = name.replace(/ /g, '-');
                result = result.replace(/-+/g, '-');
                result = result.toLowerCase();
            }

            return result;
        };

        var convertToNumberPhoneString = function (number) {
            var result = "";
            if (!number || number.length > 12)
                return number;
            for (var i = 0; i < number.length; i++)
            {
                if (i == 3 || i == 7)
                    if (number[i] != '-')
                        result += '-';
                result += number[i];
            }
            return result;
        };

        var goToDetailPage = function (rvname, city) {
            var _city = convertToAliasName(city);

            return SVCS.Web + "/" + $stateParams.lang + "/rv-rental/" + _city + "/" + rvname;
        };

        var addSpaceBeforeCapital = function (string) {
            return string.replace(/([a-z])([A-Z])/g, '$1 $2').trim();
        };

        return {
            convertToAliasName: convertToAliasName,
            convertToNumberPhoneString: convertToNumberPhoneString,
            goToDetailPage: goToDetailPage,
            addSpaceBeforeCapital: addSpaceBeforeCapital
        };
    }

})();