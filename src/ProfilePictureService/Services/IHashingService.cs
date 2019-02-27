namespace ProfilePictureService.Services
{
    public interface IHashingService
    {
        string CalculateChecksum(byte[] data);
    }
}
