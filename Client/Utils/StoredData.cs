using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoleStar.Utils
{
    class StoredData
    {
        static Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        private static bool loggedIn;
        private static bool patient;
        private static bool caregiver;
        private static String userGUID;


        public static bool checkForPreviousLogin()
        {
            try
            {
                Object loginCheck = localSettings.Values["loggedIn"];
                if ((Boolean)loginCheck)
                {
                    loadUserData();
                    return true;
                }else
                {
                    return false; //no previous login
                }
            }
            catch
            {
                return false; //no previous login
            }
        }


        public static void storePatientData(String userGUID)
        {
            localSettings.Values["loggedIn"] = true;
            localSettings.Values["caregiver"] = false;
            localSettings.Values["patient"] = true;
            localSettings.Values["userGUID"] = userGUID;
        }

        public static void storeCaregiverData(String userGUID)
        {
            localSettings.Values["loggedIn"] = true;
            localSettings.Values["caregiver"] = true;
            localSettings.Values["patient"] = false;
            localSettings.Values["userGUID"] = userGUID;
        }

        public static void loadUserData()
        {
            loggedIn = true;
            Object caregiverCheck = localSettings.Values["caregiver"];
            caregiver = (bool)caregiverCheck;

            Object patientCheck = localSettings.Values["patient"];
            patient = (bool)patientCheck;

            userGUID = localSettings.Values["userGUID"].ToString();

        }
        public static bool isCaregiver()
        {
            return caregiver;
        }

        public static bool isPatient()
        {
            return patient;
        }

        public static String getUserGUID()
        {
            return userGUID;
        }

        public static void removeAllSavedData()
        {
            localSettings.Values.Remove("loggedIn");
            localSettings.Values.Remove("caregiver");
            localSettings.Values.Remove("patient");
            localSettings.Values.Remove("userGUID");
        }

    }
}
