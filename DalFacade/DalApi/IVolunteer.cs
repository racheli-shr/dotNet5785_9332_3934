


namespace DalApi;
using DO;

public interface IVolunteer: ICrud<Volunteer> 

{
    //void Create(Volunteer item); //Creates new entity object in DAL
    //Volunteer? Read(int id); //Reads entity object by its ID
    //List<Volunteer> ReadAll(); //stage 1 only, Reads all entity objects
    //void Update(Volunteer item); //Updates entity object
    //void Delete(int id); //Deletes an object by its Id
    //void DeleteAll(); //Delete all entity objects
    bool checkPassword(string password);//check the strong level of the password
    void updatePassword(int id,string password);//check the strong level of the password
    string EncryptPassword(string password);
    string DecryptPassword(string password);
    string GenerateStrongPassword();

}
