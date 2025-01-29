
using BL.Helpers;
using BlApi;
using BLApi;
using BO;
using Helpers;
using Microsoft.VisualBasic;
using System;
using System.Linq;
namespace BLImplementation;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    
    public void AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            if (volunteer == null)
                throw new BO.Exceptions.BLInvalidDataException($"cannot add null object ");
            // Validate and retrieve coordinates
            var (latitude, longitude) = Tools.GetCoordinates(volunteer.FullAddress);
            volunteer.Latitude = latitude;
            volunteer.Longitude = longitude;
            VolunteerManager.Validation(volunteer);
            if (volunteer.Role == BO.Enums.Role.manager)
            {
                // בדיקה אם כבר קיים מנהל במערכת
                bool isDirectorExists = _dal.Volunteer.ReadAll().Any(v => v.Role == DO.Enums.Role.manager);

                if (isDirectorExists)
                {
                    throw new BO.Exceptions.BlDoesNotExistException("Cannot add a new Director. Only one Director is allowed in the system.");
                }
            }


            // Update originalVolunteer with allowed changes
            DO.Volunteer newVolunteer = VolunteerManager.ConvertBOToDO(volunteer);

            // Save updated volunteer to DAL
            _dal.Volunteer.Create(newVolunteer);

        }
        catch (Exceptions.BLInvalidDataException ex)
        {
            throw new BO.Exceptions.BLInvalidDataException($"An error occurred while adding new Volunteer : {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new BO.Exceptions.BLGeneralException($"An error occurred while adding new Volunteer : {ex.Message}");

        }
    }

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

            bool isCurrentlyHandlingCall = assignments.Any(a => a.TypeOfTreatmentTermination == DO.Enums.TypeOfTreatmentTerm.finished);

            if (isCurrentlyHandlingCall)
            {
                throw new BO.Exceptions.BlDeletionImpossible("The volunteer is currently handling a call and cannot be deleted.");
            }

            bool hasHandledCallsBefore = assignments.Any(a =>
              a.TypeOfTreatmentTermination != DO.Enums.TypeOfTreatmentTerm.selfCancelation &&
              a.TypeOfTreatmentTermination != DO.Enums.TypeOfTreatmentTerm.endTermCancelation &&
              a.TypeOfTreatmentTermination != DO.Enums.TypeOfTreatmentTerm.managerCancelation);

            if (hasHandledCallsBefore)
            {
                throw new BO.Exceptions.BlDeletionImpossible("The volunteer has handled calls in the past and cannot be deleted.");
            }

            _dal.Volunteer.Delete(volunteerId);
        }
        catch (DO.Exceptions.DalDoesNotExistException ex)
        {
            throw new BO.Exceptions.BlDoesNotExistException("The volunteer could not be deleted because the volunteer does not exist in the data layer.");
        }
        catch (Exception ex)
        {
            throw new BO.Exceptions.BLGeneralException("An error occurred while trying to delete the volunteer.", ex);
        }
    }

    public BO.Volunteer Read(int volunteerId)
    {
        try
        {
            DO.Volunteer doVolunteer = _dal.Volunteer.Read(v=>v.Id==volunteerId)
                ?? throw new BO.Exceptions.BlDoesNotExistException($"Volunteer with ID={volunteerId} does Not exist");

            IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == doVolunteer.Id);
            var groupedAssignments = assignments
                 .GroupBy(a => a.TypeOfTreatmentTermination)
                 .ToDictionary(g => g.Key, g => g.Count());

            int sumTreatedCalls = groupedAssignments.ContainsKey(DO.Enums.TypeOfTreatmentTerm.finished) ? groupedAssignments[DO.Enums.TypeOfTreatmentTerm.finished] : 0;
            int sumCallsSelfCancelled = groupedAssignments.ContainsKey(DO.Enums.TypeOfTreatmentTerm.selfCancelation) ? groupedAssignments[DO.Enums.TypeOfTreatmentTerm.selfCancelation] : 0;
            int sumExpiredCalls = groupedAssignments.ContainsKey(DO.Enums.TypeOfTreatmentTerm.endTermCancelation) ? groupedAssignments[DO.Enums.TypeOfTreatmentTerm.endTermCancelation] : 0;

            DO.Assignment? doAssignment = assignments.FirstOrDefault();
            BO.CallInProgress? callInProgress = null;

            if (doAssignment != null)
            {
                DO.Call? doCall = _dal.Call.Read(call => call.Id == doAssignment.CallId);

                callInProgress = new BO.CallInProgress
                {
                    Id = doVolunteer.Id,
                    CallId = doCall.Id,
                    CallType = (BO.Enums.CallType)doCall.CallType,
                    Description = doCall.Description,
                    Address = doCall.FullAdress,
                    OpeningTime = doCall.OpeningCallTime,
                    MaxFinishTime = doCall.MaxTimeToEnd,
                    EntryTimeToHandle = doAssignment.EntryTimeForTreatment,
                    DistanceFromVolunteer = VolunteerManager.GetDistance(doVolunteer.FullAdress, doCall.FullAdress, (DO.Enums.DistanceType)doVolunteer.DistanceType),
                    Status = CallManager.GetCallStatus(doCall.Id)
                };
            }

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
                DistanceType = (BO.Enums.DistanceType)doVolunteer.DistanceType,
                CurrentCallInProgress = callInProgress
            };
        }
        catch (Exception ex)
        {
            throw new BO.Exceptions.BlDoesNotExistException($"An error occurred while processing Volunteer ID={volunteerId} :{ex}");
        }
    }

    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive, BO.Enums.VolunteerSortField? sortField = null)
    {
        try
        {
            IEnumerable<DO.Volunteer> DOvolunteers;

            if (isActive == null)
            {
                DOvolunteers = _dal.Volunteer.ReadAll()
                    ?? throw new BO.Exceptions.BlDoesNotExistException("No volunteers exist in the system.");
            }
            else
            {
                DOvolunteers = _dal.Volunteer.ReadAll(v => v.IsActive == isActive)
                    ?? throw new BO.Exceptions.BlDoesNotExistException("No volunteers match the specified availability.");
            }

            IEnumerable<BO.VolunteerInList> volunteersInList = DOvolunteers.Select(v =>
            {
                BO.Volunteer BOvolunteer = Read(v.Id); // Retrieve volunteer details by ID

                return new VolunteerInList
                {
                    Id = BOvolunteer.Id,
                    FullName = BOvolunteer.FullName,
                    IsActive = BOvolunteer.IsActive,
                    TotalExpiredCalls = BOvolunteer.NumberOfexpiredCalls,
                    TotalCallsCanceled = BOvolunteer.NumberOfCanceledCalls,
                    TotalCallsHandled = BOvolunteer.NumberOfCalls,
                    CurrentCallId = BOvolunteer.CurrentCallInProgress?.CallId,
                    CallType = BOvolunteer.CurrentCallInProgress.CallType

                };
            });

            if (sortField.Value==null)
            {
                volunteersInList = volunteersInList.OrderBy(v => v.Id);
            }
            else
            {
                switch (sortField)
                {
                    case BO.Enums.VolunteerSortField.ID:
                        volunteersInList = volunteersInList.OrderBy(v => v.Id);
                        break;
                    case BO.Enums.VolunteerSortField.FULL_NAME:
                        volunteersInList = volunteersInList.OrderBy(v => v.FullName);
                        break;
                    case BO.Enums.VolunteerSortField.IS_AVAILABLE:
                        volunteersInList = volunteersInList.OrderBy(v => v.IsActive);
                        break;
                    case BO.Enums.VolunteerSortField.SUM_TREATED_CALLS:
                        volunteersInList = volunteersInList.OrderBy(v => v.TotalCallsHandled);
                        break;
                    case BO.Enums.VolunteerSortField.SUM_CALLS_SELF_CANCELLED:
                        volunteersInList = volunteersInList.OrderBy(v => v.TotalCallsCanceled);
                        break;
                    case BO.Enums.VolunteerSortField.SUM_EXPIRED_CALLS:
                        volunteersInList = volunteersInList.OrderBy(v => v.TotalExpiredCalls);
                        break;
                    case BO.Enums.VolunteerSortField.CALL_ID:
                        volunteersInList = volunteersInList.OrderBy(v => v.CurrentCallId);
                        break;
                    case BO.Enums.VolunteerSortField.CALL_TYPE:
                        volunteersInList = volunteersInList.OrderBy(v => v.CallType);
                        break;
                    default:
                        throw new BO.Exceptions.BLGeneralException("Invalid sort field specified.");
                }
            }

            return volunteersInList;
        }
        catch (BO.Exceptions.BlDoesNotExistException ex)
        {
            // Handle cases where no volunteers exist or match the filter
            throw new BO.Exceptions.BlDoesNotExistException("An error occurred while retrieving the volunteers list: " + ex.Message);
        }
        catch (Exception ex)
        {
            // General exception handling
            throw new BO.Exceptions.BLGeneralException("An unexpected error occurred while retrieving the volunteers list." + ex.Message);
        }
    }

    public BO.Enums.Role Login(string fullName, string password)
    {

        var volunteer = _dal.Volunteer.Read(v => v.FullName == fullName && v.Password == password);
        if (volunteer == null)
        {
            throw new BO.Exceptions.BlDoesNotExistException("Incorrect username or password");
        }
        return (BO.Enums.Role)volunteer.Role;
    }

    public void UpdateVolunteerDetails(int requesterId, BO.Volunteer volunteer)
    {
        try
        {
            if (volunteer == null)
                throw new BO.Exceptions.BLInvalidDataException($"cannot update volunteer with null object ID={requesterId}");
            // Check authorization
            if (volunteer.Role != BO.Enums.Role.manager && volunteer.Id != requesterId)
            {
                throw new BO.Exceptions.BLUnauthorizedException("Requester is not authorized to update this volunteer.");
            }

            // Validate and retrieve coordinates
            var (latitude, longitude) = Tools.GetCoordinates(volunteer.FullAddress);
            volunteer.Latitude = latitude;
            volunteer.Longitude = longitude;
            VolunteerManager.Validation(volunteer);

            // Retrieve original volunteer from DAL
            DO.Volunteer originalVolunteer = _dal.Volunteer.Read(v=>v.Id==volunteer.Id)
                ?? throw new BO.Exceptions.BlDoesNotExistException($"Volunteer with ID={volunteer.Id} does not exist.");

            // Ensure only a director can change the role of a volunteer
            if (volunteer.Role != (BO.Enums.Role)originalVolunteer.Role && originalVolunteer.Role != DO.Enums.Role.manager)
            {
                throw new BO.Exceptions.BLUnauthorizedException("Only a director can change the role of a volunteer.");
            }

            // Update originalVolunteer with allowed changes
            DO.Volunteer updatedVolunteer = VolunteerManager.ConvertBOToDO(volunteer);

            // Save updated volunteer to DAL
            _dal.Volunteer.Update(updatedVolunteer);

        }
        catch (Exception ex)
        {
            throw new BO.Exceptions.BLInvalidDataException($"An error occurred while updating Volunteer ID={requesterId}: {ex.Message}");
        }
    }

}