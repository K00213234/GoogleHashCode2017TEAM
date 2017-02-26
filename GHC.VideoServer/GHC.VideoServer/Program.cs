using GHC.VideoServer.Model;
using System;
using System.IO;
using System.Linq;

namespace GHC.VideoServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ProcessFile("me_at_the_zoo");
            //ProcessFile("videos_worth_spreading");
            //ProcessFile("trending_today");
            //ProcessFile("kittens");
        }

        private static void ProcessFile(string filename)
        {
            //input
            var fileParser = new FileParser();
            string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
            fileParser.filename = filepath + ".in";
            fileParser.Parse();

            var context = fileParser.Context;

            //algorithm
            NonDuplicateMostRequestVideosFirst(context);

            //output
            var s = new Solution { context = context };
            var output = s.ToString();
            CreateFile(output, filepath + ".out");
            Console.WriteLine("really done");
        }

        private static void NonDuplicateMostRequestVideosFirst(Context context)
        {
            foreach (var request in context.RequestDescriptionList.OrderByDescending(x => x.NumberOfReqeusts))
            {
                foreach (var connection in request.EndPoint.Connections.OrderBy(x => x.LatencyInMilliSecondsFromCacheToEndpoint))
                {
                    var cacheServer = context.CacheServerList.Find(x => x.ID == connection.CacheServerID);

                    if (cacheServer.ConsumedSpace() < request.Video.VideoSizeInMb)
                    {
                        if (cacheServer.VideoList.Any(x => x.VideoID == request.VideoID))
                        {
                            //already on this server
                            continue;
                        }
                        cacheServer.VideoList.Add(new VideoRequest
                        { //good lord
                            Video = request.Video,
                            VideoID = request.VideoID
                        });
                    }
                }
            }
        }

        private static void MostRequestVideosFirst(Context context)
        {
            foreach (var request in context.RequestDescriptionList.OrderByDescending(x => x.NumberOfReqeusts))
            {
                foreach (var connection in request.EndPoint.Connections.OrderBy(x => x.LatencyInMilliSecondsFromCacheToEndpoint))
                {
                    var cacheServer = context.CacheServerList.Find(x => x.ID == connection.CacheServerID);

                    if (cacheServer.ConsumedSpace() < request.Video.VideoSizeInMb)
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

        private static void FirstComeFirstServed(Context context)
        {
            foreach (var request in context.RequestDescriptionList)
            {
                foreach (var connection in request.EndPoint.Connections.OrderBy(x => x.LatencyInMilliSecondsFromCacheToEndpoint))
                {
                    var cacheServer = context.CacheServerList.Find(x => x.ID == connection.CacheServerID);

                    if (cacheServer.ConsumedSpace() < request.Video.VideoSizeInMb)
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

        public static void CreateFile(String text, String filePath)
        {
            //
            //	Step 1A; Delete Existing File
            //
            if (File.Exists(filePath))
                File.Delete(filePath);
            else
            {
                //
                //	Step 1B; Create Directory
                //
                String directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
            }
            //
            //	Step 2; Write File
            //
            using (StreamWriter streamWriter = File.CreateText(filePath))
            {
                streamWriter.Write(text);
            }
        }

        //private static void PrintContext(Context Context)
        //{
        //	foreach (var endpoint in Context.EndPointList)
        //	{
        //		Console.WriteLine($"Endpoint: {endpoint.EndPointID} - {endpoint.LatencyInMiliSecondsFromDataCenter}");
        //		foreach (var connection in endpoint.Connections)
        //		{
        //			Console.WriteLine($"    connection {connection.CacheServerID} - {connection.LatencyInMilliSecondsFromCacheToEndpoint}");
        //		}
        //	}

        //	foreach (var request in Context.RequestDescriptionList)
        //	{
        //		Console.WriteLine($"video {request.VideoID} - {request.NumberOfReqeusts} - {request.EndPointID}");
        //	}
        //}
    }
}

//public class FirstComeFirstServe
//{
//	private Context _context;

//	public FirstComeFirstServe(Context Context)
//	{
//		_context = Context;
//	}

//	public void Process()
//	{
//	}

//	private void SortRequests()
//	{
//		_context.RequestDescriptionList.OrderBy(x => x.NumberOfReqeusts)
//	}

//}