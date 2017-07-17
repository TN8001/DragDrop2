using System;
using System.Collections.ObjectModel;

namespace DragDrop2
{
    public class TabItemCollectionModel : ObservableCollection<TabItemModel>, IDropable
    {     
        ///<summary>受け入れ可能な型</summary>
        public Type DataType => typeof(TabItemModel);

        ///<summary>インデックス位置に挿入</summary>
        public void Insert(int index, IDragable data) => base.Insert(index, (TabItemModel)data);
        ///<summary>最初に見つかったものを削除</summary>
        public void Remove(IDragable data) => base.Remove((TabItemModel)data);
    }
}
