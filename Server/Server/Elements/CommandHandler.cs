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
                            World.Instance.Rooms[World.Instance.Players[client.Id].TeamUUID].RemoveUser(client.Id);
                            World.Instance.removeClient(client.Id);
                            client.Disconnect();
                            Logger.Log.Debug($"{client.Id} requested disconnection");
                            client.Dispose();
                            client.Close();
                        }
                        catch (Exception ex) { Logger.Log.Error($"Disconnected error: {ex}"); }


                        break;

                    case OperationCode.SetDamage:
                        await SendTo(client, new DataPacket((byte)OperationCode.SetDamage, new Dictionary<ParameterCode, object> { { ParameterCode.Count, packet } }));
                        break;

                    case OperationCode.Message:
                        if (packet.Data != null)
                        {
                            Logger.Log.Debug($"{client.Id} sent a message: {packet.Data[ParameterCode.Message]}");
                            await SendTo(client, new DataPacket((byte)OperationCode.Message, new Dictionary<ParameterCode, object> { { ParameterCode.Message, packet.Data[ParameterCode.Message] + " RETURN" } }));
                        }
                        break;

                    case OperationCode.Connect:
                        //client.Data.Parameters.Name = packet.Data[ParameterCode.Message].ToString();
                        //client.Data.pp = NetVector3(0,0,0);
                        await SendTo(client, new DataPacket((byte)OperationCode.Connect, new Dictionary<ParameterCode, object> { { ParameterCode.Message, "Connect" } }));

                        //World.Instance.getClient(client.Id).Data.NetTransform = new NetTransform ();
                        //World.Instance.getClient(client.Id).Data.NetTransform.Position = new NetVector3(1, 2, 3);
                        //await SendTo(client, new DataPacket(OperationCode.GetClientsInfo, new Dictionary<ParameterCode, object> { { ParameterCode.Client, World.Instance.getClient(client.Id).Data } }));

                        Logger.Log.Debug($"Player Connected!");
                        break;

                    case OperationCode.Authorisation:

                        using (MySqlConnection con = new MySqlConnection(DB.pathsql))
                        {
                            con.Open();
                            using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM Accounts WHERE Login = @Login;", con))
                            {
                                cmd.Parameters.AddWithValue("@Login", packet.Data[ParameterCode.Login].ToString());
                                object existingCount = cmd.ExecuteScalar();
                                using (MySqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (existingCount == null)
                                    {

                                        await SendTo(client, new DataPacket((byte)OperationCode.Authorisation, new Dictionary<ParameterCode, object>
                                        {
                                            { ParameterCode.Message, "Error" }
                                        }));

                                    }
                                    else
                                    {
                                        while (reader.Read())
                                        {
                                            try
                                            {

                                                if (reader["Login"].ToString() == packet.Data[ParameterCode.Login].ToString() &&
                                                    reader["Password"].ToString() == packet.Data[ParameterCode.Password].ToString())
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


                                                    if (!World.Instance.Players.TryAdd(client.Id, client))
                                                    {
                                                        Logger.Log.Debug($"Failed to add player {client.Id} to the player list.");
                                                        client.Socket.Close();
                                                        continue;
                                                    }
                                                    Logger.Log.Debug($"ID: {client.Id} Count: {World.Instance.Players.Count} ");
                                                    await SendTo(client, new DataPacket((byte)OperationCode.Authorisation, new Dictionary<ParameterCode, object>
                                                {
                                { ParameterCode.Name, client.Data.Name },
                                { ParameterCode.Message, "Success" },
                                { ParameterCode.Id, client.Data.ID },
                                { ParameterCode.LVL, client.Data.Lvl },
                                { ParameterCode.Exp, client.Data.Exp },
                                { ParameterCode.IconIndex, client.Data.Icon }

                                                }));
                                                }
                                                else
                                                {
                                                    await SendTo(client, new DataPacket((byte)OperationCode.Authorisation, new Dictionary<ParameterCode, object>
                            {
                                { ParameterCode.Message, "Access Denied" }
                            }));
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                await SendTo(client, new DataPacket((byte)OperationCode.Authorisation, new Dictionary<ParameterCode, object>
                        {
                            { ParameterCode.Message, "Error" }
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
                                checkCmd.Parameters.AddWithValue("@Login", packet.Data[ParameterCode.Login]);
                                int existingCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                                if (existingCount > 0)
                                {
                                    await SendTo(client, new DataPacket((byte)OperationCode.Registration, new Dictionary<ParameterCode, object>
                {
                    { ParameterCode.Message, "Login already exists" }
                }));
                                }
                                else
                                {
                                    // Логин не существует, регистрируем пользователя
                                    using (MySqlCommand registerCmd = new MySqlCommand("INSERT INTO Accounts (UUID, Login, Password, Name) VALUES (@UUID, @Login, @Password, @Name);", con))
                                    {
                                        client.Id = await NetId.Generate();
                                        registerCmd.Parameters.AddWithValue("@UUID", client.Id);
                                        registerCmd.Parameters.AddWithValue("@Login", packet.Data[ParameterCode.Login]);
                                        registerCmd.Parameters.AddWithValue("@Password", packet.Data[ParameterCode.Password]); // В реальной практике пароль должен быть хеширован
                                        registerCmd.Parameters.AddWithValue("@Name", packet.Data[ParameterCode.Name]);



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



                                            await SendTo(client, new DataPacket((byte)OperationCode.Registration, new Dictionary<ParameterCode, object>
                                        {
                                            { ParameterCode.Name, packet.Data[ParameterCode.Name] },
                                            { ParameterCode.Message, "Registration successful" },
                                            { ParameterCode.Id, client.Id },
                                            { ParameterCode.LVL, 1 },
                                            { ParameterCode.Exp, 0 },
                                            { ParameterCode.IconIndex, 1 },
                                        }));
                                        }
                                        else
                                        {
                                            await SendTo(client, new DataPacket((byte)OperationCode.Registration, new Dictionary<ParameterCode, object>
                                        {
                                            { ParameterCode.Message, "Registration failed" }
                                        }));
                                        }
                                    }
                                    using (MySqlCommand insertUserParametersCommand = new MySqlCommand("INSERT INTO UserParameters (UUID, Name, Lvl, Exp, Icon, Birth) VALUES (@UUID, @Name, @Lvl, @Exp, @Icon, @Birth)", con))
                                    {
                                        insertUserParametersCommand.Parameters.AddWithValue("@UUID", client.Data.ID);
                                        insertUserParametersCommand.Parameters.AddWithValue("@Name", packet.Data[ParameterCode.Name]);
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

                        Logger.Log.Debug("GetAllRoom");
                        Logger.Log.Debug($"RoomUUID{World.Instance.Players[_client.Id].TeamUUID}");
                        Logger.Log.Debug($"RoomCount{World.Instance.Rooms.Count}");
                        Logger.Log.Debug($"GetAllRoom{World.Instance.GetRoom(World.Instance.Players[_client.Id].TeamUUID).UUID}");
                        await SendTo(new DataPacket((byte)OperationCode.GetInfoRoom, new Dictionary<ParameterCode, object>
                                        {
                                            { ParameterCode.TeamMember,  World.Instance.GetRoom(World.Instance.Players[_client.Id].TeamUUID)},
                                        }), packet.Flag);

                        break;

                    case OperationCode.MyTransform:

                        /*await SendTo(client, new DataPacket(OperationCode.MyTransform, new Dictionary<ParameterCode, object>
                                            {
                                                { ParameterCode.X, packet.Data[ParameterCode.X] },
                                                { ParameterCode.Y, packet.Data[ParameterCode.Y] },
                                                { ParameterCode.Z, packet.Data[ParameterCode.Z] }
                                            }));*/
                        await SendTo(new DataPacket((byte)OperationCode.MyTransform, new Dictionary<ParameterCode, object>
                                        {
                                            { ParameterCode.X, packet.Data[ParameterCode.X] },
                                            { ParameterCode.Y, packet.Data[ParameterCode.Y] },
                                            { ParameterCode.Z, packet.Data[ParameterCode.Z] },
                                            { ParameterCode.Id, packet.Data[ParameterCode.Id] }
                                        }), packet.Flag);


                        break;

                    //Добавляемся в новую тиму соло
                    case OperationCode.SetTeam:

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
                        await SendTo(new DataPacket((byte)OperationCode.SetTeam, new Dictionary<ParameterCode, object>
                                        {
                                            { ParameterCode.TeamUUID, World.Instance.Players[_client.Id].TeamUUID }
                                        }), packet.Flag);
                        break;

                    default:
                        break;
                }
            }
            else {

                await SendTo(packet, packet.Flag);


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
