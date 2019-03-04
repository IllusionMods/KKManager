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
            try
            {
                if (button == MouseButtons.Left)
                {
                    return new DataObject(
                        DataFormats.FileDrop,
                        olv.SelectedObjects.OfType<Card>().Select(x => x.CardFile.FullName).ToArray());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        public override void EndDrag(object dragObject, DragDropEffects effect)
        {
            try
            {
                base.EndDrag(dragObject, effect);
                AfterDrag?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public event EventHandler AfterDrag;
    }
}