(function() {
    var connection = new signalR
        .HubConnectionBuilder()
        .withUrl("/chat")
        .build();

    async function start() {
        try {
            await connection.start();
            await connection.invoke("LoadMessages");
        } catch (e) {
            console.error(e);
            setTimeout(start, 5000);
        }
    };

    connection.onclose(start);

    start();

    var index = 0;
    var submitButton = document.querySelector("#chat-submit");
    submitButton.addEventListener("click",
        async function(e) {
            e.preventDefault();
            var message = $("chat-input").val();
            if (message.trim() === "") {
                return false;
            }

            await connection.invoke("Send", message);
        });

    connection.on("NewMessage",
        function(roomMessages) {
            if (Array.isArray(roomMessages)) {
                $(".chat-logs").val("");
                for (var i = 0; i < roomMessages.length; i++) {
                    generate_message(roomMessages[i]);
                }
            } else {
                generate_message(roomMessages);
            }
        });

    function generate_message(message) {
        index++;
        var str = "";
        var type = "";

        if (message.isByRoomOwner) {
            type = "self";
        } else {
            type = "user";
        }

        str += "<div id='cm-msg-" + index + "' class=\"chat-msg " + type + "\">";
        str += "          <div class=\"cm-msg-text\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Baraka\">";
        str += escapeHtml(msg.message);
        str += "          <\/div>";
        str += "        <\/div>";
        $(".chat-logs").append(str);
        $("#cm-msg-" + index).hide().fadeIn(300);
        if (type == 'self') {
            $("#chat-input").val('');
        }
        $(".chat-logs").stop().animate({ scrollTop: $(".chat-logs")[0].scrollHeight }, 1000);
    }


    $("#chat-circle").click(function() {
        $("#chat-circle").toggle('scale');
        $(".chat-box").toggle('scale');
        $(".chat-logs").stop().animate({ scrollTop: $(".chat-logs")[0].scrollHeight }, 1000);
    })

    $(".chat-box-toggle").click(function() {
        $("#chat-circle").toggle('scale');
        $(".chat-box").toggle('scale');
    })

    function escapeHtml(unsafe) {
        return unsafe
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }
})();
