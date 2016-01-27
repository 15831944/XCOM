using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace RebarPosCommands
{
    public partial class MyCommands
    {
        public static string LicensedAppName = "XCOM Bundle";
        public static string LicenseRegistryKey = "SOFTWARE\\" + DeveloperSymbol + "\\" + LicensedAppName;

        private DateTime LastLicenseCheck = DateTime.MinValue;
        private TimeSpan LicenseCheckInterval = TimeSpan.FromHours(1);

        private bool CheckLicense()
        {
            if (DateTime.Now - LastLicenseCheck < LicenseCheckInterval) return true;

            LicenseCheck.License license = LicenseCheck.License.FromRegistry(LicenseRegistryKey, LicensedAppName);
            if (license.Status == LicenseCheck.License.LicenseStatus.Valid)
            {
                license.SaveToRegistry(LicenseRegistryKey);

                LastLicenseCheck = DateTime.Now;
                return true;
            }

            using (LicenseCheck.RequestLicenseForm form = new LicenseCheck.RequestLicenseForm())
            {
                form.ActivationCode = LicenseCheck.License.FormatActivationCode(LicenseCheck.License.GetActivationCode(LicensedAppName));
                if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(null, form, false) != System.Windows.Forms.DialogResult.OK) return false;

                license = LicenseCheck.License.FromString(form.LicenseKey, LicensedAppName);
                if (license.Status != LicenseCheck.License.LicenseStatus.Valid) return false;

                license.SaveToRegistry(LicenseRegistryKey);

                LastLicenseCheck = DateTime.Now;

                // License information
                LicenseInformation();

                return true;
            }
        }

        private void LicenseInformation()
        {
            LicenseCheck.License license = LicenseCheck.License.FromRegistry(LicenseRegistryKey, LicensedAppName);

            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            if (license.Status == LicenseCheck.License.LicenseStatus.Valid)
            {
                ed.WriteMessage("\nLisans Bilgisi");
                ed.WriteMessage("\n--------------");
                ed.WriteMessage("\nSon Erişim Tarihi    : " + license.LastUsed.ToString("dd/MM/yyyy HH:mm:ss"));
                ed.WriteMessage("\nLisans Bitiş Tarihi  : " + license.Expires.ToString("dd/MM/yyyy HH:mm:ss"));
                ed.WriteMessage("\nKalan Kullanım Süresi: " + (license.Expires - DateTime.Now).Days.ToString() + " gün");
            }
            else
            {
                ed.WriteMessage("\nGeçerli lisans bulunamadı.");
            }
        }
    }
}
