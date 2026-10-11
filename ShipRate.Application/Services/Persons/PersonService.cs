using ShipRate.Application.DTOs.Persons;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Infrastructure.Repositories;

namespace ShipRate.Application.Services.Persons
{

    public class PersonService

    {
        private readonly Repository<Person> repository;
        public PersonService(Repository<Person> repository)
        {
            this.repository = repository;
        }

        public Person GetPerson(PersonDto persondto)
        {
            //var person = new Person(persondto.Id,persondto.PersonalityTypeId, persondto.PersonName, persondto.PersonPersianName,
            //    persondto.FirstName, persondto.LastName, persondto.BirthDate, persondto.CountryId, persondto.EmailAddress,
            //    persondto.Phone, persondto.Address, persondto.NationalID, persondto.Mobile, persondto.FaxNo, persondto.ZipCode,
            //    persondto.EcoCode, persondto.RegNo, persondto.PersonRegDate);

            return null;
        }

        public async Task<Guid> AddPersonAsync(PersonDto personDto)
        {
            var person = GetPerson(personDto);
            await repository.AddAsync(person);
            return person.Id;
        }

        public async Task EditPersonAsync(PersonDto personDto)
        {
            var person = GetPerson(personDto);
            await repository.UpdateAsync(person);
        }

        public async Task DeletePersonAsync(PersonDto personDto)
        {
            var person = await repository.GetByGuidAsync(personDto.Id);
            if (person == null)
            {
                throw new Exception();
            }
            await repository.DeleteAsync(person);
        }



    }
}
