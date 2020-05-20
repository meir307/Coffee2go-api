using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Common.Tools;
using Dal;
using static Common.Tools.CommonFunctions;

namespace BL
{
    public class Shop
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public float Lng { get; set; }
        public float Lat { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string MenuObj { get; set; }
        public string BuisnessName { get; set; }
        public string PhoneNo { get; set; }
        //public bool? isValid { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string SessionId { get; set; }
        public DateTime? SessionTime { get; set; }
        public string Logo { get; set; }

        public string OtherFields { get; set; }
        public bool NowOpen { get; set; }

        public string ConfirmReg { get; set; }
        
        CRUD dal;
        GlobalData gd;
        private string password;
       
        public Shop()
        {
            
        }

        public Shop(GlobalData gd, string userName, string password)    //login
        {
            dal = new CRUD(gd.ConnectionString);
            
            DataTable dt = new DataTable();
            getShop(userName, password, ref dt);

            if (dt.Rows.Count == 0)
                throw new Exception("Wrong username or password.");

            if ((int)dt.Rows[0]["Active"] == 0)
                throw new Exception("The account is not activated!");


            DataTable2Obj(dt);

            this.SessionId = Guid.NewGuid().ToString();
            login(dt);
            
        }

        public Shop(GlobalData gd, string sessionId)       //authentication 
        {
            dal = new CRUD(gd.ConnectionString);
            this.gd = gd;
            
            DataTable dt = new DataTable();

            getShop(sessionId, ref dt);

            if (dt.Rows.Count == 0)
                throw new Exception("SessionId is not valid.");

            if (CommonFunctions.sessionExpired(dt))
                throw new Exception("SessionId is not valid.");

            this.Id = (int)dt.Rows[0]["Id"];

            DataTable2Obj(dt);
            this.SessionId = sessionId;
            updateSessionTime();
        }

        public void UpdateData(Shop newShop)
        {
            //string msg = "";
            //if (!dataValid(newShop, ref msg))
            //    throw new Exception(msg);

            //if (ShopExists(ref msg))
            //    throw new Exception(msg);


            StringBuilder sSql = new StringBuilder();
            sSql.Append("update shops set");
            sSql.Append(" Email='" + newShop.Email +"',");
            //sSql.Append(" password='" + newShop.Password + "',");
            sSql.Append(" BuisnessName='" + newShop.BuisnessName + "',");
            sSql.Append(" Lat='" + newShop.Lat + "',");
            sSql.Append(" Lng='" + newShop.Lng + "',");
          //  sSql.Append(" MenuObj='" +newShop.MenuObj + "',");
            sSql.Append(" PhoneNo='" + newShop.PhoneNo + "',");
            sSql.Append(" Logo='" + newShop.Logo + "',");
            sSql.Append(" OtherFields='" + newShop.OtherFields + "'");
            sSql.Append(" where Id=" + this.Id);

            dal = new CRUD(this.gd.ConnectionString);
            dal.ExecuteNonQuery(sSql.ToString());
        }

        public void SendRegistrationEmail(string domainName, GlobalData gd)
        {
            int l = domainName.IndexOf("api/");
            domainName = domainName.Substring(0, l);

            MailSender ms = new MailSender(gd.GetParameterValueByKey("smtpServer"), int.Parse(gd.GetParameterValueByKey("smtpPort")), gd.GetParameterValueByKey("smtpEmail"),
                                            gd.GetParameterValueByKey("smtpPassword"), bool.Parse(gd.GetParameterValueByKey("smtpEnableSsl")));
            string to = this.Email;
            string subject = "WazeEat Registration";
            string body = getRegistrationMailBody(domainName);

            ms.SendEmail(this.Email, "", "", subject, body, true);
        }

        private string getRegistrationMailBody(string domainName)
        {
            string path = AppContext.BaseDirectory + "App_Data\\RegistrationMail.html";
            path=path.Replace("\\\\","\\");

            string body="";
            using (StreamReader reader = File.OpenText(path)) 
            {                                                       
                body = reader.ReadToEnd();
            }

            body = body.Replace("{{BuisnessName}}", this.BuisnessName);
            body = body.Replace("{{domainName}}", domainName);
            body = body.Replace("{{Email}}", this.Email);
            return body;
        }

        private bool ShopExists(CheckType ct, ref string msg)
        {
            StringBuilder sSql = new StringBuilder();
            string count = "";

            sSql.Append("select count(*) from shops where (Email='" + this.Email + "' or PhoneNo='" + this.PhoneNo + "')");
            if (ct==CheckType.Update)
                sSql.Append(" and Id <> " + this.Id);

            dal = new CRUD(gd.ConnectionString);
            dal.ExecuteScalar(sSql.ToString(), ref count);

            if (int.Parse(count) > 0)
            {
                msg = "Shop with this Email or phone No already exists";
                return true;
            }
            return false;
        }

        public void ActivateAccount(GlobalData gd, string email)
        {
            dal = new CRUD(gd.ConnectionString);
            string sSql = "update shops set Active = true where Email = '" + email + "'";
            dal.ExecuteNonQuery(sSql);
        }

        public void setOpen(int open)
        {
            if (open != 1 && open != 0) throw new Exception("illegal value for open parameter (can be 1 or 0)");

            dal = new CRUD(gd.ConnectionString);
            string sSql = "update shops set NowOpen = " + open + " where Id = " + this.Id;
            dal.ExecuteNonQuery(sSql);
        }

        public static List<Shop> GetShops(GlobalData GD, float lng, float lat, [Optional] string distance)
        {
            if (distance == null)
                distance = GD.GetParameterValueByKey("DistanceSearch");

            StringBuilder sSql = new StringBuilder();

            sSql.Append("select Id,BuisnessName, PhoneNo, lat,lng,MenuObj, NowOpen,Logo, (6371 * acos(cos(radians(" + lat + ")) * cos(radians(lat)) * cos(radians(lng)");
            sSql.Append("- radians(" + lng + ")) + sin(radians(" + lat + ")) * sin(radians(lat))))");
            sSql.Append("AS distance FROM shops where Active=1 and NowOpen=1 HAVING distance < " + distance);

            //MySQL select coordinates within range
            //This link could be useful: https://developers.google.com/maps/articles/phpsqlsearch_v3

            CRUD dal = new CRUD(GD.ConnectionString);
            DataTable dt = new DataTable();
            dal.ExecuteQuery(sSql.ToString(), ref dt);

            List<Shop> Shops = new List<Shop>();
            Shop shop;
            foreach (DataRow dr in dt.Rows)
            {
                shop = convertToShop(dr);
                Shops.Add(shop);
            }
            return Shops;
        }

        private static Shop convertToShop(DataRow dr)
        {
            Shop shop = new Shop();
            shop.Id = int.Parse(dr["Id"].ToString());
            shop.BuisnessName = dr["BuisnessName"].ToString();
            shop.Lat = float.Parse(dr["lat"].ToString());
            shop.Lng = float.Parse(dr["lng"].ToString());
            shop.MenuObj = dr["MenuObj"].ToString();
            shop.NowOpen = dr["NowOpen"].ToString() == "1";
            shop.PhoneNo = dr["PhoneNo"].ToString();
            shop.Logo = dr["Logo"].ToString();

            return shop;
        }
        private void updateSessionTime()
        {
            this.SessionTime = DateTime.Now;
            string SessionTime = this.SessionTime.HasValue ? this.SessionTime.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty;
            
            StringBuilder sSql = new StringBuilder();
            sSql.Append("update shops set SessionTime = '");
            sSql.Append(SessionTime + "' ");
            sSql.Append("where SessionId = ");
            //sSql.Append("unhex(\"" + this.SessionId + "\")");
            sSql.Append(("UNHEX(REPLACE(\"" + this.SessionId + "\", \"-\",\"\"))"));

            dal.ExecuteNonQuery(sSql.ToString());
        }

        private void getShop(string sessionId, ref DataTable dt)
        {
            StringBuilder sSql = new StringBuilder();
            sSql.Append("select Id,Email,Password,PhoneNo,Lat, Lng,BuisnessName,NowOpen, MenuObj, Logo, OtherFields, HEX(sessionid) sessionId from shops where SessionId=");
            sSql.Append("UNHEX(REPLACE(\"" + sessionId + "\", \"-\",\"\"))");
            sSql.Append(" and Active=true");
            dal.ExecuteQuery(sSql.ToString(), ref dt);
        }

        private void getShop(string userName, string password, ref DataTable dt)
        {
            StringBuilder sSql = new StringBuilder();
            sSql.Append("select Id,Email,Password,PhoneNo,Lat, Lng,BuisnessName,NowOpen,Active, MenuObj , Logo, OtherFields from shops where ");
            sSql.Append(" Email='" + userName + "' and");
            sSql.Append(" password='" + password + "'");
            
            dal.ExecuteQuery(sSql.ToString(), ref dt);
        }

        

        

        public void Register(GlobalData gd)
        {
            this.gd = gd;
            long lastInsertedId = 0;
            dal = new CRUD(gd.ConnectionString);
            this.RegistrationDate = DateTime.Now;
            string sSql = sqlShopRegistration();
            dal.ExecuteNonQuery(sSql,ref lastInsertedId);
            this.Id = (int)lastInsertedId;
        }

        private void DataTable2Obj(DataTable dt)
        {
            this.BuisnessName = dt.Rows[0]["BuisnessName"].ToString();
            this.LastLoginAt = DateTime.Parse(DateTime.Now.ToString());
            
            this.SessionTime = DateTime.Parse(DateTime.Now.ToString());
            this.PhoneNo = dt.Rows[0]["PhoneNo"].ToString();
            this.Lat = float.Parse(dt.Rows[0]["Lat"].ToString());
            this.Lng = float.Parse(dt.Rows[0]["Lng"].ToString());
            //this.RegistrationDate = DateTime.Parse(dt.Rows[0]["RegistrationDate"].ToString());
            this.MenuObj = dt.Rows[0]["MenuObj"].ToString();
            this.Email = dt.Rows[0]["Email"].ToString();
            this.NowOpen = (int)dt.Rows[0]["NowOpen"] != 0;
            this.Logo = dt.Rows[0]["Logo"].ToString();
            this.OtherFields = dt.Rows[0]["OtherFields"].ToString();

            this.password= dt.Rows[0]["Password"].ToString();
        }

       

        private void login(DataTable dt)
        {
            string SessionTime = this.SessionTime.HasValue ? this.SessionTime.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty;
            string LastLoginAt = this.LastLoginAt.HasValue ? this.LastLoginAt.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty;

            StringBuilder sSql = new StringBuilder();

            sSql.Append("update shops set SessionTime = '");
            sSql.Append(SessionTime + "' ");
            sSql.Append(", SessionId =");
            sSql.Append(("UNHEX(REPLACE(\"" + this.SessionId + "\", \"-\",\"\"))"));
            sSql.Append(", lastLoginAt ='");
            sSql.Append(LastLoginAt + "' ");
            sSql.Append("where Id = ");
            sSql.Append(dt.Rows[0]["Id"].ToString());
            //sSql.Append("unhex(\"" + dt.Rows[0]["Id"].ToString() + "\")");
           
            dal.ExecuteNonQuery(sSql.ToString());
        }

        public void ChangePassword(string oldPassword, string newPassword)
        {
            if (this.password != oldPassword)
                throw new Exception("Old password is wrong.");

            StringBuilder sSql = new StringBuilder();

            sSql.Append("update shops set Password = '" + newPassword +"'");
            sSql.Append(" where Id = ");
            sSql.Append(this.Id);

            dal.ExecuteNonQuery(sSql.ToString());
        }

        private string sqlShopRegistration()
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(this);

            string RegistrationDate = this.RegistrationDate.HasValue ? this.RegistrationDate.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty;

            //string msg = "";
            //if (!dataValid(this, ref msg))
            //    throw new Exception(msg);

            //if (ShopExists(ref msg))
            //    throw new Exception(msg);
            
            StringBuilder sSql = new StringBuilder();
            sSql.Append("insert into shops (Email,Password,PhoneNo,Lat, Lng,RegistrationDate,BuisnessName,Logo,Active, OtherFields) values (");

            //sSql.Append(("UNHEX(REPLACE(\"" + id.ToString() + "\", \"-\",\"\"))"));
                
            sSql.Append("'" + this.Email + "',");
            sSql.Append("'" + this.Password + "',");
            sSql.Append("'" + this.PhoneNo + "',");
            sSql.Append("'" + this.Lat + "',");
            sSql.Append("'" + this.Lng + "',");
            sSql.Append("'" + RegistrationDate + "',");
            sSql.Append("'" + this.BuisnessName + "',");
            sSql.Append("'" + this.Logo + "',");
            sSql.Append("'" + 0 + "',");
            sSql.Append("'" + this.OtherFields + "')");

            return sSql.ToString();

        }

        public void ValidateData(GlobalData gd, CheckType ct)
        {
            this.gd = gd;
            Validator.validatePhone(this.PhoneNo);
            Validator.validateEmail(this.Email);

            if (ct == CheckType.Register)
                Validator.validatePassword(this.Password);

            Validator.validateLocation(this.Lat, this.Lng);

            if (string.IsNullOrEmpty(this.BuisnessName))
                throw new Exception("BuisnessName is missing.");

            string msg="";
            if (ShopExists(ct, ref msg))
                throw new Exception(msg);
        }

        private bool dataValid(Shop newShop, ref string msg)
        {
            if (string.IsNullOrEmpty(newShop.Email))
            {
                msg = "Email is missing.";
                return false;
            }
            if (string.IsNullOrEmpty(newShop.BuisnessName))
            {
                msg = "BuisnessName is missing.";
                return false;
            }
            if (string.IsNullOrEmpty(newShop.Lat.ToString()))
            {
                msg = "Lat value is missing.";
                return false;
            }
            if (string.IsNullOrEmpty(newShop.Lng.ToString()))
            {
                msg = "Lng value is missing.";
                return false;
            }
            //if (string.IsNullOrEmpty(newShop.MenuObj))
            //{
            //    msg = "Menu is missing.";
            //    return false;
            //}
            if (string.IsNullOrEmpty(newShop.PhoneNo))
            {
                msg = "PhoneNo is missing.";
                return false;
            }
            return true;
        }
    }
}
