namespace BlImplementation;
using BlApi;
using BLApi;
using BLImplementation;

internal class Bl : IBl
{
    public ICall Call { get; } = new CallImplementation();
    public IAdmin Admin { get; } = new AdminImplementation();
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

}