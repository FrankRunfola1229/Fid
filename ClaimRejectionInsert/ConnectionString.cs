using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClaimRejectionInsert.Models
{
    public class ConnectionString
    {
           // "DefaultConnection1": "Data Source=DEVFCTASE11;Port=47654;Database=APP_CONFIG;Uid=FD179893;Pwd=Ready4gocaleb!;",
        public string ConnectionStringS { get; set; }
        public string DataSource { get; set; }
        public string Port { get; set; }
        public string Database { get; set; }
        public string Uid { get; set; }
        public string Pwd { get; set; }
        public bool Verified { get; set; } = false;

        public void PrintConnectionStringError(string error)
        {
            System.Console.WriteLine("*********************************************************************");
            System.Console.WriteLine(" ConnectionStringSybase ERROR!!!");
            System.Console.WriteLine("********************************************************************");
            System.Console.WriteLine("ERROR = {0}", error);
            System.Console.WriteLine("{0,-20} {1} {2}", "ConnectionStringS", "=", ConnectionStringS);
            System.Console.WriteLine("{0,-20} {1} {2}", "Driver", "=", DataSource);
            System.Console.WriteLine("{0,-20} {1} {2}", "Server", "=", Port);
            System.Console.WriteLine("{0,-20} {1} {2}", "Port", "=", Database);
            System.Console.WriteLine("{0,-20} {1} {2}", "Database", "=", Uid);
            System.Console.WriteLine("{0,-20} {1} {2}", "Uid", "=", Pwd);
            System.Console.WriteLine("{0,-20} {1} {2}", "Pwd", "=", Pwd);
        }
    }
}