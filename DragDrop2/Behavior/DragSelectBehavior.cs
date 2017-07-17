using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace DragDrop2
{
    ///<summary>アイテムのドラッグで選択状態を切り替えるBehavior</summary>
    public class DragSelectBehavior : Behavior<ListBox>
    {
        public Type DataType { get; set; }

        private ListBox @ListBox => AssociatedObject;

        protected override void OnAttached() => @ListBox.PreviewDragOver += PreviewDragOver;
        protected override void OnDetaching() => @ListBox.PreviewDragOver -= PreviewDragOver;

        private void PreviewDragOver(object sender, DragEventArgs e)
        {
            var dragData = e.Data.GetData(typeof(DragData)) as DragData;
            if(DataType != dragData?.Item?.GetType()) return;

            var container = @ListBox.ContainerFromElement((DependencyObject)e.OriginalSource);
            if(container == null) return;

            @ListBox.SelectedIndex = @ListBox.GetIndexFromContainer(container);
        }
    }
}