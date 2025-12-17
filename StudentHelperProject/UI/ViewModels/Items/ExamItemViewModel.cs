using System;

namespace StudentHelper.WPF.UI.ViewModels.Items
{
    public class ExamItemViewModel : ViewModelBase
    {
        private int _examId;
        private string _title = "";
        private string? _description;
        private string _subject = "";
        private DateTime _date = DateTime.UtcNow;
        private bool _isPassed;
        private int? _subjectId;

        public int ExamId
        {
            get => _examId;
            set => SetProperty(ref _examId, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string? Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Subject
        {
            get => _subject;
            set => SetProperty(ref _subject, value);
        }

        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public bool IsPassed
        {
            get => _isPassed;
            set => SetProperty(ref _isPassed, value);
        }

        public int? SubjectId
        {
            get => _subjectId;
            set => SetProperty(ref _subjectId, value);
        }
    }
}
