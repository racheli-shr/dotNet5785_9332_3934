using BL.Helpers;
using BLApi;
using BO;
using DO;
using Helpers;
using static DO.Exceptions;
namespace BLImplementation;
internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Adds a new volunteer to the system.
    /// </summary>
    
    public void AddVolunteer(BO.Volunteer newVolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        // כל הבדיקות הלוגיות מחוץ ל-lock
        if (!VolunteerManager.IsValidIsraeliID(newVolunteer.Id.ToString()))
            throw new BO.Exceptions.BLInvalidDataException("Invalid ID");

        if (!VolunteerManager.IsValidName(newVolunteer.FullName))
            throw new BO.Exceptions.BLInvalidDataException("Name in invalid format");

        if (!VolunteerManager.IsValidEmail(newVolunteer.Email))
            throw new BO.Exceptions.BLInvalidDataException("Email in invalid format");

        if (!VolunteerManager.IsValidPhoneNumber(newVolunteer.Phone))
            throw new BO.Exceptions.BLInvalidDataException("Invalid phone number");

        // יצירת סיסמה והצפנתה מחוץ ל-lock
        string generatedPassword = _dal.Volunteer.GenerateStrongPassword();
        string encryptedPassword = _dal.Volunteer.EncryptPassword(generatedPassword);

        // חישוב קואורדינטות מחוץ ל-lock

        bool didCreate = false;

        // נעילה רק על הפניות ל-DAL
        lock (AdminManager.BlMutex)
        {
            var existingVolunteer = _dal.Volunteer.Read(newVolunteer.Id);
            if (existingVolunteer != null)
                throw new BO.Exceptions.BlAlreadyExistsException($"Volunteer with ID={newVolunteer.Id} already exists.");
            try
            {
                DO.Volunteer volunteer = new(
                Id: newVolunteer.Id,
                FullName: newVolunteer.FullName,
                Phone: newVolunteer.Phone,
                Email: newVolunteer.Email,
                Role: (DO.Enums.Role)newVolunteer.Role,
                IsActive: newVolunteer.IsActive,
                DistanceType: (DO.Enums.DistanceType)newVolunteer.DistanceType,
                Password: encryptedPassword,
                FullAdress: newVolunteer.FullAddress,
                Latitude: null,
                longtitude: null,
                MaxDistance: newVolunteer.MaxDistance
            );

                _dal.Volunteer.Create(volunteer);
                didCreate = true;
            }
            catch (DalAlreadyExistsException ex)
            {
                throw new BO.Exceptions.BlAlreadyExistsException($"Error creating the volunteer with ID {newVolunteer.Id}: {ex.Message}");
            }


        }

        // notify מחוץ לנעילה, רק אם נוצר מתנדב חדש
        if (didCreate)
        {
            VolunteerManager.Observers.NotifyListUpdated();
        }

        _ = VolunteerManager.updateCoordinatesForVolunteerAddressAsync(newVolunteer.Id, newVolunteer.FullAddress!);


        Console.WriteLine($"Generated password for {newVolunteer.FullName}: {generatedPassword}");
    }


    /// <summary>
    /// Deletes a volunteer by ID if allowed.
    /// </summary>
    public void DeleteVolunteer(int volunteerId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        try
        {
            DO.Volunteer volunteer;
            lock (AdminManager.BlMutex) //stage 7
                volunteer = _dal.Volunteer.Read(v => v.Id == volunteerId);
            if (volunteer == null)
            {
                throw new BO.Exceptions.BlDoesNotExistException("The volunteer with the provided ID does not exist.");
            }

            IEnumerable<DO.Assignment> assignments;
            lock (AdminManager.BlMutex) //stage 7
                assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);
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
            lock (AdminManager.BlMutex) //stage 7
                _dal.Volunteer.Delete(volunteerId);
            VolunteerManager.Observers.NotifyItemUpdated(volunteerId);  //stage 5
            VolunteerManager.Observers.NotifyListUpdated();
        }
        catch (Exception ex)
        {
            throw new BO.Exceptions.BLGeneralException($"An error occurred while trying to delete the volunteer.{ex.Message}", ex);
        }
    }

    /// <summary>
    /// Retrieves a volunteer's details by ID.
    /// </summary>
    public BO.Volunteer Read(int volunteerId)
    {
        try
        {
            DO.Volunteer doVolunteer;
            lock (AdminManager.BlMutex) //stage 7
                doVolunteer = _dal.Volunteer.Read(v => v.Id == volunteerId)
                ?? throw new BO.Exceptions.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist");
            IEnumerable <DO.Assignment> TreatedAssignments;
            IEnumerable <DO.Assignment> SelfCanceledAssignments;
            IEnumerable <DO.Assignment> ExpiredAssignments;
            DO.Assignment assignedCall;
            BO.CallInProgress callInProgress;
            DO.Call calllAssignment;

            lock (AdminManager.BlMutex) //stage 7
            {
                TreatedAssignments = _dal.Assignment.ReadAll(a => a.VolunteerId == doVolunteer.Id && a.AssignmentStatus == DO.Enums.AssignmentStatus.TREATED);
                SelfCanceledAssignments = _dal.Assignment.ReadAll(a => a.VolunteerId == doVolunteer.Id && a.AssignmentStatus == DO.Enums.AssignmentStatus.SELF_CANCELLED);
                ExpiredAssignments = _dal.Assignment.ReadAll(a => a.VolunteerId == doVolunteer.Id && a.AssignmentStatus == DO.Enums.AssignmentStatus.EXPIRED);
                assignedCall= _dal.Assignment.Read(a => a.VolunteerId == doVolunteer.Id && a.AssignmentStatus == DO.Enums.AssignmentStatus.AssignedAndInProgress);
                if(assignedCall!=null) { 
                    calllAssignment= _dal.Call.Read(c => c.Id == assignedCall.CallId);
                    callInProgress = new CallInProgress
                    {
                        Id = assignedCall.Id,
                        CallId = assignedCall.CallId,
                        CallType = (BO.Enums.CallType)calllAssignment.CallType,
                        Description = calllAssignment.Description,
                        Address = calllAssignment.FullAdress,
                        OpeningTime = calllAssignment.OpeningCallTime,
                        MaxFinishTime = calllAssignment.MaxTimeToEnd,
                        EntryTimeToHandle = assignedCall.EntryTimeForTreatment,
                        DistanceFromVolunteer = VolunteerManager.GetDistance(doVolunteer.Latitude, doVolunteer.longtitude, calllAssignment.Latitude, calllAssignment.longtitude, doVolunteer.DistanceType),
                        Status = CallManager.GetCallStatus(calllAssignment.MaxTimeToEnd, calllAssignment.Id)
                    };
                }
                else
                {
                    callInProgress = null;
                }
            }

            
            return new BO.Volunteer
            {
                Id = volunteerId,
                FullName = doVolunteer.FullName,
                Phone = doVolunteer.Phone,
                Email = doVolunteer.Email,
                FullAddress = doVolunteer.FullAdress,
                Latitude = doVolunteer.Latitude,
                longtitude = doVolunteer.longtitude,
                Role = (BO.Enums.Role)doVolunteer.Role,
                IsActive = doVolunteer.IsActive,
                MaxDistance = doVolunteer.MaxDistance,
                DistanceType = (BO.Enums.DistanceType)doVolunteer.DistanceType,
                NumberOfCalls = TreatedAssignments.Count(),
                NumberOfCanceledCalls=SelfCanceledAssignments.Count(),
                NumberOfexpiredCalls=ExpiredAssignments.Count(),
                CurrentCallInProgress=callInProgress
                
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
                lock (AdminManager.BlMutex) //stage 7
                    DOvolunteers = _dal.Volunteer.ReadAll()
                    ?? throw new BO.Exceptions.BlDoesNotExistException("No volunteers exist in the system.");
            }
            else
            {
                // Retrieve volunteers based on their availability status
                lock (AdminManager.BlMutex) //stage 7
                DOvolunteers = _dal.Volunteer.ReadAll(v => v.IsActive == isActive)
                    ?? throw new BO.Exceptions.BlDoesNotExistException("No volunteers match the specified availability.");
            }


            IEnumerable<BO.VolunteerInList> volunteersInList = DOvolunteers.Select(v =>
            {
                BO.Volunteer BOvolunteer;
                lock (AdminManager.BlMutex) // stage 7
                {
                    BOvolunteer = Read(v.Id);
                }

                return new BO.VolunteerInList
                {
                    Id = BOvolunteer.Id,
                    FullName = BOvolunteer.FullName,
                    IsActive = BOvolunteer.IsActive,
                    SumTreatedCalls = BOvolunteer.NumberOfCalls,
                    SumCallsSelfCancelled = BOvolunteer.NumberOfCanceledCalls,
                    SumExpiredCalls = BOvolunteer.NumberOfexpiredCalls,
                    CallId = BOvolunteer.CurrentCallInProgress?.CallId,
                    CallType = BOvolunteer.CurrentCallInProgress?.CallType ?? BO.Enums.CallType.NONE
                };
            });

            // שלב 3: מיון בהתאם לפרמטר
            volunteersInList = sortField switch
            {
                BO.Enums.VolunteerSortField.ID => volunteersInList.OrderBy(v => v.Id),
                BO.Enums.VolunteerSortField.FULL_NAME => volunteersInList.OrderBy(v => v.FullName),
                BO.Enums.VolunteerSortField.IS_AVAILABLE => volunteersInList.OrderBy(v => v.IsActive),
                BO.Enums.VolunteerSortField.SUM_TREATED_CALLS => volunteersInList.OrderBy(v => v.SumTreatedCalls),
                BO.Enums.VolunteerSortField.SUM_CALLS_SELF_CANCELLED => volunteersInList.OrderBy(v => v.SumCallsSelfCancelled),
                BO.Enums.VolunteerSortField.SUM_EXPIRED_CALLS => volunteersInList.OrderBy(v => v.SumExpiredCalls),
                BO.Enums.VolunteerSortField.CALL_ID => volunteersInList.OrderBy(v => v.CallId),
                BO.Enums.VolunteerSortField.CALL_TYPE => volunteersInList.OrderBy(v => v.CallType),
                _ => volunteersInList.OrderBy(v => v.Id)
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
    /// Authenticates a volunteer using his id and password.
    /// </summary>
    public BO.Enums.Role Login(int id, string password)
    {

        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex) //stage 7
            volunteer = _dal.Volunteer.Read(v => v.Id == id && v.Password == _dal.Volunteer.EncryptPassword(password));
        if (volunteer == null)
        {
            throw new BO.Exceptions.BlDoesNotExistException("Incorrect userId or password");
        }
        return (BO.Enums.Role)volunteer.Role;
    }

    /// <summary>
    /// Updates the details of an existing volunteer.
    /// </summary>
    
    public void UpdateVolunteerDetails(int volunteerId, BO.Volunteer updatedVolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        DO.Volunteer dovolunteer;

        lock (AdminManager.BlMutex)
        {
            try
            {
                dovolunteer = _dal.Volunteer.Read(updatedVolunteer.Id)
                    ?? throw new BO.Exceptions.BlDoesNotExistException("המתנדב עם תעודת זהות זו לא נמצא במערכת.");
            }
            catch (Exception ex)
            {
                throw new BO.Exceptions.BLGeneralException("שגיאה בקריאת המתנדב ממאגר הנתונים: " + ex.Message);
            }
        }

        if (volunteerId != updatedVolunteer.Id)
            throw new BO.Exceptions.BLGeneralException("לא ניתן לעדכן פרטי מתנדב אחר.");

        if (!VolunteerManager.IsValidIsraeliID(updatedVolunteer.Id.ToString()))
            throw new BO.Exceptions.BLInvalidDataException("תעודת זהות לא תקינה.");

        if (!VolunteerManager.IsValidName(updatedVolunteer.FullName))
            throw new BO.Exceptions.BLInvalidDataException("שם לא בפורמט תקין");

        if (!VolunteerManager.IsValidEmail(updatedVolunteer.Email))
            throw new BO.Exceptions.BLInvalidDataException("כתובת אימייל לא תקינה.");

        if (!VolunteerManager.IsValidPhoneNumber(updatedVolunteer.Phone))
            throw new BO.Exceptions.BLInvalidDataException("מספר טלפון לא תקין.");

        bool notifyItem = false, notifyList = false, notifyCallList = false;

        lock (AdminManager.BlMutex)
        {
            try
            {
                bool hasChanges =
                    dovolunteer.FullName != updatedVolunteer.FullName ||
                    dovolunteer.Phone != updatedVolunteer.Phone ||
                    dovolunteer.Email != updatedVolunteer.Email ||
                    dovolunteer.Role != (DO.Enums.Role)updatedVolunteer.Role ||
                    dovolunteer.IsActive != updatedVolunteer.IsActive ||
                    dovolunteer.Password != updatedVolunteer.Password ||
                    dovolunteer.FullAdress != updatedVolunteer.FullAddress ||
                    dovolunteer.MaxDistance != updatedVolunteer.MaxDistance ||
                    dovolunteer.DistanceType != (DO.Enums.DistanceType)updatedVolunteer.DistanceType;

                if (!hasChanges)
                    return;

                dovolunteer = new DO.Volunteer
                (
                    Id: updatedVolunteer.Id,
                    FullName: updatedVolunteer.FullName,
                    Phone: updatedVolunteer.Phone,
                    Email: updatedVolunteer.Email,
                    Role: (DO.Enums.Role)updatedVolunteer.Role,
                    IsActive: updatedVolunteer.IsActive,
                    Password: _dal.Volunteer.EncryptPassword(updatedVolunteer.Password),
                    FullAdress: updatedVolunteer.FullAddress,
                    Latitude: null,         // לא מחשב כאן – מחשב אח"כ
                    longtitude: null,
                    MaxDistance: updatedVolunteer.MaxDistance,
                    DistanceType: (DO.Enums.DistanceType)updatedVolunteer.DistanceType
                );

                _dal.Volunteer.Update(dovolunteer);

                notifyItem = true;
                notifyList = true;
                notifyCallList = true;
            }
            catch (Exception ex)
            {
                throw new BO.Exceptions.BLGeneralException("שגיאה בעת עדכון המתנדב בשכבת הנתונים: " + ex.Message);
            }
        }

        // מזמן את המשימה ברקע מבלי להמתין לה:
        _ = VolunteerManager.updateCoordinatesForVolunteerAddressAsync(updatedVolunteer.Id, updatedVolunteer.FullAddress!);

        if (notifyItem)
            VolunteerManager.Observers.NotifyItemUpdated(dovolunteer.Id);
        if (notifyList)
            VolunteerManager.Observers.NotifyListUpdated();
        if (notifyCallList)
            CallManager.Observers.NotifyListUpdated();
    }

    public void AddObserver(Action listObserver)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    }
    public void AddObserver(int id, Action observer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    }
    public void RemoveObserver(Action listObserver)
    {
        VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    }
    public void RemoveObserver(int id, Action observer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
    }
    public BO.Call? checkIfExistingAssignment(BO.Volunteer v)
    {
        DO.Assignment assignment;
        lock (AdminManager.BlMutex) //stage 7
            assignment = _dal.Assignment.Read(a => a.VolunteerId == v.Id && a.AssignmentStatus == DO.Enums.AssignmentStatus.AssignedAndInProgress);

        if (assignment!=null)
        {
            DO.Call doCall;
            lock (AdminManager.BlMutex) //stage 7
                doCall = _dal.Call.Read(assignment.CallId);

            var boCall = new BO.Call
            {
                Id = doCall.Id,
                CallType = (BO.Enums.CallType)doCall.CallType,
                Description = doCall.Description,
                FullAddress = doCall.FullAdress,
                Latitude = doCall.Latitude,
                longtitude = doCall.longtitude,
                OpeningTime = doCall.OpeningCallTime,
                MaxFinishTime = doCall.MaxTimeToEnd
            };

            return boCall;
        }

        return null;
    }

    #region Stage 5


    #endregion Stage 5
}
