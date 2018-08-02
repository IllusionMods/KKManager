using System;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using KKManager.Cards.Data;

namespace KKManager.Cards
{
    public class FileDragSource : SimpleDragSource
    {
        public override object StartDrag(ObjectListView olv, MouseButtons button, OLVListItem item)
        {
            if (button != MouseButtons.Left)
                return (object)null;
            return new DataObject(DataFormats.FileDrop,
                olv.SelectedObjects.OfType<Card>().Select(x => x.CardFile.FullName).ToArray());
        }

        public override void EndDrag(object dragObject, DragDropEffects effect)
        {
            base.EndDrag(dragObject, effect);
            AfterDrag?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler AfterDrag;
    }
}