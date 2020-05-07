using System.Collections.Generic;
using MessagePack;

namespace KKManager.Data.Cards.AI
{
	[MessagePackObject(true)]
	public class BlockHeader
	{
		public List<Info> lstInfo { get; set; } = new List<Info>();

		public Info SearchInfo(string name)
		{
			return lstInfo.Find(x => x.name == name);
		}

		[MessagePackObject(true)]
		public class Info
		{
			public string name { get; set; }

			public string version { get; set; }

			public long pos { get; set; }

			public long size { get; set; }
		}
	}
}