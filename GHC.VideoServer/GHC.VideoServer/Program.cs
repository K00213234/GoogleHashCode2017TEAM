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
			TrafficParser parser = new TrafficParser();
            string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "me_at_the_zoo.in"); ;
            parser.filename = filepath;
            parser.Parse();

            Console.WriteLine(parser.context.ToString());
			Console.Read();
		}
	}
}
