
// initialize websockets:
var ws_uri = "ws://127.0.0.1:10000";
var websocket = new WebSocket(ws_uri);


// on websocket open:
websocket.onopen = function(event) {
    console.log("Socket Opened");
};


// on websocket close:
websocket.onclose = function(event) {
    console.log("Socket Closed");
};


// on websocket error:
websocket.onerror = function(event) {
    console.log("Failed to Connect to Socket");
	console.log(event);
};


// on websocket message received:
websocket.onmessage = function(event) {
	
		
        document.getElementById("inString").value = '';
        document.getElementById("outString").value = event.data;
	
};


// on Enterpress send event to websocket
// on Enterpress send event to websocket
document.getElementById("inString").addEventListener("keypress", function(event) {
    if(event.key === "Enter"){
    
    var message = document.getElementById("inString").value;
		
        MessageSend(message);
    
}}, false);


// send Message to Client
function MessageSend(message) {
    websocket.send(message);
}

