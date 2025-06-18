using Gymora.CustomControls;

namespace Gymora.Models
{
    public class CalendarModel : PropertyChangedModel
    {
        private DateTime _date;
        public CalendarView Parent { get; set; }
        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        private bool _isCurrentDate;
        public bool IsCurrentDate
        {
            get => _isCurrentDate;
            set => SetProperty(ref _isCurrentDate, value);
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged();
                    if (Parent is CalendarView calendar)
                        calendar.SaveCheckedDates();
                }
            }
        }


    }
}
