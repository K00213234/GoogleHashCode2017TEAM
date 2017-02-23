using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GHC.pizza
{
    class TrafficParser
    {
        public int[,] array;
        int[] videoArray;
        public int videoCount;
        public int endpointCount;
        public int requestDescriptors;
        public int cacheServersCount;
        public int cacheServersCapacityMB;

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
            this.videoArray = new int[this.videoCount];
            this.array = new int[this.videoCount, this.endpointCount];

            for (int lineNumber = 0; lineNumber < this.videoCount; lineNumber++)
            {
                this.ParseRow(lineNumber, lines[lineNumber + 1]);
            }

        }
        public void ParseDefinitionLine(String line)
        {
            String[] parts = line.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            this.videoCount = int.Parse(parts[0]);
            this.endpointCount = int.Parse(parts[1]);
            this.requestDescriptors = int.Parse(parts[2]);
            this.cacheServersCount = int.Parse(parts[3]);
            this.cacheServersCapacityMB = int.Parse(parts[4]);
        }
        public void ParseVideoLine(int rowIndex, string line)
        {
            for (int columnIndex = 0; columnIndex < this.endpointCount; columnIndex++)
            {
                this.array[rowIndex, columnIndex] = line[columnIndex] == TrafficParser.Tatmato ? 1 : 0;
            }
        }
        public void ParseRow(int rowIndex, string line)
        {
            for (int columnIndex = 0; columnIndex < this.endpointCount; columnIndex++)
            {
                this.array[rowIndex, columnIndex] = line[columnIndex] == TrafficParser.Tatmato ? 1 : 0;
            }
        }
        public static String ReadAllFile(String filename)
        {
            String text = String.Empty;

            StreamReader streamReader = new StreamReader(filename);
            text = streamReader.ReadToEnd();
            streamReader.Close();

            return text;
        }
        public String PrintPizza()
        {
            string output = string.Empty;
            for (int row = 0; row < this.videoCount; row++)
            {
                for (int column = 0; column < this.endpointCount; column++)
                {
                    output += array[row, column] == 1 ? TrafficParser.Tatmato : TrafficParser.Mushroom;
                }
                output += Environment.NewLine;
            }
            return output;

        }

        public static char Mushroom = 'M';
        public static char Tatmato = 'T';

    }
}//git
