using System;
using System.ComponentModel;

namespace DragDrop2
{
    public abstract class EditableModelBase<T> : BindableBase, IEditableObject
    {
        private T Cache;

        public void BeginEdit()
        {
            Cache = Activator.CreateInstance<T>();

            foreach(var info in GetType().GetProperties())
            {
                if(!info.CanRead || !info.CanWrite) continue;
                var oldValue = info.GetValue(this, null);
                Cache.GetType().GetProperty(info.Name).SetValue(Cache, oldValue, null);
            }
        }
        public void EndEdit() => Cache = default(T);
        public void CancelEdit()
        {
            foreach(var info in GetType().GetProperties())
            {
                if(!info.CanRead || !info.CanWrite) continue;
                var oldValue = info.GetValue(Cache, null);
                GetType().GetProperty(info.Name).SetValue(this, oldValue, null);
            }
        }
    }
}
