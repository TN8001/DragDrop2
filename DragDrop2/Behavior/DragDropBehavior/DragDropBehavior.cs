using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace DragDrop2
{
    interface IBehavior
    {
        void OnAttached();
        void OnDetaching();
    }
    ///<summary>ドラッグ＆ドロップでアイテムを入れ替えるBehavior</summary>
    public class DragDropBehavior : Behavior<FrameworkElement>
    {
        private IList<IBehavior> behaviorList = new List<IBehavior>();

        protected override void OnAttached()
        {
            if(AssociatedObject is ItemsControl itemsControl)
                behaviorList.Add(new DropItemsControlBehavior(itemsControl));
            else
            {
                behaviorList.Add(new DragBehavior(AssociatedObject));
                behaviorList.Add(new DropBehavior(AssociatedObject));
            }

            foreach(var behavior in behaviorList)
                behavior.OnAttached();
        }
        protected override void OnDetaching()
        {
            foreach(var behavior in behaviorList)
                behavior.OnDetaching();
        }
    }
}