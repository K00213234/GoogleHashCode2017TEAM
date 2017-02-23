using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GHC.VideoServer
{

    public class Context
    {
       public  List<Video> Videos = new List<Video>();
        public List<EndPoint> EndPointList = new List<EndPoint>();
        public List<RequestDescription> RequestDescriptionList = new List<RequestDescription>();
        public FileDescriptor FileDescriptor { get; set; }
    }

    public class TrafficParser
    {
        public Context context = new Context();
        public FileDescriptor FileDescriptor { get; set; }

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
            for (int endPointIndex = 0; endPointIndex < this.FileDescriptor.EndpointCount; endPointIndex++)
            {
                EndPoint endPoint = ParseEndPoint(lines[lineNumber]);
                lineNumber++;
                for (int i = 0; i < endPoint.NumberOfConnectedCacheServers; i++, lineNumber++)
                {

                    var connectedCacheServer = this.ParseConnectedCacheServer(lines[lineNumber]);
                    endPoint.Connections.Add(connectedCacheServer);
                    connectedCacheServer.EndPoint = endPoint;
                }
                context .EndPointList.Add(endPoint);
            }

            for (int requestIndex = 0; requestIndex < this.FileDescriptor.RequestDescriptorCount; requestIndex++)
            {
                String[] parts = lines[lineNumber].Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                context .RequestDescriptionList.Add(new RequestDescription
                {
                    VideoID = int.Parse(parts[0]),
                    EndPointID = int.Parse(parts[1]),
                    NumberOfReqeusts = int.Parse(parts[2])
                });

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

            this.FileDescriptor = new FileDescriptor
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
                context.Videos.Add(new VideoServer.Video
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
                EndPointID = 0,
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
