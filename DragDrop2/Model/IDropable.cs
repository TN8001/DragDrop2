using System;

namespace DragDrop2
{
    ///<summary>ドロップを受け入れ可能なコレクション</summary>
    public interface IDropable
    {
        ///<summary>受け入れ可能な型</summary>
        Type DataType { get; }
        ///<summary>コレクション内の個数</summary>
        int Count { get; }

        ///<summary>インデックス位置に挿入</summary>
        void Insert(int index, IDragable data);
        ///<summary>最初に見つかったものを削除</summary>
        void Remove(IDragable data);
    }
}
