using GHC.VideoServer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GHC.VideoServer
{
    public class FileParser
    {
        public Context Context = new Context();

        public string filename;

        public void Parse()
        {
            var text = ReadAllFile(this.filename);
            this.ParseText(text);
        }

        public void ParseText(string text)
        {
            //FileDescriptor
            string[] lines = text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Context.FileDescriptor = ParseDefinitionLine(lines[0]);

            //CacheServers
            Context.CacheServers = new List<CacheServer>();
            
            for(int i = 0; i < Context.FileDescriptor.CacheServersCount; i++)
            {
                Context.CacheServers.Add(new CacheServer
                {
                    ID = i,
                    MaxMB = Context.FileDescriptor.CacheServersCapacityMB,   
                });
            }

            //Videos
            int lineNumber = 1;
            Context.Videos = ParseVideos(lines[lineNumber]);
            lineNumber++;

            //EndPoint
            Context.EndPoints = new List<EndPoint>();
            for (int i = 0; i < this.Context.FileDescriptor.EndpointCount; i++)
            {
                var endPoint = ParseEndPoint(lines[lineNumber]);
                endPoint.EndPointID = i;
                lineNumber++;
                for (int j = 0; j < endPoint.NumberOfConnectedCacheServers; j++, lineNumber++)
                {
                    var connectedCacheServer = this.ParseConnectedCacheServer(lines[lineNumber]);
                    endPoint.Connections.Add(connectedCacheServer);
                    connectedCacheServer.EndPoint = endPoint;
                }
                Context.EndPoints.Add(endPoint);
            }

            //requests
            Context.Requests = new List<RequestDescription>();
            for (int requestIndex = 0; requestIndex < this.Context.FileDescriptor.RequestDescriptorCount; requestIndex++)
            {
                string[] parts = lines[lineNumber].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                var request = new RequestDescription
                {
                    ID = requestIndex,
                    VideoID = int.Parse(parts[0]),
                    EndPointID = int.Parse(parts[1]),
                    NumberOfReqeusts = int.Parse(parts[2])
                };

                request.Video = Context.Videos.Find(x => x.VideoID == request.VideoID);
                request.EndPoint = Context.EndPoints.Find(x => x.EndPointID == request.EndPointID);

                //is there an existing request from the same endpoint to for the same video
                var dupes = Context.Requests.Where(x => x.VideoID == request.VideoID && x.EndPointID == request.EndPointID).ToList();

                if (dupes.Count > 1)
                {
                    throw new Exception("esfsefds");
                }

                if (dupes.Any())
                {
                    dupes.First().NumberOfReqeusts += request.NumberOfReqeusts;
                }
                else
                {
                    Context.Requests.Add(request);
                }
                lineNumber++;
            }
        }

        public EndPointToCacheServerConnection ParseConnectedCacheServer(string line)
        {
            string[] parts = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var result = new EndPointToCacheServerConnection
            {
                CacheServerID = int.Parse(parts[0]),
                LatencyInMilliSecondsFromCacheToEndpoint = int.Parse(parts[1])
            };
            return result;
        }

        public FileDescriptor ParseDefinitionLine(string line)
        {
            string[] parts = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            var result = new FileDescriptor
            {
                VideoCount = int.Parse(parts[0]),
                EndpointCount = int.Parse(parts[1]),
                RequestDescriptorCount = int.Parse(parts[2]),
                CacheServersCount = int.Parse(parts[3]),
                CacheServersCapacityMB = int.Parse(parts[4]),
            };
            return result;
        }

        public List<Video> ParseVideos(string line)
        {
            var result = new List<Video>();
            string[] parts = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                result.Add(new Video
                {
                    VideoID = i,
                    VideoSizeInMb = int.Parse(parts[i])
                });
            }

            return result;
        }

        public EndPoint ParseEndPoint(string line)
        {
            String[] parts = line.Split(new String[] { " " }, 2, StringSplitOptions.RemoveEmptyEntries);

            var result = new EndPoint
            {
                LatencyInMiliSecondsFromDataCenter = int.Parse(parts[0]),
                NumberOfConnectedCacheServers = int.Parse(parts[1])
            };

            return result;
        }

        public static String ReadAllFile(String filename)
        {
            String text = String.Empty;

            StreamReader streamReader = new StreamReader(filename);
            text = streamReader.ReadToEnd();
            streamReader.Close();

            return text;
        }
    }
}//git