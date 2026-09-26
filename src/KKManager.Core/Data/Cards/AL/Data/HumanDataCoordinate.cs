using System;

namespace KKManager.Data.Cards.AL.Data
{
    // TODO doesn't work 
    public class HumanDataCoordinate
    {
        internal static readonly string BlockName = "Coordinate";
        
		public int ProductNo { get; set; }

		public string Tag { get; set; }
        
		public HumanDataAbout About;

		public HumanDataAccessory Accessory;

		public HumanDataBodyMakeup BodyMakeup;

		public HumanDataClothes Clothes;

		public string CoordinateName;

		public HumanDataFaceMakeup FaceMakeup;

		public HumanDataHair Hair;

		public byte[] PngData;

		public byte Sex;
        
		//private HumanData.Define.LoadErrorType _lastLoadErrorCode;

		//private int _tagIndex;

		public static class LoadFileInfo
		{
			[Flags]
			public enum Flags
			{
				None = 0,
				Png = 1,
				Clothes = 2,
				Accessory = 4,
				Hair = 8,
				FaceMakeup = 16,
				BodyMakeup = 32,
				About = 64,
				IgnoreTag = 128,
				BaseData = 126,
				CharaCard = 126,
				Default = 126,
				FileView = 64,
				Convert = 128
			}
		}

		public static class LoadLimited
		{
			[Flags]
			public enum Flags
			{
				None = 0,
				Clothes = 1,
				Accessory = 2,
				Hair = 4,
				FaceMakeup = 8,
				BodyMakeup = 16
			}
		}

		public static class SaveFileInfo
		{
			[Flags]
			public enum Flags
			{
				None = 0,
				Png = 1,
				Clothes = 2,
				Accessory = 4,
				Hair = 8,
				FaceMakeup = 16,
				BodyMakeup = 32,
				About = 64,
				BaseData = 126,
				CharaCard = 126,
				All = 127
			}
		}

		private enum SeekIndex
		{
			Clothes,
			Accessory,
			Hair,
			FaceMakeup,
			BodyMakeup,
			About
		}
	}
}
