const connection = new signalR.HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Debug)
    .withUrl("https://localhost:44319/Index")
    .configureLogging(signalR.LogLevel.Information)
    .build();
connection.start().then(() => {
});
//define unique id for send message
var receiverId = function (id) {
    $('#chat-with-id').text(id);
}
var sendMessage = function () {
    const message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", message, $('#chat-with-id').text()).then((result) => {
    }).catch(err => console.error(err.toString()));
    event.preventDefault();
}
document.getElementById("sendButton").addEventListener("click", event => {
    sendMessage();
});
$("#messageInput").keydown(function (e) {
    if (e.keyCode === 13) {
        sendMessage();
        e.preventDefault();
    }
});
connection.on("OnlineUserList", (connectionId) => {
    console.log(connectionId)
    $('#onlineUsersList').append('<li class= "active" onclick=receiverId(' + "'" + connectionId + "'" + ')>' +
        '<div class="d-flex bd-highlight">' +
        '<div class="img_cont">' +
        '<img src="http://www.findandsolve.com/icon.png" alt="find and solve" class="rounded-circle user_img">' +
        '<span class="online_icon"></span></div>' +
        '<div class="user_info">' +
        '<span>Unique User Id</span>' +
        '<p>' + connectionId + '</p></div> </div></li>'
    )
});
connection.on("OwnMessage", (message) => {
    console.log('ownmessage');
    console.log(message);
    $('#messageListId').append('<li><div class="d-flex justify-content-end mb-4">' +
        '<div class= ""msg_cotainer">' + message + '<span class= "msg_time_send" ></span></div >' +
        '<div class="img_cont_msg">' +
        '<img src="http://www.findandsolve.com/icon.png" alt="find and solve" class="rounded-circle user_img_msg"> </div> </div></li>')
});
connection.on("ReceiveMessage", (message, senderId) => {
    $('#chat-with-id').text(senderId);
    $('#messageListId').append('<li><div class="d-flex justify-content-start mb-4">' +
        '<div class="img_cont_msg">' +
        '<img src="http://www.findandsolve.com/icon.png" alt="find and solve" class="rounded-circle user_img_msg"> </div> ' +
        '<div class= ""msg_cotainer">' + message + '<span class= "msg_time_send" ></span></div >' +
        '</div ></li>')
});
