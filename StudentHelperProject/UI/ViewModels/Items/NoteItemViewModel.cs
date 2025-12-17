using System;

namespace StudentHelper.WPF.UI.ViewModels.Items
{
    public class NoteItemViewModel : ViewModelBase
    {
        private int _noteId;
        private Guid _id = Guid.NewGuid();
        private string _title = "";
        private string _content = "";
        private DateTime _created = DateTime.UtcNow;
        private bool _isPinned;

        public int NoteId
        {
            get => _noteId;
            set => SetProperty(ref _noteId, value);
        }

        public Guid Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        public DateTime Created
        {
            get => _created;
            set => SetProperty(ref _created, value);
        }

        public bool IsPinned
        {
            get => _isPinned;
            set => SetProperty(ref _isPinned, value);
        }
    }
}
