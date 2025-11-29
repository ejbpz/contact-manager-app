using ContactsManager.Models;
using System.Linq.Expressions;

namespace ContactsManager.RepositoryContracts
{
    /// <summary>
    /// Represents the data access logic to manage Country model.
    /// </summary>
    public interface IPeopleRepository
    {
        /// <summary>
        /// Add a person object into data store.
        /// </summary>
        /// <param name="person">Person's object to be added.</param>
        /// <returns>Returns the person object added.</returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// Returns all the people in the data store.
        /// </summary>
        /// <returns>All the people in the table.</returns>
        Task<List<Person>> GetPeople();

        /// <summary>
        /// Retrieves the person based on the person ID.
        /// </summary>
        /// <param name="personId">GUID referes to the person ID.</param>
        /// <returns>Returns a person or null.</returns>
        Task<Person?> GetPersonByPersonId(Guid personId);

        /// <summary>
        /// Retrieves all the people that match with the expression.
        /// </summary>
        /// <param name="predicate">LINQ expression to check.</param>
        /// <returns>Returns a list of people based on the expression or null.</returns>
        Task<List<Person>?> GetFilteredPeople(Expression<Func<Person, bool>> predicate);

        /// <summary>
        /// Deletes a person object based on its ID.
        /// </summary>
        /// <param name="personId">GUID referes to the person ID.</param>
        /// <returns>Returns true/false if it was deleted or not.</returns>
        Task<bool> DeletePersonByPersonId(Guid personId);

        /// <summary>
        /// Updates a person object into data store.
        /// </summary>
        /// <param name="person">Person's object to be updated.</param>
        /// <returns>Returns the person object updated.</returns>
        Task<Person> UpdatePerson(Person personToUpdate, Person newPerson);
    }
}