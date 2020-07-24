using System.Windows.Forms;
using KKManager.Data.Cards;

namespace KKManager.Windows.Dialogs
{
    public partial class RenameCards : Form
    {
        public RenameCards()
        {
            InitializeComponent();
        }

        private Card[] _cardsToRename;
        public static void ShowDialog(IWin32Window owner, Card[] cardsToRename)
        {
            using (var w = new RenameCards())
            {
                w._cardsToRename = cardsToRename;
                w.ShowDialog(owner);
            }
            //todo show a list of things to add, dotted 
            //show text field, insert tokens into it that later get replaced
            //list goes through card fields, goes recursively into classes contained in this assembly only, get dotted names that later get resolved
        }
    }
}
