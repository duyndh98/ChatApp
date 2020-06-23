using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyDu.Model;

namespace CyDu.Ultis
{
    public class AppInstance
    {
        private static AppInstance _instance = null;
        private static  User _user;
        //private static long _lastTimespan;
        private static List<Conversation> _conversationList;
        private static Dictionary<long, string> _dirFullname;
        public static AppInstance getInstance()
        {
            if (_instance==null)
            {
                _instance = new AppInstance();
            }
            return _instance;
        }

        protected AppInstance()
        {
            _user = new User();
            //_lastTimespan = 0;
            _dirFullname = new Dictionary<long, string>();

        }

        public  void SetUser(User user)
        {
            _user = user;
            SetFullname(user.Id, user.FullName);
            //_user.Token = "Bearer  " + user.Token;
        }

        public  User GetUser()
        {
            return _user;
        }

        public void SetConversation(List<Conversation> listcvs)
        {
            _conversationList = listcvs;
        }

        public List<Conversation> GetConversations()
        {
            return _conversationList;
        }

        public void SetFullname(long id,string name)
        {
            if (!   _dirFullname.ContainsKey(id))
            {
                _dirFullname.Add(id, name);

            }
           
        }

        public string GetFullname(long id)
        {
            return _dirFullname[id];
        }
        //public long GetLastTimespan()
        //{
        //    return _lastTimespan;
        //}

        //public void SetLastTimespan(long timepsan)
        //{
        //    _lastTimespan = timepsan;
        //}
    }
}
