using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrightIdeasSoftware;

namespace KKManager.Util
{
    public class ModMatchFilter : IModelFilter
    {
        public StringComparison StringComparison
        {
            get { return textMatchFilter.StringComparison; }
            set { textMatchFilter.StringComparison = value; }
        }

        public IEnumerable<string> ContainsStrings
        {
            get { return _ContainsStrings; }
            set { SetContainsStrings(value); }
        }

        public ObjectListView ListView { get { return textMatchFilter.ListView; } }

        IEnumerable<string> _ContainsStrings;
        TextMatchFilter textMatchFilter;
        HashSet<Data.ModInfoBase> sameMods;
        string sameCol;
        bool isMod;
        bool isFilterCol;


        public ModMatchFilter(ObjectListView olv)
        {
            textMatchFilter = new TextMatchFilter(olv);
            sameMods = new HashSet<Data.ModInfoBase>();
        }

        public bool Filter(object modelObject)
        {
            bool textMatch = textMatchFilter.Filter(modelObject);

            if (isMod && isFilterCol && textMatch)
            {
                return sameMods.Contains(modelObject);
            }
            return textMatch;
        }


        private void SetContainsStrings(IEnumerable<string> strs)
        {
            _ContainsStrings = strs;
            sameCol = null;
            isFilterCol = false;
            isMod = false;
            sameMods.Clear();
            if (strs == null)
            {
                textMatchFilter.ContainsStrings = null;
                return;
            }

            foreach (var item in ListView.Objects)
            {
                isMod = item is Data.ModInfoBase;
                break;
            }


            List<string> textStrs = new List<string>();
            foreach (var item in strs)
            {
                string words = item;
                if (item.Contains("same:guid"))   // same:guid
                {
                    words = item.Replace("same:guid", "");
                    sameCol = "guid";
                }
                else if (item.Contains("same:name"))   // same:name
                {
                    words = item.Replace("same:name", "");
                    sameCol = "name";
                }
                textStrs.Add(words.Trim());
            }

            isFilterCol = !string.IsNullOrWhiteSpace(sameCol);
            textMatchFilter.ContainsStrings = textStrs;

            if (isMod)
            {
                var mods = new List<Data.ModInfoBase>(500);
                foreach (var item in ListView.Objects)
                {
                    mods.Add(item as Data.ModInfoBase);
                }
                FilterSameColumn(mods);
            }
        }


        private IEnumerable FilterSameColumn(IEnumerable<Data.ModInfoBase> modelObjects)
        {
            IEnumerable<IGrouping<string, Data.ModInfoBase>> filterList;

            switch (sameCol)
            {
                case "guid":
                    filterList = from r in modelObjects
                                 group r by r.Guid into g
                                 where g.Count() > 1
                                 select g;
                    break;
                case "name":
                    filterList = from r in modelObjects
                                 group r by r.Name into g
                                 where g.Count() > 1
                                 select g;
                    break;
                default:
                    return modelObjects;
            }

            sameMods.Clear();
            foreach (var item in filterList)
            {
                foreach (var mod in item)
                {
                    sameMods.Add(mod);
                }
            }

            return sameMods;
        }


    }
}
