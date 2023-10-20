using GameLibrary.Extension;
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
