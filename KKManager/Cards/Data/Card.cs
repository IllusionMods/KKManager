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
	    public FileInfo CardFile { get; }
        public ChaFileParameter Parameter { get; private set; }
		public Dictionary<string, PluginData> Extended { get; private set; }

        public Image GetCardImage()
        {
            using (var stream = CardFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                return Image.FromStream(stream);
        }

        public Image GetCardFaceImage()
        {
            using (var stream = CardFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
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

	    private Card(FileInfo cardFile)
        {
            CardFile = cardFile;
        }
        
		public static bool TryParseCard(FileInfo file, out Card card)
		{
			card = null;

		    using (var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
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

				    card = new Card(file);
                    
					var info = blockHeader.SearchInfo(ChaFileParameter.BlockName);
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
                    
					//reader.BaseStream.Seek(position + num2, SeekOrigin.Begin);
				}
				catch (EndOfStreamException)
				{
					return false;
				}

				return true;
			}
		}

	    public string GetCharaName()
	    {
            return $"{Parameter.lastname} {Parameter.firstname}";
        }
    }
}
