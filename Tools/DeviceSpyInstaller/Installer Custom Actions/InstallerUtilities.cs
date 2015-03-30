using System;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CustomActions
{
    public static class InstallerUtilities
    {
        internal const int MaxGuidChars = 38;
        internal const int NoError = 0;
        internal const int ErrorNoMoreItems = 259;
        internal const int ErrorUnknownProduct = 1605;
        internal const int ErrorUnknownProperty = 1608;
        internal const int ErrorMoreData = 234;

        [DllImport("msi.dll", CharSet = CharSet.Unicode, PreserveSig = true, SetLastError = true)]
        private static extern int MsiOpenPackage(string szPackagePath, ref int hProduct);

        [DllImport("msi.dll", CharSet = CharSet.Unicode, PreserveSig = true, SetLastError = true)]
        private static extern int MsiOpenPackageExW(string szPackagePath, UInt32 dwOptions, ref int hProduct);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        private static extern int MsiGetProductInfo(string product, string property, [Out] StringBuilder valueBuf, ref Int32 len);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        private static extern int MsiGetProperty(int hInstall, string szName, [Out] StringBuilder szValueBuf, ref int pchValueBuf);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        private static extern int MsiCloseHandle(int hInstall);

        [DllImport("msi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int MsiEnumRelatedProducts(string strUpgradeCode, int reserved, int iIndex, StringBuilder sbProductCode);

        private const string INSTALLPROPERTY_PACKAGENAME = "PackageName";
        private const string INSTALLPROPERTY_TRANSFORMS = "Transforms";
        private const string INSTALLPROPERTY_LANGUAGE = "Language";
        private const string INSTALLPROPERTY_PRODUCTNAME = "ProductName";
        private const string INSTALLPROPERTY_ASSIGNMENTTYPE = "AssignmentType";
        private const string INSTALLPROPERTY_PACKAGECODE = "PackageCode";
        private const string INSTALLPROPERTY_VERSION = "Version";
        private const string INSTALLPROPERTY_PRODUCTICON = "ProductIcon";
        private const string INSTALLPROPERTY_INSTALLEDPRODUCTNAME = "InstalledProductName";
        private const string INSTALLPROPERTY_VERSIONSTRING = "VersionString";
        private const string INSTALLPROPERTY_HELPLINK = "HelpLink";
        private const string INSTALLPROPERTY_HELPTELEPHONE = "HelpTelephone";
        private const string INSTALLPROPERTY_INSTALLLOCATION = "InstallLocation";
        private const string INSTALLPROPERTY_INSTALLSOURCE = "InstallSource";
        private const string INSTALLPROPERTY_INSTALLDATE = "InstallDate";
        private const string INSTALLPROPERTY_PUBLISHER = "Publisher";
        private const string INSTALLPROPERTY_LOCALPACKAGE = "LocalPackage";
        private const string INSTALLPROPERTY_URLINFOABOUT = "URLInfoAbout";
        private const string INSTALLPROPERTY_URLUPDATEINFO = "URLUpdateInfo";
        private const string INSTALLPROPERTY_VERSIONMINOR = "VersionMinor";
        private const string INSTALLPROPERTY_VERSIONMAJOR = "VersionMajor";


        private const string ERROR_INSTALL_SERVICE_FAILURE = "The Windows Installer service could not be accessed. Contact your support personnel to verify that the Windows Installer service is properly registered.";
        private const string ERROR_INSTALL_USEREXIT = "User cancel installation.";
        private const string ERROR_INSTALL_FAILURE = "Fatal error during installation.";
        private const string ERROR_INSTALL_SUSPEND = "Installation suspended, incomplete.";
        private const string ERROR_UNKNOWN_PRODUCT = "This action is only valid for products that are currently installed.";
        private const string ERROR_UNKNOWN_FEATURE = "Feature ID not registered.";
        private const string ERROR_UNKNOWN_COMPONENT = "Component ID not registered.";
        private const string ERROR_UNKNOWN_PROPERTY = "Unknown property.";
        private const string ERROR_INVALID_HANDLE_STATE = "Handle is in an invalid state.";
        private const string ERROR_BAD_CONFIGURATION = "The configuration data for this product is corrupt. Contact your support personnel.";
        private const string ERROR_INDEX_ABSENT = "Component qualifier not present.";
        private const string ERROR_INSTALL_SOURCE_ABSENT = "The installation source for this product is not available. Verify that the source exists and that you can access it.";
        private const string ERROR_INSTALL_PACKAGE_VERSION = "This installation package cannot be installed by the Windows Installer service. You must install a Windows service pack that contains a newer version of the Windows Installer service.";
        private const string ERROR_PRODUCT_UNINSTALLED = "Product is uninstalled.";
        private const string ERROR_BAD_QUERY_SYNTAX = "SQL query syntax invalid or unsupported.";
        private const string ERROR_INVALID_FIELD = "Record field does not exist.";
        private const string ERROR_INSTALL_ALREADY_RUNNING = "Another installation is already in progress. Complete that installation before proceeding with this install.";
        private const string ERROR_INSTALL_PACKAGE_OPEN_FAILED = "This installation package could not be opened. Verify that the package exists and that you can access it, or contact the application vendor to verify that this is a valid Windows Installer package.";
        private const string ERROR_INSTALL_PACKAGE_INVALID = "This installation package could not be opened. Contact the application vendor to verify that this is a valid Windows Installer package.";
        private const string ERROR_INSTALL_UI_FAILURE = "There was an error starting the Windows Installer service user interface. Contact your support personnel.";
        private const string ERROR_INSTALL_LOG_FAILURE = "Error opening installation log file. Verify that the specified log file location exists and is writable.";
        private const string ERROR_INSTALL_LANGUAGE_UNSUPPORTED = "This language of this installation package is not supported by your system.";
        private const string ERROR_INSTALL_PACKAGE_REJECTED = "This installation is forbidden by system policy. Contact your system administrator.";
        private const string ERROR_FUNCTION_NOT_CALLED = "Function could not be executed.";
        private const string ERROR_FUNCTION_FAILED = "Function failed during execution.";
        private const string ERROR_INVALID_TABLE = "Invalid or unknown table specified.";
        private const string ERROR_DATATYPE_MISMATCH = "Data supplied is of wrong type.";
        private const string ERROR_UNSUPPORTED_TYPE = "Data of this type is not supported.";
        private const string ERROR_CREATE_FAILED = "The Windows Installer service failed to start. Contact your support personnel.";
        private const string ERROR_INSTALL_TEMP_UNWRITABLE = "The temp folder is either full or inaccessible. Verify that the temp folder exists and that you can write to it.";
        private const string ERROR_INSTALL_PLATFORM_UNSUPPORTED = "This installation package is not supported on this platform. Contact your application vendor.";
        private const string ERROR_INSTALL_NOTUSED = "Component not used on this machine.";
        private const string ERROR_INSTALL_TRANSFORM_FAILURE = "Error applying transforms. Verify that the specified transform paths are valid.";
        private const string ERROR_PATCH_PACKAGE_OPEN_FAILED = "This patch package could not be opened. Verify that the patch package exists and that you can access it, or contact the application vendor to verify that this is a valid Windows Installer patch package.";
        private const string ERROR_PATCH_PACKAGE_INVALID = "This patch package could not be opened. Contact the application vendor to verify that this is a valid Windows Installer patch package.";
        private const string ERROR_PATCH_PACKAGE_UNSUPPORTED = "This patch package cannot be processed by the Windows Installer service. You must install a Windows service pack that contains a newer version of the Windows Installer service.";
        private const string ERROR_PRODUCT_VERSION = "Another version of this product is already installed. Installation of this version cannot continue. To configure or remove the existing version of this product, use Add/Remove Programs on the Control Panel.";
        private const string ERROR_INVALID_COMMAND_LINE = "Invalid command line argument. Consult the Windows Installer SDK for detailed command line help.";
        private const string ERROR_SUCCESS_REBOOT_REQUIRED = "A restart is required to complete the install. This does not include installs where the ForceReboot action is run. Note that this error will not be available until future version of the installer. ";
        private const string ERROR_UNKNOWN = "An unknown error has occurred.";


        /// <summary>
        /// Get the assignment type of a given product code.
        /// </summary>
        /// <param name="upgradecode">The updatecode to look for</param>
        /// <returns>0 = "Per user", "1" = "Per machine", -1 = "Not found", -2 = "More than one found", -3, -4, -5, -6 = "Error"</returns>
        public static int GetAssignmentTypeAndProductCode(string upgradecode, out string productcode)
        {
            int index = 0;
            int len = 16000;
            int type = -1;
            productcode = null;
            StringBuilder sbBuffer = new StringBuilder(len);
            int returnValue = 0;

            while (MsiEnumRelatedProducts(upgradecode, 0, index, sbBuffer) == 0)
            {
                productcode = sbBuffer.ToString();
                len = sbBuffer.Capacity;
                returnValue = MsiGetProductInfo(productcode, INSTALLPROPERTY_ASSIGNMENTTYPE, sbBuffer, ref len);

                if (returnValue != 0)
                {
                    productcode = GetErrorMessage(returnValue);
                    return -3;
                }

                if (len != 1)
                {
                    productcode = "An internal error has occurred.";
                    return -4;
                }
                if (sbBuffer[0] == '0')
                    type = 0;
                else if (sbBuffer[0] == '1')
                    type = 1;
                else
                {
                    productcode = "An internal error has occurred.";
                    return -5;
                }
                index++;
            }

            if (index > 1)
            {
                productcode = "Multiple product installations were detected.";
                return -2;
            }
            return type;
        }

        /// <summary>
        /// Retreive the installation path for a given product code.
        /// </summary>
        /// <param name="productcode">Product code of an installed product</param>
        /// <returns>The install path or NULL if not found</returns>
        public static string GetProductPath(string productcode)
        {
            int len = 16000;
            int errorCode = 0;

            StringBuilder sbBuffer = new StringBuilder(len);
            errorCode = MsiGetProductInfo(productcode, INSTALLPROPERTY_INSTALLLOCATION, sbBuffer, ref len);
            if (errorCode != 0) return null;
            if (len == 0) return null;
            return sbBuffer.ToString();
        }


        private static string GetErrorMessage(int ErrorCode)
        {
            string errorMessage = string.Empty;

            switch (ErrorCode)
            {
                case 1601:
                    errorMessage = string.Format("{0}({1})", ERROR_INSTALL_SERVICE_FAILURE, ErrorCode);
                    break;

                case 1602:
                    errorMessage = string.Format("{0}({1})", ERROR_INSTALL_USEREXIT, ErrorCode);
                    break;

                case 1603:
                    errorMessage = string.Format("{0}({1})", ERROR_INSTALL_FAILURE, ErrorCode);
                    break;

                case 1604:
                    errorMessage = string.Format("{0}({1})", ERROR_INSTALL_SUSPEND, ErrorCode);
                    break;

                case 1605:
                    errorMessage = string.Format("{0}({1})", ERROR_UNKNOWN_PRODUCT, ErrorCode);
                    break;

                case 1606:
                    errorMessage = string.Format("{0}({1})", ERROR_UNKNOWN_FEATURE, ErrorCode);
                    break;

                case 1607:
                    errorMessage = string.Format("{0}({1})", ERROR_UNKNOWN_COMPONENT, ErrorCode);
                    break;

                case 1608:
                    errorMessage = string.Format("{0}({1})", ERROR_UNKNOWN_PROPERTY, ErrorCode);
                    break;

                case 1609:
                    errorMessage = string.Format("{0}({1})", ERROR_INVALID_HANDLE_STATE, ErrorCode);
                    break;

                case 1610:
                    errorMessage = string.Format("{0}({1})", ERROR_BAD_CONFIGURATION, ErrorCode);
                    break;

                case 1611:
                    errorMessage = string.Format("{0}({1})", ERROR_INDEX_ABSENT, ErrorCode);
                    break;

                case 1612:
                    errorMessage = string.Format("{0}({1})", ERROR_INSTALL_SOURCE_ABSENT, ErrorCode);
                    break;

                case 1613:
                    errorMessage = string.Format("{0}({1})", ERROR_INSTALL_PACKAGE_VERSION, ErrorCode);
                    break;

                case 1614:
                    errorMessage = string.Format("{0}({1})", ERROR_PRODUCT_UNINSTALLED, ErrorCode);
                    break;

                case 1615:
                    errorMessage = string.Format("{0}({1})", ERROR_BAD_QUERY_SYNTAX, ErrorCode);
                    break;

                case 1616:
                    errorMessage = string.Format("{0}({1})", ERROR_INVALID_FIELD, ErrorCode);
                    break;

                case 1618:
                    errorMessage = string.Format("{0}({1})", ERROR_INSTALL_ALREADY_RUNNING, ErrorCode);
                    break;

                case 1619:
                    errorMessage = string.Format("{0}({1})", ERROR_INSTALL_PACKAGE_OPEN_FAILED, ErrorCode);
                    break;

                case 1620:
                    errorMessage = string.Format("{0}({1})", ERROR_INSTALL_PACKAGE_INVALID, ErrorCode);
                    break;

                case 1621:
                    errorMessage = string.Format("{0}({1})", ERROR_INSTALL_UI_FAILURE, ErrorCode);
                    break;

                case 1622:
                    errorMessage = string.Format("{0}({1})", ERROR_INSTALL_LOG_FAILURE, ErrorCode);
                    break;

                case 1623:
                    errorMessage = string.Format("{0}({1})", ERROR_INSTALL_LANGUAGE_UNSUPPORTED, ErrorCode);
                    break;

                case 1625:
                    errorMessage = string.Format("{0}({1})", ERROR_INSTALL_PACKAGE_REJECTED, ErrorCode);
                    break;

                case 1626:
                    errorMessage = string.Format("{0}({1})", ERROR_FUNCTION_NOT_CALLED, ErrorCode);
                    break;

                case 1627:
                    errorMessage = string.Format("{0}({1})", ERROR_FUNCTION_FAILED, ErrorCode);
                    break;

                case 1628:
                    errorMessage = string.Format("{0}({1})", ERROR_INVALID_TABLE, ErrorCode);
                    break;

                case 1629:
                    errorMessage = string.Format("{0}({1})", ERROR_DATATYPE_MISMATCH, ErrorCode);
                    break;

                case 1630:
                    errorMessage = string.Format("{0}({1})", ERROR_UNSUPPORTED_TYPE, ErrorCode);
                    break;

                case 1631:
                    errorMessage = string.Format("{0}({1})", ERROR_CREATE_FAILED, ErrorCode);
                    break;

                case 1632:
                    errorMessage = string.Format("{0}({1})", ERROR_INSTALL_TEMP_UNWRITABLE, ErrorCode);
                    break;

                case 1633:
                    errorMessage = string.Format("{0}({1})", ERROR_INSTALL_PLATFORM_UNSUPPORTED, ErrorCode);
                    break;

                case 1634:
                    errorMessage = string.Format("{0}({1})", ERROR_INSTALL_NOTUSED, ErrorCode);
                    break;

                case 1624:
                    errorMessage = string.Format("{0}({1})", ERROR_INSTALL_TRANSFORM_FAILURE, ErrorCode);
                    break;

                case 1635:
                    errorMessage = string.Format("{0}({1})", ERROR_PATCH_PACKAGE_OPEN_FAILED, ErrorCode);
                    break;

                case 1636:
                    errorMessage = string.Format("{0}({1})", ERROR_PATCH_PACKAGE_INVALID, ErrorCode);
                    break;

                case 1637:
                    errorMessage = string.Format("{0}({1})", ERROR_PATCH_PACKAGE_UNSUPPORTED, ErrorCode);
                    break;

                case 1638:
                    errorMessage = string.Format("{0}({1})", ERROR_PRODUCT_VERSION, ErrorCode);
                    break;

                case 1639:
                    errorMessage = string.Format("{0}({1})", ERROR_INVALID_COMMAND_LINE, ErrorCode);
                    break;

                case 3010:
                    errorMessage = string.Format("{0}({1})", ERROR_SUCCESS_REBOOT_REQUIRED, ErrorCode);
                    break;

                default:
                    errorMessage = string.Format("{0}({1})", ERROR_UNKNOWN, ErrorCode);
                    break;
            };

            return errorMessage;
        }

    }
}
