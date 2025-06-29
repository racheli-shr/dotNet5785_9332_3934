namespace BLApi;

using BO;
using static BO.Enums;

/// <summary>
/// Interface for handling call-related operations.
/// </summary>
public interface ICall:BlApi.IObservable
{
    /// <summary>
    /// Retrieves an array of call counts by status.
    /// </summary>
    IEnumerable<BO.StatusCounter> GetCallCountsByStatus();//TODO: chenge return value type

    /// <summary>
    /// Assigns a volunteer to a call.
    /// </summary>
    void AssignCallToVolunteer(int volunteerId, int callId);
    /// <summary>
    /// Assigns a volunteer to a call.
    /// </summary>
    void DeleteAssignmentToCall(String volunteerId, BO.CallInList call);

    /// <summary>
    /// Retrieves a filtered and sorted list of calls.
    /// </summary>
    public IEnumerable<BO.CallInList> GetFilteredAndCallList(BO.Enums.CallInListFields? filterByField = null, object? filterValue = null, BO.Enums.CallInListFields? sortByField = null);

    /// <summary>
    /// Retrieves the details of a specific call.
    /// </summary>
    BO.Call Read(int callId);

    /// <summary>
    /// Updates an existing call.
    /// </summary>
    void UpdateCall(BO.Call call);

    /// <summary>
    /// Deletes an existing call.
    /// </summary>
    void DeleteCall(int callId);

    /// <summary>
    /// Adds a new call to the system.
    /// </summary>
    void AddCall(BO.Call call);

    /// <summary>
    /// Retrieves closed calls handled by a specific volunteer.
    /// </summary>
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, Func<BO.ClosedCallInList, bool>? predicate = null);

    /// <summary>
    /// Completes the treatment of a call.
    /// </summary>
    void CompleteCallTreatment(int volunteerId, int assignmentId);

    /// <summary>
    /// Cancels the treatment of a call.
    /// </summary>
    void CancelCallTreatment(int requesterId, int assignmentId);
    ///
    public bool closeLastAssignmentByCallId(int callId);
    //public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, Func<BO.OpenCallInList, bool> predicate = null);
    //public IEnumerable<BO.OpenCallInList> GetFilteredAndOpenCalls(int volunteerId, BO.Enums.CallType? callType, BO.Enums.OpenCallInListFields? sortByField);
    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, Func<BO.OpenCallInList, bool> predicate = null);
    public IEnumerable<BO.OpenCallInList> FilterOpenCalls(int VolunteerId, BO.Enums.OpenCallInListFields? filterField = null, object? filterValue = null);
    public IEnumerable<BO.OpenCallInList> SortOpenCalls(int volunteerId, BO.Enums.OpenCallInListFields? sortField = BO.Enums.OpenCallInListFields.Id);

}
