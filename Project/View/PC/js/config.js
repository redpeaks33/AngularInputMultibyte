main.config(['$translateProvider', function ($translateProvider) {
    $translateProvider.useStaticFilesLoader({
        prefix: '/View/locale/',
        suffix: '.js'
    });
    $translateProvider.preferredLanguage('ja');
    $translateProvider.fallbackLanguage('en');
}]);