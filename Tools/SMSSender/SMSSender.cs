using globalsms_api;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMSSender
{
    public class SMSSender
    {
        string _apiKey;
        public SMSSender(string apiKey)
        {
            _apiKey = apiKey;
        }
        public void SendSMSAsync(string from, string to, string msg)
        {
            
            WsSMSSoapClient.EndpointConfiguration c = new WsSMSSoapClient.EndpointConfiguration();
            globalsms_api.WsSMSSoapClient wsSMS = new globalsms_api.WsSMSSoapClient(c);
            wsSMS.sendSmsToRecipientsAsync(_apiKey, from, to, msg, "", "WaysEat");
        }
    }
}