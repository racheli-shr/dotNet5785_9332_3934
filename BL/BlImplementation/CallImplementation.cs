using BL.Helpers;
using BLApi;
using BO;
using Helpers;
namespace BLImplementation;


internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddCall(BO.Call call)
    {
        try
        {
            if (call == null)
                throw new BO.Exceptions.BLInvalidDataException($"cannot update call with null object");


            // Validate and retrieve coordinates
            var (latitude, longtitude) = Tools.GetCoordinates(call.FullAddress);
            call.Latitude = latitude;
            call.longtitude = longtitude;
            CallManager.Validation(call);

            // Update originalVolunteer with allowed changes
            DO.Call updatedCall = CallManager.ConvertBOToDO(call);

            // Save updated volunteer to DAL
            _dal.Call.Create(updatedCall);
            CallManager.Observers.NotifyListUpdated(); //stage 5  
        }

        catch (BO.Exceptions.BLInvalidDataException ex)
        {
            throw new BO.Exceptions.BLInvalidDataException($"An error occurred while adding Volunteer : {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new BO.Exceptions.BLGeneralException($"An error occurred while adding Volunteer : {ex.Message}", ex);
        }
    }

    public void AssignCallToVolunteer(int volunteerId, int callId)
    {
        try
        {
            // Fetch the necessary data from the data layer
            var call = _dal.Call.Read(callId);
            var volunteer = _dal.Volunteer.Read(volunteerId);
            var assignments = _dal.Assignment.ReadAll(a => a.CallId == callId);

            // Using 'let' to create a variable for filtering the assignments
            var openAssignments = from assignment in assignments
                                  let isAssigned = assignment.AssignmentStatus == DO.Enums.AssignmentStatus.TREATED || assignment.AssignmentStatus == DO.Enums.AssignmentStatus.EXPIRED
                                  where isAssigned
                                  select assignment;

            // Check if the call is already being handled or has expired
            if (openAssignments.Any())
                throw new BO.Exceptions.BLGeneralException("The call is already being handled or has expired.");

            // Check if the call has expired based on the max finish time
            if (call.MaxTimeToEnd < AdminManager.Now)
                throw new BO.Exceptions.BLGeneralException("The call has expired.");

            double Latitude = volunteer.Latitude ?? 0;
            double longtitude = volunteer.longtitude ?? 0;
            var distanceBetweenVolToCall = VolunteerManager.GetDistance(Latitude, longtitude, call.Latitude, call.longtitude, volunteer.DistanceType);
            if (volunteer.MaxDistance < distanceBetweenVolToCall)
                throw new BO.Exceptions.BLInvalidDataException("The call is further from volunteer max distance .");

            // Create a new assignment if the call is open and valid
            var newAssignment = new DO.Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                AssignmentStatus = null,
                EntryTimeForTreatment = AdminManager.Now,
                ActualTreatmentEndTime = null,// Not known at this stage

            };

            // Add the new assignment to the data layer
            _dal.Assignment.Create(newAssignment);
            CallManager.Observers.NotifyListUpdated();  //stage 5  
        }
        catch (Exception ex)
        {
            // General exception handling for assignment failure
            throw new BO.Exceptions.BLGeneralException($"Error assigning call to volunteer: {ex.Message}", ex);
        }
    }

    public void CancelCallTreatment(int volunteerId, int assignmentId)
    {
        try
        {
            // Retrieve the assignment and volunteer details from the database
            var assignment = _dal.Assignment.Read(assignmentId);
            var volunteer = _dal.Volunteer.Read(volunteerId);
            // Check if the volunteer is allowed to cancel the assignment
            // The volunteer can only cancel their own assignment, or if they are a director
            if (assignment.VolunteerId != volunteerId && volunteer.Role != DO.Enums.Role.manager)
                throw new BO.Exceptions.BLGeneralException($"invalid action,cannot cancel assignment that is not yours if yor not the director ");
            // Check if the assignment status is not open (i.e., it has already been treated, expired, or completed)
            if (assignment.AssignmentStatus == DO.Enums.AssignmentStatus.TREATED ||
               assignment.AssignmentStatus == DO.Enums.AssignmentStatus.EXPIRED ||
               assignment.ActualTreatmentEndTime != null)
            {
                throw new BO.Exceptions.BLGeneralException($"invalid action,cannot cancel assignment that is not open ");
            }
            // Set the new assignment status based on who is canceling (volunteer or manager)
            var AssignmentStatus = assignment.VolunteerId == volunteerId ? DO.Enums.AssignmentStatus.SELF_CANCELLED : DO.Enums.AssignmentStatus.MANAGER_CANCELLED;
            var updatedAssignment = new DO.Assignment()
            {
                Id = assignment.Id,
                CallId = assignment.CallId,
                VolunteerId = volunteerId,
                EntryTimeForTreatment = assignment.EntryTimeForTreatment,
                ActualTreatmentEndTime = AdminManager.Now,
                AssignmentStatus = AssignmentStatus
            };
            // Create the updated assignment object with the new status and end time
            _dal.Assignment.Update(updatedAssignment);
            CallManager.Observers.NotifyItemUpdated(updatedAssignment.Id);  //stage 5

            CallManager.Observers.NotifyListUpdated();  //stage 5  

        }
        catch (Exception ex)
        {
            throw new BO.Exceptions.BlDoesNotExistException($"cannot complete call treatment : {ex.Message}", ex);
        }
    }

    public void CompleteCallTreatment(int volunteerId, int assignmentId)
    {
        try
        {
            // Retrieve the assignment from the database
            var assignment = _dal.Assignment.Read(assignmentId);
            // Check if the assignment belongs to the volunteer, if not throw an error
            if (assignment.VolunteerId != volunteerId)
                throw new BO.Exceptions.BLGeneralException($"invalid action,cannot complete assignment that is not yours ");
            // Check if the assignment is in a state that cannot be completed (e.g., already treated, cancelled, expired, or completed)
            if (assignment.AssignmentStatus == DO.Enums.AssignmentStatus.TREATED ||
               assignment.AssignmentStatus == DO.Enums.AssignmentStatus.SELF_CANCELLED ||
               assignment.AssignmentStatus == DO.Enums.AssignmentStatus.MANAGER_CANCELLED ||
               assignment.AssignmentStatus == DO.Enums.AssignmentStatus.EXPIRED ||
               assignment.ActualTreatmentEndTime != null)
            {
                throw new BO.Exceptions.BLGeneralException($"invalid action,cannot complete assignment that is not open ");
            }
            var updatedAssignment = new DO.Assignment()
            {
                Id = assignment.Id,
                CallId = assignment.CallId,
                VolunteerId = volunteerId,
                EntryTimeForTreatment = assignment.EntryTimeForTreatment,
                ActualTreatmentEndTime = AdminManager.Now,
                AssignmentStatus = DO.Enums.AssignmentStatus.TREATED
            };
            // Update the assignment in the database
            _dal.Assignment.Update(updatedAssignment);
            CallManager.Observers.NotifyItemUpdated(updatedAssignment.Id);  //stage 5
            CallManager.Observers.NotifyListUpdated();  //stage 5  
        }
        catch (Exception ex)
        {
            throw new BO.Exceptions.BlDoesNotExistException($"cannot complete call treatment : {ex.Message}", ex);
        }
    }

    public void DeleteCall(int callId)
    {
        try
        {
            var DOcall = _dal.Call.Read(c => c.Id == callId);
            var BOcall = Read(DOcall.Id);

            if (BOcall == null)
            {
                throw new BO.Exceptions.BlDoesNotExistException($"Call with ID {callId} does not exist.");
            }

            // Check if the call status allows deletion
            if (BOcall.Status != BO.Enums.CallStatus.Open)
            {
                throw new BO.Exceptions.BLInvalidDataException("Only calls with status 'Open' can be deleted.");
            }

            // Check if the call has assignments
            var assignments = _dal.Assignment.ReadAll(assignment => assignment.CallId == callId);
            if (assignments.Any())
            {
                throw new BO.Exceptions.BLInvalidDataException("Cannot delete the call because it has been assigned to volunteers.");
            }

            // Delete the call from the data layer
            _dal.Call.Delete(callId);
            CallManager.Observers.NotifyListUpdated();  //stage 5    
        }
        catch (BO.Exceptions.BlDoesNotExistException ex)
        {
            // Log and rethrow exception with a modified message
            throw new BO.Exceptions.BlDoesNotExistException($"The call with ID {callId} does not exist in the data layer.");
        }
        catch (BO.Exceptions.BLInvalidDataException ex)
        {
            // Log validation error
            Console.WriteLine($"Validation error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            // Handle unexpected errors
            Console.WriteLine($"Unexpected error: {ex.Message}");
            throw new BO.Exceptions.BLGeneralException("An unexpected error occurred while attempting to delete the call.", ex);
        }
    }


    public IEnumerable<BO.StatusCounter> GetCallCountsByStatus()//TODO: find return value type
    {
        IEnumerable<DO.Call> dalCalls = _dal.Call.ReadAll();
        IEnumerable<BO.Call> blCalls = dalCalls.Select(call => Read(call.Id));
        int maxStatusIndex = Enum.GetValues(typeof(BO.Enums.CallStatus)).Cast<int>().Max();

        // Use LINQ to group calls by status and construct the array directly.
        var groups = from call in blCalls
                     group call by call.Status into statusGroup
                     select new BO.StatusCounter { status = statusGroup.Key, Count = statusGroup.Count() };
        return groups;
        //Enumerable.Range(0, maxStatusIndex + 1)
        //.Select(index => blCalls.Count(call => (int)call.CallType == index)) // Count calls for each status.
        //.ToArray(); // Convert the result to an array.
    }

    public BO.Call Read(int callId)
    {
        try
        {

            var DOCall = _dal.Call.Read(c => c.Id == callId);
            var DOAssignment = _dal.Assignment.ReadAll(c => c.CallId == callId);
            var callStatus = CallManager.GetCallStatus(callId);
            List<BO.CallAssignInList>? Assignments = null;
            if (DOAssignment != null)
            {
                Assignments = DOAssignment.Select(assignment =>
                {
                    var volunteerId = assignment.VolunteerId;
                    var DOVolunteers = _dal.Volunteer.Read(v => v.Id == volunteerId);
                    var volunteerName = DOVolunteers.FullName;
                    return new BO.CallAssignInList()
                    {
                        VolunteerId = volunteerId,
                        VolunteerName = volunteerName,
                        EntryTime = assignment.EntryTimeForTreatment,
                        CompletionTime = assignment.ActualTreatmentEndTime,
                        EndType = (BO.Enums.AssignmentStatus)assignment.AssignmentStatus
                    };

                }).ToList();
            }
            return new BO.Call
            {
                Id = callId,
                CallType = (BO.Enums.CallType)DOCall.CallType,
                Description = DOCall.Description,
                FullAddress = DOCall.FullAdress,
                Latitude = DOCall.Latitude,
                longtitude = DOCall.longtitude,
                OpeningTime = DOCall.OpeningCallTime,
                MaxFinishTime = DOCall.MaxTimeToEnd,
                Status = callStatus,
                Assignments = Assignments
            };

        }
        catch (Exception ex)
        {
            throw new BO.Exceptions.BlDoesNotExistException($"Error fetching Call or Assignments: {ex.Message}");
        }
    }

    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.Enums.CallType? callType, BO.Enums.ClosedCallInListFields? sortByField)
    {
        // Step 1: Fetch all assignments for the specific volunteer
        var volunteerAssignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);

        // Step 2: Create a dictionary of assignments mapped by CallId for direct access
        var assignmentDictionary = volunteerAssignments.ToDictionary(a => a.CallId);

        // Step 3: Fetch closed calls related to the volunteer's assignments
        var closedCalls = volunteerAssignments
            .Select(a => Read(a.CallId)) // For each assignment, fetch the corresponding call
            .Where(c => c.Status == BO.Enums.CallStatus.Closed); // Filter by closed status

        // Step 4: Create the list of ClosedCallInList objects with the necessary data
        var closedCallInList = closedCalls.Select(c =>
        {
            // Get the assignment matching the current call
            var assignment = assignmentDictionary[c.Id];

            // Create the ClosedCallInList object and populate its fields
            return new BO.ClosedCallInList
            {
                Id = c.Id,
                CallType = c.CallType,
                Address = c.FullAddress,
                OpeningTime = c.OpeningTime,
                EntryTimeToHandle = assignment.EntryTimeForTreatment,  // Use the assignment data if available
                ActualFinishTime = assignment.ActualTreatmentEndTime,  // Use the assignment data if available
                FinishType = (BO.Enums.TypeOfTreatmentTerm)assignment.AssignmentStatus  // Use the assignment data if available
            };
        });

        // Step 5: Filter by call type if provided
        if (callType.HasValue)
        {
            closedCallInList = closedCallInList.Where(c => c.CallType == callType.Value);
        }

        // Step 6: Sort the list by the provided field, if given
        switch (sortByField.Value)
        {
            case BO.Enums.ClosedCallInListFields.Id:
                closedCallInList = closedCallInList.OrderBy(c => c.Id); // Sort by call ID
                break;
            case BO.Enums.ClosedCallInListFields.ActualEndTime:
                closedCallInList = closedCallInList.OrderBy(c => c.ActualFinishTime); // Sort by completion date
                break;
            case BO.Enums.ClosedCallInListFields.EndType:
                closedCallInList = closedCallInList.OrderBy(c => c.FinishType); // Sort by status (EndType)
                break;
            default:
                closedCallInList = closedCallInList.OrderBy(c => c.Id); // Default: Sort by call number (ID)
                break;
        }

        return closedCallInList;
    }
    public IEnumerable<BO.CallInList> GetFilteredAndCallList(BO.Enums.CallInListFields? filterByField = null, object? filterValue = null, BO.Enums.CallInListFields? sortByField = null)
    {

        var DOCalls = _dal.Call.ReadAll();
        var BOCall = DOCalls.Select(call => Read(call.Id));
        var nextId = 1;
        IEnumerable<BO.CallInList> callInLists = BOCall.Select(bocall =>
        {
            var assignments = _dal.Assignment.ReadAll(assignment => assignment.CallId == bocall.Id);
            Console.WriteLine(assignments.Count());
            var orderedAssignments = assignments.OrderByDescending(assignment => assignment.EntryTimeForTreatment);
            DateTime currentTime = AdminManager.Now;
            var volunteerId = orderedAssignments.FirstOrDefault()?.Id;
            string lastVolunteerName = "no assignment";
            if (volunteerId != null)
                lastVolunteerName = _dal.Volunteer.Read(v => v.Id == volunteerId).FullName;
            TimeSpan? TotalHandlingTime = null;
            int totalAssignments = orderedAssignments.Count();
            if (bocall.Status == BO.Enums.CallStatus.Closed)
                TotalHandlingTime = orderedAssignments.FirstOrDefault().ActualTreatmentEndTime - bocall.OpeningTime;

            return new BO.CallInList
            {
                Id = nextId++,//orderedAssignments.FirstOrDefault().Id,
                CallId = bocall.Id,
                CallType = bocall.CallType,
                OpeningTime = bocall.OpeningTime,
                RemainingTime = bocall.MaxFinishTime - currentTime,
                LastVolunteerName = lastVolunteerName,
                HandlingDuration = TotalHandlingTime,
                Status = bocall.Status,
                AssignmentCount = totalAssignments
            };
        });



        switch (filterByField)
        {
            case BO.Enums.CallInListFields.Id:
                callInLists = callInLists.Where(callInList => callInList.Id == (int)filterValue);
                break;
            case BO.Enums.CallInListFields.CallId:
                callInLists = callInLists.Where(call => call.CallId == (int)filterValue);
                break;
            case BO.Enums.CallInListFields.CallType:
                callInLists = callInLists.Where(call => call.CallType == (BO.Enums.CallType)filterValue);
                break;
            case BO.Enums.CallInListFields.OpeningTime:
                callInLists = callInLists.Where(call => call.OpeningTime == (DateTime)filterValue);
                break;
            case BO.Enums.CallInListFields.RemainingTime:
                callInLists = callInLists.Where(call => call.RemainingTime == (TimeSpan?)filterValue);
                break;
            case BO.Enums.CallInListFields.LastVolunteerName:
                callInLists = callInLists.Where(call => call.LastVolunteerName.Contains((string)filterValue));
                break;
            case BO.Enums.CallInListFields.TotalHandlingTime:
                callInLists = callInLists.Where(call => call.HandlingDuration == (TimeSpan?)filterValue);
                break;
            case BO.Enums.CallInListFields.CallStatus:
                callInLists = callInLists.Where(call => call.Status == (BO.Enums.CallStatus)filterValue);
                break;
            case BO.Enums.CallInListFields.TotalAssignments:
                callInLists = callInLists.Where(call => call.AssignmentCount == (int)filterValue);
                break;
            default:
                callInLists = callInLists;
                break;
        }


        switch (sortByField)
        {
            case BO.Enums.CallInListFields.Id:
                callInLists = callInLists.OrderBy(call => call.Id);
                break;
            case BO.Enums.CallInListFields.CallId:
                callInLists = callInLists.OrderBy(call => call.CallId);
                break;
            case BO.Enums.CallInListFields.CallType:
                callInLists = callInLists.OrderBy(call => call.CallType);
                break;
            case BO.Enums.CallInListFields.OpeningTime:
                callInLists = callInLists.OrderBy(call => call.OpeningTime);
                break;
            case BO.Enums.CallInListFields.RemainingTime:
                callInLists = callInLists.OrderBy(call => call.RemainingTime);
                break;
            case BO.Enums.CallInListFields.LastVolunteerName:
                callInLists = callInLists.OrderBy(call => call.LastVolunteerName);
                break;
            case BO.Enums.CallInListFields.TotalHandlingTime:
                callInLists = callInLists.OrderBy(call => call.HandlingDuration);
                break;
            case BO.Enums.CallInListFields.CallStatus:
                callInLists = callInLists.OrderBy(call => call.Status);
                break;
            case BO.Enums.CallInListFields.TotalAssignments:
                callInLists = callInLists.OrderBy(call => call.AssignmentCount);
                break;
            default:
                callInLists = callInLists.OrderBy(call => call.Id);
                break;
        }

        return callInLists;
    }

    public IEnumerable<BO.OpenCallInList> GetOpenCalls(int volunteerId, BO.Enums.CallType? callType, BO.Enums.OpenCallInListFields? sortByField)
    {
        var volunteer = _dal.Volunteer.Read(volunteerId);

        // Step 1: Retrieve all assignments for the given volunteer
        var volunteerAssignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);

        // Step 2: Create a dictionary to map assignments by CallId for quick access
        var assignmentDictionary = volunteerAssignments.ToDictionary(a => a.CallId);

        // Step 3: Filter only open or at-risk calls
        var openCalls = volunteerAssignments
            .Select(a => Read(a.CallId)) // Convert CallId to actual Call object
            .Where(c => c.Status == BO.Enums.CallStatus.Open || c.Status == BO.Enums.CallStatus.AtRisk) // Filter by status
            .ToList(); // Store as a list to optimize further operations

        double Latitude = volunteer.Latitude ?? 0;
        double longtitude = volunteer.longtitude ?? 0;

        // Step 4: Map open calls to OpenCallInList DTO
        var openCallInList = openCalls.Select(c => new BO.OpenCallInList
        {
            Id = c.Id,
            CallType = c.CallType,
            Description = c.Description,
            Address = c.FullAddress,
            OpeningTime = c.OpeningTime,
            MaxFinishTime = c.MaxFinishTime,
            DistanceFromVolunteer = VolunteerManager.GetDistance(Latitude, longtitude, c.Latitude, c.longtitude, volunteer.DistanceType)
        }).ToList(); // Store in a list to avoid multiple IEnumerable evaluations

        // Step 5: Filter by call type if a specific type is provided
        if (callType.HasValue)
        {
            openCallInList = openCallInList.Where(c => c.CallType == callType.Value).ToList();
        }

        // Step 6: Sort the list if a sorting field is provided
        if (sortByField.HasValue)
        {
            switch (sortByField.Value)
            {
                case BO.Enums.OpenCallInListFields.Id:
                    openCallInList = openCallInList.OrderBy(c => c.Id).ToList();
                    break;
                case BO.Enums.OpenCallInListFields.CallType:
                    openCallInList = openCallInList.OrderBy(c => c.CallType).ToList();
                    break;
                case BO.Enums.OpenCallInListFields.Description:
                    openCallInList = openCallInList.OrderBy(c => c.Description).ToList();
                    break;
                case BO.Enums.OpenCallInListFields.FullAddress:
                    openCallInList = openCallInList.OrderBy(c => c.Address).ToList();
                    break;
                case BO.Enums.OpenCallInListFields.OpenTime:
                    openCallInList = openCallInList.OrderBy(c => c.OpeningTime).ToList();
                    break;
                case BO.Enums.OpenCallInListFields.MaxEndTime:
                    openCallInList = openCallInList.OrderBy(c => c.MaxFinishTime).ToList();
                    break;
                case BO.Enums.OpenCallInListFields.DistanceFromVolunteer:
                    openCallInList = openCallInList.OrderBy(c => c.DistanceFromVolunteer).ToList();
                    break;
                default:
                    openCallInList = openCallInList.OrderBy(c => c.Id).ToList();
                    break;
            }
        }

        return openCallInList;
    }



    public void UpdateCall(BO.Call call)
    {
        try
        {
            if (call == null)
                throw new BO.Exceptions.BLInvalidDataException($"cannot update call with null object");
            // Check authorization


            // Validate and retrieve coordinates
            var (latitude, longtitude) = Tools.GetCoordinates(call.FullAddress);
            call.Latitude = latitude;
            call.longtitude = longtitude;
            CallManager.Validation(call);

            // Update originalVolunteer with allowed changes
            DO.Call updatedCall = CallManager.ConvertBOToDO(call);

            // Save updated volunteer to DAL
            _dal.Call.Update(updatedCall);
            CallManager.Observers.NotifyItemUpdated(updatedCall.Id);  //stage 5
            CallManager.Observers.NotifyListUpdated();  //stage 5

        }

        catch (BO.Exceptions.BLInvalidDataException ex)
        {
            throw new BO.Exceptions.BLInvalidDataException($"An error occurred while updating Volunteer : {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new BO.Exceptions.BLGeneralException($"An error occurred while updating Volunteer : {ex.Message}");
        }
    }
    public void AddObserver(Action listObserver) =>
CallManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
CallManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
CallManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
CallManager.Observers.RemoveObserver(id, observer); //stage 5

    #region Stage 5
    
    public IDisposable Subscribe(IObserver<Call> observer)
    {
        throw new NotImplementedException();
    }
    #endregion Stage 5
}
