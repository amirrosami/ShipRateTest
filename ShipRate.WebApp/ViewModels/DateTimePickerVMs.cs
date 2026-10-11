namespace ShipRate.WebApp.ViewModels
{
    public class DateTimePickerViewModel
    {
        public string Id { get; set; } = Guid.CreateVersion7().ToString("N");
        public string Name { get; set; }
        public string Label { get; set; }
        public DateTime? Value { get; set; }
        public DateTime? ValueEnd { get; set; } // For range mode
        public bool IsRequired { get; set; }
        public bool ShowTime { get; set; } = true;
        public bool IsRange { get; set; }
        public string CalendarType { get; set; } = "jalali"; // jalali or gregorian
        public string Placeholder { get; set; }
        public string CssClass { get; set; }
        public bool ShowQuickSelect { get; set; } = true;
        public string Format { get; set; } // e.g., "yyyy/MM/dd HH:mm"
    }
    // ViewModels/DateTimePickerProViewModel.cs
    public class DateTimePickerProViewModel
    {
        public string Id { get; set; } = "dtppro_" + Guid.CreateVersion7().ToString("N")[..8];
        public string Name { get; set; }
        public string Label { get; set; }
        public DateTime? Value { get; set; }
        public DateTime? ValueEnd { get; set; }
        public bool IsRequired { get; set; }
        public bool ShowTime { get; set; } = true;
        public bool IsRange { get; set; }
        public string CalendarType { get; set; } = "jalali"; // jalali | gregorian
        public string Placeholder { get; set; }
        public string PlaceholderEnd { get; set; }
        public string CssClass { get; set; }
        public bool ShowQuickSelect { get; set; } = true;
        public bool ShowClearButton { get; set; } = true;
        public bool ShowTodayButton { get; set; } = true;
        public string HelpText { get; set; }
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
        public string ValidationMessage { get; set; }
        public bool EnableKeyboardNav { get; set; } = true;
        public bool EnableDragSelect { get; set; } = true;
        public string Format { get; set; } = "yyyy/MM/dd HH:mm";
        public string Theme { get; set; } = "auto"; // auto | light | dark
        public bool AutoClose { get; set; } = true;
        public string Position { get; set; } = "auto"; // auto | top | bottom
    }

}
