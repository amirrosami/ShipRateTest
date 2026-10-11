namespace ShipRate.WebApp.ViewModels
{
    public enum PersonPictureType
    {
        Latin = 1,
        Local = 2
    }

    public class PersonPictureViewModel
    {
        public Guid PersonId { get; set; }
        public bool HasPicture { get; set; }
        public PersonPictureType PictureType { get; set; }
    }
}
