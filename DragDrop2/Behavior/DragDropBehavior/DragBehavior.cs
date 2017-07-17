using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DragDrop2
{
    //作りが変なのは単独でテストしていたBehaviorをDragDropBehaviorに一本化したため

    ///<summary>FrameworkElementをDrag開始する  ItemsControl.ItemTemplateのElementにアタッチ</summary>
    internal class DragBehavior : IBehavior
    {
        private FrameworkElement @Element;
        private DragAdorner dragGhost;
        private bool isDraging;
        private Point dragStartPos; // スクリーン座標
        private int originIndex = -1;

        private ItemsControl itemsControl => @Element.FindAncestor<ItemsControl>();
        private FrameworkElement container => itemsControl.ContainerFromElement(@Element) as FrameworkElement;
        private int index => itemsControl.ItemContainerGenerator.IndexFromContainer(container);

        public DragBehavior(FrameworkElement element)
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

            @Element.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDown;
            @Element.PreviewMouseLeftButtonUp += PreviewMouseLeftButtonUp;
            @Element.PreviewMouseMove += PreviewMouseMove;
        }
        public void OnDetaching()
        {
            @Element.PreviewMouseLeftButtonDown -= PreviewMouseLeftButtonDown;
            @Element.PreviewMouseLeftButtonUp -= PreviewMouseLeftButtonUp;
            @Element.PreviewMouseMove -= PreviewMouseMove;
        }

        void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(index < 0) return;

            originIndex = index;
            dragStartPos = e.GetPosition(null);
        }
        void PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            originIndex = -1;
            isDraging = false;
        }
        private void PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if(isDraging) return; // Templateによっては再入することがあるのでスキップ
            if(e.LeftButton != MouseButtonState.Pressed) return;
            if(originIndex < 0) return;

            if(0 <= index && index == originIndex)
            {
                var nowPos = e.GetPosition(null);
                if(!CanStartDraging(nowPos, dragStartPos)) return;
            }
            else { } // マウスがドラッグアイテムの外に出た場合は確認なしですぐスタート

            var dragItem = @Element.DataContext as IDragable;
            if(dragItem == null) return;

            var offset = e.GetPosition(@Element) - new Point();
            using(dragGhost = new DragAdorner(itemsControl, @Element, offset))
            {
                var data = new DragData(dragItem, itemsControl, originIndex, dragGhost)
                {
                    CurrentContainer = container,
                };
                container.Opacity = 0;

                isDraging = true;
                DragDrop.DoDragDrop(itemsControl, data, DragDropEffects.Move); // D&Dが終わるまで帰ってこない
                isDraging = false;

                if(data.CurrentContainer != null)
                    data.CurrentContainer.Opacity = 1;
            }

            originIndex = -1;
            e.Handled = true;
        }

        private bool CanStartDraging(Point p1, Point p2)
            => Math.Abs(p1.X - p2.X) > SystemParameters.MinimumHorizontalDragDistance
            || Math.Abs(p1.Y - p2.Y) > SystemParameters.MinimumVerticalDragDistance;
    }
}
