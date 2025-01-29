
namespace BLImplementation;
using BLApi;
using Helpers;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using Helpers;


internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    //public void AddCall(BO.Call call)
    //{
    //    throw new NotImplementedException();
    //}

    //public void AssignCallToVolunteer(int volunteerId, int callId)
    //{
    //    throw new NotImplementedException();
    //}

    //public void CancelCallTreatment(int requesterId, int assignmentId)
    //{
    //    throw new NotImplementedException();
    //}

    //public void CompleteCallTreatment(int volunteerId, int assignmentId)
    //{
    //    throw new NotImplementedException();
    //}

    //public void DeleteCall(int callId)
    //{
    //    try
    //    {
    //        var DOcall = _dal.Call.Read(callId);
    //        var BOcall = Read(DOcall.Id);

    //        if (BOcall == null)
    //        {
    //            throw new BO.Exceptions.BLDoesNotExistException($"Call with ID {callId} does not exist.");
    //        }

    //        // בדיקת סטטוס הקריאה
    //        if (BOcall.CallStatus != BO.Enums.CallStatus.Open)
    //        {
    //            throw new BO.Exceptions.BLInvalidDataException("Only calls with status 'Open' can be deleted.");
    //        }
    //        // בדיקת הקצאות
    //        var assignments = _dal.Assignment.ReadAll(assignment => assignment.CallId == callId);
    //        if (assignments.Any())
    //        {
    //            throw new BO.Exceptions.BLInvalidDataException("Cannot delete the call because it has been assigned to volunteers.");
    //        }
    //        // מחיקה משכבת הנתונים
    //        _dal.Call.Delete(callId);
    //    }
    //    catch (BO.Exceptions.BLDoesNotExistException ex)
    //    {
    //        // לוג והתאמת חריגה
    //        //Console.WriteLine($"DAL error: {ex.Message}");
    //        throw new BO.Exceptions.BLDoesNotExistException($"The call with ID {callId} does not exist in the data layer.");
    //    }
    //    catch (BO.Exceptions.BLInvalidDataException ex)
    //    {
    //        // לוג עבור שגיאה ידועה
    //        Console.WriteLine($"Validation error: {ex.Message}");
    //        throw;
    //    }
    //    catch (Exception ex)
    //    {
    //        // טיפול כללי לשגיאות לא צפויות
    //        Console.WriteLine($"Unexpected error: {ex.Message}");
    //        throw new BO.Exceptions.BLGeneralException("An unexpected error occurred while attempting to delete the call.", ex);
    //    }
    //}

    //public int[] GetCallCountsByStatus()
    //{
    //    IEnumerable<DO.Call> dalCalls = _dal.Call.ReadAll();
    //    IEnumerable<BO.Call> blCalls = dalCalls.Select(call => Read(call.Id));
    //    int maxStatusIndex = Enum.GetValues(typeof(BO.Enums.CallStatus)).Cast<int>().Max();

    //    // Use LINQ to group calls by status and construct the array directly.
    //    return Enumerable.Range(0, maxStatusIndex + 1)
    //        .Select(index => blCalls.Count(call => (int)call.CallStatus == index)) // Count calls for each status.
    //        .ToArray(); // Convert the result to an array.


    //}

    //public BO.Call Read(int callId)
    //{
    //    try
    //    {

    //        var DOCall = _dal.Call.Read(c => c.Id == callId);
    //        var DOAssignment = _dal.Assignment.ReadAll(c => c.CallId == callId);
    //        var callStatus = CallManager.GetCallStatus(callId);
    //        List<BO.CallAssignInList>? Assignments = null;
    //        if (DOAssignment != null)
    //        {
    //            Assignments = DOAssignment.Select(assignment =>
    //            {
    //                var volunteerId = assignment.VolunteerId;
    //                var DOVolunteers = _dal.Volunteer.Read(v => v.Id == volunteerId);
    //                var volunteerName = DOVolunteers.fullName;
    //                return new BO.CallAssignInList()
    //                {
    //                    VolunteerId = volunteerId,
    //                    VolunteerName = volunteerName,
    //                    AssignmentStartTime = assignment.entryTime,
    //                    AssignmentEndTime = assignment.actualEndTime,
    //                    CompletionType = (BO.Enums.AssignmentStatus)assignment.AssignmentStatus
    //                };

    //            }).ToList();
    //        }
    //        return new BO.Call
    //        {
    //            Id = callId,
    //            CallType = (BO.Enums.TypeHosting)DOCall.callType,
    //            Description = DOCall.Description,
    //            Latitude = DOCall.Latitude,
    //            Longitude = DOCall.Longitude,
    //            OpenDate = DOCall.openDate,
    //            MaxTimeFinish = DOCall.maxTimeFinish,
    //            CallStatus = callStatus,
    //            Assignments = Assignments
    //        };
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new BO.Exceptions.BLDoesNotExistException($"Error fetching Call or Assignments: {ex.Message}");
    //    }
    //}

    //public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.Enums.TypeHosting? callType, BO.Enums.ClosedCallInListFields? sortByField)
    //{
    //    // Step 1: Fetch all assignments for the specific volunteer
    //    var volunteerAssignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);

    //    // Step 2: Create a dictionary of assignments mapped by CallId for direct access
    //    var assignmentDictionary = volunteerAssignments.ToDictionary(a => a.CallId);

    //    // Step 3: Fetch closed calls related to the volunteer's assignments
    //    var closedCalls = volunteerAssignments
    //        .Select(a => Read(a.CallId)) // For each assignment, fetch the corresponding call
    //        .Where(c => c.CallStatus == BO.Enums.CallStatus.Closed); // Filter by closed status

    //    // Step 4: Create the list of ClosedCallInList objects with the necessary data
    //    var closedCallInList = closedCalls.Select(c =>
    //    {
    //        // Get the assignment matching the current call
    //        var assignment = assignmentDictionary[c.Id];

    //        // Create the ClosedCallInList object and populate its fields
    //        return new BO.ClosedCallInList
    //        {
    //            Id = c.Id,
    //            CallType = c.CallType,
    //            FullAddress = c.CallAddress,
    //            OpenTime = c.OpenDate,
    //            EntryTime = assignment.entryTime,  // Use the assignment data if available
    //            ActualEndTime = assignment.actualEndTime,  // Use the assignment data if available
    //            EndType = (BO.Enums.AssignmentStatus)assignment.AssignmentStatus  // Use the assignment data if available
    //        };
    //    });

    //    // Step 5: Filter by call type if provided
    //    if (callType.HasValue)
    //    {
    //        closedCallInList = closedCallInList.Where(c => c.CallType == callType.Value);
    //    }

    //    // Step 6: Sort the list by the provided field, if given
    //    switch (sortByField.Value)
    //    {
    //        case BO.Enums.ClosedCallInListFields.Id:
    //            closedCallInList = closedCallInList.OrderBy(c => c.Id); // Sort by call ID
    //            break;
    //        case BO.Enums.ClosedCallInListFields.ActualEndTime:
    //            closedCallInList = closedCallInList.OrderBy(c => c.ActualEndTime); // Sort by completion date
    //            break;
    //        case BO.Enums.ClosedCallInListFields.EndType:
    //            closedCallInList = closedCallInList.OrderBy(c => c.EndType); // Sort by status (EndType)
    //            break;
    //        default:
    //            closedCallInList = closedCallInList.OrderBy(c => c.Id); // Default: Sort by call number (ID)
    //            break;
    //    }

    //    return closedCallInList;
    //}


    //public IEnumerable<BO.CallInList> GetFilteredAndCallList(BO.Enums.CallInListFields? filterByField = null, object? filterValue = null, BO.Enums.CallInListFields? sortByField = null)
    //{

    //    var DOCalls = _dal.Call.ReadAll();
    //    var BOCall = DOCalls.Select(call => Read(call.Id));

    //    IEnumerable<BO.CallInList> callInLists = BOCall.Select(bocall =>
    //    {
    //        var assignments = _dal.Assignment.ReadAll(assignment => assignment.CallId == bocall.Id);
    //        var orderedAssignments = assignments.OrderByDescending(assignment => assignment.entryTime);
    //        DateTime currentTime = ClockManager.Now;
    //        var volunteerId = orderedAssignments.FirstOrDefault().Id;
    //        var volunteerById = _dal.Volunteer.Read(v => v.Id == volunteerId);
    //        TimeSpan? TotalHandlingTime = null;
    //        int totalAssignments = orderedAssignments.Count();
    //        if (bocall.CallStatus == BO.Enums.CallStatus.Closed)
    //            TotalHandlingTime = orderedAssignments.FirstOrDefault().actualEndTime - bocall.OpenDate;

    //        return new BO.CallInList
    //        {
    //            Id = orderedAssignments.FirstOrDefault().Id,
    //            CallId = bocall.Id,
    //            CallType = bocall.CallType,
    //            OpeningTime = bocall.OpenDate,
    //            RemainingTime = bocall.MaxTimeFinish - currentTime,
    //            LastVolunteerName = volunteerById.fullName,
    //            TotalHandlingTime = TotalHandlingTime,
    //            CallStatus = bocall.CallStatus,
    //            TotalAssignments = totalAssignments
    //        };
    //    });



    //    switch (filterByField)
    //    {
    //        case BO.Enums.CallInListFields.Id:
    //            callInLists = callInLists.Where(callInList => callInList.Id == (int)filterValue);
    //            break;
    //        case BO.Enums.CallInListFields.CallId:
    //            callInLists = callInLists.Where(call => call.CallId == (int)filterValue);
    //            break;
    //        case BO.Enums.CallInListFields.CallType:
    //            callInLists = callInLists.Where(call => call.CallType == (BO.Enums.TypeHosting)filterValue);
    //            break;
    //        case BO.Enums.CallInListFields.OpeningTime:
    //            callInLists = callInLists.Where(call => call.OpeningTime == (DateTime)filterValue);
    //            break;
    //        case BO.Enums.CallInListFields.RemainingTime:
    //            callInLists = callInLists.Where(call => call.RemainingTime == (TimeSpan?)filterValue);
    //            break;
    //        case BO.Enums.CallInListFields.LastVolunteerName:
    //            callInLists = callInLists.Where(call => call.LastVolunteerName.Contains((string)filterValue));
    //            break;
    //        case BO.Enums.CallInListFields.TotalHandlingTime:
    //            callInLists = callInLists.Where(call => call.TotalHandlingTime == (TimeSpan?)filterValue);
    //            break;
    //        case BO.Enums.CallInListFields.CallStatus:
    //            callInLists = callInLists.Where(call => call.CallStatus == (BO.Enums.CallStatus)filterValue);
    //            break;
    //        case BO.Enums.CallInListFields.TotalAssignments:
    //            callInLists = callInLists.Where(call => call.TotalAssignments == (int)filterValue);
    //            break;
    //        default:
    //            callInLists = callInLists;
    //            break;
    //    }


    //    switch (sortByField)
    //    {
    //        case BO.Enums.CallInListFields.Id:
    //            callInLists = callInLists.OrderBy(call => call.Id);
    //            break;
    //        case BO.Enums.CallInListFields.CallId:
    //            callInLists = callInLists.OrderBy(call => call.CallId);
    //            break;
    //        case BO.Enums.CallInListFields.CallType:
    //            callInLists = callInLists.OrderBy(call => call.CallType);
    //            break;
    //        case BO.Enums.CallInListFields.OpeningTime:
    //            callInLists = callInLists.OrderBy(call => call.OpeningTime);
    //            break;
    //        case BO.Enums.CallInListFields.RemainingTime:
    //            callInLists = callInLists.OrderBy(call => call.RemainingTime);
    //            break;
    //        case BO.Enums.CallInListFields.LastVolunteerName:
    //            callInLists = callInLists.OrderBy(call => call.LastVolunteerName);
    //            break;
    //        case BO.Enums.CallInListFields.TotalHandlingTime:
    //            callInLists = callInLists.OrderBy(call => call.TotalHandlingTime);
    //            break;
    //        case BO.Enums.CallInListFields.CallStatus:
    //            callInLists = callInLists.OrderBy(call => call.CallStatus);
    //            break;
    //        case BO.Enums.CallInListFields.TotalAssignments:
    //            callInLists = callInLists.OrderBy(call => call.TotalAssignments);
    //            break;
    //        default:
    //            callInLists = callInLists.OrderBy(call => call.Id);
    //            break;
    //    }


    //    //// עבור כל קריאה, מבצע סינון כך שכל קריאה תופיע רק פעם אחת עם ההקצאה האחרונה שלה
    //    //var filteredCalls = calls
    //    //    .GroupBy(call => call.Id)
    //    //    .Select(group => group.OrderByDescending(call => call.OpenDate).First()) // ממיין לפי תאריך פתיחה ומחזיר את הקריאה האחרונה
    //    //    .ToList();

    //    return callInLists;
    //}

    //public IEnumerable<BO.OpenCallInList> GetOpenCalls(int volunteerId, BO.Enums.TypeHosting? callType, Enum? sortByField)
    //{
    //    throw new NotImplementedException();
    //}

    //public void UpdateCall(BO.Call call)
    //{
    //    try
    //    {
    //        if (call == null)
    //            throw new BO.Exceptions.BLInvalidDataException($"cannot update call with null object");
    //        // Check authorization


    //        // Validate and retrieve coordinates
    //        var (latitude, longitude) = Tools.GetCoordinates(call.CallAddress);
    //        call.Latitude = latitude;
    //        call.Longitude = longitude;
    //        CallManager.Validation(call);

    //        // Update originalVolunteer with allowed changes
    //        DO.Call updatedCall = CallManager.ConvertBOToDO(call);

    //        // Save updated volunteer to DAL
    //        _dal.Call.Update(updatedCall);

    //    }

    //    catch (BO.Exceptions.BLInvalidDataException ex)
    //    {
    //        throw new BO.Exceptions.BLInvalidDataException($"An error occurred while updating Volunteer : {ex.Message}");
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new BO.Exceptions.BLGeneralException($"An error occurred while updating Volunteer : {ex.Message}");
    //    }
    //}
}