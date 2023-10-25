using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GameLibrary;
using GameLibrary.Common;
using GameLibrary.Extension;
using GameLibrary.Tools;
using Server.Elements;
using Server.Settings;

public class GameServer
{
    private static GameServer instance;
    private readonly object playersLock;
    private readonly CommandHandler commandHandler;

    public static GameServer Instance => instance ?? (instance = new GameServer());

    private GameServer()
    {
        World.Instance.Players = new ConcurrentDictionary<string, NetClient>();
        World.Instance.Rooms = new ConcurrentDictionary<string, Room>();
        playersLock = new object();
        commandHandler = new CommandHandler();
    }

    public async Task StartServer(IPAddress ipAddress, int port)
    {
        IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


        try
        {
            listener.Bind(endPoint);
            listener.Listen(10);
            Logger.Log.Debug("Server started.");
            Logger.Log.Debug($"IP: {ipAddress} | Port: {port}");
            Logger.Log.Debug("Waiting for connections...");

            // Запустить задачу проверки активности игроков
            _ = StartPlayerActivityCheck();

            while (true)
            {
                try
                {
                    Socket clientSocket = await listener.AcceptAsync();
                    NetClient client = new NetClient { Socket = clientSocket };

                    Logger.Log.Debug($"Connected.");
                    client.LastActivityTime = DateTime.Now;

                    /* client.Id = await NetId.Generate();
                     client.Data = new ClientData();
                     //client.Data.Parameters = new PlayerParameters();
                     client.Data.ID = client.Id;
                     

                     Logger.Log.Debug($"{client.Id} connected.");

                     if (!World.Instance.Players.TryAdd(client.Id, client))
                     {
                         Logger.Log.Debug($"Failed to add player {client.Id} to the player list.");
                         client.Socket.Close();
                         continue;
                     }*/

                    _ = HandlePlayer(client);
                }
                catch (Exception ex)
                {
                    Logger.Log.Debug("ErrorConnect: " + ex);
                }
            }
        }
        catch (SocketException ex)
        {
            Logger.Log.Debug($"SocketException occurred: {ex.SocketErrorCode}");
        }
        catch (Exception ex)
        {
            Logger.Log.Debug($"Server error: {ex.Message}");
        }
        finally
        {
            listener.Close();
        }
    }

    private async Task StartPlayerActivityCheck()
    {
        while (true)
        {
            await Task.Delay(TimeSpan.FromMinutes(1)); // Проверять активность каждую минуту

            List<string> inactivePlayers = new List<string>();

            // Получить список неактивных игроков
            lock (playersLock)
            {
                foreach (var player in World.Instance.Players.Values)
                {
                    if (DateTime.Now - player.LastActivityTime > TimeSpan.FromMinutes(5))
                    {
                        inactivePlayers.Add(player.Id);
                    }
                }
            }

            // Удалить неактивных игроков
            foreach (var playerName in inactivePlayers)
            {
                if (World.Instance.Players.TryRemove(playerName, out var inactivePlayer))
                {
                    Logger.Log.Debug($"Player {inactivePlayer.Id} was inactive for more than 5 minutes and has been removed.");
                    inactivePlayer.Socket.Close();
                }
            }
        }
    }

    private async Task HandlePlayer(NetClient player)
    {
        try
        {
            byte[] headerBuffer = new byte[sizeof(int)]; // Буфер для чтения заголовка пакета
            int bytesRead = 0; // Количество прочитанных байт заголовка
            int packetSize = 0; // Размер пакета

            List<byte[]> packetList = new List<byte[]>(); // Текущая пачка пакетов
            Queue<List<byte[]>> packetQueue = new Queue<List<byte[]>>(); // Очередь пачек пакетов

            int MAX_BATCH_SIZE = Settings.Data.MAX_BATCH_SIZE; // Максимальный размер пачки пакетов
            int MAX_CONCURRENT_BATCHES = Settings.Data.MAX_CONCURRENT_BATCHES; // Максимальное количество одновременно обрабатываемых пачек пакетов

            SemaphoreSlim semaphore = new SemaphoreSlim(MAX_CONCURRENT_BATCHES);

            while (player.Socket.Connected)
            {
                int bytesReceived = await player.Socket.ReceiveAsync(headerBuffer, SocketFlags.None);

                if (bytesReceived > 0)
                {
                    bytesRead += bytesReceived;

                    // Если прочитан весь заголовок пакета
                    if (bytesRead == sizeof(int))
                    {
                        packetSize = BitConverter.ToInt32(headerBuffer, 0);

                        // Проверка размера пакета
                        if (packetSize < 0)
                        {
                            Logger.Log.Debug($"Invalid packet received from player {player.Id}: Invalid packet size.");
                            bytesRead = 0;
                            continue;
                        }

                        byte[] packetBuffer = new byte[packetSize];
                        int packetBytesRead = 0;

                        // Чтение пакета
                        while (packetBytesRead < packetSize)
                        {
                            bytesReceived = await player.Socket.ReceiveAsync(packetBuffer, SocketFlags.None);
                            if (bytesReceived == 0)
                            {
                                break; // Клиент отключился, выходим из цикла чтения
                            }
                            packetBytesRead += bytesReceived;
                        }

                        if (packetBytesRead == packetSize)
                        {
                            // Добавить пакет в текущую пачку
                            packetList.Add(packetBuffer);

                            // Если накопилось MAX_BATCH_SIZE пакетов или нет ожидающих пачек, добавить пачку в очередь и запустить обработку
                            try
                            {
                                if (packetList.Count >= MAX_BATCH_SIZE || (packetList.Count > 0 && semaphore.Wait(0)))
                                {

                                    packetQueue.Enqueue(packetList);
                                    packetList = new List<byte[]>();
                                    semaphore.Release();

                                    // Запустить обработку пачки пакетов, если есть свободный слот или в очереди есть пакеты
                                    try
                                    {
                                        if (semaphore.Wait(0) && packetQueue.Count > 0)
                                        {

                                            await ProcessNextPacketBatch(player, packetQueue, semaphore);

                                        }
                                    }
                                    finally
                                    {
                                        semaphore.Release();
                                    }
                                }
                            }
                            finally
                            {
                                semaphore.Release();
                                bytesRead = 0;
                            }
                            bytesRead = 0; // Сбросить счетчик чтения заголовка
                        }
                        else
                        {
                            Logger.Log.Debug($"Invalid packet received from player {player.Id}: Incomplete packet received.");
                            bytesRead = 0;
                        }
                    }
                }
                else
                {
                    break; // Клиент отключился, выходим из цикла чтения
                }
            }
        }
        catch (SocketException ex)
        {
            Logger.Log.Debug($"SocketException occurred for player {player.Id}: {ex.SocketErrorCode}");
        }
        catch (IOException ex)
        {
            Logger.Log.Debug($"IOException occurred for player {player.Id}: {ex.Message}");
        }
        catch (Exception ex)
        {
            Logger.Log.Debug($"Error occurred for player {player.Id}: {ex.Message}");
        }
        finally
        {
            try
            {
                if (World.Instance.Players.TryRemove(player.Id, out var inactivePlayer))
                {
                    Logger.Log.Debug($"{player.Id} disconnected.");
                }

                player.Socket.Close();
            }
            catch (Exception ex)
            {
                Logger.Log.Debug($"Error occurred: {ex.Message}");
            }
        }
    }



    private async Task ProcessNextPacketBatch(NetClient player, Queue<List<byte[]>> packetQueue, SemaphoreSlim semaphore)
    {
        semaphore.Release();
        await semaphore.WaitAsync();

        if (packetQueue.Count > 0)
        {
            List<byte[]> batch = packetQueue.Dequeue();
            await ProcessPacketBatch(player, batch);
        }

        semaphore.Release();
    }

    private async Task ProcessPacketBatch(NetClient player, List<byte[]> packetBatch)
    {
        foreach (byte[] packetData in packetBatch)
        {
            await commandHandler.HandleCommand(player, packetData);
        }
    }

}



public class GameServerApp
{

    [Obsolete]
    public static void Main()
    {
        Server.Settings.Settings.Instance.LoadSettings();
        IPAddress ipAddress = IPAddress.Any;
        int port = Settings.Data.PORT;
        string logConfigFilePath = "log4net.config";
        log4net.Config.XmlConfigurator.Configure(new FileInfo(logConfigFilePath));
        GameServer.Instance.StartServer(ipAddress, port).Wait();
    }

}
