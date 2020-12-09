var http				= require('http');
var express 			= require('express');
var bodyParser 			= require('body-parser');
var expressValidator	= require('express-validator');
var cookieParser 		= require('cookie-parser');
var csrf 				= require('csurf');
var session 			= require('express-session');
var store 				= require('store');
var generalConfig 		= require('./../service/Config/GeneralConfig');
var port 				= 4200;
require('./broker.js');
require('./subscriber.js');

global.app 					= express();
var server 					= http.createServer(app);
global.dateFormat 			= require('dateformat');
global.io 					= require('socket.io')(server);
global._ 					= require('underscore');
global.multipart 			= require('connect-multiparty');
global.multipartMiddleware 	= multipart();

app.use(cookieParser());
app.use(session({
	secret: 'BD7I4um/JuiAxgLkirV6KRT4FVaMq1cLxulaFB5hbC8=',
	resave: false,
	saveUninitialized: false
}));

app.use(function (req, res, next) {
	res.setHeader('Access-Control-Allow-Origin', '*');
	res.setHeader('Access-Control-Allow-Methods', 'GET, POST, OPTIONS, PUT, PATCH, DELETE');
	res.setHeader('Access-Control-Allow-Headers', 'X-Requested-With,content-type');
	res.setHeader('Access-Control-Allow-Credentials', true);
	next();
});

app.set('appPath', __dirname + '/dist');
app.use(express.static(__dirname + '/dist'));

app.use(bodyParser.json({
	limit: "50mb"
}));
app.use(bodyParser.urlencoded({
	limit: "50mb",
	extended: true,
	parameterLimit: 50000
}));
app.use(expressValidator());
app.use(csrf({
	cookie: true
}));
app.use('/images/', express.static(__dirname + '/images'));

app.get('/token/form-token', function (req, res, next) {
	var token = req.csrfToken();
	if (store.get('user_iot') && store.get('user_iot').iot_access_token != undefined) {
		var iot_access_token = store.get('user_iot').iot_access_token;
		var iot_access_token_expires = store.get('user_iot').expires;
		res.send({
			"success"					: true,
			"token"						: token,
			"iot_access_token"			: iot_access_token,
			iot_access_token_expires	: iot_access_token_expires
		});
	} else {
		res.send({
			"success"	: true,
			"token"		: token
		});
	}
});

app.use(function (err, req, res, next) {
	if (err.code !== 'EBADCSRFTOKEN') return next(err)
	res.send({
		"success"	: false,
		"message"	: "Invalid session token.!",
		"token"		: ""
	})
});

app.use('/*', function (req, res, next) {
	res.sendFile(app.get('appPath') + '/index.html');
});

server.listen(port, function () {
	generalConfig.getsubscriberData();
	console.log('Smart Office server is listining on port : ' + port);
});

exports = module.exports = app;