using KKManager.Cards.Data;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BrightIdeasSoftware;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Cards
{
    public partial class CardWindow : DockContent
    {
        public CardWindow()
        {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.Dpi;
            _loader = new CardLoader();
            _loader.CardsChanged += Loader_CardsChanged;

            olvColumnName.AspectGetter = rowObject => (rowObject as Card)?.GetCharaName();
            olvColumnFilename.AspectGetter = rowObject => (rowObject as Card)?.CardFile.Name;
            olvColumnModDate.AspectGetter = rowObject => (rowObject as Card)?.CardFile.LastWriteTime;

            listView.LargeImageList = new ImageList
            {
                ImageSize = new Size(183, 256),
                ColorDepth = ColorDepth.Depth24Bit
            };

            listView.SmallImageList = new ImageList
            {
                ImageSize = new Size(38, 51),
                ColorDepth = ColorDepth.Depth24Bit
            };
            
            olvColumnName.ImageGetter = delegate (object rowObject)
            {
                var card = (rowObject as Card);
                if (card == null) return null;

                var key = card.CardFile.FullName;
                if (!listView.LargeImageList.Images.ContainsKey(key))
                {
                    listView.SmallImageList.Images.Add(key, card.CardFaceImage);
                    listView.LargeImageList.Images.Add(key, card.CardImage);
                }
                return key;
            };

            Details(this, EventArgs.Empty);

            ((OLVColumn) listView.Columns[listView.Columns.Count - 1]).FillsFreeSpace = true;
        }

        private void Loader_CardsChanged(object sender, EventArgs e)
        {
            listView.SmallImageList?.Images.Clear();
            listView.LargeImageList?.Images.Clear();

            listView.SetObjects(_loader.Cards);
            listView.AutoResizeColumns();
        }

        private readonly CardLoader _loader;

        public void OpenCardDirectory(DirectoryInfo directory)
        {
            _loader.Read(directory);
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            //InitCards();

            listView.Sort(olvColumnModDate, SortOrder.Descending);
        }

        #region Cards


        /*private List<ListViewItem> lsvItems = new List<ListViewItem>();

        private List<Size> ListViewCardSizes { get; set; } = new List<Size>
        {
            new Size(92, 128),
            new Size(183, 256)
        };

        private ImageList activeImageList { get; set; }

        private ConcurrentDictionary<string, Image> masterImageList { get; set; }*/

        #endregion

        private void fastObjectListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.SelectedObject != null)
                MainWindow.Instance.DisplayInPropertyViewer(listView.SelectedObject);
        }

        private void Details(object sender, EventArgs e)
        {
            listView.View = View.Details;
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            toolStripViewSelect.Text = detailsToolStripMenuItem.Text;
        }

        private void SmallIcons(object sender, EventArgs e)
        {
            listView.View = View.SmallIcon;

            toolStripViewSelect.Text = smallIconsToolStripMenuItem.Text;
        }

        private void LargeIcons(object sender, EventArgs e)
        {
            listView.View = View.LargeIcon;

            toolStripViewSelect.Text = largeIconsToolStripMenuItem.Text;
        }
    }
}
