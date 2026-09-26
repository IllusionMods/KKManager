using MessagePack;
using System.ComponentModel;

namespace KKManager.Data.Cards.AL.Data
{
	[MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HumanDataPresetPaintInfo : HumanDataPaintInfo
	{
		public int layoutID { get; set; }
	}
}
