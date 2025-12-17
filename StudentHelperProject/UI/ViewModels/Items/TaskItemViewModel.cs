using System;

namespace StudentHelper.WPF.UI.ViewModels.Items
{
    public class TaskItemViewModel : ViewModelBase
    {
        private int _taskId;
        private string _title = "";
        private string _description = "";
        private string _subject = "";
        private string _category = "";
        private DateTime _dueDate = DateTime.UtcNow;
        private bool _isCompleted;
        private int? _subjectId;

        public int TaskId
        {
            get => _taskId;
            set => SetProperty(ref _taskId, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Subject
        {
            get => _subject;
            set => SetProperty(ref _subject, value);
        }

        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        public DateTime DueDate
        {
            get => _dueDate;
            set => SetProperty(ref _dueDate, value);
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set => SetProperty(ref _isCompleted, value);
        }

        public int? SubjectId
        {
            get => _subjectId;
            set => SetProperty(ref _subjectId, value);
        }
    }
}
