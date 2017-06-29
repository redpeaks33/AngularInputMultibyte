var main = angular.module("Main",
    [
        'ui.router',
        'smart-table',
        'ui.bootstrap',
        'ngSanitize'
    ]);

main.controller('MainCtrl', ['$scope', '$state', '$http', '$interval', '$sce', '$location',
    function ($scope, $state, $http, $interval, $sce, $location) {
        $scope.list = []
        $scope.searchstring = undefined;
        $scope.LevelCategory = 0;
        $scope.onRandom = false;
        $scope.onlevel = 'SVL1';
        $scope.onAuto = false;
        $scope.showTable = true;
        $scope.isLoading = true;
        $scope.searchstring = undefined;
        $scope.tableHeader = undefined;
        $scope.wordIndex = undefined;
        $scope.wordlist = [];
        $scope.meaning = {};

        $scope.articleContainer = [];
        $scope.imageContainer = [];
        $scope.meaningContainer = [];
        $scope.live = new Object();

        $scope.initializeMain = function (DisplayType) {
            $state.go('List');
        }

        $scope.initializeView = function () {
            $state.go('List.View');
            $scope.changeLevelCategory(0);
        }

        $scope.changeLevelCategory = function (levelCategory) {
            $scope.LevelCategory = levelCategory;
            $scope.levelSearch('WORDLEVEL' + ($scope.LevelCategory + 1))
        }
        //$scope.GetList = function () {
        //    $http.get("api/Contents/").
        //        success(function (data, status, headers, config) {
        //            CreateWordTable('SVL1', data);
        //        }).error(function (data, status, headers, config) {
        //            alert('errors ');
        //        });
        //};

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

        $scope.SelectItem = function (item) {
            $http.get("/api/Contents/select/word/" + item.id)
                .success(function (data, status, headers, config) {
                    $scope.articleContainer = angular.fromJson(data);
                    $scope.imageContainer = angular.fromJson(data);
                    $scope.meaningContainer = angular.fromJson(data);
                })
                .error(function (data, status, headers, config) {
                    alert('errors select');
                });
        };

        //"Custom/html/Image.aspx?16/10"
        $scope.setDetailView = function (item) {
            $scope.SelectItem(item);
            $scope.wordIndex = item.id;
            $scope.live = item;
            //SetMeaning(item);
            $scope.isArticleshow = item.articleInfoList.length > 0;
        }

        var SetMeaning = function (item) {
            $scope.meaning = {};
            $.each(item.meaningInfoList, function (i, v) {
                switch (v.regionID) {
                    case 1: $scope.meaning.en = v.meaning; break;
                    case 2: $scope.meaning.jp = v.meaning; break;
                    default: break;
                }
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

            $scope.tableHeader = searchstring + ' ' + $scope.wordlist.length + '語';
            $scope.isLoading = false;
        }

        $scope.randomSearch = function (searchstring) {
            $scope.onRandom = !$scope.onRandom;

            if (searchstring == undefined) {
                searchstring = '';
            }
            $scope.Search(searchstring === '' ? $scope.onlevel : searchstring);
        }
        $scope.levelSearch = function (searchLevel) {
            $scope.searchstring = '';
            $scope.onlevel = searchLevel;
            $scope.Search(searchLevel);
        }
        $scope.autoplay = function () {
            //(max,span,function)
            $scope.onAuto = !$scope.onAuto;

            loopSleep($scope.wordlist.length, 3000, function (i) {
                if (!$scope.onAuto) {
                    return false;
                }
                $scope.setDetailView($scope.wordlist[i]);
            });
        }

        $scope.changeTableFormat = function () {
            $scope.showTable = !$scope.showTable;
        }

        $scope.range = function (min, max, step) {
            step = step || 1;
            var input = [];
            for (var i = min; i <= max; i += step) {
                input.push(i);
            }
            return input;
        };

        var shuffle = function (array) {
            var m = array.length, t, i;
            while (m) {
                i = Math.floor(Math.random() * m--);
                t = array[m];
                array[m] = array[i];
                array[i] = t;
            }
        }

        var loopSleep = function (_loopLimit, _interval, _mainFunc) {
            var loopLimit = _loopLimit;
            var interval = _interval;
            var mainFunc = _mainFunc;
            var i = 0;
            var loopFunc = function () {
                var result = mainFunc(i);
                if (result === false) {
                    // break機能
                    return;
                }
                i = i + 1;
                if (i < loopLimit) {
                    setTimeout(loopFunc, interval);
                }
            }
            loopFunc();
        }

        $scope.changePage = function () {
            //$scope.setDetailView($scope.wordlist[Math.floor(Math.random() * $scope.list.length)]);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        $scope.onWord = true;
        $scope.onPronounce = true;
        $scope.onTranslation = true;

        $scope.stepBackward = function () {
            var index = GetSelectedWordIndex();

            index = (index - 1 < 0) ? $scope.wordlist.length - 1 : index - 1;

            $scope.setDetailView($scope.wordlist[index]);
        }

        $scope.stepForward = function () {
            var index = GetSelectedWordIndex();

            index = (index + 1 >= $scope.wordlist.length) ? 0 : index + 1;

            $scope.setDetailView($scope.wordlist[index]);
        }

        $scope.playPronounce = function () {
            document.getElementById("pronounce").play();
        }
        var GetSelectedWordIndex = function () {
            var i;
            ang

            ular.forEach($scope.wordlist, function (value, index) {
                if ($scope.wordIndex === value.id) {
                    i = index;
                }
            });

            return i;
        }
    }]);