using BL.Helpers;
using BLApi;
using BO;
using DalApi;
using DO;
using Helpers;
namespace BLImplementation;


internal class CallImplementation : BLApi.ICall
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
                AssignmentStatus = DO.Enums.AssignmentStatus.AssignedAndInProgress,
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
               assignment.AssignmentStatus == DO.Enums.AssignmentStatus.EXPIRED 
              )
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
            var HisAssignment = _dal.Assignment.Read(c => c.CallId == callId);
            if (BOcall == null)
            {
                throw new BO.Exceptions.BlDoesNotExistException($"Call with ID {callId} does not exist.");
            }

            // Check if the call status allows deletion
            if (BOcall.Status != BO.Enums.CallStatus.Open && HisAssignment != null)
            {
                throw new BO.Exceptions.BLInvalidDataException(" calls with status 'Open' can be deleted.");
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
    //if an opened assignment was found he will cancel it and sending an email- תוספת
    public bool closeLastAssignmentByCallId(int callId, DO.Enums.AssignmentStatus canceledBy)
    {
        var DOAssignment = _dal.Assignment.ReadAll(c => c.CallId == callId);
        foreach (var assignment in DOAssignment)
        {
            if (assignment.AssignmentStatus == DO.Enums.AssignmentStatus.AssignedAndInProgress)
            {
                //update the manager cancelation of assignments
                _dal.Assignment.Update(assignment with { AssignmentStatus = canceledBy});
                //searching the call
                var call = _dal.Call.Read(c => c.Id == callId);
                ////sending an email to the concerned volunteer
                //var volunteer = _dal.Volunteer.Read(v => v.Id == assignment.VolunteerId);
                //var subject = $"Assignment Cancelled for Call #{callId}";
                //string typeOfUser = canceledBy == DO.Enums.AssignmentStatus.MANAGER_CANCELLED ? "manager" : "volunteer";
                //var body = $"Dear {volunteer.FullName},\n\nThe assignment for call #{callId} has been cancelled by the {typeOfUser}.\n\nCall details:\nType: {call.CallType}.\nLocation: {call.FullAdress}.\nDescription: {call.Description} .\n\nBest regards,\nManagement System";
                //MailHelper.SendEmail(volunteer.Email, subject, body);
                ////return success
                return true;
            }
        }
        return false;
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
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, Func<BO.ClosedCallInList, bool>? predicate = null)
    {
        // Retrieve all closed calls for the volunteer from the assignments table.  
        var closedCalls = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.AssignmentStatus != null&&(a.AssignmentStatus!=DO.Enums.AssignmentStatus.AssignedAndInProgress))
            .Join(_dal.Call.ReadAll(),
            a => a.CallId, c => c.Id,
            (a, c) => new BO.ClosedCallInList
            {
                Id = c.Id,
                CallType = (BO.Enums.CallType)c.CallType,
                Address = c.FullAdress,
                OpeningTime = c.OpeningCallTime,
                EntryTimeToHandle = a.EntryTimeForTreatment,
                ActualFinishTime = a.ActualTreatmentEndTime,
                FinishType = a.AssignmentStatus != null ? (BO.Enums.TypeOfTreatmentTerm)a.AssignmentStatus : null
            });

        // Apply the predicate filter if specified.  
        if (predicate != null)
        {
            closedCalls = closedCalls.Where(predicate);
        }

        return closedCalls;
    }

    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, Func<BO.OpenCallInList, bool> predicate = null /*BO.Enums.OpenCallInListFields? filterByField = null, object? filterValue = null, BO.Enums.OpenCallInListFields? sortByField = null*/)
    {
        var volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BO.Exceptions.BlDoesNotExistException($"volunteer with ID={volunteerId} does not exist");

        // Retrieve all open or open-in-risk calls for the volunteer.  
        var openCalls = _dal.Call.ReadAll(c => CallManager.CalculateCallStatus(c) == BO.Enums.CallStatus.Open ||
            CallManager.CalculateCallStatus(c) == BO.Enums.CallStatus.OpenAtRisk)
            .Select(c => new BO.OpenCallInList
            {
                Id = c.Id,
                CallType = (BO.Enums.CallType)c.CallType,
                Description = c.Description,
                Address = c.FullAdress,
                OpeningTime = c.OpeningCallTime,
                MaxFinishTime = c.MaxTimeToEnd,
                DistanceFromVolunteer = Tools.CalculateDistance(volunteerId, c.Latitude, c.longtitude)
            });

        //Apply predicate filter if specified.
        if (predicate != null)
                openCalls = openCalls.Where(predicate);

        openCalls = openCalls.Where(c => c.DistanceFromVolunteer <= volunteer.MaxDistance); // Filter out calls that are too far away.  
        //openCalls = sortByField switch
        //{
        //    BO.Enums.OpenCallInListFields.Id => openCalls.OrderBy(call => call.Id),
        //    BO.Enums.OpenCallInListFields.CallType => openCalls.OrderBy(call => call.CallType),
            
        //    BO.Enums.OpenCallInListFields.OpenTime => openCalls.OrderBy(call => call.OpeningTime),
        //    BO.Enums.OpenCallInListFields.MaxEndTime => openCalls.OrderBy(call => call.MaxFinishTime),
        //    BO.Enums.OpenCallInListFields.DistanceFromVolunteer => openCalls.OrderBy(call => call.DistanceFromVolunteer),
        //    //BO.Enums.OpenCallInListFields.TotalHandlingTime => openCalls.OrderBy(call => call.HandlingDuration),
        //    //BO.Enums.OpenCallInListFields.CallStatus => openCalls.OrderBy(call => call.Status),
        //    //BO.Enums.OpenCallInListFields.TotalAssignments => openCalls.OrderBy(call => call.AssignmentCount
        //    ),
        //    _ => openCalls.OrderBy(call => call.Id)
        //};

        return openCalls;
    }

 
    public IEnumerable<BO.CallInList> GetFilteredAndCallList(BO.Enums.CallInListFields? filterByField = null, object? filterValue = null, BO.Enums.CallInListFields? sortByField = null)

    {
        IEnumerable<DO.Call> DOCalls = _dal.Call.ReadAll();
        var BOCall = DOCalls.Select(call => Read(call.Id)).ToList();

        IEnumerable<BO.CallInList> callInLists = BOCall.Select(call =>
        {
            var assignments = _dal.Assignment.ReadAll(a => a.CallId == call.Id);
            var lastAssignment = assignments.OrderByDescending(a => a.EntryTimeForTreatment).FirstOrDefault();
            var lastVolunteer = lastAssignment != null
                ? _dal.Volunteer.ReadAll().FirstOrDefault(v => v.Id == lastAssignment.VolunteerId)
                : null;

            TimeSpan? totalCompletionTime = (call.MaxFinishTime != null && call.OpeningTime != null)
                ? call.MaxFinishTime - call.OpeningTime
                : null;

            DateTime currentTime = AdminManager.Now;

            return new BO.CallInList
            {
                Id = call.Id,
                CallId = call.Id,
                CallType = (BO.Enums.CallType)call.CallType,
                OpeningTime = call.OpeningTime,
                RemainingTime = call.MaxFinishTime - currentTime,
                LastVolunteerName = lastVolunteer?.FullName,
                HandlingDuration= totalCompletionTime,
                Status = CallManager.GetCallStatus(call.Id),
                AssignmentCount = assignments.Count()
            };
        });

        if (filterByField != null && filterValue is string strValue)
        {
            callInLists = callInLists.Where(call => filterByField switch
            {
                BO.Enums.CallInListFields.Id => int.TryParse(strValue, out var id) && call.Id == id,
                BO.Enums.CallInListFields.CallId => int.TryParse(strValue, out var callId) && call.CallId == callId,
                BO.Enums.CallInListFields.CallType => Enum.TryParse<BO.Enums.CallType>(strValue, out var type) && call.CallType == type,
                BO.Enums.CallInListFields.OpeningTime => DateTime.TryParse(strValue, out var date) && call.OpeningTime == date,
                BO.Enums.CallInListFields.RemainingTime => TimeSpan.TryParse(strValue, out var remaining) && call.RemainingTime == remaining,
                BO.Enums.CallInListFields.LastVolunteerName => call.LastVolunteerName?.Contains(strValue, StringComparison.OrdinalIgnoreCase) == true,
                BO.Enums.CallInListFields.TotalHandlingTime => TimeSpan.TryParse(strValue, out var handling) && call.HandlingDuration == handling,
                BO.Enums.CallInListFields.CallStatus => Enum.TryParse<BO.Enums.CallStatus>(strValue, out var status) && call.Status == status,
                BO.Enums.CallInListFields.TotalAssignments => int.TryParse(strValue, out var total) && call.AssignmentCount == total,
                _ => true
            });
        }


        // מיון הרשימה
        callInLists = sortByField switch
        {
            BO.Enums.CallInListFields.Id => callInLists.OrderBy(call => call.Id),
            BO.Enums.CallInListFields.CallId => callInLists.OrderBy(call => call.CallId),
            BO.Enums.CallInListFields.CallType => callInLists.OrderBy(call => call.CallType),
            BO.Enums.CallInListFields.OpeningTime => callInLists.OrderBy(call => call.OpeningTime),
            BO.Enums.CallInListFields.RemainingTime => callInLists.OrderBy(call => call.RemainingTime),
            BO.Enums.CallInListFields.LastVolunteerName => callInLists.OrderBy(call => call.LastVolunteerName),
            BO.Enums.CallInListFields.TotalHandlingTime => callInLists.OrderBy(call => call.HandlingDuration),
            BO.Enums.CallInListFields.CallStatus => callInLists.OrderBy(call => call.Status),
            BO.Enums.CallInListFields.TotalAssignments => callInLists.OrderBy(call => call.AssignmentCount
            ),
            _ => callInLists.OrderBy(call => call.Id)
        };

        return callInLists;

    }
    public IEnumerable<BO.OpenCallInList> SortOpenCalls(int volunteerId, BO.Enums.OpenCallInListFields? sortField = BO.Enums.OpenCallInListFields.Id)
    {
        var volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BO.Exceptions.BlDoesNotExistException($"volunteer with ID={volunteerId} does not exist");

        // Retrieve all open or open-in-risk calls for the volunteer.  
        var openCalls = _dal.Call.ReadAll(c => CallManager.CalculateCallStatus(c) == BO.Enums.CallStatus.Open ||
            CallManager.CalculateCallStatus(c) == BO.Enums.CallStatus.OpenAtRisk)
            .Select(c => new BO.OpenCallInList
            {
                Id = c.Id,
                CallType = (BO.Enums.CallType)c.CallType,
                Description = c.Description,
                Address = c.FullAdress,
                OpeningTime = c.OpeningCallTime,
                MaxFinishTime = c.MaxTimeToEnd,
                DistanceFromVolunteer = Tools.CalculateDistance(volunteerId, c.Latitude, c.longtitude)
            });

        openCalls = openCalls.Where(c => c.DistanceFromVolunteer <= volunteer.MaxDistance); // Filter out calls that are too far away.  

        return openCalls.OrderBy(item =>
            item.GetType().GetProperty(sortField.ToString())?.GetValue(item));
    }
    public IEnumerable<BO.OpenCallInList> FilterOpenCalls(int volunteerId, BO.Enums.OpenCallInListFields? filterField = null, object? filterValue = null)
    {
        var volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BO.Exceptions.BlDoesNotExistException($"volunteer with ID={volunteerId} does not exist");

        // Retrieve all open or open-in-risk calls for the volunteer.  
        var openCalls = _dal.Call.ReadAll(c => CallManager.CalculateCallStatus(c) == BO.Enums.CallStatus.Open ||
            CallManager.CalculateCallStatus(c) == BO.Enums.CallStatus.OpenAtRisk)
            .Select(c => new BO.OpenCallInList
            {
                Id = c.Id,
                CallType = (BO.Enums.CallType)c.CallType,
                Description = c.Description,
                Address = c.FullAdress,
                OpeningTime = c.OpeningCallTime,
                MaxFinishTime = c.MaxTimeToEnd,
                DistanceFromVolunteer = Tools.CalculateDistance(volunteerId, c.Latitude, c.longtitude)
            });

        if (filterValue != null)
            openCalls = openCalls.Where(call =>
            {
                var prop = call.GetType().GetProperty(filterField.ToString());
                var val = prop?.GetValue(call);

                Console.WriteLine($"Checking: {val} == {filterValue} → {val?.ToString() == filterValue?.ToString()}");

                return val?.ToString() == filterValue?.ToString();
            }).ToList();
        return openCalls;
    }
    //אני רוצה פונקציה שתבדוק האם לקריאה מסויימת יש הקצאות


    //public IEnumerable<BO.OpenCallInList> GetFilteredAndOpenCalls(int volunteerId, BO.Enums.CallType? callType, BO.Enums.OpenCallInListFields? sortByField)
    //    {
    //        var volunteer = _dal.Volunteer.Read(volunteerId);

    //        // Step 1: Retrieve all assignments for the given volunteer
    //        var volunteerAssignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);

    //        // Step 2: Create a dictionary to map assignments by CallId for quick access
    //        var assignmentDictionary = volunteerAssignments.ToDictionary(a => a.CallId);

    //        // Step 3: Filter only open or at-risk calls
    //        var openCalls = volunteerAssignments
    //            .Select(a => Read(a.CallId)) // Convert CallId to actual Call object
    //            .Where(c => c.Status == BO.Enums.CallStatus.Open || c.Status == BO.Enums.CallStatus.OpenAtRisk) // Filter by status
    //            .ToList(); // Store as a list to optimize further operations

    //        double Latitude = volunteer.Latitude ?? 0;
    //        double longtitude = volunteer.longtitude ?? 0;

    //        // Step 4: Map open calls to OpenCallInList DTO
    //        var openCallInList = openCalls.Select(c => new BO.OpenCallInList
    //        {
    //            Id = c.Id,
    //            CallType = c.CallType,
    //            Description = c.Description,
    //            Address = c.FullAddress,
    //            OpeningTime = c.OpeningTime,
    //            MaxFinishTime = c.MaxFinishTime,
    //            DistanceFromVolunteer = VolunteerManager.GetDistance(Latitude, longtitude, c.Latitude, c.longtitude, volunteer.DistanceType)
    //        }).ToList(); // Store in a list to avoid multiple IEnumerable evaluations

    //        // Step 5: Filter by call type if a specific type is provided
    //        if (callType.HasValue)
    //        {
    //            openCallInList = openCallInList.Where(c => c.CallType == callType.Value).ToList();
    //        }

    //        // Step 6: Sort the list if a sorting field is provided
    //        if (sortByField.HasValue)
    //        {
    //            switch (sortByField.Value)
    //            {
    //                case BO.Enums.OpenCallInListFields.Id:
    //                    openCallInList = openCallInList.OrderBy(c => c.Id).ToList();
    //                    break;
    //                case BO.Enums.OpenCallInListFields.CallType:
    //                    openCallInList = openCallInList.OrderBy(c => c.CallType).ToList();
    //                    break;
    //                case BO.Enums.OpenCallInListFields.Description:
    //                    openCallInList = openCallInList.OrderBy(c => c.Description).ToList();
    //                    break;
    //                case BO.Enums.OpenCallInListFields.FullAddress:
    //                    openCallInList = openCallInList.OrderBy(c => c.Address).ToList();
    //                    break;
    //                case BO.Enums.OpenCallInListFields.OpenTime:
    //                    openCallInList = openCallInList.OrderBy(c => c.OpeningTime).ToList();
    //                    break;
    //                case BO.Enums.OpenCallInListFields.MaxEndTime:
    //                    openCallInList = openCallInList.OrderBy(c => c.MaxFinishTime).ToList();
    //                    break;
    //                case BO.Enums.OpenCallInListFields.DistanceFromVolunteer:
    //                    openCallInList = openCallInList.OrderBy(c => c.DistanceFromVolunteer).ToList();
    //                    break;
    //                default:
    //                    openCallInList = openCallInList.OrderBy(c => c.Id).ToList();
    //                    break;
    //            }
    //        }

    //        return openCallInList;
    //    }

    public bool isExistingAssignmentToCall(int callId)
    {
        var assignment = _dal.Assignment.Read(a => a.CallId == callId && a.AssignmentStatus==DO.Enums.AssignmentStatus.AssignedAndInProgress);
        return (assignment == null);

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

    public IDisposable Subscribe(IObserver<DO.Call> observer)
    {
        throw new NotImplementedException();
    }
    #endregion Stage 5
    public void DeleteAssignmentToCall(int callId)
    {
        var call = _dal.Call.Read(callId);
        var status = CallManager.CalculateCallStatus(call);
        if (status != BO.Enums.CallStatus.InProgress && status != BO.Enums.CallStatus.InProgressAtRisk) throw new BO.Exceptions.BLUnauthorizedException($"Cant delete a non opened call status");
        var AssignmentToDelete = _dal.Assignment.ReadAll(a => a.CallId == call.Id && (BO.Enums.AssignmentStatus)a.AssignmentStatus == BO.Enums.AssignmentStatus.AssignedAndInProgress).FirstOrDefault();
        closeLastAssignmentByCallId(callId, DO.Enums.AssignmentStatus.SELF_CANCELLED);
    }
    public void CompleteAssignmentToCall(int volunteerId, int callId)
    {
        var call = _dal.Call.Read(callId);
        var status = CallManager.CalculateCallStatus(call);
        if (status != BO.Enums.CallStatus.InProgress && status != BO.Enums.CallStatus.InProgressAtRisk) throw new BO.Exceptions.BLUnauthorizedException($"Cant delete a non opened call status");
        var AssignmentToComplete = _dal.Assignment.ReadAll(a => a.CallId == call.Id && (BO.Enums.AssignmentStatus)a.AssignmentStatus == BO.Enums.AssignmentStatus.AssignedAndInProgress).FirstOrDefault();
        CompleteCallTreatment(volunteerId, AssignmentToComplete.Id);

    }
}
