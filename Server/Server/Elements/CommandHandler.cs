using GameLibrary;
using Server.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using GameLibrary.Tools;
using GameLibrary.Common;
using GameLibrary.Extension;
using log4net;
using GameLibrary.Extension;
using MySql.Data;
using MySql.Data.MySqlClient;

public class CommandHandler
{
    
    NetClient _client;
    
    public async Task HandleCommand(NetClient client, byte[] data)
    {
        _client = client;

        _client.LastActivityTime = DateTime.Now;
        
        try
        {
                DataPacket packet = await Serializer.DeserializeAsync<DataPacket>(data);

            if (packet.Rpc == false)
            {
                switch ((OperationCode)packet.operationCode)
                {
                    case OperationCode.Unknown:
                        Logger.Log.Debug($"Unknown command received from {client.Id}");
                        break;

                    case OperationCode.Disconnect:
                        try
                        {
                           // if (World.Instance.Rooms[World.Instance.Players[client.Id].TeamUUID].Team.Single(r => r.netClient.Id == client.Id) != null) // Team.Single(r => r.netClient.Id == client.Id)
                            //World.Instance.Rooms[World.Instance.Players[client.Id].TeamUUID].RemoveUser(client.Id);
                           //if (World.Instance.Players.ContainsKey(client.Id))
                                //World.Instance.removeClient(client.Id);
                            Logger.Log.Debug($"{client.Id} requested disconnection");
                        }
                        catch (Exception ex) { Logger.Log.Error($"Disconnected error: {ex}"); }


                        break;

                    case OperationCode.SetDamage:
                        await SendTo(client, new DataPacket((byte)OperationCode.SetDamage, new Dictionary<byte, object> { { (byte)ParameterCode.Count, packet } }));
                        break;

                    case OperationCode.Message:
                        try
                        {
                            if (packet.Data != null)
                            {
                                //Logger.Log.Debug($"{client.Id} sent a message: {packet.Data[(byte)ParameterCode.Message]}");
                                await SendTo(client, new DataPacket((byte)OperationCode.Message, packet.Data));
                            }
                        }
                        catch (Exception ex) { Logger.Log.Error($"MessageError: {ex}"); }
                        break;

                    case OperationCode.Connect:
                        //client.Data.Parameters.Name = packet.Data[ParameterCode.Message].ToString();
                        //client.Data.pp = NetVector3(0,0,0);
                        try
                        {
                            //if (!World.Instance.Players.ContainsKey(client.Id))
                            //{
                                //World.Instance.addClient(client);
                           // }
                            //else
                           // {

                               // World.Instance.Players[client.Id] = client;

                           // }
                            await SendTo(client, new DataPacket((byte)OperationCode.Connect, new Dictionary<byte, object> { { (byte)ParameterCode.Message, "Connect" } }));
                            Logger.Log.Debug($"Player Connected!");
                        }
                        catch (Exception ex) { Logger.Log.Error($"ConnectionError: {ex}"); }

                        //World.Instance.getClient(client.Id).Data.NetTransform = new NetTransform ();
                        //World.Instance.getClient(client.Id).Data.NetTransform.Position = new NetVector3(1, 2, 3);
                        //await SendTo(client, new DataPacket(OperationCode.GetClientsInfo, new Dictionary<ParameterCode, object> { { ParameterCode.Client, World.Instance.getClient(client.Id).Data } }));

                        
                        break;

                    case OperationCode.Authorisation:

                        using (MySqlConnection con = new MySqlConnection(DB.pathsql))
                        {
                            con.Open();
                            using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM Accounts WHERE Login = @Login;", con))
                            {
                                cmd.Parameters.AddWithValue("@Login", packet.Data[(byte)ParameterCode.Login].ToString());
                                object existingCount = cmd.ExecuteScalar();
                                using (MySqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (existingCount == null)
                                    {

                                        await SendTo(client, new DataPacket((byte)OperationCode.Authorisation, new Dictionary<byte, object>
                                        {
                                            { (byte)ParameterCode.Message, "Error" }
                                        }));

                                    }
                                    else
                                    {
                                        while (reader.Read())
                                        {
                                            try
                                            {
                                                if (!World.Instance.Players.ContainsKey(reader["UUID"].ToString()))
                                                {

                                                    if (reader["Login"].ToString() == packet.Data[(byte)ParameterCode.Login].ToString() &&
                                                    reader["Password"].ToString() == packet.Data[(byte)ParameterCode.Password].ToString())
                                                    {
                                                    
                                                        client.Id = reader["UUID"].ToString();
                                                        client.Data = new ClientData();
                                                        client.Data.ID = client.Id;

                                                        using (MySqlConnection con1 = new MySqlConnection(DB.pathsql))
                                                        {
                                                            con1.Open();
                                                            using (MySqlCommand infoCmd = new MySqlCommand(
                                                                "SELECT UP.UUID, UP.Name, UP.Lvl, UP.Exp, UI.IconID, I.Name AS IconName, I.URL AS IconURL, I.Description AS Description " +
                                                                "FROM UserParameters UP " +
                                                                "LEFT JOIN UserIcons UI ON UP.UUID = UI.UUID " +
                                                                "LEFT JOIN Icons I ON UI.IconID = I.id " +
                                                                "WHERE UP.UUID = @UUID;", con1))
                                                            {
                                                                infoCmd.Parameters.AddWithValue("@UUID", client.Data.ID);

                                                                using (MySqlDataReader infoReader = infoCmd.ExecuteReader())
                                                                {
                                                                    while (infoReader.Read())
                                                                    {

                                                                        client.Data.Name = infoReader["Name"].ToString();
                                                                        client.Data.Icon = infoReader["IconID"].ToString();
                                                                        client.Data.Exp = infoReader["Exp"].ToString();
                                                                        client.Data.Lvl = infoReader["Lvl"].ToString();
                                                                        //string iconName = infoReader["IconName"].ToString();
                                                                        //string iconURL = infoReader["IconURL"].ToString();
                                                                        //string iconDescription = infoReader["Description"].ToString();

                                                                    }
                                                                }
                                                            }
                                                        }

                                                        //добавляем в world
                                                        if (!World.Instance.Players.TryAdd(client.Id, client))
                                                        {
                                                            Logger.Log.Debug($"Failed to add player {client.Id} to the player list.");
                                                            client.Socket.Close();
                                                            continue;
                                                        }
                                                        Logger.Log.Debug($"ID: {client.Id} Count: {World.Instance.Players.Count} ");
                                                        await SendTo(client, new DataPacket((byte)OperationCode.Authorisation, new Dictionary<byte, object>
                                                        {
                                                            { (byte)ParameterCode.Name, client.Data.Name },
                                                            { (byte)ParameterCode.Message, "Success" },
                                                            { (byte)ParameterCode.Id, client.Data.ID },
                                                            { (byte)ParameterCode.LVL, client.Data.Lvl },
                                                            { (byte)ParameterCode.Exp, client.Data.Exp },
                                                            { (byte)ParameterCode.IconIndex, client.Data.Icon
                                                        }}));
                                                    }
                                                    else 
                                                    {
                                                        await SendTo(client, new DataPacket((byte)OperationCode.Authorisation, new Dictionary<byte, object>{{ (byte)ParameterCode.Message, "Access Denied" }}));

                                                    }
                                                }
                                                else
                                                {
                                                    await SendTo(client, new DataPacket((byte)OperationCode.Authorisation, new Dictionary<byte, object>{{ (byte)ParameterCode.Message, "Error: Client is already authorized" } }));
                                                    await SendTo(World.Instance.getClient(reader["UUID"].ToString()), new DataPacket((byte)OperationCode.Message, new Dictionary<byte, object> { { (byte)ParameterCode.Message, "Произошла попытка входа в ваш аккаунт! Если это не вы, срочно измените пароль!!!" } }));
                                                }
                                                
                                            }
                                            catch (Exception ex)
                                            {
                                                await SendTo(client, new DataPacket((byte)OperationCode.Authorisation, new Dictionary<byte, object>
                                            {
                                                { (byte)ParameterCode.Message, "Error" }
                                            }));
                                            }
                                        }
                                    }



                                }
                            }
                        }
                        break;

                    case OperationCode.Registration:

                        Logger.Log.Debug($"ID: {client.Id} Count: {World.Instance.Players.Count} ");
                        using (MySqlConnection con = new MySqlConnection(DB.pathsql))
                        {
                            con.Open();
                            // Проверяем, существует ли уже логин
                            using (MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM Accounts WHERE Login = @Login;", con))
                            {
                                checkCmd.Parameters.AddWithValue("@Login", packet.Data[(byte)ParameterCode.Login]);
                                int existingCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                                if (existingCount > 0)
                                {
                                    await SendTo(client, new DataPacket((byte)OperationCode.Registration, new Dictionary<byte, object>
                {
                    { (byte)ParameterCode.Message, "Login already exists" }
                }));
                                }
                                else
                                {
                                    // Логин не существует, регистрируем пользователя
                                    using (MySqlCommand registerCmd = new MySqlCommand("INSERT INTO Accounts (UUID, Login, Password, Name) VALUES (@UUID, @Login, @Password, @Name);", con))
                                    {
                                        client.Id = await NetId.Generate();
                                        registerCmd.Parameters.AddWithValue("@UUID", client.Id);
                                        registerCmd.Parameters.AddWithValue("@Login", packet.Data[(byte)ParameterCode.Login]);
                                        registerCmd.Parameters.AddWithValue("@Password", packet.Data[(byte)ParameterCode.Password]); // В реальной практике пароль должен быть хеширован
                                        registerCmd.Parameters.AddWithValue("@Name", packet.Data[(byte)ParameterCode.Name]);



                                        int rowsAffected = await registerCmd.ExecuteNonQueryAsync();
                                        if (rowsAffected > 0)
                                        {

                                            client.Data = new ClientData();
                                            client.Data.ID = client.Id;
                                            if (!World.Instance.Players.TryAdd(client.Id, client))
                                            {
                                                Logger.Log.Debug($"Failed to add player {client.Id} to the player list.");
                                                client.Socket.Close();
                                                break;
                                            }



                                            await SendTo(client, new DataPacket((byte)OperationCode.Registration, new Dictionary<byte, object>
                                        {
                                            {(byte) ParameterCode.Name, packet.Data[(byte)ParameterCode.Name] },
                                            { (byte)ParameterCode.Message, "Registration successful" },
                                            { (byte)ParameterCode.Id, client.Id },
                                            { (byte)ParameterCode.LVL, 1 },
                                            { (byte)ParameterCode.Exp, 0 },
                                            { (byte)ParameterCode.IconIndex, 1 },
                                        }));
                                        }
                                        else
                                        {
                                            await SendTo(client, new DataPacket((byte)OperationCode.Registration, new Dictionary<byte, object>
                                        {
                                            { (byte)ParameterCode.Message, "Registration failed" }
                                        }));
                                        }
                                    }
                                    using (MySqlCommand insertUserParametersCommand = new MySqlCommand("INSERT INTO UserParameters (UUID, Name, Lvl, Exp, Icon, Birth) VALUES (@UUID, @Name, @Lvl, @Exp, @Icon, @Birth)", con))
                                    {
                                        insertUserParametersCommand.Parameters.AddWithValue("@UUID", client.Data.ID);
                                        insertUserParametersCommand.Parameters.AddWithValue("@Name", packet.Data[(byte)ParameterCode.Name]);
                                        insertUserParametersCommand.Parameters.AddWithValue("@Lvl", 1);
                                        insertUserParametersCommand.Parameters.AddWithValue("@Exp", 0);
                                        insertUserParametersCommand.Parameters.AddWithValue("@Icon", 1);
                                        insertUserParametersCommand.Parameters.AddWithValue("@Birth", 0000 - 00 - 00);

                                        int rowsAffectedUserParametersCommand = await insertUserParametersCommand.ExecuteNonQueryAsync();

                                        if (rowsAffectedUserParametersCommand <= 0)
                                        {
                                            Logger.Log.Debug("Error Add PlayerParameters in Database");
                                        }
                                    }
                                    using (MySqlCommand insertUserIconsCommand = new MySqlCommand("INSERT INTO UserIcons (UUID, IconID) VALUES (@UUID, @IconID)", con))
                                    {
                                        insertUserIconsCommand.Parameters.AddWithValue("@UUID", client.Data.ID);
                                        insertUserIconsCommand.Parameters.AddWithValue("@IconID", 1);

                                        int rowsAffectedUserIcons = await insertUserIconsCommand.ExecuteNonQueryAsync();

                                        if (rowsAffectedUserIcons <= 0)
                                        {
                                            Logger.Log.Debug("Error Add UserIcon in Database");
                                        }
                                    }

                                }
                            }
                        }
                        break;

                    case OperationCode.GetInfoRoom:
                        try
                        {
                            Logger.Log.Debug("GetAllRoom");
                            Logger.Log.Debug($"RoomUUID{World.Instance.Players[_client.Id].TeamUUID}");
                            Logger.Log.Debug($"RoomCount{World.Instance.Rooms.Count}");
                            Logger.Log.Debug($"GetAllRoom{World.Instance.GetRoom(World.Instance.Players[_client.Id].TeamUUID).UUID}");
                            await SendTo(new DataPacket((byte)OperationCode.GetInfoRoom, new Dictionary<byte, object>
                                        {
                                            { (byte)ParameterCode.TeamMember,  World.Instance.GetRoom(World.Instance.Players[_client.Id].TeamUUID)},
                                        }), packet.Flag);
                        }
                        catch (Exception ex) { Logger.Log.Error($"GetInfoRoomError: {ex}"); }

                        break;

                    case OperationCode.MyTransform:

                        /*await SendTo(client, new DataPacket(OperationCode.MyTransform, new Dictionary<ParameterCode, object>
                                            {
                                                { ParameterCode.X, packet.Data[ParameterCode.X] },
                                                { ParameterCode.Y, packet.Data[ParameterCode.Y] },
                                                { ParameterCode.Z, packet.Data[ParameterCode.Z] }
                                            }));*/
                        try
                        {
                            await SendTo(new DataPacket((byte)OperationCode.MyTransform, new Dictionary<byte, object>
                                        {
                                            { (byte)ParameterCode.X, packet.Data[(byte)ParameterCode.X] },
                                            { (byte)ParameterCode.Y, packet.Data[(byte)ParameterCode.Y] },
                                            {(byte)ParameterCode.Z, packet.Data[(byte)ParameterCode.Z] },
                                            { (byte)ParameterCode.Id, packet.Data[(byte)ParameterCode.Id] }
                                        }), packet.Flag);
                        }
                        catch (Exception ex) { Logger.Log.Error($"MyTransformError: {ex}"); }

                        break;

                    //Добавляемся в новую тиму соло
                    case OperationCode.SetTeam:
                        try
                        {
                            Logger.Log.Debug("Setteam");
                            //Если группа уже есть, то выходим
                            if (World.Instance.Players[_client.Id].TeamUUID != "0" && World.Instance.Players[_client.Id].TeamUUID != null && World.Instance.Players[_client.Id].TeamUUID != "")
                            {
                                Logger.Log.Debug($"Deleteteam{World.Instance.Players[_client.Id].TeamUUID}");
                                try
                                {
                                    World.Instance.Rooms[World.Instance.Players[_client.Id].TeamUUID].RemoveUser(_client.Id);
                                }
                                catch (Exception ex) { }
                            }
                            //заходим в новую
                            Logger.Log.Debug($"TeamSetMyUUID{_client.Id}");
                            World.Instance.Players[_client.Id].TeamUUID = World.Instance.FindRoomForMember(_client.Id);
                            _client.TeamUUID = World.Instance.Players[_client.Id].TeamUUID;
                            Logger.Log.Debug($"TeamSetTeamUUID{_client.TeamUUID}");
                            Logger.Log.Debug($"TeamSetMyUUID{_client.Id}");
                            Logger.Log.Debug("AddNewteam");
                            await SendTo(new DataPacket((byte)OperationCode.SetTeam, new Dictionary<byte, object>
                                        {
                                            { (byte)ParameterCode.TeamUUID, World.Instance.Players[_client.Id].TeamUUID },
                                            { (byte)ParameterCode.UUID, World.Instance.Rooms[_client.TeamUUID].GetTeamMember(_client.Id) }
                                        }), SendClientFlag.FullRoom);
                        }
                        catch (Exception ex) { Logger.Log.Error($"SetTeamError: {ex}"); }
                    break;

                    default:
                        break;
                }
            }
            else {
                try
                {
                    await SendTo(packet, packet.Flag);
                }
                catch (Exception ex) { Logger.Log.Error($"RPCError: {ex}"); }


            }
                //await SendToAllPlayers(data);
            
        }
        catch (SocketException ex)
        {
            Logger.Log.Debug($"SocketException occurred for player: {ex.SocketErrorCode}");
        }
        catch (IOException ex)
        {
            Logger.Log.Debug($"IOException occurred for player: {ex.Message}");
        }
        catch (Exception ex)
        {
            //тут ошибка
            Logger.Log.Debug($"Error occurred for player Handler: {ex.Message}");
        }
    }

    public async Task SendTo(NetClient client, DataPacket packet)
    {
        try
        {
            var serializedPacket = await Serializer.SerializeAsync(packet);

            // Получите размер данных пакета в виде байтового массива
            byte[] sizeBytes = BitConverter.GetBytes(serializedPacket.Length);

            // Объедините размер данных и сам пакет в один буфер
            byte[] buffer = sizeBytes.Concat(serializedPacket).ToArray();

            //Console.WriteLine("BUFFER: " + buffer.Length);

            // Отправьте данные клиенту
            await client.Socket.SendAsync(buffer, SocketFlags.None);
        }
        catch (SocketException ex)
        {
            Logger.Log.Debug($"SocketException occurred while sending data: {ex.SocketErrorCode}");
        }
        catch (IOException ex)
        {
            Logger.Log.Debug($"IOException occurred while sending data: {ex.Message}");
        }
        catch (Exception ex)
        {
            Logger.Log.Debug($"Error occurred while sending data: {ex.Message}");
        }
    }
   

    public async Task SendTo( DataPacket packet, SendClientFlag Flag = SendClientFlag.Me)
    {
        switch (Flag)
        {

            case SendClientFlag.Me:

                try
                {
                    var serializedPacket = await Serializer.SerializeAsync(packet);
                    byte[] sizeBytes = BitConverter.GetBytes(serializedPacket.Length);
                    byte[] buffer = sizeBytes.Concat(serializedPacket).ToArray();

                    await _client.Socket.SendAsync(buffer, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    Logger.Log.Debug($"SocketException occurred while sending data: {ex.SocketErrorCode}");
                }
                catch (IOException ex)
                {
                    Logger.Log.Debug($"IOException occurred while sending data: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Logger.Log.Debug($"Error occurred while sending data: {ex.Message}");
                }
                break;

            case SendClientFlag.All:

                try
                {
                    var serializedPacket = await Serializer.SerializeAsync(packet);
                    byte[] sizeBytes = BitConverter.GetBytes(serializedPacket.Length);
                    byte[] buffer = sizeBytes.Concat(serializedPacket).ToArray();

                    foreach (NetClient client in World.Instance.AllWorldClients())
                    {
                        await client.Socket.SendAsync(buffer, SocketFlags.None);
                    }

                }
                catch (SocketException ex)
                {
                    Logger.Log.Debug($"SocketException occurred while sending data: {ex.SocketErrorCode}");
                }
                catch (IOException ex)
                {
                    Logger.Log.Debug($"IOException occurred while sending data: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Logger.Log.Debug($"Error occurred while sending data: {ex.Message}");
                }
                break;

            case SendClientFlag.FullRoom:

                try
                {
                    var serializedPacket = await Serializer.SerializeAsync(packet);
                    byte[] sizeBytes = BitConverter.GetBytes(serializedPacket.Length);
                    byte[] buffer = sizeBytes.Concat(serializedPacket).ToArray();

                    foreach (TeamMember client in World.Instance.GetRoom(_client.TeamUUID).Team)
                    {
                        await client.netClient.Socket.SendAsync(buffer, SocketFlags.None);
                    }

                }
                catch (SocketException ex)
                {
                    Logger.Log.Debug($"SocketException occurred while sending data: {ex.SocketErrorCode}");
                }
                catch (IOException ex)
                {
                    Logger.Log.Debug($"IOException occurred while sending data: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Logger.Log.Debug($"Error occurred while sending data: {ex.Message}");
                }
                break;

            case SendClientFlag.MyTeam:

                try
                {
                    var serializedPacket = await Serializer.SerializeAsync(packet);
                    byte[] sizeBytes = BitConverter.GetBytes(serializedPacket.Length);
                    byte[] buffer = sizeBytes.Concat(serializedPacket).ToArray();

                    foreach (TeamMember client in World.Instance.GetRoom(_client.TeamUUID).Team)
                    {
                        if(client.Team == _client.Team)
                        await client.netClient.Socket.SendAsync(buffer, SocketFlags.None);
                    }

                }
                catch (SocketException ex)
                {
                    Logger.Log.Debug($"SocketException occurred while sending data: {ex.SocketErrorCode}");
                }
                catch (IOException ex)
                {
                    Logger.Log.Debug($"IOException occurred while sending data: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Logger.Log.Debug($"Error occurred while sending data: {ex.Message}");
                }
                break;
        }

    }

}
