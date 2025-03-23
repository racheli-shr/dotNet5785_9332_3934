using BL.Helpers;
using BLApi;
using Helpers;
namespace BLImplementation;
internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Adds a new volunteer to the system.
    /// </summary>
    public void AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            if (volunteer == null)
                throw new BO.Exceptions.BLInvalidDataException("Cannot add null object");

            var (latitude, longitude) = Tools.GetCoordinates(volunteer.FullAddress);
            volunteer.Latitude = latitude;
            volunteer.Longitude = longitude;
            VolunteerManager.Validation(volunteer);

            bool isDirectorExists = _dal.Volunteer.ReadAll().Any(v => v.Role == DO.Enums.Role.manager);
            if (isDirectorExists&&volunteer.Role==BO.Enums.Role.manager)
            {
                throw new BO.Exceptions.BlDoesNotExistException("Cannot add a new Director. Only one Director is allowed in the system.");
            }

            DO.Volunteer newVolunteer = VolunteerManager.ConvertBOToDO(volunteer);
            _dal.Volunteer.Create(newVolunteer);
        }
        catch (Exception ex)
        {
            throw new BO.Exceptions.BLGeneralException($"An error occurred while adding new Volunteer: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes a volunteer by ID if allowed.
    /// </summary>
    public void DeleteVolunteer(int volunteerId)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(v => v.Id == volunteerId);
            if (volunteer == null)
            {
                throw new BO.Exceptions.BlDoesNotExistException("The volunteer with the provided ID does not exist.");
            }

            var assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);
            bool isCurrentlyHandlingCall = assignments.Any(a => a.AssignmentStatus == DO.Enums.AssignmentStatus.TREATED);

            if (isCurrentlyHandlingCall)
            {
                throw new BO.Exceptions.BlDeletionImpossible("The volunteer is currently handling a call and cannot be deleted.");
            }

            bool hasHandledCallsBefore = assignments.Any(a =>
                a.AssignmentStatus != DO.Enums.AssignmentStatus.SELF_CANCELLED &&
                a.AssignmentStatus != DO.Enums.AssignmentStatus.EXPIRED &&
                a.AssignmentStatus != DO.Enums.AssignmentStatus.MANAGER_CANCELLED);

            if (hasHandledCallsBefore)
            {
                throw new BO.Exceptions.BlDeletionImpossible("The volunteer has handled calls in the past and cannot be deleted.");
            }

            _dal.Volunteer.Delete(volunteerId);
        }
        catch (Exception ex)
        {
            throw new BO.Exceptions.BLGeneralException("An error occurred while trying to delete the volunteer.", ex);
        }
    }

    /// <summary>
    /// Retrieves a volunteer's details by ID.
    /// </summary>
    public BO.Volunteer Read(int volunteerId)
    {
        try
        {
            DO.Volunteer doVolunteer = _dal.Volunteer.Read(v => v.Id == volunteerId)
                ?? throw new BO.Exceptions.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist");

            return new BO.Volunteer
            {
                Id = volunteerId,
                FullName = doVolunteer.FullName,
                Phone = doVolunteer.Phone,
                Email = doVolunteer.Email,
                FullAddress = doVolunteer.FullAdress,
                Latitude = doVolunteer.Latitude,
                Longitude = doVolunteer.Longitude,
                Role = (BO.Enums.Role)doVolunteer.Role,
                IsActive = doVolunteer.IsActive,
                DistanceType = (BO.Enums.DistanceType)doVolunteer.DistanceType
            };
        }
        catch (Exception ex)
        {
            throw new BO.Exceptions.BlDoesNotExistException($"An error occurred while processing Volunteer ID={volunteerId}: {ex}");
        }
    }

    /// <summary>
    /// Retrieves a list of volunteers with optional filters and sorting.
    /// </summary>
    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive, BO.Enums.VolunteerSortField? sortField = null)
    {
        try
        {
            IEnumerable<DO.Volunteer> DOvolunteers;
            if (isActive == null)
            {
                // Retrieve all volunteers if isActive is null (no filtering by availability)
                DOvolunteers = _dal.Volunteer.ReadAll()
                    ?? throw new BO.Exceptions.BlDoesNotExistException("No volunteers exist in the system.");
            }
            else
            {
                // Retrieve volunteers based on their availability status
                DOvolunteers = _dal.Volunteer.ReadAll(v => v.IsActive == isActive)
                    ?? throw new BO.Exceptions.BlDoesNotExistException("No volunteers match the specified availability.");
            }
            var volunteersInList = from v in DOvolunteers
                                   let BOvolunteer = Read(v.Id) // Retrieve volunteer details by ID
                                   select new BO.VolunteerInList
                                   {
                                       Id = BOvolunteer.Id,
                                       FullName = BOvolunteer.FullName,
                                       IsActive = BOvolunteer.IsActive,
                                       SumTreatedCalls = BOvolunteer.NumberOfCalls,
                                        SumCallsSelfCancelled= BOvolunteer.NumberOfCanceledCalls,
                                       SumExpiredCalls = BOvolunteer.NumberOfexpiredCalls,
                                       CallId = BOvolunteer.CurrentCallInProgress?.CallId,
                                       CallType = BOvolunteer.CurrentCallInProgress?.CallType ?? BO.Enums.CallType.NONE
                                   };
            // Apply sorting based on the selected field
            if (sortField == null)
            {
                // Default sorting by volunteer ID
                volunteersInList = volunteersInList.OrderBy(v => v.Id);
            }
            else
            {
                // Apply sorting using query syntax
                volunteersInList = sortField switch
                {
                    BO.Enums.VolunteerSortField.ID => from v in volunteersInList orderby v.Id select v,
                    BO.Enums.VolunteerSortField.FULL_NAME => from v in volunteersInList orderby v.FullName select v,
                    BO.Enums.VolunteerSortField.IS_AVAILABLE => from v in volunteersInList orderby v.IsActive select v,
                    BO.Enums.VolunteerSortField.SUM_TREATED_CALLS => from v in volunteersInList orderby v.SumTreatedCalls select v,
                    BO.Enums.VolunteerSortField.SUM_CALLS_SELF_CANCELLED => from v in volunteersInList orderby v.SumCallsSelfCancelled select v,
                    BO.Enums.VolunteerSortField.SUM_EXPIRED_CALLS => from v in volunteersInList orderby v.SumExpiredCalls select v,
                    BO.Enums.VolunteerSortField.CALL_ID => from v in volunteersInList orderby v.CallId select v,
                    BO.Enums.VolunteerSortField.CALL_TYPE => from v in volunteersInList orderby v.CallType select v,
                    _ => from v in volunteersInList orderby v.Id select v // Default sorting by ID
                };

            }

            return volunteersInList;
        }
        catch (BO.Exceptions.BlDoesNotExistException ex)
        {
            // Handle cases where no volunteers exist or match the filter
            throw new BO.Exceptions.BlDoesNotExistException("An error occurred while retrieving the volunteers list: ", ex);
        }
        catch (Exception ex)
        {
            // General exception handling
            throw new BO.Exceptions.BLGeneralException("An unexpected error occurred while retrieving the volunteers list.", ex);
        }
    }

    /// <summary>
    /// Authenticates a volunteer using their name and password.
    /// </summary>
    public BO.Enums.Role Login(string fullName, string password)
    {
        
        var volunteer = _dal.Volunteer.Read(v => v.FullName == fullName && v.Password == _dal.Volunteer.EncryptPassword(password));
        if (volunteer == null)
        {
            throw new BO.Exceptions.BlDoesNotExistException("Incorrect username or password");
        }
        return (BO.Enums.Role)volunteer.Role;
    }

    /// <summary>
    /// Updates the details of an existing volunteer.
    /// </summary>
    public void UpdateVolunteerDetails(int requesterId, BO.Volunteer volunteer)
    {
        try
        {
            if (volunteer == null)
                throw new BO.Exceptions.BLInvalidDataException($"Cannot update volunteer with null object ID={requesterId}");

            if (volunteer.Role != BO.Enums.Role.manager && volunteer.Id != requesterId)
            {
                throw new BO.Exceptions.BLUnauthorizedException("Requester is not authorized to update this volunteer.");
            }

            var (latitude, longitude) = Tools.GetCoordinates(volunteer.FullAddress);
            volunteer.Latitude = latitude;
            volunteer.Longitude = longitude;
            VolunteerManager.Validation(volunteer);

            DO.Volunteer originalVolunteer = _dal.Volunteer.Read(v => v.Id == volunteer.Id)
                ?? throw new BO.Exceptions.BlDoesNotExistException($"Volunteer with ID={volunteer.Id} does not exist.");

            DO.Volunteer updatedVolunteer = VolunteerManager.ConvertBOToDO(volunteer);
            _dal.Volunteer.Update(updatedVolunteer);
        }
        catch (Exception ex)
        {
            throw new BO.Exceptions.BLInvalidDataException($"An error occurred while updating Volunteer ID={requesterId}: {ex.Message}");
        }
    }
}
