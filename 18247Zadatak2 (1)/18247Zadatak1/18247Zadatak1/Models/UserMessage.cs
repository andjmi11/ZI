namespace _18247Zadatak1.Models
{
    public class UserMessage
    {
        public string Username { get; set; }
        public string Message { get; set; }
        public string EncryptedMessage { get; set; }
        public bool CurrentUser { get; set; }
        public DateTime DateSent { get; set; }

        public string FilePath { get; set; }
    }
}
