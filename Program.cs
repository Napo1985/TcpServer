using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpServer.DataBase;

namespace TcpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpManager server = new TcpManager("127.0.0.1",5000,new InMemoryDB());
            server.ServerStart();
        }
    }
}



/* Notes:
DONE:
    Register status returns   
    Connections for multi clients 
    Message to all registered clients, except the sender 
    Latter count
Not Done:
    register return message format
    private message
    Remove and close client 
    

*/