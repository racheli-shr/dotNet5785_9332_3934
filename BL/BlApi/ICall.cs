namespace BLApi;
using static BO.Enums;

/// <summary>
/// Interface for handling call-related operations.
/// </summary>
public interface ICall
{
    /// <summary>
    /// Retrieves an array of call counts by status.
    /// </summary>
    /// <returns>
    /// An array where each index represents a call status and the value represents the count of calls with that status.
    /// </returns>
    //int[] GetCallCountsByStatus();

    /// <summary>
    /// Retrieves a filtered and sorted list of calls.
    /// </summary>
    /// <param name="filterByField">The field to filter by (nullable).</param>
    /// <param name="filterValue">The value to filter by (nullable).</param>
    /// <param name="sortByField">The field to sort by (nullable).</param>
    /// <returns>
    /// A collection of filtered and sorted calls.
    /// </returns>
    //IEnumerable<BO.CallInList> GetFilteredAndCallList(CallInListFields? filterByField = null, object? filterValue = null, CallInListFields? sortByField = null);

    /// <summary>
    /// Retrieves the details of a specific call.
    /// </summary>
    /// <param name="callId">The unique identifier of the call.</param>
    /// <returns>
    /// A call object containing detailed information.
    /// </returns>
    //BO.Call Read(int callId);

    /// <summary>
    /// Updates an existing call.
    /// </summary>
    /// <param name="call">The call object containing updated information.</param>
    //void UpdateCall(BO.Call call);

    /// <summary>
    /// Deletes an existing call.
    /// </summary>
    /// <param name="call">The call object to be deleted.</param>
    //void DeleteCall(BO.Call call);

    /// <summary>
    /// Adds a new call to the system.
    /// </summary>
    /// <param name="call">The call object to be added.</param>
    //void AddCall(BO.Call call);

    /// <summary>
    /// Retrieves a list of closed calls handled by a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The volunteer's ID.</param>
    /// <param name="callType">The call type for filtering (nullable).</param>
    /// <param name="sortByField">The field to sort by (nullable).</param>
    /// <returns>
    /// A collection of closed calls handled by the specified volunteer.
    /// </returns>
    //IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, TypeHosting? callType, ClosedCallInListFields? sortByField);

    /// <summary>
    /// Retrieves a list of open calls available for volunteer selection.
    /// </summary>
    /// <param name="volunteerId">The volunteer's ID.</param>
    /// <param name="callType">The call type for filtering (nullable).</param>
    /// <param name="sortByField">The field to sort by (nullable).</param>
    /// <returns>
    /// A collection of open calls available for selection.
    /// </returns>
    //IEnumerable<BO.OpenCallInList> GetOpenCalls(int volunteerId, Enum? callType, Enum? sortByField);

    /// <summary>
    /// Completes the treatment of a call.
    /// </summary>
    /// <param name="volunteerId">The volunteer's ID.</param>
    /// <param name="assignmentId">The ID of the call assignment.</param>
    /// <exception cref="UnauthorizedAccessException">If the volunteer is not authorized to complete the call.</exception>
    /// <exception cref="InvalidOperationException">If the call is already completed.</exception>
    //void CompleteCallTreatment(int volunteerId, int assignmentId);

    /// <summary>
    /// Cancels the treatment of a call.
    /// </summary>
    /// <param name="requesterId">The ID of the requester (volunteer or manager).</param>
    /// <param name="assignmentId">The ID of the call assignment.</param>
    /// <exception cref="UnauthorizedAccessException">If the requester is not authorized to cancel.</exception>
    /// <exception cref="InvalidOperationException">If the call is already completed.</exception>
    //void CancelCallTreatment(int requesterId, int assignmentId);

    /// <summary>
    /// Assigns a volunteer to a specific call.
    /// </summary>
    /// <param name="volunteerId">The volunteer's ID.</param>
    /// <param name="callId">The ID of the call to be assigned.</param>
    /// <exception cref="InvalidOperationException">If the call is not available for assignment.</exception>
    //void AssignCallToVolunteer(int volunteerId, int callId);
    //BO.Call GetCallById(int callId);
}