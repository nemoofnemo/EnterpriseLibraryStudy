using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;
using DALSQLServer;
using Models;
using BLL;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            MessageManager mm = new MessageManager();
            mm.StartServer();
        }
    }
}
