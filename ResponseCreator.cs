using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TcpServer
{
	class ResponseCreator
	{
		public string FormatTheResponse (string response)
		{
			try
			{
				return DateTime.Now.ToString("hh:mm:ss") + "|" + response;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		public string RemoveUserName(string userName, string response)
		{
			try
			{
				var regex = new Regex(Regex.Escape(userName + "-"));
				string x = regex.Replace(response, "", 1);
				return x;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}

		}
	}
}
