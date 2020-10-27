using System;
using System.Net;
using System.Net.Sockets;

namespace PierreFeuilleCiseauxClient
{
  class Program
  {
    static void Main(string[] args)
    {
      Client client = new Client();

      client.PrintRules();
      client.Connect();

      client.WaitAllPlayers();


      try
      {
        client.Game();
      }
      catch (Exception e)
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(e.Message);
        Console.ForegroundColor = ConsoleColor.White;
      }
      //client.Close();
    }
  }
}
