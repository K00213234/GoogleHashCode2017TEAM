using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GHC.VideoServer
{
	class Program
	{
		static void Main(string[] args)
		{
			TrafficParser pizzaParser = new TrafficParser();
            string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "small.in"); ;
            pizzaParser.filename = filepath;
			pizzaParser.Parse();

			Console.WriteLine(pizzaParser.PrintPizza());
			Console.Read();
		}
	}
}
