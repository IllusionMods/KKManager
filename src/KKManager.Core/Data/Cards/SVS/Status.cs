using System;
using System.ComponentModel;
using MessagePack;

namespace KKManager.Data.Cards.SVS
{
    [MessagePackObject(true)]
    [ReadOnly(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Status
    {
        [IgnoreMember] public static readonly string BlockName = "Status";
        [IgnoreMember] public static readonly Version CurrentVersion = new Version("0.0.0");

        /*
         {
             "version": "0.0.0",
             "coordinateType": 0,
             "backCoordinateType": 2,
             "clothesState": "AAAAAAAAAAA=",
             "showAccessory": [
               true,
               true,
               true,
               true,
               true,
               true,
               true,
               true,
               true,
               true,
               true,
               true,
               true,
               true,
               true,
               true,
               true,
               true,
               true,
               true
             ],
             "eyebrowPtn": 8,
             "eyebrowOpenMax": 1,
             "eyesPtn": 0,
             "eyesOpenMax": 1,
             "eyesBlink": false,
             "eyesYure": false,
             "mouthPtn": 10,
             "mouthOpenMax": 0.5779667,
             "mouthOpenMin": 0,
             "mouthFixed": true,
             "mouthAdjustWidth": true,
             "tongueState": 0,
             "eyesLookPtn": 1,
             "eyesTargetType": 0,
             "eyesTargetAngle": 0,
             "eyesTargetRange": 1,
             "eyesTargetRate": 0.5,
             "neckLookPtn": 3,
             "neckTargetType": 999,
             "neckTargetAngle": 0,
             "neckTargetRange": 1,
             "neckTargetRate": 0.5,
             "disableMouthShapeMask": false,
             "disableBustShapeMask": [
               2,
               10,
               [
                 false,
                 false,
                 false,
                 false,
                 false,
                 false,
                 false,
                 false,
                 false,
                 false,
                 false,
                 false,
                 false,
                 false,
                 false,
                 false,
                 false,
                 false,
                 false,
                 false
               ]
             ],
             "nipStandRate": 0,
             "skinTuyaRate": 0,
             "hohoAkaRate": 0,
             "tearsLv": 0,
             "hideEyesHighlight": false,
             "siruLv": "AAAAAAA=",
             "visibleSon": false,
             "visibleSonAlways": false,
             "visibleHeadAlways": true,
             "visibleBodyAlways": true,
             "visibleSimple": false,
             "visibleGomu": false,
             "simpleColor": [
               0.188,
               0.286,
               0.8,
               0.5
             ],
             "enableShapeHand": [
               false,
               false
             ],
             "shapeHandPtn": [
               2,
               2,
               [
                 0,
                 0,
                 0,
                 0
               ]
             ],
             "shapeHandBlendValue": [
               0,
               0
             ],
             "siriAkaRate": 0,
             "wetRate": 0,
             "sweatRate": 0
           }
         */

        public Version version { get; set; }
        public int coordinateType { get; set; }
        public int backCoordinateType { get; set; }
        public string clothesState { get; set; }
        public bool[] showAccessory { get; set; }
        public int eyebrowPtn { get; set; }
        public int eyebrowOpenMax { get; set; }
        public int eyesPtn { get; set; }
        public int eyesOpenMax { get; set; }
        public bool eyesBlink { get; set; }
        public bool eyesYure { get; set; }
        public int mouthPtn { get; set; }

        public float mouthOpenMax { get; set; }
        public float mouthOpenMin { get; set; }
        public bool mouthFixed { get; set; }
        public bool mouthAdjustWidth { get; set; }
        public int tongueState { get; set; }
        public int eyesLookPtn { get; set; }
        public int eyesTargetType { get; set; }
        public int eyesTargetAngle { get; set; }
        public int eyesTargetRange { get; set; }

        public float eyesTargetRate { get; set; }
        public int neckLookPtn { get; set; }
        public int neckTargetType { get; set; }
        public int neckTargetAngle { get; set; }
        public int neckTargetRange { get; set; }
        public float neckTargetRate { get; set; }
        public bool disableMouthShapeMask { get; set; }
        //public object[] disableBustShapeMask { get; set; } // BUG unknown type
        public float nipStandRate { get; set; }
        public float skinTuyaRate { get; set; }
        public float hohoAkaRate { get; set; }
        public int tearsLv { get; set; }
        public bool hideEyesHighlight { get; set; }
        public string siruLv { get; set; }
        public bool visibleSon { get; set; }
        public bool visibleSonAlways { get; set; }
        public bool visibleHeadAlways { get; set; }
        public bool visibleBodyAlways { get; set; }
        public bool visibleSimple { get; set; }
        public bool visibleGomu { get; set; }
        public float[] simpleColor { get; set; }
        public bool[] enableShapeHand { get; set; }
        // public object[] shapeHandPtn { get; set; } // BUG unknown type
        public float[] shapeHandBlendValue { get; set; }
        public float siriAkaRate { get; set; }
        public float wetRate { get; set; }
        public float sweatRate { get; set; }

    }
}
