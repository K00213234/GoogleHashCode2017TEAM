using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GHC.VideoServer
{
	public class Context
	{
		public  List<Video> Videos = new List<Video>();
		public List<EndPoint> EndPointList = new List<EndPoint>();
		public List<RequestDescription> RequestDescriptionList = new List<RequestDescription>();
		public FileDescriptor FileDescriptor { get; set; }

		public void CalcVideoRequests()
		{
		
			foreach(RequestDescription item in this.RequestDescriptionList)
			{
				this.AddToList(item.VideoID, item.NumberOfReqeusts);
			}


		}


		public List<VideoRequest> SortByFrequencyDescending()
		{
			var result = this.VideoREquestList.OrderByDescending(x => x.RequestTotal);
			return result;
		}

		public void AddToList(int videoId, int request)
		{
			foreach(VideoRequest item in this.VideoREquestList)
			{
				if(item.VideoID == videoId)
				{
					item.RequestTotal+= request;
					return;
				}
			}
			VideoRequest newitem = new VideoRequest{VideoID=videoId, RequestTotal=request};
			this.VideoREquestList.Add(newitem);

		}
		public List<VideoRequest> VideoREquestList = new List<VideoRequest>();
	}
	public class VideoRequest
	{
		public int VideoID;
		public int RequestTotal;
	}

	public class Solution
	{
		public List<CacheServer> CacheServerList = new List<CacheServer>();

		public override String ToString()
		{
			String output = this.CacheServerList.Count + Environment.NewLine;
			foreach(CacheServer item in this.CacheServerList)
			{
				output += item.ID;
				for(int index = 0; index <= item.VideoList.Count; index++)
				{
					output += " " + item.VideoList[index].VideoID;
				}
				output += Environment.NewLine;
			}
			return output;
		}
	}
	public class CacheServer
	{
		public int ID { get; set; }
		public List<Video> VideoList = new List<Video>();
	}
}