using ShipRate.Application.DTOs.Persons;
using ShipRate.Domain.Entities.Customers;
using ShipRate.Infrastructure.Repositories;

namespace ShipRate.Application.Services.Persons
{
    public class CustomerService
    {
        private readonly Repository<Customer> _repository;
        private readonly PersonService personService;
        public CustomerService(Repository<Customer> repository, PersonService personService)
        {
            _repository = repository;
            this.personService = personService;
        }


        public async Task<List<CustomerDto>> getAllAsync()
        {
            var customers = await _repository.GetAllAsync();
            var dtos = customers.Select(c => new CustomerDto
            {
                PersonDto = new PersonDto(c.Id, c.Person.PersonalityTypeId, c.Person.PersonName, c.Person.PersonPersianName,
                c.Person.FirstName, c.Person.LastName, c.Person.BirthDate, c.Person.CountryId, c.Person.EmailAddress,
                c.Person.Phone, c.Person.Address, c.Person.NationalID, c.Person.Mobile, c.Person.FaxNo, c.Person.ZipCode, c.Person.EcoCode,
                c.Person.RegNo, c.Person.PersonRegDate)

            });
            return dtos.ToList();
        }

        public async Task AddCustomer(CustomerDto customerDto)
        {
            customerDto.PersonDto.Id = Guid.CreateVersion7();
            customerDto.Id = Guid.CreateVersion7();
            try
            {
                await _repository.BeginTransactionAsync();
                await personService.AddPersonAsync(customerDto.PersonDto);
                var customer = new Customer { Id = customerDto.PersonDto.Id };
                await _repository.AddAsync(customer);
                await _repository.CommitTransactionAsync();
            }
            catch (Exception e)
            {
                await _repository.RollBackTransactionAsync();
                throw e;
            }
        }

        public async Task UpdateCustomer(CustomerDto customerDto)
        {
            var customer = new Customer { };
            try
            {
                await _repository.BeginTransactionAsync();
                await personService.EditPersonAsync(customerDto.PersonDto);
                await _repository.UpdateAsync(customer);
                await _repository.CommitTransactionAsync();
            }
            catch (Exception)
            {
                await _repository.RollBackTransactionAsync();
                throw;
            }

        }
        public async Task DeleteCustomer(Guid customerId)
        {
            var customer = await _repository.GetByGuidAsync(customerId);
            if (customer == null)
            {
                throw new Exception("Customer does not exist.");
            }

            await _repository.DeleteAsync(customer);
        }
    }
}
