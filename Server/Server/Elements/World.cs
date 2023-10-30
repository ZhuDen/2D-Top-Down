using GameLibrary.Extension;
using GameLibrary.Tools;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Elements
{
    public class World
    {
        private static World instance;
        public static World Instance => instance ?? (instance = new World());
        public ConcurrentDictionary<string, NetClient> Players;
        //public ConcurrentDictionary<string, Player> Players => players;

        public ConcurrentDictionary<string, Room> Rooms;


        //Rooms 
        public async Task<string> CreateRoom()
        {

            try
            {
                Room room = new Room();
                room.Team = new List<TeamMember>();
                room.UUID = await NetId.Generate();
                Logger.Log.Debug($"RoomUUID: {room.UUID}");
                Rooms.TryAdd(room.UUID, room);
                Logger.Log.Debug($"RoomCount: {Rooms.Count}");
                return room.UUID;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public void UpdateRoom( string UUID){
            // что либо обновить в комнате

        }

        public Room GetRoom(string UUID) {

            try
            {
                return Rooms[UUID];
            }
            catch (Exception e) { return null; }

        }

        // Ищет, создает, добавляет и вообще надо юзать когда игрок ищет пачку
        public string FindRoomForMember(string MemberUUID) {

            try
            {

                bool found = false;
                if (Rooms.Count > 0)
                {
                    foreach (var room in Rooms)
                    {

                        if (room.Value.GetTeamCount() > 0)
                        {

                            room.Value.AddTeamMember(Players[MemberUUID]); // само блять все сделается, я уже запутался
                            found = true;
                            return room.Key;
                        }

                    }
                }
                if (found == false)
                {
                    Logger.Log.Debug("Createroom");
                    string bufferRoomUUID = CreateRoom().Result;
                    Rooms[bufferRoomUUID].AddTeamMember(Players[MemberUUID]);
                    Logger.Log.Debug($"UnderCreateRoom: {bufferRoomUUID}");
                    return bufferRoomUUID;
                }
                return null;
            }
            catch (Exception ex) { return null;}
        }




        public NetClient getClient(string Id) 
        {
            if (Players.TryGetValue(Id, out var client)) return client;
            else return null;
        }
        public bool isContain(string Id, ref NetClient client)
        {
            return Players.TryGetValue(Id,out client);
        }
        public bool isContain(string Id)
        {
            return Players.TryGetValue(Id, out var client);
        }

        public bool addClient(NetClient client) 
        { 
            return Players.TryAdd(client.Id, client);
        }
        public bool removeClient(string Id)
        {
            return Players.TryRemove(Id, out _);
        }
        public List<NetClient> AllWorldClients()
        {
            return Players.Values.ToList();
        }


        public World()
        {

        }
    }
}
