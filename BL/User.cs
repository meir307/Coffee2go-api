
using Common.Tools;
using Dal;
using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace BL
{
    public class User 
    {

        private string _MobilePhoneNo;
        public int Id { get; set; }
        public string FullName { get; set; }

        public string Email { get; set; }
        //public string UserName { get; set; }
        public string Password { get; set; }
        public string MobilePhoneNo
        {
            get
            {
                return _MobilePhoneNo;
            }
            set
            {
                _MobilePhoneNo = Regex.Replace(value, @"[^\d]", "");
            }
        }
        public DateTime? RegistrationDate { get; set; }
        public string ActivationCode { get; private set; }
        public DateTime? LastLoginAt { get; set; }
        public string SessionId { get; set; }
        public DateTime SessionTime { get; set; }
        
        CRUD dal;
        GlobalData gd;
        public User()
        { 
        
        }

        public User(GlobalData gd)
        {
            this.gd = gd;
        }

        public User(GlobalData gd, string userName, string password)    //login
        {
            this.gd = gd;
            dal = new CRUD(gd.ConnectionString);
            DataTable dt = new DataTable();
            getUser(userName, password,ref dt);

            if (dt.Rows.Count == 0)
                throw new Exception("Wrong username or password");
            
            toObject(dt);
            login(dt);
        }

        public User(GlobalData gd, string sessionId)       //authentication 
        {
            this.gd = gd;
            dal = new CRUD(gd.ConnectionString);

            DataTable dt = new DataTable();
            getUser(sessionId, ref dt);

            if (dt.Rows.Count == 0)
                throw new Exception("SessionId is not valid.");

            if (CommonFunctions.sessionExpired(dt))
                throw new Exception("SessionId is not valid.");

            toObject(dt);
            updateSessionTime();
        }

        public void UpdateData(User newUser)
        {
            string msg = "";
            if (!dataValid(newUser, ref msg))
                throw new Exception(msg);

            if (UserExists(ref msg))
                throw new Exception(msg);

            StringBuilder sSql = new StringBuilder();
            sSql.Append("update users set");
            sSql.Append(" Email='" + newUser.Email + "',");
            sSql.Append(" FullName='" + newUser.FullName + "',");
            sSql.Append(" MobilePhoneNo='" + newUser.MobilePhoneNo + "'");
            sSql.Append(" where Id=" + this.Id);

            dal = new CRUD(this.gd.ConnectionString);
            dal.ExecuteNonQuery(sSql.ToString());
        }

        private bool dataValid(User newUser, ref string msg)
        {
            if (string.IsNullOrEmpty(newUser.Email))
            {
                msg = "Email is missing.";
                return false;
            }
            if (string.IsNullOrEmpty(newUser.FullName))
            {
                msg = "FullName is missing.";
                return false;
            }
            if (string.IsNullOrEmpty(newUser.MobilePhoneNo))
            {
                msg = "MobilePhoneNo is missing.";
                return false;
            }
            return true;
        }

        

        public void Register(GlobalData gd)
        {
            string msg = "";
            if (!dataValid(this, ref msg))
                throw new Exception(msg);

            dal = new CRUD(gd.ConnectionString);

            if (UserExists(ref msg))
                throw new Exception(msg);

            this.RegistrationDate = DateTime.Now;
            this.ActivationCode = Common.Tools.CommonFunctions.getRandomString(4);

            string smsMsg = "שלום " + this.FullName + ". קוד ההרשמה שלך הוא " + this.ActivationCode;

            string sSql = UserSqlProc.UserRegistration(this);
            dal.ExecuteNonQuery(sSql);
            
            SMSSender.SMSSender sms = new SMSSender.SMSSender(gd.GetParameterValueByKey("GlobalSMS_ApiKey"));
            sms.SendSMSAsync(gd.GetParameterValueByKey("GlobalSMS_FromPhone"), this.MobilePhoneNo, smsMsg);
            
            //MailSender ms = new MailSender();
            //ms.SendEmail("mmandeles@gmail.com", "meir", "mmandeles@gmail.com", "", "", "test1", "hello there", false);
        }

        public void ActivateAccount(string phoneNum, string activationCode)
        {
            CRUD dal = new CRUD(gd.ConnectionString);
            DataTable dt = new DataTable();
            StringBuilder sSql = new StringBuilder();

            phoneNum = Regex.Replace(phoneNum, @"[^\d]", "");

            sSql.Append("select * from users where ");
            sSql.Append("MobilePhoneNo = '" + phoneNum + "'");
            sSql.Append(" and ActivationCode = '" + activationCode + "'");
            sSql.Append(" and Active = 0");

            dal.ExecuteQuery(sSql.ToString(), ref dt);

            if (dt.Rows.Count == 0)
                throw new Exception("Wrong activation code");

            if (dt.Rows.Count == 1)
            {
                sSql = new StringBuilder();
                sSql.Append("update users set Active =1,ActivationCode=null where ");
                sSql.Append(" Id = " + dt.Rows[0]["Id"]);
                
                dal.ExecuteNonQuery(sSql.ToString());
            }

            if (dt.Rows.Count > 1)
                throw new Exception(dt.Rows.Count + " rows with the same activation code in the DB ---   ");

        }

        private bool UserExists(ref string msg)
        {
            StringBuilder sSql = new StringBuilder();
            DataTable dt= new DataTable();

            sSql.Append("select * from users where (Email='" + this.Email + "' or MobilePhoneNo='" + this.MobilePhoneNo + "') and Id <> " + this.Id);
            dal.ExecuteQuery(sSql.ToString(), ref dt);

            if (dt.Rows.Count == 0)
                return false;
                                  
            msg = "User with this Email or phone No already exists";
            return true;
        }

        private void updateSessionTime()
        {
            this.SessionTime = DateTime.Now;
            string sSql = UserSqlProc.UpdateSessionTime(this);
            dal.ExecuteNonQuery(sSql.ToString());
        }


        private void login(DataTable dt)
        {
            this.LastLoginAt = DateTime.Now;
            this.SessionId = Guid.NewGuid().ToString();
            this.SessionTime = DateTime.Now;

            string sSql = UserSqlProc.SetSessionAndSessionTime(this, dt);
            dal.ExecuteNonQuery(sSql.ToString());
        }

        private void getUser(string userName, string password, ref DataTable dt)
        {
            string sSql = UserSqlProc.GetUser(userName, password);
            dal.ExecuteQuery(sSql, ref dt);
        }

        private void getUser(string sessionId, ref DataTable dt)
        {
            string sSql = UserSqlProc.GetUser(sessionId);
            dal.ExecuteQuery(sSql, ref dt);
        }

        private void toObject(DataTable dt)
        {

            this.Id= (int)dt.Rows[0]["Id"];
            this.FullName = dt.Rows[0]["fullName"].ToString();
            this.Email = dt.Rows[0]["Email"].ToString();
            this.MobilePhoneNo = dt.Rows[0]["mobilePhoneNo"].ToString();
            this.LastLoginAt = CommonFunctions.ConvertToDateTime(dt.Rows[0]["lastLoginAt"]);
            this.RegistrationDate = CommonFunctions.ConvertToDateTime(dt.Rows[0]["registrationDate"]);
            this.SessionId = CommonFunctions.ConvertToGuid(dt.Rows[0]["sessionId"].ToString());
        }
    }
}
