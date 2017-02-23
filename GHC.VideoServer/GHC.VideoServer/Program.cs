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
			//context.LoadServers();
            FirstComeFirstServed(context);            
            Solution s = new Solution();
			s.context = context;

            
			String output= s.ToString();
            Console.WriteLine(output);
            //PrintContext(parser.context);         
			Console.Read();

            //context.CacheServerList

         
		}


        private static void FirstComeFirstServed(Context context)
        {
            foreach (var request in context.RequestDescriptionList)
            {
                foreach (var connection in request.EndPoint.Connections.OrderBy(x => x.LatencyInMilliSecondsFromCacheToEndpoint))
                {
                    var cacheServer = context.CacheServerList.Find(x => x.ID == connection.CacheServerID);

                    if (cacheServer.ConsumedSpace() < request.Video.VideoSizeInMB)
                    {
                        cacheServer.VideoList.Add(new VideoRequest
                        { //good lord
                            Video = request.Video,
                            VideoID = request.VideoID
                        });
                    }
                }
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
