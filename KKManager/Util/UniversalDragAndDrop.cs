using System;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using KKManager.Data;

namespace KKManager.Util
{
    public class UniversalDragAndDrop : SimpleDragSource
    {
        public static void SetupDragAndDrop(ObjectListView listView, 
            EventHandler<OlvDropEventArgs> onDropped, EventHandler<OlvDropEventArgs> canDrop, EventHandler afterDrag)
        {
            var simpleDropSink = new SimpleDropSink
            {
                AcceptExternal = true,
                EnableFeedback = false,
                UseDefaultCursors = true
            };
            simpleDropSink.Dropped += onDropped;
            simpleDropSink.CanDrop += canDrop;
            var target = listView;
            target.DropSink = simpleDropSink;
            target.AllowDrop = true;

            var fileDragSource = new UniversalDragAndDrop();
            fileDragSource.AfterDrag += afterDrag;
            target.DragSource = fileDragSource;
        }

        private UniversalDragAndDrop() { }

        public override object StartDrag(ObjectListView olv, MouseButtons button, OLVListItem item)
        {
            try
            {
                if (button == MouseButtons.Left)
                {
                    return new DataObject(
                        DataFormats.FileDrop,
                        olv.SelectedObjects.OfType<IFileInfoBase>().Select(x => x.Location.FullName).ToArray());
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

        private event EventHandler AfterDrag;
    }
}