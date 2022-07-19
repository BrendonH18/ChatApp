namespace server.Models
{
    public class ChangeChannelModel
    {
        public virtual User User { get; set; }
        public virtual Channel enterChannel { get; set; }
        public virtual Channel exitChannel { get; set; }
    }
}
