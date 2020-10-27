using System;
using System.Net;
using System.Net.Sockets;

namespace PierreFeuilleCiseauxServer
{
  class Program
  {
    public const string Ip = "91.175.127.86";

    public const int Port = 16384;

    public const int LengthData = 1024;

    static void Main(string[] args)
    {
      // Initialisation Serveur
      Serveur serveur = new Serveur();

      // On attend la connexion de 2 personnes
      serveur.WaitPlayers();

      serveur.SendAll("ready");
      Console.WriteLine($"--------------------------------------------------------");

      try
      {
        serveur.Game();
      }
      catch (Exception e)
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(e.Message);
        Console.ForegroundColor = ConsoleColor.White;
      }

      //client.Close();
      //serveur.Stop();
    }

  }
}
