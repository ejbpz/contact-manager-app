namespace ContactsManager.ServiceContracts
{
    /// <summary>
    /// Business Logic for Person model manipulation.
    /// </summary>
    public interface IPeopleDeleterService
    {
        /// <summary>
        /// Deletes a person using its id.
        /// </summary>
        /// <param name="personId">Id to the person to be deleted.</param>
        /// <returns>Returns bool to know if was deleted.</returns>
        Task<bool> DeletePerson(Guid? personId);
    }
}
