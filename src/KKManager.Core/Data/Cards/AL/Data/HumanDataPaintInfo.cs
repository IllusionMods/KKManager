using MessagePack;
using System.ComponentModel;
using UnityEngine;

namespace KKManager.Data.Cards.AL.Data
{
	[MessagePackObject(true)]
	[Union(0, typeof(HumanDataPresetPaintInfo))]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HumanDataPaintInfo
	{
		public Color color { get; set; }

		public int ID { get; set; }

		public Vector4 layout { get; set; }
	}
}
