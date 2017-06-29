main.controller('MainController', ['$scope', '$state', '$http', '$interval', '$sce', '$location', '$translate',
    function ($scope, $state, $http, $interval, $sce, $location, $translate) {
        $scope.list = []
        $scope.searchstring = undefined;
        $scope.LevelCategory = 0;
        $scope.onRandom = false;
        $scope.onlevel = 'WORDLEVEL1';
        $scope.onAuto = false;
        $scope.showTable = true;
        $scope.isLoading = true;
        $scope.searchstring = undefined;
        $scope.tableHeader = undefined;
        $scope.wordIndex = undefined;
        $scope.wordlist = [];
        $scope.meaning = {
        };
        $scope.onWord = true;
        $scope.onPronounce = true;
        $scope.onTranslation = true;

        $scope.articleContainer = [];
        $scope.imageContainer = [];
        $scope.meaningContainer = [];
        $scope.live = new Object();
        $scope.isEnglish = true;

        $scope.initializeMain = function (DisplayType) {
            $scope.changeLevelCategory(0);
            setLang();
        }

        $scope.changeLevelCategory = function (levelCategory) {
            $scope.LevelCategory = levelCategory;
            $scope.levelSearch('WORDLEVEL' + ($scope.LevelCategory + 1))
        }

        $scope.SearchLevel = function (level) {
            if (!level) {
                alert('input number 1 ~ 30 ');
            }
            $scope.changeLevelCategory(level - 1);
        }

        $scope.levelSearch = function (searchLevel) {
            $scope.searchstring = '';
            $scope.onlevel = searchLevel;
            $scope.Search(searchLevel);
        }
        $scope.Search = function (searchstring) {
            if (searchstring === undefined) {
                searchstring = '';
            }

            $http.get("/api/Contents/topSearch/word/" + searchstring).
                success(function (data, status, headers, config) {
                    CreateWordTable(searchstring, data, true);
                    LeftOverSearch(searchstring);
                    $location.path('/')
                }).error(function (data, status, headers, config) {
                    alert('errors search');
                });
        }
        var LeftOverSearch = function (searchstring) {
            $http.get("/api/Contents/search/word/" + searchstring).
               success(function (data, status, headers, config) {
                   CreateWordTable(searchstring, data, false);
               }).error(function (data, status, headers, config) {
                   alert('errors search');
               });
        }

        var CreateWordTable = function (searchstring, data, executeSetDetail) {
            $scope.list = [];
            var num = 5;

            $scope.wordlist = angular.fromJson(data);

            if ($scope.onRandom) {
                shuffle($scope.wordlist);
            }
            for (var i = 0; i < $scope.wordlist.length; i = i + num) {
                var words = new Array();
                for (var j = i; j < i + num; j++) {
                    if ($scope.wordlist[j] != undefined) {
                        words.push($scope.wordlist[j]);
                    } else {
                        words.push(new Object());
                    }
                }
                $scope.list.push(words);
            }

            searchstring = searchstring == '' ? 'SVL1' : searchstring;
            if (executeSetDetail) {
                $scope.setDetailView($scope.wordlist[Math.floor(Math.random() * $scope.list.length)]);
            }

            $scope.tableHeader = searchstring + ' ' + $scope.wordlist.length + ' words';
            $scope.isLoading = false;
        }

        $scope.setDetailView = function (item) {
            $scope.SelectItem(item);
            $scope.wordIndex = item.id;
            $scope.live = item;
        }

        $scope.SelectItem = function (item) {
            $http.get("/api/Contents/select/word/" + item.id)
                .success(function (data, status, headers, config) {
                    $scope.articleContainer = angular.fromJson(data);
                    $scope.imageContainer = angular.fromJson(data);
                    $scope.meaningContainer = angular.fromJson(data);
                    $scope.existSound = $scope.meaningContainer.existsound;
                    if (true) {
                        $scope.playPronounce();
                    }
                })
                .error(function (data, status, headers, config) {
                    alert('errors select');
                });
        };

        //#region Sound
        var sound = undefined;
        $scope.playPronounce = function () {
            $http.get("/api/Contents/select/sound/" + $scope.wordIndex)
                .success(function (data, status, headers, config) {
                    var result = angular.fromJson(data);
                    if (result) {
                        sound = new Audio("data:audio/x-wav;base64, " + result.soundBinary);
                        sound.play();
                    }
                })
                .error(function (data, status, headers, config) {
                    alert('errors select');
                });
        }

        //#endregion
        $scope.play = function () {
            sound.play();
        }

        //#region For PC
        $scope.range = function (min, max, step) {
            step = step || 1;
            var input = [];
            for (var i = min; i <= max; i += step) {
                input.push(i);
            }
            return input;
        };
        //#endregion

        //#region
        var setLang = function () {
            var lang = $location.absUrl().split('?')[1];
            if (lang === '') {
                lang = 'ja';
            }
            $scope.changeLang(lang);
    }
        $scope.changeLang = function (langKey) {
            $translate.use(langKey);
            if (langKey === 'ja') {
                $scope.isJapanese = true;
            }
            else {
                $scope.isJapanese = false;
        }
    };
        //#endregion
}]);