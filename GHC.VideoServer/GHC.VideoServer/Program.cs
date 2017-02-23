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
			TrafficParser parser = new TrafficParser();
            string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "me_at_the_zoo.in"); ;
            parser.filename = filepath;
            parser.Parse();

			Context  context = parser.context;
			context.MakeCacheServers();
			context.LoadServers();
			Solution s = new Solution();
			s.context = context;
			String output= s.ToString();

            //PrintContext(parser.context);         
			Console.Read();
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
