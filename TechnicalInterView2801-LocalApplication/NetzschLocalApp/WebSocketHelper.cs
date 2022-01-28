using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace NetzschLocalApp
{
    internal class WebSocketHelper : IDisposable
    {
        MainWindow mw;
        public String outMessage{get;set;}
        public String inMessage{get;set;}
        public WebSocketHelper(MainWindow MW)
        {
            mw = MW;
        }
      
        
        public int ReceiveBufferSize { get; set; } = 8192;

        //Connects asynchronously to a Websocket a url
        public async Task ConnectAsync(string url)
        {
            if (WebSocket != null)
            {   //if a websocket is already open return it else dispose of it and start creating a new one
                if (WebSocket.State == WebSocketState.Open) return;
                else WebSocket.Dispose();
            }
            WebSocket = new ClientWebSocket();

            if (CTS != null) CTS.Dispose();
            CTS = new CancellationTokenSource();
            //Connect to the Socket
            await WebSocket.ConnectAsync(new Uri(url), CTS.Token);
            //Start the receive loop
            await Task.Factory.StartNew(ReceiveLoop, CTS.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
        
        //Disconnects the sockets
        public async Task DisconnectAsync()
        {   
            if (WebSocket is null) return;
            
            if (WebSocket.State == WebSocketState.Open)
            {
                CTS.CancelAfter(TimeSpan.FromSeconds(2));
                await WebSocket.CloseOutputAsync(WebSocketCloseStatus.Empty, "", CancellationToken.None);
                await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }
            Clear();
        }

        //Disposes and nulls Websocket and CTS
        private void Clear()
        {
            WebSocket.Dispose();
            WebSocket = null;
            CTS.Dispose();
            CTS = null;
        }

        //async loop that receives data from websocket
        private async Task ReceiveLoop()
        {
            var loopToken = CTS.Token;
            MemoryStream outputStream = null;
            WebSocketReceiveResult receiveResult = null;
            var buffer = new byte[ReceiveBufferSize];
            try
            {
                //While no Cancellation is Requested loop
                while (!loopToken.IsCancellationRequested)
                {
                    outputStream = new MemoryStream(ReceiveBufferSize);
                    do
                    {   //waits for ReceiveAsync to trigger
                        receiveResult = await WebSocket.ReceiveAsync(buffer, CTS.Token);
                        if (receiveResult.MessageType != WebSocketMessageType.Close)
                            outputStream.Write(buffer, 0, receiveResult.Count);
                    }
                    while (!receiveResult.EndOfMessage);
                    if (receiveResult.MessageType == WebSocketMessageType.Close) break;
                    outputStream.Position = 0;
                    ResponseReceived(buffer, receiveResult.Count);
                }
            }
            catch (TaskCanceledException) { }
            finally
            {
                
                outputStream?.Dispose();
            }
        }

        //Very basic method for sending raw Messages using tcp/ip
        public async Task<byte[]> SendMessageAsync<RequestType>(RequestType sMessage)
        {
            
            //encode the data
            byte[] ba = System.Text.Encoding.ASCII.GetBytes(sMessage.ToString());
            //Send the data to websocket
            await WebSocket.SendAsync(ba, 0, true, CTS.Token);
            return ba;
            
        }

        //Sends the response String to MainViewModel
        private void ResponseReceived(byte[] data, int i)
        {   
            //Decodes the message
            String message = System.Text.Encoding.ASCII.GetString(data, 0, i);
            //Triggers on PropertyChang
            //mw.OnPropertyChanged(new PropertyChangedEventArgs("inStringTB"));
            //Sends Data to MainViewModel and Triggers OnPropertyChanged
            mw.ChangeTB(message);

        }

        public void Dispose() => DisconnectAsync().Wait();

        private ClientWebSocket WebSocket;
        private CancellationTokenSource CTS;
    }
}
