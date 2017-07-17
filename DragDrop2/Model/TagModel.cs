using System;

namespace DragDrop2
{
    ///<summary>Tagテキスト保持</summary>
    public class TagModel : EditableModelBase<TagModel>, IDragable
    {
        ///<summary>自身の型</summary>
        public Type DataType => typeof(TagModel);
        ///<summary>Tagテキスト</summary>
        public string Text { get => _Text; set => SetProperty(ref _Text, value); }
        private string _Text;

        public TagModel()
        {
            Text = "New Item";
        }
        public TagModel(string text)
        {
            Text = text;
        }

        public override string ToString() => $"Text:{Text}";
    }
}
