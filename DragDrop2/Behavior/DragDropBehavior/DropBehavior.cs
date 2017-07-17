using System.Windows;
using System.Windows.Controls;

namespace DragDrop2
{
    //作りが変なのは単独でテストしていたBehaviorをDragDropBehaviorに一本化したため

    ///<summary>アイテムのDropで位置を入れ替える  ItemsControl.ItemTemplateのElementにアタッチ</summary>
    internal class DropBehavior : IBehavior
    {
        private FrameworkElement @Element;
        private int wait;
        private DragData dragData;

        private ItemsControl itemsControl => @Element.FindAncestor<ItemsControl>();
        private FrameworkElement container => itemsControl.ContainerFromElement(@Element) as FrameworkElement;
        private IDropable collection => itemsControl.ItemsSource as IDropable;
        private int index => itemsControl.ItemContainerGenerator.IndexFromContainer(container);

        public DropBehavior(FrameworkElement element)
        {
            @Element = element;
        }

        public void OnAttached()
        {
            if(itemsControl == null)
                throw new DragDropBehaviorException("ItemsControlがみつかりません");
            if(itemsControl.ItemsSource == null)
                throw new DragDropBehaviorException("ItemsSourceがありません");
            if(itemsControl.ItemsSource as IDropable == null)
                throw new DragDropBehaviorException("ItemsSourceがIDropableを実装していません");

            @Element.AllowDrop = true;

            @Element.PreviewDragOver += PreviewDragOver;
            @Element.DragOver += DragOver;
            @Element.Drop += Drop;
        }
        public void OnDetaching()
        {
            @Element.AllowDrop = false;

            @Element.PreviewDragOver -= PreviewDragOver;
            @Element.DragOver -= DragOver;
            @Element.Drop -= Drop;
        }

        private void PreviewDragOver(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(typeof(DragData))) return;

            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }
        private void DragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
            dragData = e.Data.GetData(typeof(DragData)) as DragData;
            var dragItem = dragData.Item;

            if(index < 0) return;
            if(dragItem == @Element.DataContext) return;

            var targetItem = itemsControl.GetContainerFromIndex(index);
            var mousePosition = e.GetPosition(itemsControl);
            if(!CanStartMoving(mousePosition, targetItem)) return;
            if(wait++ < 10) return; // 入れ替えが連続してプルプルするのを防止するウェイト（少ないと防止できない 大きいとレスポンスが悪くなる）
            wait = 0;

            var i = index; // Removeでずれるのでバックアップ
            dragData.Move(collection, i);

            dragData.CurrentContainer = itemsControl.GetContainerFromIndex(i);
            dragData.CurrentContainer.Opacity = 0;

        }
        private void Drop(object sender, DragEventArgs e)
        {
            e.Handled = true;
            dragData = null;
        }

        private bool CanStartMoving(Point mousePosition, FrameworkElement targetItem)
        {
            var topLeft = targetItem.TransformToAncestor(itemsControl).Transform(new Point(0, 0));
            var rect = new Rect(topLeft, new Size(targetItem.ActualWidth, targetItem.ActualHeight));
            rect.Inflate(-rect.Width / 6, -rect.Height / 6); // ターゲットの2/3以上に侵入したら入れ替え
            return rect.Contains(mousePosition);
        }
    }
}
