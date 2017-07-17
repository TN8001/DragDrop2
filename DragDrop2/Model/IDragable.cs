using System;

namespace DragDrop2
{
    ///<summary>ドラッグ移動可能なアイテム</summary>
    public interface IDragable
    {
        ///<summary>自身の型</summary>
        Type DataType { get; }
    }
}
