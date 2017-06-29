var main = angular.module("app", ['ui.router', 'multiByteInputHelper']);

main.controller('MyController', ['$scope', '$http', function ($scope, $http) {
    $scope.Message = 'Click Button';

    $scope.text = 'text imnida';
    $scope.textarea = 'textarea imnida';
    $scope.GetStart = function () {
        $http.get("api/Contents/1").success(function (data, status, headers, config) {
            $scope.Message += ':' + data;
        }).error(function (data, status, headers, config) {
            alert('error');
        });
    };
}]);