using KKManager.Data.Cards;
using WeifenLuo.WinFormsUI.Docking;

namespace KKManager.Windows.ToolWindows.Properties.Viewers
{
    public partial class CardInfoViewerBase : PropertyViewerBase
    {
        private Card _currentCard;

        public CardInfoViewerBase()
        {
            InitializeComponent();
            SupportedTypes = new[] { typeof(Card) };
        }

        public Card CurrentCard
        {
            get => _currentCard;
            set
            {
                _currentCard = value;

                propertyGrid1.SelectedObject = _currentCard;

                imgCard.Image?.Dispose();
                imgCardFace.Image?.Dispose();
                imgCard.Image = _currentCard?.GetCardImage();
                imgCardFace.Image = _currentCard?.GetCardFaceImage();

                lsvCardExtData.Items.Clear();
                if (_currentCard?.Extended != null)
                {
                    foreach (var key in _currentCard.Extended.Keys)
                        lsvCardExtData.Items.Add(key);
                }
            }
        }

        public override void DisplayObjectProperties(object obj, DockContent source)
        {
            CurrentCard = obj as Card;
        }
    }
}
