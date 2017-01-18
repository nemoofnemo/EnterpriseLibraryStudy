using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;
using DALSQLServer;
using Models;
using BLL;
using Application;
using System.Collections;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            SystemCreator sc = new SystemCreator();
            sc.StartServer();
        }
    }
}
