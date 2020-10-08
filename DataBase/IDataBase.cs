using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer
{
    interface IDataBase
    {
        bool AddClient(string name, TcpClient stream);
        bool RemoveClient(string name);
        TcpClient GetClient(string name);
        List<TcpClient> GetAllClient();
        bool IsClientExist(string name);
    }
}
