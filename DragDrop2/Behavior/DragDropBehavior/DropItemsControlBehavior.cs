using System.Windows;
using System.Windows.Controls;

namespace DragDrop2
{
    //作りが変なのは単独でテストしていたBehaviorをDragDropBehaviorに一本化したため

    ///<summary>アイテムのDropを受け入れ追加する  ItemsControlにアタッチ</summary>
    internal class DropItemsControlBehavior : IBehavior
    {
        private ItemsControl @ItemsControl;
        private DragData dragData;

        private IDropable collection => @ItemsControl.ItemsSource as IDropable;

        public DropItemsControlBehavior(ItemsControl itemsControl)
        {
            ItemsControl = itemsControl;
        }

        public void OnAttached()
        {
            @ItemsControl.AllowDrop = true;

            @ItemsControl.QueryContinueDrag += QueryContinueDrag;
            @ItemsControl.PreviewDragOver += PreviewDragOver;
            @ItemsControl.DragOver += DragOver;
            @ItemsControl.Drop += Drop;
        }
        public void OnDetaching()
        {
            @ItemsControl.AllowDrop = false;

            @ItemsControl.QueryContinueDrag -= QueryContinueDrag;
            @ItemsControl.PreviewDragOver -= PreviewDragOver;
            @ItemsControl.DragOver -= DragOver;
            @ItemsControl.Drop -= Drop;
        }

        private void QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if(e.EscapePressed || e.KeyStates.HasFlag(DragDropKeyStates.RightMouseButton))
                MoveCancel();
            dragData?.UpdatePosition();
        }
        private void PreviewDragOver(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(typeof(DragData)))
            {
                dragData = e.Data.GetData(typeof(DragData)) as DragData;
                if(collection?.DataType == dragData.Item.DataType)
                    return;
            }

            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }
        private void DragOver(object sender, DragEventArgs e) => e.Handled = true;
        private void Drop(object sender, DragEventArgs e)
        {
            if(dragData.CurrentCollection == collection) return;

            dragData.Move(collection, collection.Count);
            dragData = null;

            e.Handled = true;
        }

        private void MoveCancel()
        {
            dragData.Cancel();
            dragData = null;
        }
    }
}