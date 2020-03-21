using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal
{
    public class CRUD
    {

        MySql.Data.MySqlClient.MySqlTransaction transaction = null;
        MySql.Data.MySqlClient.MySqlConnection conn;
        string myConnectionString;
        public CRUD(string connStr)
        {
            myConnectionString = connStr;   //"server=127.0.0.1;uid=root;pwd=Aa123456;database=Coffee2Go2";
            conn = new MySql.Data.MySqlClient.MySqlConnection(myConnectionString);
            

        }

        public void BeginTransaction()
        {
            conn.Open();
            transaction = conn.BeginTransaction();
        }
        public void CommitTransaction()
        {
            transaction.Commit();
            transaction = null;
            conn.Close();
        }
        public void RollbackTransaction()
        {
            transaction.Rollback();
            transaction = null;
            conn.Close();
        }

        public int ExecuteNonQuery(string query)
        {
            MySqlCommand myCommand = new MySqlCommand(query, conn);
            if (transaction != null)
                myCommand.Transaction = transaction;
            else
                conn.Open();

            int numberOfRecords = myCommand.ExecuteNonQuery();

            if (transaction == null)
                conn.Close();

            return numberOfRecords; 
        }

        public void ExecuteNonQuery(string query, ref long lastInsertedId)
        {
            MySqlCommand myCommand = new MySqlCommand(query, conn);
            if (transaction != null)
                myCommand.Transaction = transaction;
            else
                conn.Open();

            myCommand.ExecuteNonQuery();
            lastInsertedId = myCommand.LastInsertedId;

            if (transaction == null)
                conn.Close();
        }

        public void ExecuteScalar(string query, ref string val)
        {
            conn.Open();
            MySqlCommand myCommand = new MySqlCommand(query, conn);
            val = myCommand.ExecuteScalar().ToString();
            conn.Close();
        }



        public void ExecuteQuery(string query, ref DataTable dt)
        {
            conn.Open();
            MySqlDataAdapter returnVal = new MySqlDataAdapter(query, conn);
            dt = new DataTable();
            returnVal.Fill(dt);
            conn.Close();
        }


    }
}
