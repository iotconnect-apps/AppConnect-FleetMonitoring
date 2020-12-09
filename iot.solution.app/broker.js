var mosca = require('mosca');
var settings = {
	port: 1884,
	cleansession: true
}

var server = new mosca.Server(settings);

server.on('clientConnected', function (client) {
	console.log('Client Connected:', client.id);
});

server.on('ready', function () { });