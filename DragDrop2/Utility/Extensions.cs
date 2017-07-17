using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DragDrop2
{
    internal static class ItemsControlExtensions
    {
        ///<summary>拡張メソッド 指定インデックスのItemContainerを取得します</summary>
        public static FrameworkElement GetContainerFromIndex(this ItemsControl @this, int index)
            => 0 <= index && index < @this.Items.Count
            ? @this.ItemContainerGenerator.ContainerFromIndex(index) as FrameworkElement
            : null;

        ///<summary>拡張メソッド 指定アイテムのItemContainerを取得します</summary>
        public static FrameworkElement GetContainerFromItem(this ItemsControl @this, object item)
            => @this.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;

        ///<summary>拡張メソッド IEditableCollectionViewを取得します</summary>
        public static IEditableCollectionView GetEditableCollectionView(this ItemsControl @this)
            => CollectionViewSource.GetDefaultView(@this.ItemsSource) as IEditableCollectionView;

        ///<summary>拡張メソッド 指定コンテナのインデックスを取得します</summary>
        public static int GetIndexFromContainer(this ItemsControl @this, DependencyObject obj)
            => @this.ItemContainerGenerator.IndexFromContainer(obj);
    }

    internal static class DependencyObjectExtensions
    {
        ///<summary>拡張メソッド 特定の型の最初に見つかった祖先要素を取得します</summary>
        public static T FindAncestor<T>(this DependencyObject @this) where T : DependencyObject
        {
            do
            {
                @this = VisualTreeHelper.GetParent(@this);

            } while(@this != null && !(@this is T));

            return @this as T;
        }

        ///<summary>拡張メソッド 特定の型もしくは派生型の最初に見つかった祖先要素を取得します</summary>
        public static T FindAncestorAssignableFrom<T>(this DependencyObject @this) where T : DependencyObject
        {
            do
            {
                @this = VisualTreeHelper.GetParent(@this);
            }
            while(@this != null && !typeof(T).IsAssignableFrom(@this.GetType()));

            return @this as T;
        }

        ///<summary>拡張メソッド 子要素を取得します</summary>
        public static IEnumerable<DependencyObject> GetChildren(this DependencyObject @this)
        {
            var count = VisualTreeHelper.GetChildrenCount(@this);
            if(count == 0)
                yield break;

            for(var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(@this, i);
                if(child != null)
                    yield return child;
            }
        }

        ///<summary>拡張メソッド 子孫要素を取得します</summary>
        public static IEnumerable<DependencyObject> GetDescendants(this DependencyObject @this)
        {
            foreach(var child in @this.GetChildren())
            {
                yield return child;
                foreach(var grandChild in child.GetDescendants())
                    yield return grandChild;
            }
        }

        ///<summary>拡張メソッド 特定の型の子要素を取得します</summary>
        public static IEnumerable<T> GetChildren<T>(this DependencyObject @this) where T : DependencyObject
            => @this.GetChildren().OfType<T>();

        ///<summary>拡張メソッド 特定の型の子孫要素を取得します</summary>
        public static IEnumerable<T> GetDescendants<T>(this DependencyObject @this) where T : DependencyObject
            => @this.GetDescendants().OfType<T>();
    }
}
