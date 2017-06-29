angular.module('multiByteInputHelper', [])
    .directive('multiByte', [function () {
        return {
            priority: 2,
            restrict: 'A',
            compile: function (element) {
                element.on('compositionstart', function(e) {
                    e.stopImmediatePropagation();
                });
            }
        }
    }]);