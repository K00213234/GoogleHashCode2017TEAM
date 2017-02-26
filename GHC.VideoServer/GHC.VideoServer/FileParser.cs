using GHC.VideoServer.Model;
using System;
using System.Collections.Generic;
using System.IO;

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
            Context.CacheServerList = new List<CacheServer>();
            
            for(int i = 0; i < Context.FileDescriptor.CacheServersCount; i++)
            {
                Context.CacheServerList.Add(new CacheServer
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
            Context.EndPointList = new List<EndPoint>();
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
                Context.EndPointList.Add(endPoint);
            }

            //requests
            Context.RequestDescriptionList = new List<RequestDescription>();
            for (int requestIndex = 0; requestIndex < this.Context.FileDescriptor.RequestDescriptorCount; requestIndex++)
            {
                string[] parts = lines[lineNumber].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                var request = new RequestDescription
                {
                    VideoID = int.Parse(parts[0]),
                    EndPointID = int.Parse(parts[1]),
                    NumberOfReqeusts = int.Parse(parts[2])
                };

                request.Video = Context.Videos.Find(x => x.VideoID == request.VideoID);
                request.EndPoint = Context.EndPointList.Find(x => x.EndPointID == request.EndPointID);
              
                Context.RequestDescriptionList.Add(request);

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