namespace BlImplementation;
using BlApi;
internal class Bl : IBl
{
    public ICall Call { get; } = new CallImplementation();
    
    public IAdmin Admin { get; } = new AdminImplementation();

}