
const express = require('express')
const app = express()
const server = require('http').createServer(app);
const WebSocket = require('ws');

const wss = new WebSocket.Server({server:server});

wss.on('connection',function connection(ws){

    console.log("a new Client Connected")

    ws.on('message',function incoming(message){

        console.log('received: %s', message);

        wss.clients.forEach(function each(client){

            if(client != ws && client.readyState === WebSocket.OPEN){
                client.send(message.toString());
            }
         });
});
});

//Get index.html
app.get('/', (req,res)=>res.send("Hello World! From WSS"));



server.listen(10000, ()=>console.log("Listening on port 10000"));





