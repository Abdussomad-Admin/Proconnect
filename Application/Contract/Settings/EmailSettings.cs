namespace Application.Contract.Settings
{
    public class EmailSettings
    {
        public string From { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string ApiKey { get; set; } = default!;
    }
}