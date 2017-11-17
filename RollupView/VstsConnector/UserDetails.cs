using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VstsConnector.ER;
//using VstsConnector.AzureDB;

namespace VstsConnector
{
    public class UserDetails
    {
        RollUpEntities RollUpDB = new RollUpEntities();
        public string ErrorMessage = string.Empty;
        public User GetLoginDetails(string username, string Password)
        {
            User msgStatus = new User();

            try
            {
                List<User> userList = RollUpDB.Users.ToList();

                msgStatus = (from s in userList
                             where s.Username.ToLower() == username.ToLower() && Encoding.UTF8.GetString(s.Password) == Password
                             select s).FirstOrDefault();

            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return null;
            }
            return msgStatus;
        }
        public string AddUser(string userName,string email, string password)
        {
            string returnMsg = "";
            try
            {
                User objEmpDetail = new User();
                objEmpDetail.Username = userName;
                objEmpDetail.Email = email;
                objEmpDetail.Password = Encoding.UTF8.GetBytes(password);
                RollUpDB.Users.Add(objEmpDetail);
                RollUpDB.SaveChanges();
                returnMsg = "Success";
            }
            catch (Exception ex)
            {
                returnMsg = ex.Message;
            }
            return returnMsg;
        }
        public string GetUserExist(string sUserName, string sEmail, int iUserId = 0)
        {
            int eCount = 0;
            int uCount = 0;

            if (uCount == 0)
            {
                uCount = RollUpDB.Users.Where(x => x.Username.ToLower().Trim() == sUserName.ToLower().Trim()).Count();
            }
            if (eCount == 0)
            {
                eCount = RollUpDB.Users.Where(x => x.Email.ToLower().Trim() == sEmail.ToLower().Trim()).Count();
            }
           
            if (uCount > 0)
            {
                return "Username Found";
            }
            else if (eCount > 0)
            {
                return "Email Found";
            }
            else
            {
                return "Not Found";
            }
        }
    }
}
