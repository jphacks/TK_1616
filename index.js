var express =require('express');
var app = express();
var http = require('http').Server(app);
var io =require('socket.io')(http);

app.use(express.static(__dirname));

app.get('/',function(req, res){
        res.sendfile('index.html');
    });

io.on('connection', function(socket){

        socket.on('unity', function(){
            socket.emit('Start_rec');
        });

        socket.on('result', function(msg){
                socket.emit('ShowResult', {value: msg.value});
        });
});



// サーバーをポート3000番で起動
http.listen(3000, function(){
        console.log('listening on *:3000');
});
