using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using KKManager.Cards.Data.Internal;
using MessagePack;

namespace KKManager.Cards.Data
{
	public class Card
	{
		protected Func<Stream> streamGenerator { get; set; }
		
		public ChaFileParameter Parameter { get; protected set; }
		public Dictionary<string, PluginData> Extended { get; protected set; }

		public Image CardImage
		{
			get
			{
				using (var stream = streamGenerator())
					return Image.FromStream(stream);
			}
		}

		public Image CardFaceImage
		{
			get
			{
				using (var stream = streamGenerator())
				using (BinaryReader reader = new BinaryReader(stream))
				{
					stream.Position = Utility.SearchForIEND(stream);
					int loadProductNo = reader.ReadInt32();
					string marker = reader.ReadString();
					var loadVersion = new Version(reader.ReadString());
					int faceLength = reader.ReadInt32();

					using (MemoryStream memStream = new MemoryStream(reader.ReadBytes(faceLength)))
						return Image.FromStream(memStream);
				}
			}
		}

		protected Card(Func<Stream> streamGenerator)
		{
			this.streamGenerator = streamGenerator;
		}

		public static bool TryParseCard(Func<Stream> streamFunc, out Card card)
		{
			card = null;

			using (Stream stream = streamFunc())
			using (BinaryReader reader = new BinaryReader(stream))
			{
				long IEND = Utility.SearchForIEND(stream);

				if (IEND == -1 || IEND >= stream.Length)
					return false;


				stream.Position = IEND;

				try
				{
					int loadProductNo = reader.ReadInt32();
					if (loadProductNo > 100)
					{
						return false;
					}

					string marker = reader.ReadString();
					if (marker != "【KoiKatuChara】")
					{
						return false;
					}

					var loadVersion = new Version(reader.ReadString());
					if (0 > new Version("0.0.0").CompareTo(loadVersion))
					{
						return false;
					}

					int faceLength = reader.ReadInt32();
					if (faceLength > 0)
					{
						//this.facePngData = reader.ReadBytes(num);
						stream.Seek(faceLength, SeekOrigin.Current);
					}

					int count = reader.ReadInt32();
					byte[] bytes = reader.ReadBytes(count);
					BlockHeader blockHeader = MessagePackSerializer.Deserialize<BlockHeader>(bytes);
					long num2 = reader.ReadInt64();
					long position = reader.BaseStream.Position;
                    
					BlockHeader.Info info;

					card = new Card(streamFunc);

					//BlockHeader.Info info = blockHeader.SearchInfo(ChaFileCustom.BlockName);
					//if (info != null)
					//{
					//	Version version = new Version(info.version);
					//	if (0 > ChaFileDefine.ChaFileCustomVersion.CompareTo(version))
					//	{
					//		this.lastLoadErrorCode = -2;
					//	}
					//	else
					//	{
					//		reader.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
					//		byte[] data = reader.ReadBytes((int)info.size);
					//		this.SetCustomBytes(data, version);
					//	}
					//}
					//info = blockHeader.SearchInfo(ChaFileCoordinate.BlockName);
					//if (info != null)
					//{
					//	Version version2 = new Version(info.version);
					//	if (0 > ChaFileDefine.ChaFileCoordinateVersion.CompareTo(version2))
					//	{
					//		this.lastLoadErrorCode = -2;
					//	}
					//	else
					//	{
					//		reader.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
					//		byte[] data2 = reader.ReadBytes((int)info.size);
					//		this.SetCoordinateBytes(data2, version2);
					//	}
					//}

					info = blockHeader.SearchInfo(ChaFileParameter.BlockName);
					if (info != null)
					{
						Version value = new Version(info.version);
						if (0 <= ChaFileParameter.CurrentVersion.CompareTo(value))
						{
							reader.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
							byte[] parameterBytes = reader.ReadBytes((int)info.size);

							card.Parameter = MessagePackSerializer.Deserialize<ChaFileParameter>(parameterBytes);
							card.Parameter.ComplementWithVersion();
						}
					}

					info = blockHeader.SearchInfo(ChaFileExtended.BlockName);
					if (info != null)
					{
						reader.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
						byte[] parameterBytes = reader.ReadBytes((int)info.size);
						
						card.Extended = MessagePackSerializer.Deserialize<Dictionary<string, PluginData>>(parameterBytes);
					}

					//if (!noLoadStatus)
					//{
					//	info = blockHeader.SearchInfo(ChaFileStatus.BlockName);
					//	if (info != null)
					//	{
					//		Version value2 = new Version(info.version);
					//		if (0 > ChaFileDefine.ChaFileStatusVersion.CompareTo(value2))
					//		{
					//			this.lastLoadErrorCode = -2;
					//		}
					//		else
					//		{
					//			reader.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
					//			byte[] statusBytes = reader.ReadBytes((int)info.size);
					//			this.SetStatusBytes(statusBytes);
					//		}
					//	}
					//}
					reader.BaseStream.Seek(position + num2, SeekOrigin.Begin);
				}
				catch (EndOfStreamException)
				{
					return false;
				}

				return true;
			}
		}
	}
}
