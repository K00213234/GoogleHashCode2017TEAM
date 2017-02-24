using GHC.VideoServer.Model;
using System;
using System.IO;

namespace GHC.VideoServer
{
    public class FileParser
    {
        public Context Context = new Context();

        public String filename;

        public void Parse()
        {
            String text = ReadAllFile(this.filename);
            this.Parsetext(text);
        }

        public void Parsetext(String text)
        {
            String[] lines = text.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            this.ParseDefinitionLine(lines[0]);

            int lineNumber = 1;
            this.ParseAndSetVideoList(lines[lineNumber]);
            lineNumber++;
            for (int endPointIndex = 0; endPointIndex < this.Context.FileDescriptor.EndpointCount; endPointIndex++)
            {
                EndPoint endPoint = ParseEndPoint(lines[lineNumber]);
                endPoint.EndPointID = endPointIndex;
                lineNumber++;
                for (int i = 0; i < endPoint.NumberOfConnectedCacheServers; i++, lineNumber++)
                {
                    var connectedCacheServer = this.ParseConnectedCacheServer(lines[lineNumber]);
                    endPoint.Connections.Add(connectedCacheServer);
                    connectedCacheServer.EndPoint = endPoint;
                }
                Context.EndPointList.Add(endPoint);
            }

            for (int requestIndex = 0; requestIndex < this.Context.FileDescriptor.RequestDescriptorCount; requestIndex++)
            {
                String[] parts = lines[lineNumber].Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                var request = new RequestDescription
                {
                    VideoID = int.Parse(parts[0]),
                    EndPointID = int.Parse(parts[1]),
                    NumberOfReqeusts = int.Parse(parts[2])
                };

                var video = Context.Videos.Find(x => x.VideoID == request.VideoID);
                request.Video = video;
                var endpoint = Context.EndPointList.Find(x => x.EndPointID == request.EndPointID);
                request.EndPoint = endpoint;
                Context.RequestDescriptionList.Add(request);

                lineNumber++;
            }
        }

        public EndPointToCacheServerConnection ParseConnectedCacheServer(string line)
        {
            String[] parts = line.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            EndPointToCacheServerConnection result = new EndPointToCacheServerConnection
            {
                CacheServerID = int.Parse(parts[0]),
                LatencyInMilliSecondsFromCacheToEndpoint = int.Parse(parts[1])
            };
            return result;
        }

        public void ParseDefinitionLine(String line)
        {
            String[] parts = line.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            this.Context.FileDescriptor = new FileDescriptor
            {
                VideoCount = int.Parse(parts[0]),
                EndpointCount = int.Parse(parts[1]),
                RequestDescriptorCount = int.Parse(parts[2]),
                CacheServersCount = int.Parse(parts[3]),
                CacheServersCapacityMB = int.Parse(parts[4]),
            };
        }

        public void ParseAndSetVideoList(string line)
        {
            String[] parts = line.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            for (int columnIndex = 0; columnIndex < parts.Length; columnIndex++)
            {
                Context.Videos.Add(new Video
                {
                    VideoID = columnIndex,
                    VideoSizeInMB = int.Parse(parts[columnIndex])
                });
            }
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