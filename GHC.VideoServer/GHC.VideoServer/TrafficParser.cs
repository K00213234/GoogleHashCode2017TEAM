using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GHC.VideoServer
{


    public class TrafficParser
    {
        public int[,] array;
        int[] videoArray;
        List<Video> Videos = new List<Video>();
        EndPoint[] endPointArray;
        //public int videoCount;
        //public int endpointCount;
        //public int requestDescriptors;
        //public int cacheServersCount;
        //public int cacheServersCapacityMB;

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
            this.videoArray = new int[this.FileDescriptor.VideoCount];
            this.endPointArray = new EndPoint[this.FileDescriptor.EndpointCount];
            //this.array = new int[this.videoCount, this.endpointCount];

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
                endPointArray[endPointIndex] = endPoint;
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
                RequestDescriptors = int.Parse(parts[2]),
                CacheServersCount = int.Parse(parts[3]),
                CacheServersCapacityMB = int.Parse(parts[4]),
            };

        }

        //public RequestDescription ParseRequest(string line)
        //{
        //    var result = new RequestDescription
        //    {
        //        I
        //    }
        //}
        public void ParseAndSetVideoList(string line)
        {
            String[] parts = line.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            for (int columnIndex = 0; columnIndex < parts.Length; columnIndex++)
            {
                this.Videos.Add(new VideoServer.Video
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
        //public void ParseRow(int rowIndex, string line)
        //{
        //    for (int columnIndex = 0; columnIndex < this.endpointCount; columnIndex++)
        //    {
        //        this.array[rowIndex, columnIndex] = line[columnIndex] == TrafficParser.Tatmato ? 1 : 0;
        //    }
        //}
        public static String ReadAllFile(String filename)
        {
            String text = String.Empty;

            StreamReader streamReader = new StreamReader(filename);
            text = streamReader.ReadToEnd();
            streamReader.Close();

            return text;
        }
        //public String PrintPizza()
        //{
        //    string output = string.Empty;
        //    for (int row = 0; row < this.videoCount; row++)
        //    {
        //        for (int column = 0; column < this.endpointCount; column++)
        //        {
        //            output += array[row, column] == 1 ? TrafficParser.Tatmato : TrafficParser.Mushroom;
        //        }
        //        output += Environment.NewLine;
        //    }
        //    return output;

        //}

        public static char Mushroom = 'M';
        public static char Tatmato = 'T';

    }
}//git
