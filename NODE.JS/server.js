var net = require('net');
const { StringDecoder } = require('string_decoder');
const decoder = new StringDecoder('utf8');

var _port = 8000;
var IDcounter = -1;

function RandomInt(min, max){
	return Math.floor(Math.random() * (max - min + 1)) + min;
}
function RandomFloat(min, max){
	return (Math.random() * (max - min + 1)) + min;
}

var clients = [];

function Client(_socket, _id){
	this.nickname = '<noname>';
	this.socket = _socket;
	this.ID = _id;
}

var s = net.Server(function (socket) {
	console.log("Client connected");
	IDcounter++;
	_client = new Client(socket, ""+IDcounter);
	_client.socket.write('["SYSTEM","ID","'+ IDcounter +'"]');
	clients.push(_client);

	socket.on('data', function (msg) {
		var msg2 = ''+msg;
		msg2 = decoder.write(msg); // декодируем из UTF8
		console.log("Input: " + msg2); 
		data = "" + msg2;
		var j = JSON.parse(data);
		for (var i = 0; i<j.length; i++){
			console.log("	#"+i+" = "+j[i]);
		}

		if (j[0] === 'MESSAGE'){
			// For now, just send the message to everyone
			clients.forEach( function(c){
				c.socket.write(msg);
				console.log('Message: ' + msg2);
			});
		}

		if (j[0] === 'SYSTEM'){
			if (j[1] === 'NICKNAME'){
				clients.forEach( function(c){
					if (c.ID === j[2]){
						c.nickname = j[3];
						console.log(c.ID + ' set nick ' + c.nickname + ' ('+j[3]);
					}
				})
			}
			if (j[1] === 'LIST'){
				var s = '["SYSTEM","USERS","'+j[2]+'","';
				clients.forEach( function(c){
					s += c.nickname+'\n';
				});
				s += '"]';
				console.log('== '+s);
				clients.forEach( function(c){
					if (c.ID === j[2]) {
						c.socket.write(s);
						console.log('Message to: ' + j[2] + ' (' + c.ID + ') ' + s);
					}
				});
	
				}
		}

	});

	socket.on('error', function() {});

	socket.on('end', function () {
		console.log("Client disconnected");
		var i = clients.indexOf(socket);
		for (var i in clients) 
			if (clients[i].socket === socket)
				clients.splice(i, 1);

		for (var i in clients) {
			try {
				clients[i].socket.write("user left");
			}
			catch (ex) {}
		}
	});
});

s.listen(_port);

//* Handle ctrl+c event
process.on('SIGINT', function () {
	for (var i in clients) {
		clients[i].write("Server shutdown");
		clients[i].destroy();
	}
	console.log('Exiting properly');
	process.exit(0);
});

console.log("Chat Server listening " + s.address().address + ":" + s.address().port);