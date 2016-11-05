var express =require('express');
var app = express();
var http  = require("http").Server(app);

var fs    = require('fs');
var ws    = require('ws').Server;

var webPort = 3001;

app.use(express.static(__dirname));

app.get('/',function(req, res){
    //        res.sendfile('sr.html');
});

app.get('/result', function(req, res){
    res.sendfile('result.html');
});

http.listen(webPort, function(){
        console.log('listening on *:' + webPort);
});


/*
http.createServer(function(req, res) {
    fs.readFile('index.html', function(err, data) {
	
	res.writeHead(200, {'Content-Type': 'text/html'});
	res.end(data.toString());
	//res.end('Hello World\n');
    });
}).listen(webPort);
console.log('Server running at http://localhost:' + webPort + '/');

*/


var unityWebSocket = null;
var srWebSocket = null;

var unityServer = new ws({port: 3000});
var srServer = new ws({port: 3002});

unityServer.on('connection', function(ws) {
    console.log('Unity connected!');
    unityWebSocket = ws;
    ws.on('message', function(message) {
	console.log("from Unity=> "+message);
	console.log("to "+  srWebSocket + "=> " + message);
	srWebSocket.send(message);
    });
    ws.on('close', function() {
	console.log('Unity disconnected...');
	unityWebSocket = null;
    });
});


srServer.on('connection', function(ws) {
    console.log('Browser connected!');
    srWebSocket = ws;
    ws.on('message', function(message) {
	console.log("from browser=> "+message);
	if(unityWebSocket){
	    console.log("to "+  unityWebSocket + "going to send => " + message);
	    unityWebSocket.send(message);
	}else{
	    console.log("There is no connection to Unity");
	}
    });
    ws.on('close', function() {
	console.log('Browser disconnected...');
	srWebSocket = null;
    });
});

