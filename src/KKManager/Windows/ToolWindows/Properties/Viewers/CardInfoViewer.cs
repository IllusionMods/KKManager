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
            }
        }

        public override void DisplayObjectProperties(object obj, DockContent source)
        {
            CurrentCard = obj as Card;
        }
    }
}
