main.config(['$stateProvider', function ($stateProvider) {
    $stateProvider
    .state('List', {
        url: '/List',
        templateUrl: '/View/PC/html/WordList.html'
    })
    .state('List.View', {
        url: '/List.View',
        templateUrl: '/View/PC/html/View.html'
    })
    //#region mobile
    .state('MobileList', {
        url: '/List',
        templateUrl: '/Custom/html/WordList.html'
    })
    .state('Mobile.View', {
        url: '/List.View',
        templateUrl: '/Custom/html/View.html'
    })
    //#endregion
}])