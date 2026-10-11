namespace ShipRate.Application.Interfaces
{
    public interface ISensitiveDataProtector
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
        string Hash(string plainText);
    }
}