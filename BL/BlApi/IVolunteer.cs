using BO;

namespace BLApi;

/// <summary>
/// Interface for managing volunteer-related operations in the business logic layer.
/// </summary>
public interface IVolunteer:BlApi.IObservable //stage 5 הרחבת ממשק
{
    /// <summary>
    /// Authenticates a user and returns their role.
    /// </summary>
    /// <param name="fullName">The fullName of the volunteer.</param>
    /// <param name="password">The password of the volunteer.</param>
    /// <returns>The role of the authenticated volunteer.</returns>
    public BO.Enums.Role Login(string fullName, string password);


    /// <summary>
    /// Retrieves a list of volunteers based on filter and sort criteria.
    /// </summary>
    /// <param name="isActive">Optional parameter to filter by active or inactive volunteers.</param>
    /// <param name="sortField">Optional parameter to specify the field by which to sort the list.</param>
    /// <returns>A collection of volunteers matching the criteria.</returns>
    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive, BO.Enums.VolunteerSortField? sortField = null);

    /// <summary>
    /// Retrieves detailed information about a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The unique identifier of the volunteer.</param>
    /// <returns>The volunteer details.</returns>
    public BO.Volunteer Read(int volunteerId);

    /// <summary>
    /// Updates the details of a volunteer. The requester must have the appropriate permissions.
    /// </summary>
    /// <param name="requesterId">The ID of the person requesting the update (admin or the same volunteer).</param>
    /// <param name="volunteer">The volunteer object containing updated details.</param>
    public void UpdateVolunteerDetails(int requesterId, BO.Volunteer volunteer);

    /// <summary>
    /// Deletes a volunteer from the system. The volunteer must not be currently handling any calls.
    /// </summary>
    /// <param name="volunteerId">The unique identifier of the volunteer to delete.</param>
    public void DeleteVolunteer(int volunteerId);


    /// <summary>
    /// Adds a new volunteer to the system.
    /// </summary>
    /// <param name="volunteer">The volunteer object containing the details of the new volunteer.</param>
    public void AddVolunteer(BO.Volunteer volunteer);

}