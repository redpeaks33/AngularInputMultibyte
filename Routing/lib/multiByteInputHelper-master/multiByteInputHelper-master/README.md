# multiByteInputHelper
`한글과 같은 멀티바이트 캐릭터를 <input>이나 <textarea>에서 입력 시 ngModel 디렉티브를 통해 바인딩 된 model에 글자단위로 저장되는 것을 (keyup이벤트에 따라)자소 단위로 입력되도록 개선합니다.`

# Install
```
bower install multi-byte-input-helper --save
```

# Code
디렉티브 모듈을 어플리케이션 모듈에 등록합니다.
```
angular.module('MyApp', ['multiByteInputHelper'])
```

# HTML
반영하고자 하는 input/textarea 엘리먼트에 디렉티브를 삽입합니다.
```
<input type="text" ng-model="title" multi-byte />
```
