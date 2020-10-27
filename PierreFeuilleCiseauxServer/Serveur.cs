using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PierreFeuilleCiseauxServer
{
  public class Serveur
  {
    public const string Ip = "91.175.127.86";

    public const int Port = 16384;

    public const int LengthData = 1024;

    public const int NombreJoueur = 2;

    TcpListener serveur;

    TcpClient[] clients;

    int winJoueur1;

    int winJoueur2;

    public Serveur()
    {
      clients = new TcpClient[NombreJoueur];
      serveur = new TcpListener(IPAddress.Any, Port);
      serveur.Start();
      WriteServer(">> Serveur démarré");
    }

    public void Game()
    {
      Task<string>[] coupsWait = new Task<string>[NombreJoueur];
      Dictionary<int, string> coups = new Dictionary<int, string>();
      int win = 0;
      while (true)
      {
        WriteServer($"En attente des {NombreJoueur} joueurs");
        for (int i = 0; i < NombreJoueur; i += 1)
        {
          coupsWait[i] = ReadAsync(i);
        }

        Task.WaitAll(coupsWait);
        for (int i = 0; i < NombreJoueur; i += 1)
        {
          var coup = TransformCoup(coupsWait[i].Result);
          WriteServer($"Joueur {i} a joué: \"{coup}\"");
          coups.Add(i, coup);
        }

        // 0 = égalité - 1 = Joueur1 - 2 = Joueur2
        win = CalculeResultat(coups[0], coups[1]);

        if (win == 1)
        {
          winJoueur1 += 1;
        }
        else if (win == 2)
        {
          winJoueur2 += 1;
        }

        SendResult(coups[0], coups[1], win);
        coups.Clear();
      }
    }

    private void SendResult(string coup1, string coup2, int win)
    {
      string message;
      string winMessage = string.Empty;

      // 1er joueur
      message = $"Vous avez joué {coup1} et votre adversaire {coup2} - ";
      switch (win)
      {
        case 0:
          winMessage = "Égalité";
          break;
        case 1:
          winMessage = "Gagné";
          break;
        case 2:
          winMessage = "Perdu";
          break;
      }

      message += winMessage;
      SendMessage(message, 0);

      // 2eme joueur
      // 1er joueur
      message = $"Vous avez joué {coup2} et votre adversaire {coup1} - ";
      switch (win)
      {
        case 0:
          winMessage = "Égalité";
          break;
        case 2:
          winMessage = "Gagné";
          break;
        case 1:
          winMessage = "Perdu";
          break;
      }

      message += winMessage;
      SendMessage(message, 1);

      message = $"Le score est de { winJoueur1 } à { winJoueur2 }";
      SendAll(message);

    }

    private int CalculeResultat(string coupJoueur1, string coupJoueur2)
    {
      var res = 0;
      switch (coupJoueur1)
      {
        case "pierre":
          res = CalculePierre(coupJoueur2);
          break;
        case "feuille":
          res = CalculeFeuille(coupJoueur2);
          break;
        case "ciseaux":
          res = CalculeCiseaux(coupJoueur2);
          break;
      }

      return res;
    }

    private int CalculeCiseaux(string coup)
    {
      switch (coup)
      {
        case "pierre":
          return 2;
        case "feuille":
          return 1;
        default:
          return 0;
      }
    }

    private int CalculeFeuille(string coup)
    {
      switch (coup)
      {
        case "ciseaux":
          return 2;
        case "pierre":
          return 1;
        default:
          return 0;
      }
    }

    private int CalculePierre(string coup)
    {
      switch (coup)
      {
        case "feuille":
          return 2;
        case "ciseaux":
          return 1;
        default:
          return 0;
      }
    }

    private string TransformCoup(string coup)
    {
      var lettreCoup = coup[0];
      switch (lettreCoup)
      {
        case 'p':
          return "pierre";
        case 'f':
          return "feuille";
        case 'c':
          return "ciseaux";
        default:
          return coup;
      }
    }

    public async Task<string> ReadAsync(int player)
    {
      string res;
      byte[] dataBytes = new byte[LengthData];
      NetworkStream networkData = clients[player].GetStream();
      await networkData.ReadAsync(dataBytes, 0, dataBytes.Length);
      networkData.Flush();

      res = System.Text.Encoding.UTF8.GetString(dataBytes);
      res = res.Replace("\0", string.Empty).Trim();
      return res;
    }

    internal void WaitPlayers()
    {
      for (int i = 0; i < NombreJoueur; i += 1)
      {
        clients[i] = serveur.AcceptTcpClient();
        //if (i + 1 != NombreJoueur)
        //  SendWaitOtherPlayer(i);

        WriteServer("> 1 joueur est arrivé");
      }

      WriteServer("> Les 2 joueurs sont présents");
    }

    private void SendWaitOtherPlayer(int player)
    {
      SendMessage("En attente d'un autre joueur", player);
    }

    public void SendAll(string message)
    {
      for (int i = 0; i < NombreJoueur; i += 1)
        SendMessage(message, i);
    }

    public void SendMessage(string message, int player)
    {
      byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(message);
      NetworkStream networkData = clients[player].GetStream();
      networkData.Write(dataBytes, 0, dataBytes.Length);
      networkData.Flush();

      WriteServer($"Envoi du message: \"{message}\" au joueur {player}");
    }

    private void WriteServer(string message)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine(message);
      Console.ForegroundColor = ConsoleColor.White;
    }
  }
}
