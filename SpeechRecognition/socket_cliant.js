    //Web socket
    var ws = new WebSocket("ws://" + window.location.host + "/java_ee_example/websocket_simple")
    
    ws.onopen = function(){
        console.log("websocket connected")
    };

    ws.onmessage = function(data){
        $("#WebSocket")
    };

    $("#send").click(function(){
        var text = $("#input").val()
        ws.send(text)
    });



