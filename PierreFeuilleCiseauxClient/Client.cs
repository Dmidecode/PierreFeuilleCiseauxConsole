using System;
using System.Net;
using System.Net.Sockets;

namespace PierreFeuilleCiseauxClient
{
  public class Client
  {
    public const string Ip = "91.175.127.86";

    public const int Port = 16384;

    public const int LengthData = 1024;

    TcpClient client;
    IPAddress ip;

    NetworkStream networkStream;

    public Client()
    {
      client = new TcpClient();
      ip = IPAddress.Parse(Ip);
    }

    public void Connect()
    {
      client.Connect(ip, Port);
      networkStream = client.GetStream();
      Console.WriteLine("> Client connecté au serveur");
    }

    public void WaitAllPlayers()
    {
      string message;
      while ((message = ReadMessage()) != "ready")
        Console.WriteLine($">{message}<");
      Console.WriteLine("> Tous les joueurs sont connectés, la partie va démarrer");
    }

    public void Game()
    {
      while (true)
      {
        var coup = ChoixCoup();
        SendMessage(coup);
        Console.WriteLine("> En attente du résultat");
        Console.WriteLine(ReadMessage());
        Console.WriteLine(ReadMessage());
      }
    }

    public string ChoixCoup()
    {
      Console.WriteLine("Choisissez votre coup (p, f, c) et appuyez sur entrée:");
      bool played = false;
      string coup;
      do
      {
        coup = Console.ReadLine();
        played = CheckCoup(coup);
        if (!played)
          Console.WriteLine("Veuillez écrire un coup existant !");
      } while (!played);
      return coup;
    }

    private bool CheckCoup(string coup)
    {
      var car = coup[0];
      return car == 'p' || car == 'f' || car == 'c';
    }

    public void SendMessage(string message)
    {
      byte[] dataWrite = System.Text.Encoding.UTF8.GetBytes(message);
      networkStream.WriteAsync(dataWrite, 0, dataWrite.Length);
      networkStream.Flush();
    }

    public string ReadMessage()
    {
      byte[] dataRead = new byte[LengthData];
      networkStream.Read(dataRead, 0, dataRead.Length);
      networkStream.Flush();
      var res = System.Text.Encoding.UTF8.GetString(dataRead);
      res = res.Replace("\0", string.Empty).Trim();
      return res;
    }

    public void PrintRules()
    {
      Console.ForegroundColor = ConsoleColor.Blue;
      Console.WriteLine("Bienvenue sur le Pierre-Feuille-Ciseaux du pauvre");
      Console.WriteLine();
      Console.WriteLine("Vous connaissez déjà les règles.");
      Console.WriteLine("Une fois connecté au serveur et tous les joueurs prêt, il faudra écrire l'un des coups suivant");
      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.WriteLine("- pierre (ou 'p')");
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("- feuille (ou 'f')");
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("- ciseaux (ou 'c')");
      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine("Pour vous connecter au serveur, appuyez sur entrée");
      Console.ReadLine();
    }
  }
}
