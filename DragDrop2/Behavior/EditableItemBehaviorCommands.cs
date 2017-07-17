using System.Windows.Input;

namespace DragDrop2
{
    public static class EditableItemBehaviorCommands
    {
        public static readonly ICommand Delete = new RoutedUICommand("削除", "Delete", typeof(EditableItemBehavior));
        public static readonly ICommand Add = new RoutedUICommand("追加", "Add", typeof(EditableItemBehavior));
        public static readonly ICommand Edit = new RoutedUICommand("編集", "Edit", typeof(EditableItemBehavior));
        public static readonly ICommand Cancel = new RoutedUICommand("キャンセル", "Cancel", typeof(EditableItemBehavior));
        public static readonly ICommand Commit = new RoutedUICommand("確定", "Commit", typeof(EditableItemBehavior));
    }
}
