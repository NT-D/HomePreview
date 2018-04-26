(function () {
    "use strict";
    // hubインスタンス
    var sample = $.connection.sample;
    // クライアントメソッドの実装
    $.extend(sample.client, {
        show: function (message) {
            _show(message);
        }
    });
    // ヘルパー関数の定義
    function _show(message) {
        $("<li class=’list-group-item’>" + message + "</li>").appendTo("#messages");
    }
    function _through() {
        var message = $("#through-message").val();
        sample.server.through(message)
            .done(function (response) {
                _show(response);
            });
    }
    function _echo() {
        var message = $("#echo-message").val();
        sample.server.echo(message);
    }
    function _echoToAll() {
        var message = $("#echo2all-message").val();
        sample.server.echoToAll(message);
    }
    // コネクションを確立させたらクリックイベントにバインド
    $.connection.hub.start().done(function () {
        $(function () {
            $("#through-btn").click(_through);
            $("#echo-btn").click(_echo);
            $("#echo2all-btn").click(_echoToAll);
        });
    });
}())