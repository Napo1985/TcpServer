using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer.DataBase
{
    class InMemoryDB : IDataBase
    {

        Dictionary<string, TcpClient> m_connectedClientsList = new Dictionary<string, TcpClient>();

        public bool AddClient(string name, TcpClient client)
        {
            if (m_connectedClientsList.ContainsKey(name))
            {
                return false;
            }
            m_connectedClientsList.Add(name, client);
            return true;
        }

        public bool RemoveClient(string name)
        {
            if (m_connectedClientsList.ContainsKey(name))
            {
                return false;
            }
            m_connectedClientsList.Remove(name);
            return true;
        }
       
        public TcpClient GetClient(string name)
        {
            if (m_connectedClientsList.ContainsKey(name))
            {
                return m_connectedClientsList[name];
            }
            else
            {
                return null;
            }
        }

        public List<TcpClient> GetAllClient()
        {
            List<TcpClient> allClients = new List<TcpClient>();
            foreach (var item in m_connectedClientsList)
            {
                allClients.Add(item.Value);
            }
            return allClients;
        }

        public bool IsClientExist(string name)
        {
            return m_connectedClientsList.ContainsKey(name) ? true : false;
        }
    }
}
