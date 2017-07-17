using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DragDrop2
{
    ///<summary>曲 使用イメージサンプルデータ</summary>
    public class PlayItem : BindableBase
    {
        public string Name { get => _Name; set => SetProperty(ref _Name, value); }
        private string _Name;
        public bool IsSelected { get => _IsSelected; set => SetProperty(ref _IsSelected, value); }
        private bool _IsSelected;
        public ObservableCollection<string> Tags { get; } = new ObservableCollection<string>();
    }

    ///<summary>プレイリスト 使用イメージサンプルデータ</summary>
    public class PlayList : List<PlayItem> { }

    public class ViewModel : BindableBase
    {
        ///<summary>プレイリスト 使用イメージサンプルデータ</summary>
        public PlayList PlayList { get; }
        ///<summary>タブアイテムコレクション 使用イメージサンプルデータ</summary>
        public TabItemCollectionModel TabItemCollection { get; }
        ///<summary>タグ追加コマンド</summary>
        public DelegateCommand<string> AddTagCommand { get; }

        public ViewModel()
        {
            PlayList = new PlayList
            {
                new PlayItem{ Name = "01 a-ha - Take On Me" },
                new PlayItem{ Name = "02 Culture Club - Karma Chameleon" },
                new PlayItem{ Name = "03 Duran Duran - The Reflex" },
                new PlayItem{ Name = "04 Belinda Carlisle - Heaven Is A Place On Earth" },
                new PlayItem{ Name = "05 ABC - The Look Of Love (Part 1)" },
            };

            TabItemCollection = new TabItemCollectionModel
            {
                new TabItemModel("Genres", new TagCollectionModel
                {
                    new TagModel("Pop"),
                    new TagModel("Rock"),
                    new TagModel("Jazz"),
                    new TagModel("Classic"),
                }) { IsSelected = true } ,
                new TabItemModel("Favorites", new TagCollectionModel
                {
                    new TagModel("★"),
                    new TagModel("★★"),
                    new TagModel("★★★"),
                    new TagModel("★★★★"),
                    new TagModel("★★★★★"),
                }),
            };

            AddTagCommand = new DelegateCommand<string>((s) =>
            {
                var n = PlayList.Where(x => x.IsSelected).FirstOrDefault();
                if(n?.Tags?.Contains(s) == false)
                    n.Tags.Add(s);
            });
        }
    }
}
