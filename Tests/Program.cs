using System;
using System.Threading;

namespace Tests
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelKey);
      while (true) ;
      //for (int i = 0; i < 30; i += 1)
      //{
      //  Console.WriteLine($"{i} secondes");
      //  Thread.Sleep(1000);
      //}
    }

    public static void CancelKey(object o, ConsoleCancelEventArgs args)
    {
      Console.WriteLine("Dans l'interruption");
    }
  }
}
