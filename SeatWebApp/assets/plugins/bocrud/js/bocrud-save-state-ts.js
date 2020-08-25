/// <refrence path="jquery.d.ts" />
var clientStorage = (function () {
    function clientStorage() {
        if (!this.supports_html5_storage())
            throw new EventException();
    }
    clientStorage.prototype.supports_html5_storage = function () {
        try {
            return 'localStorage' in window && window['localStorage'] !== null;
        }
        catch (e) {
            return false;
        }
    };
    clientStorage.prototype.getItem = function (key) {
        return localStorage.getItem(key);
    };
    clientStorage.prototype.setItem = function (key, value) {
        localStorage.setItem(key, value);
    };
    clientStorage.prototype.removeItem = function (key, oncomplete) {
        localStorage.removeItem(key);
    };
    return clientStorage;
})();
var serverStorage = (function () {
    function serverStorage(url) {
        this._url = url;
        this._c = 1;
    }
    serverStorage.prototype.getItem = function (key) {
        var result;
        $.ajax({
            url: this._url + '/GetUserStateItem?' + this._c++,
            type: 'get',
            error: function (data) {
                console.error(data);
            },
            success: function (data) {
                if (data && data.indexOf('error:') == 0)
                    console.error(data);
                result = data;
            },
            async: false,
            data: { key: key }
        });
        return result;
    };
    serverStorage.prototype.setItem = function (key, value) {
        $.ajax({
            url: this._url + '/SetUserStateItem',
            type: 'post',
            cache: false,
            error: function (data) {
                console.error(data);
            },
            async: true,
            data: { key: key, value: value }
        });
    };
    serverStorage.prototype.removeItem = function (key, oncomplete) {
        $.ajax({
            url: this._url + '/RemoveUserStateItem' ,
            type: 'get',
            cache:false,
            error: function (data) {
                console.error(data);
            },
            async: true,
            data: { key: key },
            complete: oncomplete
        });
    };
    return serverStorage;
})();
