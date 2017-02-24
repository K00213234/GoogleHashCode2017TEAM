using System.Collections.Generic;
using System.Linq;

namespace GHC.VideoServer.Model
{
	public class Context
	{
		public List<Video> Videos = new List<Video>();
		public List<EndPoint> EndPointList = new List<EndPoint>();
		public List<RequestDescription> RequestDescriptionList = new List<RequestDescription>();
		public FileDescriptor FileDescriptor { get; set; }
		public void CalcVideoRequests()
		{
            foreach (RequestDescription item in this.RequestDescriptionList)
            {
                this.AddToList(item.VideoID, item.NumberOfReqeusts);
            }
		}


		public List<VideoRequest> SortByFrequencyDescending()
		{
			var result = this.VideoRequestList.OrderByDescending(x => x.RequestTotal);
            return result.ToList();
		}

		public void AddToList(int videoId, int request)
		{
			foreach(VideoRequest item in this.VideoRequestList)
			{
				if(item.VideoID == videoId)
				{
					item.RequestTotal+= request;
					return;
				}
			}
			VideoRequest newitem = new VideoRequest{VideoID=videoId, RequestTotal=request};
			this.VideoRequestList.Add(newitem);

		}
		public List<VideoRequest> VideoRequestList = new List<VideoRequest>();

		public void MakeCacheServers()
		{
			for(int i = 0; i < this.FileDescriptor.CacheServersCount; i++)
			{
				CacheServer cacheServer = new CacheServer();
				cacheServer.ID = i;
                cacheServer.MaxMB = FileDescriptor.CacheServersCapacityMB;               
				this.CacheServerList.Add(cacheServer);
			}
		}
        

		public void LoadServers()
		{
			this.CalcVideoRequests();

			List<VideoRequest> videolist = this.SortByFrequencyDescending();
			foreach(CacheServer item in this.CacheServerList)
			{
				item.VideoList.AddRange(videolist);
			}

		}
		public List<CacheServer> CacheServerList = new List<CacheServer>();
	}
}