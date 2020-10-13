using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpServer
{
	class TcpManager
	{
		#region ctor
		public TcpManager (string ip,int port,IDataBase db)
		{
			m_ip = ip;
			m_port = port;
			m_db = db;
			IPAddress localAddr = IPAddress.Parse(ip);
			m_server = new TcpListener(localAddr,port);
		}
		#endregion

		#region function
		bool Register(string name, TcpClient client)
		{
			lock (m_lock)
			{
				if (m_db.IsClientExist(name))
				{
					return false;
				}
				return m_db.AddClient(name, client);
			}

		}

		public void ServerStart()
		{
			//start the tcp server and set the loops variable as true
			m_server.Start();
			m_isRunning = true;

			//start a thread to check if there are new connection requests 
			Thread waitForConnactions = new Thread(WaitForConnactions);
			waitForConnactions.Start();
			Console.WriteLine("Listning to tcp connactions");

			//check for existing clients msg's
			RunServerLogic();
		}

		private void WaitForConnactions ()
		{
			while (m_isRunning)
			{
				Console.Write("Waiting for a connection... ");
				// Perform a blocking call to accept requests.
				TcpClient client = m_server.AcceptTcpClient();
				
				Console.WriteLine("Connected!");

				data = null;
				// Get a stream object for reading and writing
				NetworkStream stream = client.GetStream();
				int bytesRead;
				string response;
				// Loop to receive all the data sent by the client.
				if ((bytesRead = stream.Read(m_bytes, 0, m_bytes.Length)) != 0)
				{
					// Translate data bytes to a ASCII string.
					data = System.Text.Encoding.ASCII.GetString(m_bytes, 0, bytesRead);
					if (data.StartsWith("UR-"))
					{
						
						bool isRegistered = Register(data.Replace("UR-", string.Empty), client);
						response = isRegistered ? "UR-success" : "UR-failed";
						byte[] msg = System.Text.Encoding.ASCII.GetBytes(response);
						// Send back a response.
						stream.Write(msg, 0, msg.Length);
					}
				}

			}

		}

		private void RunServerLogic()
		{
			try
			{
				List<TcpClient> localClients;

				while (m_isRunning)
				{
					lock (m_lock)
					{
						localClients = m_db.GetAllClient();
					}
					if (localClients.Count != 0)
					{
						foreach (var item in localClients)
						{
							int bytesRead;
							string response;
							if (item.GetStream().DataAvailable)
							{
								if ((bytesRead = item.GetStream().Read(m_bytesMsg, 0, m_bytesMsg.Length)) != 0)
								{
									// Translate data bytes to a ASCII string.
									response = System.Text.Encoding.ASCII.GetString(m_bytesMsg, 0, bytesRead);
								}
								else
									response = string.Empty;

								CountAndPrintLatters(response);
								string privateUser = CheckForPrivateMsg(response);
								response = m_responseHelper.FormatTheResponse(response);
								
								if (privateUser != null && m_db.GetClient(privateUser) != null)
								{
									TcpClient privateClient = m_db.GetClient(privateUser);
									response = m_responseHelper.RemoveUserName(privateUser, response);
									byte[] msg = System.Text.Encoding.ASCII.GetBytes(response);	
									privateClient.GetStream().Write(msg, 0, msg.Length);
								}
								else
								{
									byte[] msg = System.Text.Encoding.ASCII.GetBytes(response);
									for (int i = 0; i < localClients.Count; i++)
									{
										if (localClients[i] != item)
										{
											// Send back a response.
											localClients[i].GetStream().Write(msg, 0, msg.Length);

										}

									}
								}
								



							}
						}
					}
					//// Shutdown and end connection
					//client.Close();
				}
			}
			catch (Exception ex)
			{

				Console.WriteLine(ex.Message);
			}
		}

		private string CheckForPrivateMsg(string response)
		{
			int pFrom = response.IndexOf("=>") + "=>".Length;
			int pTo = response.LastIndexOf("-");

			if (pFrom == -1 || pTo == -1)
				return null;

			return response.Substring(pFrom, pTo - pFrom);
		}

		private void CountAndPrintLatters(string response)
		{
			for (int i = 0; i < response.Length; i++)
			{
				if (m_letterApperance.ContainsKey(response[i]))
				{
					m_letterApperance[response[i]]++;
				}
				else
				{
					m_letterApperance.Add(response[i], 1);
				}
			}
			Console.WriteLine("Latters count:");
			foreach (var item in m_letterApperance)
			{
				Console.Write(item.Key.ToString() + ":" + item.Value.ToString() + "|");
			}
			Console.WriteLine(" ");
		}

		#endregion

		#region members
		//connection information
		private string m_ip;
		private int m_port;
		private IDataBase m_db;


		TcpListener m_server ;

		//buffers
		Byte[] m_bytes = new Byte[256];
		Byte[] m_bytesMsg = new Byte[256];
		string data = null;
		ResponseCreator m_responseHelper = new ResponseCreator();
		//sync object
		object m_lock = new object();
		bool m_isRunning = false;

		//misc
		Dictionary<char,int> m_letterApperance = new Dictionary<char,int>();

		#endregion


	}
}
