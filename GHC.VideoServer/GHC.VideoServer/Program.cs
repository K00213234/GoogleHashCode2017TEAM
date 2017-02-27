﻿using GHC.VideoServer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;

namespace GHC.VideoServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ProcessFile("me_at_the_zoo");            
            ProcessFile("videos_worth_spreading");
            ProcessFile("trending_today");
            ProcessFile("kittens");
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

            //algorithm
            //NonDuplicateMostRequestVideosFirst(context);
            EndpointOrientatedSizeByNumberOfRequestsStrategyAvoidDuplicateCachingWithRequestCombinationSubOptimizer(context);
            //output
            var s = new Solution { context = context };
            var output = s.ToString();
            CreateFile(output, filepath + ".out");
           

            //foreach(var cacheServer in context.CacheServerList)
            //{
            //    Console.WriteLine($"{cacheServer.ID} Consumed {cacheServer.ConsumedSpace()} - out of {cacheServer.MaxMB}");
            //} 
            PrintHitMissStatistic(context,filename);
            
        }

        private static bool IsRequestVideoCachedOnAnyConnectedServer(Context context, RequestDescription request)
        {
            var caches = context.CacheServerList.Where(c => request.EndPoint.Connections.Any(x => x.CacheServerID == c.ID)).ToList();

            foreach (var cache in caches)
            {
                var isFound = cache.VideoList.Any(v => v.VideoID == request.VideoID);

                if (isFound)
                {
                    return true;
                }
            }
                       
            return false;
        }
        private static void PrintHitMissStatistic(Context context, string fileName)
        {
            var shortFileName = fileName.Length > 11 ? fileName.Substring(0, 10) : fileName;
            int hits = 0;
            int misses = 0;

            for(int i = 0; i < context.RequestDescriptionList.Count; i++)
            {
                var request = context.RequestDescriptionList[i];
                if (IsRequestVideoCachedOnAnyConnectedServer(context, request))
                {
                    hits++;
                }
                else
                {
                    misses++;
                }
                Console.Write($"\r {shortFileName}:\t{hits}\t{misses}\t{hits}/{context.RequestDescriptionList.Count}");
            }
            Console.WriteLine();          
        }

        private static void EndpointOrientatedSizeByNumberOfRequestsStrategyAvoidDuplicateCachingWithRequestCombinationSubOptimizer(Context context)
        {
            foreach (var endpoint in context.EndPointList)
            {
                //get the most expensive videos - size by number of requests from this endpoint
                var requests = context.RequestDescriptionList.Where(r => r.EndPointID == endpoint.EndPointID).OrderByDescending(r => r.NumberOfReqeusts * r.Video.VideoSizeInMb).ToList();

                //get the connected cache server
                var caches = new List<CacheServer>();
                foreach (var connection in endpoint.Connections)
                {
                    caches.Add(context.CacheServerList.Where(x => x.ID == connection.CacheServerID).First());
                }

                foreach (var request in requests)
                {
                    bool isVideoAlreadyCachedOnAnyConnectedServer = false;
                    foreach (var cacheServer in caches)
                    {
                        if (cacheServer.VideoList.Any(v => v.VideoID == request.VideoID))
                        {
                            isVideoAlreadyCachedOnAnyConnectedServer = true;
                        }
                    }

                    if (isVideoAlreadyCachedOnAnyConnectedServer)
                    {
                        continue;
                    }

                    foreach (var cacheserver in caches)
                    {
                        if (request.Video.VideoSizeInMb < (cacheserver.MaxMB - cacheserver.ConsumedSpace()))
                        {
                            if (cacheserver.VideoList.Any(v => v.VideoID == request.VideoID))
                            {
                                break;
                            }

                            //Is there any combination of other vidoes that beat this for cost?
                            RequestDescription bestAlternative1 = null;
                            RequestDescription bestAlternative2 = null;
                            bool foundBetterOptions = false;
                            RequestCombinationSubOptimizer(context, request, ref foundBetterOptions, ref bestAlternative1, ref bestAlternative2);

                            if(bestAlternative1 != null && bestAlternative2 != null)
                            {
                                if (!cacheserver.VideoList.Any(v => v.VideoID == bestAlternative1.VideoID))
                                {
                                    cacheserver.VideoList.Add(new VideoRequest()
                                    {
                                        Video = bestAlternative1.Video,
                                        VideoID = bestAlternative1.VideoID
                                    });
                                }
                                if (!cacheserver.VideoList.Any(v => v.VideoID == bestAlternative2.VideoID))
                                {
                                    cacheserver.VideoList.Add(new VideoRequest()
                                    {
                                        Video = bestAlternative2.Video,
                                        VideoID = bestAlternative2.VideoID
                                    });
                                }
                            }
                            else
                            {
                                cacheserver.VideoList.Add(new VideoRequest
                                { //good lord
                                    Video = request.Video,
                                    VideoID = request.VideoID
                                });
                            }
                            break; //as we have added the video to a cache, no point in adding to others
                        }
                    }
                }
            }
        }

        private static int RequestCombinationSubOptimizer(Context context, RequestDescription requestToBeat, ref bool foundBetterRequestsToCache, ref RequestDescription r1, ref RequestDescription r2)
        {
            var costToBeat = requestToBeat.Video.VideoSizeInMb * requestToBeat.NumberOfReqeusts;
            var size = (requestToBeat.Video.VideoSizeInMb / 2);
            var endpointID = requestToBeat.EndPointID;

            var smallerRequests = context.RequestDescriptionList.Where(r => r.EndPointID == requestToBeat.EndPointID && r.Video.VideoSizeInMb <= size).ToList();

            if (!smallerRequests.Any())
            {
                foundBetterRequestsToCache = false;
                return 1;
            }

            RequestDescription firstRequest = smallerRequests.First();
            RequestDescription secondRequest = null;

            foreach (var request in smallerRequests)
            {
                var firstCost = firstRequest.NumberOfReqeusts * firstRequest.Video.VideoSizeInMb;
                var secondCost = request.NumberOfReqeusts * request.Video.VideoSizeInMb;

                if (firstCost + secondCost > costToBeat)
                {
                    secondRequest = request;
                    foundBetterRequestsToCache = true;
                }
            }

            if(foundBetterRequestsToCache)
            {
                r1 = firstRequest;
                r2 = secondRequest;
                foundBetterRequestsToCache = false;
                return RequestCombinationSubOptimizer(context, r1, ref foundBetterRequestsToCache, ref r1, ref r2);
            }

            return 1;
        }

        //private static void EndpointOrientatedSizeByNumberOfRequestsStrategyAvoidDuplicateCaching(Context context)
        //{
        //    foreach(var endpoint in context.EndPointList)
        //    {
        //        //get the most expensive videos - size by number of requests from this endpoint
        //        var requests = context.RequestDescriptionList.Where(r => r.EndPointID == endpoint.EndPointID).OrderByDescending(r => r.NumberOfReqeusts * r.Video.VideoSizeInMb).ToList();

        //        //get the connected cache server
        //        var caches = new List<CacheServer>();
        //        foreach(var connection in endpoint.Connections)
        //        {
        //            caches.Add(context.CacheServerList.Where(x => x.ID == connection.CacheServerID).First());
        //        }

        //        foreach(var request in requests)
        //        {
        //            bool isVideoAlreadyCachedOnAnyConnectedServer = false;
        //            foreach(var cacheServer in caches)
        //            {
        //                if (cacheServer.VideoList.Any(v => v.VideoID == request.VideoID))
        //                {
        //                    isVideoAlreadyCachedOnAnyConnectedServer = true;
        //                }
        //            }

        //            if(isVideoAlreadyCachedOnAnyConnectedServer)
        //            {
        //                continue;
        //            }

        //            foreach(var cacheserver in caches)
        //            {
        //                if(request.Video.VideoSizeInMb < (cacheserver.MaxMB - cacheserver.ConsumedSpace()))
        //                {
        //                    if(cacheserver.VideoList.Any(v => v.VideoID == request.VideoID))
        //                    {
        //                        break;
        //                    }
        //                    else
        //                    {
        //                        cacheserver.VideoList.Add(new VideoRequest
        //                        { //good lord
        //                            Video = request.Video,
        //                            VideoID = request.VideoID
        //                        });

        //                        break; //as we have added the video to a cache, no point in adding to others
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        private static void NonDuplicateMostRequestVideosFirst(Context context)
        {
            foreach (var request in context.RequestDescriptionList.OrderByDescending(x => x.NumberOfReqeusts))
            {
                foreach (var connection in request.EndPoint.Connections.OrderBy(x => x.LatencyInMilliSecondsFromCacheToEndpoint))
                {
                    var cacheServer = context.CacheServerList[connection.CacheServerID];

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