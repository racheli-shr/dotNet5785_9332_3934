using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.BO;
using BO;
using DO;
namespace BlApi;

public interface ICall
{
    // Returns call counts by status
    Task<int[]> RequestCallCountsByStatusAsync();

    // Returns filtered and sorted list of calls
    internal Task<List<BO.CallInList>> RequestCallsAsync(Enum? filterField = null, object? filterValue = null, Enum? sortField = null);

    // Returns detailed info for a specific call
    internal Task<BL.BO.Call> RequestCallDetailsAsync(int callId);

    // Updates call details in data layer
    internal Task UpdateCallDetailsAsync(BL.BO.Call call);

    // Deletes a call if conditions are met
    Task DeleteCallAsync(int callId);

    // Adds a new call to the system
    internal Task AddCallAsync(BL.BO.Call call);

    // Returns closed calls handled by a specific volunteer
    internal Task<List<BL.BO.ClosedCallInList>> RequestClosedCallsByVolunteerAsync(int volunteerId, Enum? callTypeFilter = null, Enum? sortField = null);

    // Returns open calls for a volunteer to choose from
    internal Task<List<OpenCallInList>> RequestOpenCallsForVolunteerAsync(int volunteerId, Enum? callTypeFilter = null, Enum? sortField = null);

    // Marks a call as completed
    Task MarkCallAsCompletedAsync(int volunteerId, int assignmentId);

    // Cancels a call treatment by a volunteer
    Task CancelCallTreatmentAsync(int volunteerId, int assignmentId);

    // Assigns a call to a volunteer
    Task AssignCallToVolunteerAsync(int volunteerId, int callId);


}
