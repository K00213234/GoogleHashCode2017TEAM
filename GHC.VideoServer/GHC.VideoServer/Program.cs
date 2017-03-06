using GHC.VideoServer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GHC.VideoServer.Strategies.CostBased;
using GHC.VideoServer.Strategies.InfinitePrune;

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
            Console.WriteLine("finished");
            Console.ReadKey();
        }

        private static void ProcessFile(string filename)
        {
            //input
            var fileParser = new FileParser();
            string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
            fileParser.filename = filepath + ".in";
            fileParser.Parse();

            var context = fileParser.Context;
            context.RequestsToRedistribute  = new Dictionary<int, RequestDescription>();
            context.UnOptimizedCacheServers = context.CacheServers;

            
            var infiniteCachePruneCostBasedStrategy = new InfiniteCachePruneCostBasedStrategy();
            infiniteCachePruneCostBasedStrategy.Run(context);

            var unCachedRequests = context.Requests.Where(req => !req.Value.IsCached).ToDictionary(req => req.Key, req => req.Value);

            var emptyCacheServers  = context.CacheServers.Where(cacheServer => cacheServer.Value.ConsumedSpace() == 0).ToDictionary(cacheServer => cacheServer.Key, cacheServer => cacheServer.Value);

            var ctx = new Context()
            {
                CacheServers = emptyCacheServers,
                CacheServerToEndPoint =  context.CacheServerToEndPoint,
                EndPointToCacheServer =  context.EndPointToCacheServer,
                FileDescriptor = context.FileDescriptor,
                Videos = context.Videos,
                Requests = unCachedRequests
            };

            infiniteCachePruneCostBasedStrategy.Run(ctx);  //this should be repeated while there are cache servers with nothing cached

            var plugUnusedSpaceStrategy = new PlugUnusedSpaceStrategy();
            plugUnusedSpaceStrategy.Run(context);

            //output
            var s = new Solution { Context = context };
            var output = s.ToString();
            CreateFile(output, filepath + ".out");            
           
            var sum = 0.0;
            foreach(var cacheServer in context.CacheServers)
            {
                sum += cacheServer.Value.CalculateCacheScore();
                Console.WriteLine($"{cacheServer.Value.ID}: {(double) ((double)cacheServer.Value.ConsumedSpace() / (double) cacheServer.Value.MaxMB) * 100}%\t{cacheServer.Value.CalculateCacheScore()}\t{cacheServer.Value.VideoCache.Count}/{context.Videos.Count}");
            } 

            var totalStorageSpace = context.CacheServers.Sum(x => x.Value.MaxMB);
            var storageConsumed = context.CacheServers.Sum(x => x.Value.ConsumedSpace());
            var totalVideoSize = context.Videos.Sum(x => x.VideoSizeInMb);

            Console.WriteLine($"storage: {totalStorageSpace} {storageConsumed} {totalVideoSize}");
            Console.WriteLine($"Total {sum} - {filename}");
        }

        private static void CreateFile(string text, string filePath)
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
                string directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
            }
            //
            //	Step 2; Write File
            //
            using (var streamWriter = File.CreateText(filePath))
            {
                streamWriter.Write(text);
            }
        }
    }
}
