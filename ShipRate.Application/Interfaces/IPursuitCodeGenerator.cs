namespace ShipRate.Application.Interfaces
{
    public interface IPursuitCodeGenerator
    {
        string Generate(
                string blNumber,
                string hologramPlainSerial,
                Guid CarrierId);
    }
}
