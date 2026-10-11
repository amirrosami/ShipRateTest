using ShipRate.Application.Interfaces;

namespace ShipRate.Application.Services
{
    public class PursuitCodeGenerator : IPursuitCodeGenerator
    {
        public string Generate(string blNumber, string hologramPlainSerial, Guid CarrierId)
        {
            var raw = $"{blNumber}|{hologramPlainSerial}|{CarrierId}|SECRET";
            //var hash = SHA256(raw);
            //return Base32(hash).Substring(0, 10); // Short & printable
            return "";
        }
    }
}



