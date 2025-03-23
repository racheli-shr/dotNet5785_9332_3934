namespace BLApi;

using BO;
using static BO.Enums;

/// <summary>
/// Interface for handling call-related operations.
/// </summary>
public interface ICall
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
    /// Retrieves a filtered and sorted list of calls.
    /// </summary>
    IEnumerable<BO.CallInList> GetFilteredAndCallList(BO.Enums.CallInListFields? filterByField = null, object? filterValue = null, BO.Enums.CallInListFields? sortByField = null);

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
    IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.Enums.CallType? callType, BO.Enums.ClosedCallInListFields? sortByField);

    /// <summary>
    /// Completes the treatment of a call.
    /// </summary>
    void CompleteCallTreatment(int volunteerId, int assignmentId);

    /// <summary>
    /// Cancels the treatment of a call.
    /// </summary>
    void CancelCallTreatment(int requesterId, int assignmentId);
}
