var express =require('express');
var app = express();
var http = require('http').Server(app);
var io =require('socket.io')(http);

app.use(express.static(__dirname));

app.get('/',function(req, res){
        res.sendfile('index.html');
    });

app.get('/result', function(req, res){
        res.sendfile('result.html');
    });

io.sockets.on('connection', function(socket){
    console.log("socket-> "+socket);
        socket.on('unity', function(){
            console.log("OK1");
            io.sockets.emit('start_rec');
        });

        socket.on('result', function(msg){
                console.log(msg);
                io.sockets.emit('ShowResult', msg);
        });
});


http.listen(3000, function(){
        console.log('listening on *:3000');
});
