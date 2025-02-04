
//}
namespace Dal
{
    using DalApi;
    using DO;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using System.Xml.Linq;

    internal class VolunteerImplementation : IVolunteer
    {
        public bool checkPassword(string password)//checking password strongth
        {
            if (string.IsNullOrEmpty(password) || password.Length != 8 )
                return false;
            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));
            return hasUpper && hasLower && hasDigit && hasSpecial;
        }
        public void Create(Volunteer item)//create
        {
            if (Read(a => a.Id == item.Id) != null)
            {
                throw new DO.Exceptions.DalAlreadyExistsException("Volunteer id's doesn't exsisting");
            }
            DataSource.Volunteers.Add(item);
        }

        public void Delete(int id)//delete
        {
            Volunteer v = Read(a => a.Id == id);
            if (v != null)
            {
                DataSource.Volunteers.Remove(v);
            }
            else
            {
                throw new DO.Exceptions.DalDoesNotExistException("Volunteer id's doesn't exsisting");
            }
        }

        public void DeleteAll()//delete All volunteers
        {
            DataSource.Volunteers.Clear();
        }
        public Volunteer? Read(Func<Volunteer, bool> filter)
        
        {
            Volunteer? v = DataSource.Volunteers.FirstOrDefault(filter);

            if (v != null)
            {
                // פענוח הסיסמה ועדכון האובייקט
                v = v with { Password = DecryptPassword(v.Password) };
            }

            return v; // מחזיר את האובייקט (או null אם לא נמצא)
        }

        public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
        {
            return filter != null
                ? from item in DataSource.Volunteers
                  where filter(item)
                  select item
                : from item in DataSource.Volunteers
                  select item;
        }

        public void Update(Volunteer item)//update an volunteer
        {
            Volunteer v = Read(a => a.Id == item.Id);
            if (v == null)
            {
                throw new DO.Exceptions.DalDoesNotExistException("Volunteer id's doesn't exsisting");
            }
            else
            {
                Delete(item.Id);
                DataSource.Volunteers.Add(item);
            }
        }
        public void updatePassword(int id,string password)//update the Password
        {
            Volunteer v=DataSource.Volunteers.Find(volunteer => volunteer.Id == id);
            v = v with { Password = password };
            Delete(id);
            DataSource.Volunteers.Add(v);
        }

        public  string EncryptPassword(string password)//Encrypt the Password
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty.");

            StringBuilder encryptedPassword = new StringBuilder();

            foreach (char ch in password)
            {
                encryptedPassword.Append((char)(ch + 2));
            }

            return encryptedPassword.ToString();
        }

        // פונקציה לפענוח
        public  string DecryptPassword(string encryptedPassword)
        {
            if (string.IsNullOrEmpty(encryptedPassword))
                throw new ArgumentException("Encrypted password cannot be null or empty.");

            StringBuilder decryptedPassword = new StringBuilder();

            foreach (char ch in encryptedPassword)
            {
                decryptedPassword.Append((char)(ch - 2));
            }

            return decryptedPassword.ToString();
        }
        public string GenerateStrongPassword()//ganarate a password for a new person
        {
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specialChars = "@$!%*?&";

            Random random = new Random();

            char upper = upperChars[random.Next(upperChars.Length)];
            char lower = lowerChars[random.Next(lowerChars.Length)];
            char digit = digits[random.Next(digits.Length)];
            char special = specialChars[random.Next(specialChars.Length)];

            string allChars = upperChars + lowerChars + digits + specialChars;
            string randomChars = new string(Enumerable.Range(0, 4)
                .Select(_ => allChars[random.Next(allChars.Length)])
                .ToArray());

            string password = upper.ToString() + lower + digit + special + randomChars;

            return new string(password.OrderBy(_ => random.Next()).ToArray());
        }
    }
}

