using System;

namespace DragDrop2
{
    ///<summary>タブの保持</summary>
    public class TabItemModel : EditableModelBase<TabItemModel>, IDragable
    {
        ///<summary>タブヘッダ</summary>
        public string Header { get => _Header; set => SetProperty(ref _Header, value); }
        private string _Header;
        ///<summary>選択状態</summary>
        public bool IsSelected { get => _IsSelected; set => SetProperty(ref _IsSelected, value); }
        private bool _IsSelected;
        ///<summary>コンテンツ タグのコレクション</summary>
        public TagCollectionModel Content { get; }
        ///<summary>自身の型</summary>
        public Type DataType => typeof(TabItemModel);

        public TabItemModel()
        {
            Header = "New Tab";
            Content = new TagCollectionModel();
        }
        public TabItemModel(string header, TagCollectionModel content)
        {
            Header = header;
            Content = content;
        }
    }
}
