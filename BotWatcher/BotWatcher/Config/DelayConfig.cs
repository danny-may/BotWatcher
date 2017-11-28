namespace BotWatcher.Config
{
    public class DelayConfig
    {
        public string Message { get; set; }
        public int Interval { get; set; }
        public int MaxDelay { get; set; }
        public ActionResponse OnSlow { get; set; }
    }
}