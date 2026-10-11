namespace ShipRate.Application.ViewModels
{
    public class CommitmentFileListItemVm
    {
        public Guid Id { get; set; }
        public string FileTitle { get; set; } = default!;
        public string FileName { get; set; } = default!;
        public DateTime? UploadDate { get; set; }
        public string? Description { get; set; }
        public string LatinName { get; set; }
        public string LatinDscp { get; set; }

        public CommitmentFileListItemVm(Guid id, string fileTitle, string fileName, DateTime? uploadDate, string? description, string latinName, string latinDscp) : this(id, fileTitle, fileName, uploadDate, description)
        {
            LatinName = latinName;
            LatinDscp = latinDscp;
        }

        public CommitmentFileListItemVm(Guid id, string fileTitle, string fileName, DateTime? uploadDate, string? description)
        {
            Id = id;
            FileTitle = fileTitle;
            FileName = fileName;
            UploadDate = uploadDate;
            Description = description;
        }
    }






}
