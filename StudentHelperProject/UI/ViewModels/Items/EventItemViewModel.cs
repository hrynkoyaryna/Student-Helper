using System;
using System.Windows.Media;

namespace StudentHelper.WPF.UI.ViewModels.Items
{
    public class EventItemViewModel : ViewModelBase
    {
        private int _eventId;
        private string _title = "";
        private string _location = "";
        private string _description = "";
        private string _type = "Нетипізовано";
        private DateTime _startDate = DateTime.UtcNow;
        private DateTime _endDate = DateTime.UtcNow.AddHours(1);
        private string _recurrence = "Ніколи";
        private Color _eventColor = Colors.LightBlue;

        public int EventId
        {
            get => _eventId;
            set => SetProperty(ref _eventId, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Type
        {
            get => _type;
            set
            {
                if (SetProperty(ref _type, value))
                {
                    // Auto-update color when type changes
                    if (_eventColor == GetDefaultColorForType(_type))
                    {
                        EventColor = GetDefaultColorForType(value);
                    }
                }
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        public string Recurrence
        {
            get => _recurrence;
            set => SetProperty(ref _recurrence, value);
        }

        public Color EventColor
        {
            get => _eventColor;
            set => SetProperty(ref _eventColor, value);
        }

        public static Color GetDefaultColorForType(string eventType)
        {
            return eventType switch
            {
                "Нетипізовано" => Colors.LightBlue,
                "Вчасно" => Colors.LightGreen,
                "Затримано" => Colors.LightCoral,
                _ => Colors.LightGray
            };
        }
    }
}
