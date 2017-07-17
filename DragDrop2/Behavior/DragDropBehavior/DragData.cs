using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DragDrop2
{
    ///<summary>ドラッグされるものの情報</summary>
    public class DragData
    {
        ///<summary>ドラッグされるもの本体</summary>
        public readonly IDragable Item;
        ///<summary>現在含んでいるコレクション</summary>
        public IDropable CurrentCollection;
        ///<summary>現在のコンデナ</summary>
        public FrameworkElement CurrentContainer;

        //雑多なものが多すぎですね；
        private readonly DragAdorner dragGhost;
        private readonly ItemsControl originItemsControl;
        private readonly IDropable originCollection;
        private readonly int originIndex;

        /// <summary>ドラッグされるものの情報</summary>
        /// <param name="item">ドラッグされるもの本体</param>
        /// <param name="itemsControl">含んでいるItemsControl</param>
        /// <param name="index">コレクション内のインデックス</param>
        /// <param name="dragGhost">ドラッグゴースト</param>
        public DragData(IDragable item, ItemsControl itemsControl, int index, DragAdorner dragGhost)
        {
            Item = item;
            originItemsControl = itemsControl;
            originCollection= originItemsControl.ItemsSource as IDropable;
            CurrentCollection = originCollection;
            originIndex = index;
            this.dragGhost = dragGhost;
        }

        /// <summary>アイテム移動</summary>
        /// <param name="collection">移動先コレクション</param>
        /// <param name="index">>移動先インデックス</param>
        public void Move(IDropable collection, int index)
        {
            if(Item.DataType != collection.DataType) return;
            if(Item.DataType != CurrentCollection.DataType) return;

            CurrentCollection.Remove(Item);
            collection.Insert(index, Item);
            CurrentCollection = collection;
        }
        /// <summary>移動キャンセル</summary>
        public void Cancel()
        {
            if(Item.DataType != originCollection.DataType) return;
            if(Item.DataType != CurrentCollection.DataType) return;

            CurrentCollection.Remove(Item);
            originCollection.Insert(originIndex, Item);
        }
        /// <summary>ドラッグゴースト位置更新</summary>
        public void UpdatePosition()
        {
            var p = NativeMethods.GetMousePosition(originItemsControl);
            dragGhost?.UpdatePosition(p);
        }

        private static class NativeMethods
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct POINT { public int X; public int Y; }

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool GetCursorPos(out POINT point);

            ///<summary>HighDPI非考慮</summary>
            public static Point GetMousePosition(Visual vsual)
            {
                GetCursorPos(out var p);
                return vsual.PointFromScreen(new Point(p.X, p.Y));
            }
        }
    }
}
