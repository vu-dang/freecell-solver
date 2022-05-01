using System;
using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using System.IO;

namespace FreeCellSolver.Entry
{
    class Program
    {


        static int Main(string[] args)
        {
            Solve();
            return 1;
        }
     

        // find winnable games
        static void Solve()
        {

            var w = File.AppendText("fc_games.csv");

            var excluded = new int[] { 270959, 445772, 801861, 917964, 1095224, 1399220, 1399230 };

            for (var i = 1; i < 10000000; i++)
            {
                var dealNumber = i;
                Console.WriteLine("{0}", dealNumber);
                if (excluded.Contains(dealNumber))
                {
                    w.WriteLine("{0},{1},{2},{3}", dealNumber, 0, 0, 0);
                }
                else
                {                    
                    var start = DateTime.Now;
                    var result = CommandLineHelper.RunSolver(dealNumber);
                    var diff = DateTime.Now.Subtract(start).Milliseconds;

                    if (result == null)
                    {
                        w.WriteLine("{0},{1},{2},{3}", dealNumber, 0, 0, diff);
                    }
                    else
                    {
                        w.WriteLine("{0},{1},{2},{3}", dealNumber, 0, result.Length, diff);
                    }
                }
                if (dealNumber % 1000 == 0)
                {
                    w.Flush();
                    Console.WriteLine("---------------------");
                }

            }

            w.Close();

        }
    }
}
