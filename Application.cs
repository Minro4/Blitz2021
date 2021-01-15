using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using Blitz2020;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using Blitz2021;

public static class Application
{
    public static void Main(string[] args)
    {
        Task t = startClient();
        t.Wait();
    }


    public async static Task startClient(string address = "127.0.0.1:8765")
    {
        using (ClientWebSocket webSocket = new ClientWebSocket())
        {
            Uri serverUri = new Uri("ws://" + address);
            Bot bot = new Bot();

            await webSocket.ConnectAsync(serverUri, CancellationToken.None);

            string? token = Environment.GetEnvironmentVariable("TOKEN");
            string registerPayload = token == null
                ? registerPayload = JsonConvert.SerializeObject(new { type = "REGISTER", crewName = Bot.NAME })
                : registerPayload = JsonConvert.SerializeObject(new { type = "REGISTER", token = token });


            await webSocket.SendAsync(
                new ArraySegment<byte>(Encoding.UTF8.GetBytes(registerPayload)),
                WebSocketMessageType.Text, true, CancellationToken.None);

            while (webSocket.State == WebSocketState.Open)
            {

                DefaultContractResolver contractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                };

                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.ContractResolver = contractResolver;
                settings.Converters.Add(new StringEnumConverter());

                GameMessage message = JsonConvert.DeserializeObject<GameMessage>(await readMessage(webSocket), new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    Formatting = Formatting.Indented
                });

                if (message != null)
                {
                    List<string> errors = message.getCrewsMapById[message.crewId].errors;
                    errors.ForEach(e => Console.WriteLine("Command error: " + e));

                    GameCommand command = bot.nextMove(message);
                    string serializedCommand = JsonConvert.SerializeObject(new { type = "COMMAND", actions = command.actions, tick = message.tick }, settings);

                    await webSocket.SendAsync(
                        Encoding.UTF8.GetBytes(serializedCommand),
                        WebSocketMessageType.Text,
                        true, CancellationToken.None);
                }

            }
        }
    }

    public async static Task<string> readMessage(ClientWebSocket client)
    {
        string result = "";

        WebSocketReceiveResult receiveResult;
        do
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(new byte[1024]);
            receiveResult = await client.ReceiveAsync(segment, CancellationToken.None);
            result += Encoding.UTF8.GetString(segment.Array);
        } while (!receiveResult.EndOfMessage);


        return result;
    }
}