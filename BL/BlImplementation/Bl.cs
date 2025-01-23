namespace BlImplementation;
using BlApi;
internal class Bl : IBl
{
    public ICall Call { get; } = new CallImplementation();
    public IVolunteer { get; } = new VolunteerImplementation();
    public IAdmin Admin { get; } = new AdminImplementation();

}