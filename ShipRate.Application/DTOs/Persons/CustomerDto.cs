namespace ShipRate.Application.DTOs.Persons
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public Guid PersonId { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public PersonDto PersonDto { get; set; }
    }
}
