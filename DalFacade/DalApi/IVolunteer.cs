


namespace DalApi;
using DO;

public interface IVolunteer: ICrud<Volunteer> 

{
    
    bool checkPassword(string password);//check the strong level of the password
    void updatePassword(int id,string password);//check the strong level of the password
    string EncryptPassword(string password);
    string DecryptPassword(string password);
    string GenerateStrongPassword();

}
