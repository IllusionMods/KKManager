using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using KKManager.Cards.Data;
using KKManager.Cards.Data.Internal;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Cards
{
	public partial class CardWindow : DockContent
    {
		public CardWindow()
		{
			InitializeComponent();
		    AutoScaleMode = AutoScaleMode.Dpi;
        }

		private void formMain_Load(object sender, EventArgs e)
		{
			InitCards();
		}

		#region Cards

		private ImageList activeImageList { get; set; }

		private ConcurrentDictionary<string, Image> masterImageList { get; set; }

		private List<ListViewItem> lsvItems = new List<ListViewItem>();

		private List<Size> ListViewCardSizes { get; set; } = new List<Size>
		{
			new Size(92, 128),
			new Size(183, 256)
		};


		private void InitCards()
		{
			InitCardBindings();

			masterImageList = new ConcurrentDictionary<string, Image>();
			activeImageList = new ImageList();

			activeImageList.ColorDepth = ColorDepth.Depth24Bit;
			activeImageList.ImageSize = new Size(183, 256);
			
			lsvCards.LargeImageList = activeImageList;


			lsvCards.BeginUpdate();

            var charaCardPath = Path.Combine(Program.KoikatuDirectory.FullName, @"UserData\chara\female");
		    if (!Directory.Exists(charaCardPath))
		    {
		        MessageBox.Show($"The card directory \"{charaCardPath}\" doesn't exist or is inaccessible.", "Load cards",
		            MessageBoxButtons.OK, MessageBoxIcon.Error);
		    }
            else
		    {
		        foreach (string file in Directory.EnumerateFiles(charaCardPath, "*.png", SearchOption.AllDirectories))
		        {

		            string key = Path.GetFileName(file);

		            using (MemoryStream mem = new MemoryStream(File.ReadAllBytes(file)))
		            {

		                string itemName = key;

		                if (Card.TryParseCard(() => File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read), out Card card))
		                {
		                    itemName = $"{card.Parameter.lastname} {card.Parameter.firstname}";
		                }

		                var item = new ListViewItem(itemName, key);
		                item.Name = key;
		                lsvItems.Add(item);


		                if (card != null)
		                    item.Tag = card;
		                else
		                {
		                    item.ForeColor = Color.Red;
		                    item.Font = new Font(item.Font, FontStyle.Italic);
		                }

		            }
		        }
		    }

			lsvCards.VirtualListSize = lsvItems.Count;
			lsvCards.EndUpdate();

			cmbCardsViewSize.SelectedIndex = 0;
		}

		private void cmbCardsViewSize_SelectedIndexChanged(object sender, EventArgs e)
		{
			lsvCards.BeginUpdate();

			activeImageList.ImageSize = ListViewCardSizes[cmbCardsViewSize.SelectedIndex];

			foreach (Image image in activeImageList.Images)
			{
				image.Dispose();
			}

			activeImageList.Images.Clear();




			lsvCards.EndUpdate();
		}

		#region Card Databinding

		private BindingSource cardParameterSource = new BindingSource();

		private void InitCardBindings()
		{
			cardParameterSource = new BindingSource();
			cardParameterSource.DataSource = typeof(ChaFileParameter);

			txtCardFirstName.DataBindings.Add(nameof(Label.Text), cardParameterSource, nameof(ChaFileParameter.firstname));
			txtCardLastName.DataBindings.Add(nameof(Label.Text), cardParameterSource, nameof(ChaFileParameter.lastname));
			txtCardNickname.DataBindings.Add(nameof(Label.Text), cardParameterSource, nameof(ChaFileParameter.nickname));
		}

		private void SetCardDatabindings(Card card)
		{
			cardParameterSource.DataSource = card.Parameter;

			if (imgCard.Image != null)
			{
				imgCard.Image.Dispose();
				imgCard.Image = null;
			}

			if (imgCardFace.Image != null)
			{
				imgCardFace.Image.Dispose();
				imgCardFace.Image = null;
			}

			imgCard.Image = card.CardImage;
			imgCardFace.Image = card.CardFaceImage;

			lsvCardExtData.Items.Clear();

			if (card.Extended != null)
			{
				foreach (string key in card.Extended.Keys)
					lsvCardExtData.Items.Add(key);
			}
		}

		private void WriteToBindingCard()
		{
			cardParameterSource.EndEdit();
		}

		#endregion

		#endregion

		private void lsvCards_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lsvCards.SelectedIndices.Count == 0) 
				return;
			
			if (lsvItems[lsvCards.SelectedIndices[0]].Tag is Card card)
			{
				SetCardDatabindings(card);
			}
		}

		private void lsvCards_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
		{
			e.Item = lsvItems[e.ItemIndex];

			if (activeImageList.Images.ContainsKey(e.Item.Name))
				return;

			Image image = (e.Item.Tag as Card)?.CardImage;

			if (image == null)
				image = new Bitmap(1, 1);

			activeImageList.Images.Add(e.Item.Name, image);
			e.Item.ImageKey = e.Item.Name;
			e.Item.ImageIndex = activeImageList.Images.IndexOfKey(e.Item.Name);
		}
	}
}
