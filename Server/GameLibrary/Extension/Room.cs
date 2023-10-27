using GameLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Extension
{
    [DataContract]
    public class Room
    {
        private static Room instance { get; set; }
        public static Room Instance => instance ?? (instance = new Room());

        [DataMember]
        public string UUID { get; set; }
        [DataMember]
        public List<TeamMember> Team; //возможно нужен класс team

        [DataMember]
        public int Team1Members = 0;
        [DataMember]
        public int Team2Members = 0;
        [DataMember]
        public int Team1Kill = 0;
        [DataMember]
        public int Team2Kill = 0;

        public void AddTeamMember(NetClient Member) { 
        
            TeamMember buffer = new TeamMember();
            buffer.netClient = Member;

            if (Team.Count < 10) {

                if ((Team1Members == 0 || Team1Members <= Team2Members) && Team1Members < 5)
                {
                    Team1Members++;
                    buffer.Team = 1;
                    Team.Add(buffer);

                }
                else
                    if (Team2Members < 5)
                {
                    Team2Members++;
                    buffer.Team = 2;
                    Team.Add(buffer);
                }
                else
                {
                    //Logger.Log.Error("Team Full");
                    //Возврат к подбору группы надо добавить
                }

            }
            

        }

        public void RemoveUser(string UUID) {

                TeamMember itemToRemove = Team.Single(r => r.netClient.Id == UUID);

                    if (itemToRemove.Team == 1)
                        Team1Members--;
                    else
                    if (itemToRemove.Team == 2)
                        Team2Members--;

                    Team.Remove(itemToRemove);

            }

        public TeamMember GetTeamMember(string UUID) { 
        
            return Team.Single(r => r.netClient.Id == UUID);

        }

        public int GetTeamCount() { 
        
            return  10 - Team.Count;

        }

    }
}
