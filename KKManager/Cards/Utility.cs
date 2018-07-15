using System.IO;

namespace KKManager.Cards
{
	public static class Utility
	{
		public const int BufferSize = 4096;

		private static byte[] IENDChunk = {0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82};

		public static long SearchForIEND(Stream stream)
		{
			long origPos = stream.Position;

			byte[] buffer = new byte[BufferSize];
			int read;

			byte scanByte = IENDChunk[0];

			while ((read = stream.Read(buffer, 0, BufferSize)) > 0)
			{
				for (int i = 0; i < read; i++)
				{
					if (buffer[i] != scanByte)
						continue;

					bool flag = true;

					for (int x = 1; x < IENDChunk.Length; x++)
					{
						i++;

						if (i >= BufferSize)
						{
							if ((read = stream.Read(buffer, 0, BufferSize)) < BufferSize)
								return -1;

							i = 0;
						}

						if (buffer[i] != IENDChunk[x])
						{
							flag = false;
							break;
						}
					}

					if (flag)
					{
						long result = (stream.Position + 1) - (BufferSize - i);
						stream.Position = origPos;
						return result;
					}
				}
			}

			return -1;
		}
	}
}
