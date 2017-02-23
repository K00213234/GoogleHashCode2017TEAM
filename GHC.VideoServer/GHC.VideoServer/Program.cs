using GHC.VideoServer;
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
			
			MainFile("me_at_the_zoo");
			MainFile("videos_worth_spreading");
			MainFile("trending_today");

			MainFile("kittens");
			Console.Read();
		}
		static void MainFile(string filename)
		{
			
			TrafficParser parser = new TrafficParser();
            string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
            parser.filename = filepath+".in";
            parser.Parse();

			Context  context = parser.context;
			context.MakeCacheServers();
			//context.LoadServers();
            FirstComeFirstServed(context);            
            Solution s = new Solution();
			s.context = context;

			CreateFile(output, filepath + ".out");
            //PrintContext(parser.context);         
			Console.Read();

            //context.CacheServerList

         
		}

		public static void CreateFile(String text, String filePath)
		{
			//
			//	Step 1A; Delete Existing File
			//
			if(File.Exists(filePath))
				File.Delete(filePath);
			else
			{
				//
				//	Step 1B; Create Directory
				//
				String directoryPath = Path.GetDirectoryName(filePath);
				if(!Directory.Exists(directoryPath))
					Directory.CreateDirectory(directoryPath);
			}
			//
			//	Step 2; Write File
			//
			using(StreamWriter streamWriter = File.CreateText(filePath))
			{

				streamWriter.Write(text);
			}
		}
		//private static void PrintContext(Context context)
		//{

		//	foreach (var endpoint in context.EndPointList)
		//	{
		//		Console.WriteLine($"Endpoint: {endpoint.EndPointID} - {endpoint.LatencyInMiliSecondsFromDataCenter}");
		//		foreach (var connection in endpoint.Connections)
		//		{
		//			Console.WriteLine($"    connection {connection.CacheServerID} - {connection.LatencyInMilliSecondsFromCacheToEndpoint}");
		//		}
		//	}


		//	foreach (var request in context.RequestDescriptionList)
		//	{
		//		Console.WriteLine($"video {request.VideoID} - {request.NumberOfReqeusts} - {request.EndPointID}");
		//	}
		//}


	}
}


//public class FirstComeFirstServe
//{
//	private Context _context;    

//	public FirstComeFirstServe(Context context)
//	{
//		_context = context;
//	}

//	public void Process()
//	{

//	}

//	private void SortRequests()
//	{
//		_context.RequestDescriptionList.OrderBy(x => x.NumberOfReqeusts)
//	}

//}
