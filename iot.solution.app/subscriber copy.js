var mqtt 			= require('mqtt');
var generalConfig 	= require('./../service/Config/GeneralConfig');
var conn 			= require('./../service/Config/mysql-connection');
var connection 		= conn.connection;

setInterval(function () {
	connection.query('SELECT 1');
}, 10000);

var client = mqtt.connect('mqtt://192.168.3.75');

client.on('connect', function () {
	console.log("Connection Start");
	client.subscribe('#');
})

client.on('message', mqtt_messsageReceived);

function mqtt_messsageReceived(topic, message, packet) {
	var message_str = message.toString(); //convert byte array to string
	message_str = message_str.replace(/\n$/, ''); //remove new line
	//payload syntax: clientID,topic,message
	if (countInstances(message_str) != 1) {
		console.log("Invalid payload");
	} else {
		insert_message(topic, message_str, packet);
	}
};

//insert a row into the tbl_messages table
function insert_message(topic, message_str, packet) {
	var message_arr = extract_string(message_str);
	var UserID = message_arr[0];
	var company_databasename = UserID.split("#");
	var Username = company_databasename[1];
	var UserID = company_databasename[0];
	company_databasename = company_databasename[2];

	var message = message_arr[1];
	var created = generalConfig.getDateTimeUTC();

	var query = "INSERT INTO " + company_databasename + ".`mqttchats` (`userID`, `topic`,`userName`,`message`, `created_at`) VALUES ('" + UserID + "', '" + topic + "','" + Username + "', '" + message + "', '" + created + "')";

	connection.query(query, function (error, results) {
		if (error) throw error;
		console.log("Message added: " + message_str);
	});
};

//split a string into an array of substrings
function extract_string(message_str) {
	var message_arr = message_str.split(","); //convert to array	
	return message_arr;
};

//count number of delimiters in a string
var delimiter = ",";
function countInstances(message_str) {
	var substrings = message_str.split(delimiter);
	return substrings.length - 1;
};