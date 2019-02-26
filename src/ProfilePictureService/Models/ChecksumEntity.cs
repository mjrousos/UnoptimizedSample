using Microsoft.WindowsAzure.Storage.Table;

namespace ProfilePictureService.Models
{
    public class ChecksumEntity: TableEntity
    {
        public ChecksumEntity()
        {
            // There won't be enough data in the table to need multiple partitions,
            // so leave all the data in a default partition to make queries faster
            PartitionKey = string.Empty;
        }

        public ChecksumEntity(string name, string data) :this()
        {
            Filename = name;
            Data = data;
        }

        public string Filename
        {
            get => RowKey;
            set => RowKey = value;
        }
        public string Data { get; set; }
    }
}
