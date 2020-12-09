using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public struct SubscriberRoute
    {
        public struct Name
        {
            public const string GetStripeKey = "subscriber.getstripekey";
            public const string GetCountryLookup = "subscriber.getcountrylookup";
            public const string GetStateLookup = "subscriber.getstatelookup";
            public const string GetTimeZoneLookup = "subscriber.gettimezonelookup";            
            public const string SaveCompany = "subscriber.savecompany";
            public const string ValidateCompany = "subscriber.validatecompany";
            public const string GetSubscriptionPlan = "subscriber.getsubscirptionplan";
            public const string BySearch = "subscriber.search";
            public const string GetSubscriberDetails = "subscriber.getsubscriberdetails";
            public const string GetSubscriberKitDetails = "subscriber.getsubscriberkitdetails";
            public const string GetLastSyncDetails = "subscriber.getlastsyncdetails";
        }

        public struct Route
        {
            public const string Global = "api/subscriber";
            public const string GetCountryLookup = "getcountrylookup";
            public const string GetStripeKey = "getstripekey";
            public const string GetStateLookup = "getstatelookup/{countryId}";
            public const string GetTimeZoneLookup = "gettimezonelookup";            
            public const string SaveCompany = "company";
            public const string ValidateCompany = "validatecompany";
            public const string GetSubscriptionPlan = "getsubscriptionplan";
            public const string BySearch = "search";           
            public const string GetSubscriberDetails = "getsubscriberdetails";
            public const string GetSubscriberKitDetails = "getsubscriberkitdetails";
            public const string GetLastSyncDetails = "getlastsyncdetails";
        }
    }
}
