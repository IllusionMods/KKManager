using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BrightIdeasSoftware;

namespace KKManager.Util
{
    public class ModMatchFilter : IModelFilter
    {
        public StringComparison StringComparison
        {
            get => _textMatchFilter.StringComparison;
            set => _textMatchFilter.StringComparison = value;
        }

        public IReadOnlyCollection<string> ContainsStrings
        {
            get => _containsStrings;
            set => SetContainsStrings(value);
        }

        public ObjectListView ListView => _textMatchFilter.ListView;

        private IReadOnlyCollection<string> _containsStrings;
        private readonly TextMatchFilter _textMatchFilter;
        private readonly HashSet<Data.ModInfoBase> _sameMods;
        private string _sameCol;
        private bool _isMod;
        private bool _isFilterCol;


        public ModMatchFilter(ObjectListView olv)
        {
            _textMatchFilter = new TextMatchFilter(olv);
            _sameMods = new HashSet<Data.ModInfoBase>();
        }

        public bool Filter(object modelObject)
        {
            bool textMatch = _textMatchFilter.Filter(modelObject);

            if (_isMod && _isFilterCol && textMatch)
            {
                return _sameMods.Contains(modelObject);
            }
            return textMatch;
        }


        private void SetContainsStrings(IReadOnlyCollection<string> strs)
        {
            _containsStrings = strs;
            _sameCol = null;
            _isFilterCol = false;
            _isMod = false;
            _sameMods.Clear();
            if (strs == null)
            {
                _textMatchFilter.ContainsStrings = null;
                return;
            }

            foreach (var item in ListView.Objects)
            {
                _isMod = item is Data.ModInfoBase;
                break;
            }


            List<string> textStrs = new List<string>();
            foreach (var item in strs)
            {
                string words = item;
                if (item.Contains("same:guid"))   // same:guid
                {
                    words = item.Replace("same:guid", "");
                    _sameCol = "guid";
                }
                else if (item.Contains("same:name"))   // same:name
                {
                    words = item.Replace("same:name", "");
                    _sameCol = "name";
                }
                textStrs.Add(words.Trim());
            }

            _isFilterCol = !string.IsNullOrWhiteSpace(_sameCol);
            _textMatchFilter.ContainsStrings = textStrs;

            if (_isMod)
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

            switch (_sameCol)
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

            _sameMods.Clear();
            foreach (var item in filterList)
            {
                foreach (var mod in item)
                {
                    _sameMods.Add(mod);
                }
            }

            return _sameMods;
        }


    }
}
