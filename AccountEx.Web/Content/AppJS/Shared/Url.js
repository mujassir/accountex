
var Url = function () {
    return {
        init: function () {
            var $this = this;
            $this.GetAllQueryString();


        },
        ParseUrl: function (url, keyvalues) {
            var $this = this;
            if (url.indexOf("?") === -1) {
                url += "?";
            }
            var qs = [];
            for (var d in keyvalues)
                qs.push(encodeURIComponent(key) + "=" + encodeURIComponent(keyvalues[key]));
            url += qs.join("&");
            return url;


        },
        Get: function (name) {
            var $this = this;
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        },
        GetAllQueryString: function (name) {
            var $this = this;
            var match,
             pl = /\+/g,  // Regex for replacing addition symbol with a space
            search = /([^&=]+)=?([^&]*)/g,
            decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
            query = window.location.search.substring(1);

            urlParams = {};
            while (match = search.exec(query))
                Url[decode(match[1])] = decode(match[2]);
        },



    };
}();
