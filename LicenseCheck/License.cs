﻿using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Text;

namespace LicenseCheck
{
    public class License
    {
        private static string Secret = "cpql8gzmi5pp";

        public LicenseStatus Status { get; private set; }
        public string ActivationCode { get; private set; }
        public DateTime LastUsed { get; private set; }
        public DateTime Expires { get; private set; }

        public enum LicenseStatus
        {
            Valid = 0,
            LicenseNotFound = 1,
            InvalidLicense = 2,
            LicenseDoesNotMatch = 3,
            SystemDateTampered = 4,
            Expired = 5
        }

        private License(LicenseStatus status, string activationCode, DateTime lastUsed, DateTime expires)
        {
            Status = status;
            ActivationCode = activationCode;
            LastUsed = lastUsed;
            Expires = expires;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Status: " + Status.ToString());
            sb.AppendLine("Activation Code: " + ActivationCode);
            sb.AppendLine("Last Used: " + LastUsed.ToString("dd/MM/yyyy HH:mm:ss"));
            sb.AppendLine("Expires: " + Expires.ToString("dd/MM/yyyy HH:mm:ss"));
            sb.AppendLine("Days Remaining: " + (Expires - DateTime.Now).Days.ToString());
            return sb.ToString();
        }

        public string LicenseInfo
        {
            get
            {
                if (Status == LicenseCheck.License.LicenseStatus.Valid)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Lisans Bilgisi");
                    sb.AppendLine("--------------");
                    sb.AppendLine("Son Erişim Tarihi    : " + LastUsed.ToString("dd/MM/yyyy HH:mm:ss"));
                    sb.AppendLine("Lisans Bitiş Tarihi  : " + Expires.ToString("dd/MM/yyyy HH:mm:ss"));
                    sb.AppendLine("Kalan Kullanım Süresi: " + (Expires - DateTime.Now).Days.ToString() + " gün");
                    return sb.ToString();
                }
                else
                {
                    return "Geçerli lisans bulunamadı.";
                }
            }
        }

        public static License FromRegistry(string registryKey, string app)
        {
            LicenseStatus status = LicenseStatus.LicenseNotFound;
            string activationCode = string.Empty;
            DateTime lastUsed = DateTime.MaxValue;
            DateTime expires = DateTime.MinValue;

            try
            {
                RegistryKey localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
                RegistryKey key = localKey.OpenSubKey(registryKey);
                if (key == null)
                {
                    status = LicenseStatus.LicenseNotFound;
                }
                else
                {
                    object val = key.GetValue("License");
                    if (val == null)
                    {
                        status = LicenseStatus.LicenseNotFound;
                    }
                    else
                    {
                        return FromString(val.ToString(), app);
                    }
                }
            }
            catch
            {
                status = LicenseStatus.InvalidLicense;
                activationCode = string.Empty;
                lastUsed = DateTime.MaxValue;
                expires = DateTime.MinValue;
            }

            return new License(status, activationCode, lastUsed, expires);
        }

        public static License FromFile(string licenseFile, string app)
        {
            return License.FromString(System.IO.File.ReadAllText(licenseFile), app);
        }

        public static License FromString(string licenseString, string app)
        {
            LicenseStatus status = LicenseStatus.LicenseNotFound;
            string activationCode = string.Empty;
            DateTime lastUsed = DateTime.MaxValue;
            DateTime expires = DateTime.MinValue;

            try
            {
                if (string.IsNullOrEmpty(licenseString))
                {
                    status = LicenseStatus.InvalidLicense;
                }
                else
                {
                    licenseString = Crypto.Decrypt(licenseString, Secret);
                    // MD5 hash of activation code: 32 characters + 1 character for CRC
                    activationCode = licenseString.Substring(0, 33);
                    if (string.Compare(activationCode, GetActivationCode(app), StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        status = LicenseStatus.LicenseDoesNotMatch;
                    }
                    else
                    {
                        // Last use date: "YYYYMMDDhhmmss" 14 chars    
                        if (!DateFromString(licenseString.Substring(33, 14), out lastUsed))
                        {
                            status = LicenseStatus.InvalidLicense;
                        }
                        else
                        {
                            // Expires date: "YYYYMMDDhhmmss" 14 chars    
                            if (!DateFromString(licenseString.Substring(47, 14), out expires))
                            {
                                status = LicenseStatus.InvalidLicense;
                            }
                            else
                            {
                                if (DateTime.Now < lastUsed)
                                {
                                    status = LicenseStatus.SystemDateTampered;
                                }
                                else
                                {
                                    if (DateTime.Now > expires)
                                    {
                                        status = LicenseStatus.Expired;
                                    }
                                    else
                                    {
                                        status = LicenseStatus.Valid;
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                status = LicenseStatus.InvalidLicense;
                activationCode = string.Empty;
                lastUsed = DateTime.MaxValue;
                expires = DateTime.MinValue;
            }

            return new License(status, activationCode, lastUsed, expires);
        }

        public bool SaveToRegistry(string registryKey)
        {
            try
            {
                RegistryKey localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
                RegistryKey key = localKey.CreateSubKey(registryKey);
                if (key == null)
                {
                    return false;
                }
                else
                {
                    LastUsed = DateTime.Now;
                    string licenseString = Crypto.Encrypt(ActivationCode + DateToString(LastUsed) + DateToString(Expires), Secret);
                    key.SetValue("License", licenseString);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        // Returns an activation code for this machine.
        public static string GetActivationCode(string app)
        {
            RegistryKey localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
            string machineGuid = localKey.OpenSubKey("SOFTWARE").OpenSubKey("Microsoft").OpenSubKey("Cryptography").GetValue("MachineGuid").ToString();
            string key = (app + machineGuid).ToUpper();
            string code = Crypto.GetMd5Hash(key).ToUpper();
            string crc = Crypto.GetMd5Hash(code).Substring(0, 1).ToUpper();
            return code + crc;
        }

        // Formats activation code for display
        public static string FormatActivationCode(string code)
        {
            return string.Join("-", SplitByLength(code, 4));
        }

        // Converts YYYYMMDDhhmmss string to DateTime
        private static bool DateFromString(string str, out DateTime date)
        {
            int y, m, d, hh, mm, ss;
            if (int.TryParse(str.Substring(0, 4), out y) &&
                int.TryParse(str.Substring(4, 2), out m) &&
                int.TryParse(str.Substring(6, 2), out d) &&
                int.TryParse(str.Substring(8, 2), out hh) &&
                int.TryParse(str.Substring(10, 2), out mm) &&
                int.TryParse(str.Substring(12, 2), out ss))
            {
                date = new DateTime(y, m, d, hh, mm, ss);
                return true;
            }
            else
            {
                date = DateTime.MinValue;
                return false;
            }
        }

        // Converts DateTime to YYYYMMDDhhmmss string
        private static string DateToString(DateTime date)
        {
            return string.Format("{0:yyyy}{0:MM}{0:dd}{0:HH}{0:mm}{0:ss}", date);
        }

        private static IEnumerable<string> SplitByLength(string str, int maxLength)
        {
            for (int index = 0; index < str.Length; index += maxLength)
            {
                yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
            }
        }
    }
}
