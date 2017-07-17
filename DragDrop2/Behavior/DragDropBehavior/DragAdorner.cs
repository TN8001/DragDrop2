using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace DragDrop2
{
    /// <summary>ドラッグゴースト</summary>
    public class DragAdorner : Adorner, IDisposable
    {
        private AdornerLayer layer;
        private Brush visualBrush;
        private Point position;
        private Vector offset;
        private Size size;

        /// <summary>ドラッグゴースト</summary>
        /// <param name="ownerElement">オーナー なんでもいいが表示され続けるもの</param>
        /// <param name="targetElement">ゴーストにするElement</param>
        /// <param name="offset">targetElementをクリックした位置 targetElement基準</param>
        public DragAdorner(UIElement ownerElement, FrameworkElement targetElement, Vector offset) : base(ownerElement)
        {
            Focusable = false;
            IsHitTestVisible = false;

            layer = AdornerLayer.GetAdornerLayer(ownerElement);
            visualBrush = new VisualBrush(targetElement);
            this.offset = offset;
            size = new Size(targetElement.ActualWidth, targetElement.ActualHeight);

            layer.Add(this);
        }

        ///<summary>ownerElement基準でのマウス位置</summary>
        public void UpdatePosition(Point mousePos)
        {
            if(position == mousePos) return;
            position = mousePos;
            InvalidateVisual();
        }
        protected override void OnRender(DrawingContext dc)
            => dc.DrawRectangle(visualBrush, null, new Rect(position - offset, size));

        #region IDisposable
        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if(disposed) return;

            if(disposing)
                layer.Remove(this);

            disposed = true;
        }
        #endregion
    }
}
