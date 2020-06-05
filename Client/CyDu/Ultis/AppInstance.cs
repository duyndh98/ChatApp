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
        }

        public  void setUser(User user)
        {
            _user = user;
        }

        public  User getUser()
        {
            return _user;
        }
    }
}
