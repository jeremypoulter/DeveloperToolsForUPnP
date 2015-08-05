using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO.Compression;
using System.Collections.Generic;
using System.Configuration.Install;
using Microsoft.Win32;
using Microsoft.Deployment.WindowsInstaller;

namespace CustomActions
{
    public class CustomActions
    {
        public static string SafeToString(object InputObject)
        {
            if (InputObject == null) return ""; else return InputObject.ToString();
        }

        /// <summary>
        /// The main goal is to find the install scope of any previous install of this product and 
        /// match the install scope of the upgrade installation. Additionally, it detects if multiple
        /// installations are present and throws an exception. If any exceptions happen, they get re-thrown.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult CAGetPreviousInstallInfo(Session session)
        {
            string upgradeCode = string.Empty;
            string productCode = string.Empty;
            int installScope = 0;
            string installPath = string.Empty;

            //System.Diagnostics.Debugger.Launch();


            if (session["UpgradeCode"] == null)
            {
                session.Log("Upgrade code not found!");
                MessageBox.Show("Upgrade code not found!");
                throw (new Exception("Upgrade code not found!"));
            }
            else
                upgradeCode = session["UpgradeCode"].ToString();

            installScope = InstallerUtilities.GetAssignmentTypeAndProductCode(upgradeCode, out productCode);

            // returns : 0 = "Per user", "1" = "Per machine", -1 = "Not found", -2 = "More than one found", -3, -4, -5 = "Error"</returns>
            switch (installScope)
            {
                case 0:
                    session["ALLUSERS"] = "";
                    break;
                case 1:
                    session["ALLUSERS"] = "1";
                    break;
                case -1:
                    break;
                case -2:
                    session.Log(string.Format("{0} Please uninstall them and retry!", productCode));
                    MessageBox.Show(string.Format("{0} Please uninstall them and retry!", productCode));
                    throw (new Exception(string.Format("{0} Please uninstall them and retry!", productCode)));

                default:
                    session.Log(string.Format("An exception happened trying to get previously installed package information! Code = {0}. Error = {1}", installScope, productCode));
                    MessageBox.Show(string.Format("An exception happened trying to get previously installed package information! Code = {0}. Error = {1}", installScope, productCode));
                    throw (new Exception(string.Format("An exception happened trying to get previously installed package information! Code = {0}. Error = {1}", installScope, productCode)));

            }

            // returns : The install path or NULL if not found            
            installPath = InstallerUtilities.GetProductPath(productCode);
            if (!string.IsNullOrEmpty(installPath))
            {
                session["APPLICATIONROOTDIRECTORY"] = installPath;
                session["PREVIOUS_INSTALL_LOCATION"] = installPath;
            }

            return ActionResult.Success;
        }

        /// <summary>
        /// This is an immediate type of custom action whose purpose is to capture and store 
        /// all of the user selected parameters for future use (in the deferred custom actions 
        /// defined in the <InstallExecuteSequence>).
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult CASetPropertyValues(Session session)
        {

            //System.Diagnostics.Debugger.Launch();

            StringBuilder sb = new StringBuilder();

            session["ARPINSTALLLOCATION"] = session["APPLICATIONROOTDIRECTORY"];

            sb.Append(string.Format("PREVIOUS_INSTALL_LOCATION={0};", session["PREVIOUS_INSTALL_LOCATION"]));
            sb.Append(string.Format("APPLICATIONROOTDIRECTORY={0};", session["APPLICATIONROOTDIRECTORY"]));
            sb.Append(string.Format("TEMPFILESDIRECTORY={0};", session["TEMPFILESDIRECTORY"]));
            sb.Append(string.Format("ARPINSTALLLOCATION={0};", session["APPLICATIONROOTDIRECTORY"]));
            sb.Append(string.Format("SecureCustomProperties={0};", session["SecureCustomProperties"]));

            // save the UI properties to the deferred custom action variables
            session["LaunchBeforeInstall"] = sb.ToString();
            session["LaunchCommit"] = sb.ToString();
            session["LaunchPostUninstall"] = sb.ToString();

            return ActionResult.Success;
        }

        /// <summary>
        /// We only need this custom action in order to delete the temp folder created and used during installation. 
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult CACommit(Session session)
        {
            string tempfilePath = "";
            string customActionData = SafeToString(session.CustomActionData);

            //System.Diagnostics.Debugger.Launch();

            try
            {
                if ((customActionData.Contains("TEMPFILESDIRECTORY")) && (session.CustomActionData["TEMPFILESDIRECTORY"] != null))
                {
                    tempfilePath = SafeToString(session.CustomActionData["TEMPFILESDIRECTORY"]);
                    if (Directory.Exists(tempfilePath))
                    {
                        Directory.Delete(tempfilePath, true);
                    }
                }
            }
            catch (Exception ex)
            {
                session.Log(string.Format("An error occurred during install. {0}", ex.Message));
                MessageBox.Show(string.Format("An error occurred during install. {0}", ex.Message));
                throw;
            }

            return ActionResult.Success;
        }

        /// <summary>
        /// CAPostUninstall custom action removes any files from the installation directory thet were 
        /// added apart from the install files.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult CAPostUninstall(Session session)
        {
            string installPath = "";
            string errorMessage = string.Empty;
            string customActionData = SafeToString(session.CustomActionData);

            //System.Diagnostics.Debugger.Launch();

            try
            {
                if ((customActionData.Contains("PREVIOUS_INSTALL_LOCATION")) && (session.CustomActionData["PREVIOUS_INSTALL_LOCATION"] != null))
                    installPath = SafeToString(session.CustomActionData["PREVIOUS_INSTALL_LOCATION"]);

                if (!string.IsNullOrEmpty(installPath))
                {
                    if (Directory.Exists(installPath)) { Directory.Delete(installPath, true); }
                }
                else
                {
                    session.Log(string.Format("Install path not found. Error : {0}", errorMessage));
                }
            }
            catch (Exception ex) //log any errors, but don't throw the exception.
            {
                session.Log(string.Format("An error occurred during uninstall. {0}", ex.Message));
            }

            return ActionResult.Success;
        }

    }
}
