namespace FILM_Sparepart_MVC.Models
{
    public enum ConnectStatus
    {
        AlreadyConnected,
        Connected,
        Failed
    }

    public class ConnectResultModel
    {
        public string IpAddress { get; set; }
        public ConnectStatus Status { get; set; }
        public string Message { get; set; }
    }
}
