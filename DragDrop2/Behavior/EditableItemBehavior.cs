using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml;

namespace DragDrop2
{
    ///<summary>IEditableObjectを実装するクラスを編集するBehavior</summary>
    public class EditableItemBehavior : Behavior<ItemsControl>
    {
        private class AddItemTemplateSelector : DataTemplateSelector
        {
            public DataTemplate ItemTemplate { get; set; }
            public DataTemplate AddItemTemplate { get; set; }

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
                => item != CollectionView.NewItemPlaceholder
                ? ItemTemplate
                : AddItemTemplate;
        }

        ///<summary>通常状態のテンプレート</summary>
        public DataTemplate ItemTemplate { get => templateSelector.ItemTemplate; set => templateSelector.ItemTemplate = value; }
        ///<summary>追加ボタンのテンプレート</summary>
        public DataTemplate AddItemTemplate { get => templateSelector.AddItemTemplate; set => templateSelector.AddItemTemplate = value; }
        ///<summary>編集状態のテンプレート</summary>
        public DataTemplate EditItemTemplate { get; set; }

        private CommandBinding deleteCB;
        private CommandBinding addCB;
        private CommandBinding editCB;
        private CommandBinding cancelCB;
        private CommandBinding commitCB;

        private KeyBinding cancelKB;
        private KeyBinding commitKB;

        private PropertyChangeNotifier itemsSourceChangedNotifier;
        private object currentItem;

        private ItemsControl @ItemsControl => AssociatedObject;
        private AddItemTemplateSelector templateSelector { get; set; } = new AddItemTemplateSelector();

        public EditableItemBehavior()
        {
            templateSelector.AddItemTemplate = GetDefaultAddButtonTemplate();
        }

        protected override void OnAttached()
        {
            @ItemsControl.ItemTemplateSelector = templateSelector;
            @ItemsControl.Loaded += ItemsControl_Loaded;

            itemsSourceChangedNotifier = new PropertyChangeNotifier(@ItemsControl, ItemsControl.ItemsSourceProperty);
            itemsSourceChangedNotifier.ValueChanged += Notifier_ValueChanged;
        }
        protected override void OnDetaching()
        {
            @ItemsControl.Loaded -= ItemsControl_Loaded;
            itemsSourceChangedNotifier.ValueChanged -= Notifier_ValueChanged;
            itemsSourceChangedNotifier.Dispose();

            RemoveCommandBinding();
        }

        private DataTemplate GetDefaultAddButtonTemplate()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var stringReader = new StringReader(
                @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
                                xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
                                xmlns:l=""clr-namespace:" + assemblyName + ";assembly=" + assemblyName + @""" >
                    <Button Width=""{Binding ActualHeight, Mode=OneWay, RelativeSource={RelativeSource Self}}""
                            Command=""{x:Static l:EditableItemBehaviorCommands.Add}""
                            CommandParameter=""{Binding}""
                            Content=""+"" />
                  </DataTemplate>");

            return XamlReader.Load(XmlReader.Create(stringReader)) as DataTemplate;
        }

        private void ItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            AddNewItemPlaceholder();

            CreateCommandBinding();
            AddCommandBinding();
        }
        private void Notifier_ValueChanged(object sender, EventArgs e) => AddNewItemPlaceholder();
        private void AddNewItemPlaceholder()
        {
            var view = CollectionViewSource.GetDefaultView(@ItemsControl.ItemsSource) as IEditableCollectionView;
            if(view != null)
                view.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;
        }

        private void CreateCommandBinding()
        {
            deleteCB = new CommandBinding(EditableItemBehaviorCommands.Delete, DeleteCommand_Executed);
            addCB = new CommandBinding(EditableItemBehaviorCommands.Add, AddCommand_Executed);
            editCB = new CommandBinding(EditableItemBehaviorCommands.Edit, EditCommand_Executed);
            cancelCB = new CommandBinding(EditableItemBehaviorCommands.Cancel, CancelCommand_Executed);
            commitCB = new CommandBinding(EditableItemBehaviorCommands.Commit, CommitCommand_Executed);

            cancelKB = new KeyBinding(EditableItemBehaviorCommands.Cancel, new KeyGesture(Key.Escape));
            commitKB = new KeyBinding(EditableItemBehaviorCommands.Commit, new KeyGesture(Key.Enter));
        }
        private void AddCommandBinding()
        {
            @ItemsControl.CommandBindings.Add(deleteCB);
            @ItemsControl.CommandBindings.Add(addCB);
            @ItemsControl.CommandBindings.Add(editCB);
            @ItemsControl.CommandBindings.Add(cancelCB);
            @ItemsControl.CommandBindings.Add(commitCB);

            @ItemsControl.InputBindings.Add(cancelKB);
            @ItemsControl.InputBindings.Add(commitKB);
        }
        private void RemoveCommandBinding()
        {
            @ItemsControl.CommandBindings.Remove(deleteCB);
            @ItemsControl.CommandBindings.Remove(addCB);
            @ItemsControl.CommandBindings.Remove(editCB);
            @ItemsControl.CommandBindings.Remove(cancelCB);
            @ItemsControl.CommandBindings.Remove(commitCB);

            @ItemsControl.InputBindings.Remove(cancelKB);
            @ItemsControl.InputBindings.Remove(commitKB);
        }
        private void EditCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            currentItem = e.Parameter;
            if(currentItem == null) return;

            SetTemplate(currentItem, EditItemTemplate);
            if(currentItem is IEditableObject eo)
                eo.BeginEdit();

            //EditItemTemplateにフォーカスを当てる
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var container = @ItemsControl.GetContainerFromItem(currentItem);
                container?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }), DispatcherPriority.Loaded);
        }

        private void CommitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            SetTemplate(currentItem, ItemTemplate);
            if(currentItem is IEditableObject eo)
                eo.EndEdit();

            currentItem = null;
        }

        private void CancelCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            SetTemplate(currentItem, ItemTemplate);
            if(currentItem is IEditableObject eo)
                eo.CancelEdit();

            currentItem = null;
        }
        private void AddCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            var view = @ItemsControl.GetEditableCollectionView();
            if(view.CanAddNew)
            {
                view.AddNew();
                view.CommitNew();
            }
            else Debug.WriteLine("Addするには引数なしコンストラクタが必要");
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            ((IList)@ItemsControl.ItemsSource)?.Remove(e.Parameter);
        }

        private void SetTemplate(object obj, DataTemplate template)
        {
            var container = @ItemsControl.GetContainerFromItem(obj);
            if(container is ContentPresenter cp)
                cp.ContentTemplate = template;

            if(container is ListBoxItem lbi)
                lbi.ContentTemplate = template;
        }
    }
}
