using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MoCS.Business.Objects;

namespace MoCS.WebClient.Models
{

    #region Services
    // The FormsAuthentication type is sealed and contains static members, so it is difficult to
    // unit test code that calls its members. The interface and helper class below demonstrate
    // how to create an abstract wrapper around such a type in order to make the AccountController
    // code unit testable.

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        bool ValidateUser(string userName, string password, out object providerUserKey);
        int GetUserId(string userName);
        MembershipCreateStatus CreateUser(string userName, string password, string email);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
    }

    public class AccountMembershipService : IMembershipService
    {
        private readonly MembershipProvider _provider;

        public AccountMembershipService()
            : this(null)
        {
        }

        public AccountMembershipService(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            ValidationUtil.ValidateRequiredStringValue(userName, "userName");
            ValidationUtil.ValidateRequiredStringValue(password, "password");

            return _provider.ValidateUser(userName, password);
        }

        /// <summary>
        /// THT 2010-04-08
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="providerUserKey"></param>
        /// <returns></returns>
        public bool ValidateUser(string userName, string password, out object providerUserKey)
        {
            bool result = false;

            ValidationUtil.ValidateRequiredStringValue(userName, "userName");
            ValidationUtil.ValidateRequiredStringValue(password, "password");

            result = ((MoCSMembershipProvider)_provider).ValidateUser(userName, password, out providerUserKey);

            return result;
        }

        /// <summary>
        /// THT 2010-04-08
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public int GetUserId(string userName)
        {
            int result = -1;

            ValidationUtil.ValidateRequiredStringValue(userName, "userName");

            MembershipUser user = _provider.GetUser(userName, false);
            if (user != null)
            {
                result = (int)user.ProviderUserKey;
            }
            return result;
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            ValidationUtil.ValidateRequiredStringValue(userName, "userName");
            ValidationUtil.ValidateRequiredStringValue(password, "password");
            ValidationUtil.ValidateRequiredStringValue(email, "email");

            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            ValidationUtil.ValidateRequiredStringValue(userName, "userName");
            ValidationUtil.ValidateRequiredStringValue(oldPassword, "oldPassword");
            ValidationUtil.ValidateRequiredStringValue(newPassword, "newPassword");

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {
                MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
                return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }
    }

    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignIn(int userId, string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        /// <summary>
        /// THT 2010-04-08
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="createPersistentCookie"></param>
        public void SignIn(int userId, string userName, bool createPersistentCookie)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.Now.AddMinutes(30), createPersistentCookie, userId.ToString());
            string encryptedTicket = FormsAuthentication.Encrypt(ticket);
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket));
            //FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);

        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }

    internal static class ValidationUtil
    {
        private const string _stringRequiredErrorMessage = "Value cannot be null or empty.";

        public static void ValidateRequiredStringValue(string value, string parameterName)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentException(_stringRequiredErrorMessage, parameterName);
            }
        }
    }

    internal static class SessionUtil
    {
        public static Team GetTeamFromFormsAuthentication()
        {
            Team result = null;

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsIdentity fi = (FormsIdentity)HttpContext.Current.User.Identity;

                if (fi.IsAuthenticated)
                {
                    result = new Team()
                    {
                        Id = int.Parse(fi.Ticket.UserData), //Session["teamId"],
                        Name = fi.Name
                    };
                }
            }
            return result;
        }

        public static void SetSession(Tournament t, TournamentAssignment ta, Assignment a, AssignmentEnrollment ae)
        {
            if (t != null)
            {
                HttpContext.Current.Session["tournamentId"] = t.Id;
                HttpContext.Current.Session["tournamentName"] = t.Name;

            }
            else
            {
                HttpContext.Current.Session.Remove("tournamentId");
                HttpContext.Current.Session.Remove("tournamentName");
            }
            if (ta != null)
            {
                HttpContext.Current.Session["tournamentAssignmentId"] = ta.Id;
            }
            else
            {
                HttpContext.Current.Session.Remove("tournamentAssignmentId");
            }

            if (a != null)
            {
                HttpContext.Current.Session["assignmentId"] = a.Id;
                HttpContext.Current.Session["assignmentName"] = a.Name;
            }
            else
            {
                HttpContext.Current.Session.Remove("assignmentId");
                HttpContext.Current.Session.Remove("assignmentName");
            }

            if (ae!=null)
            {
               HttpContext.Current.Session["assignmentEnrollmentId"] = ae.Id;
               HttpContext.Current.Session["assignmentEnrollmentStartDate"] = ae.StartDate;
            }
            else
            {
                HttpContext.Current.Session.Remove("assignmentEnrollmentId");
                HttpContext.Current.Session.Remove("assignmentEnrollmentStartDate");

            }
        }

        public static Tournament GetTournamentFromSession()
        {
            Tournament result = null;
            if (HttpContext.Current.Session["tournamentId"] != null)
            {
                result = new Tournament()
                   {
                       Id = (int)HttpContext.Current.Session["tournamentId"],
                       Name = (string)HttpContext.Current.Session["tournamentName"]
                   };
            }

            return result;
        }

        public static TournamentAssignment GetTournamentAssignmentFromSession()
        {
            TournamentAssignment result = null;
            if (HttpContext.Current.Session["tournamentAssignmentId"] != null)
            {
                result = new TournamentAssignment()
                {
                    Id = (int)HttpContext.Current.Session["tournamentAssignmentId"]
                };
            }
            return result;
        }

        public static Assignment GetAssignmentFromSession()
        {
            Assignment result = null;
            if (HttpContext.Current.Session["assignmentId"] != null)
            {
                result = new Assignment()
                {
                    Id = (int)HttpContext.Current.Session["assignmentId"],
                    Name = (string)HttpContext.Current.Session["assignmentName"]
                };
            }

            return result;
        }

        public static AssignmentEnrollment GetAssignmentEnrollmentFromSession()
        {
            AssignmentEnrollment result = null;
            if (HttpContext.Current.Session["assignmentEnrollmentId"] != null)
            {
                result = new AssignmentEnrollment()
                {
                    Id = (int)HttpContext.Current.Session["assignmentEnrollmentId"],
                    StartDate = (DateTime)HttpContext.Current.Session["assignmentEnrollmentStartDate"]
                };
            }

            return result;
        }
    }
    #endregion

    #region Models
    [PropertiesMustMatch("NewPassword", "ConfirmPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Current password")]
        public string OldPassword { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [DisplayName("New password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirm new password")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [DisplayName("User Id")]
        public int UserId { get; set; }

        [Required]
        [DisplayName("User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("Remember me?")]
        public bool RememberMe { get; set; }
    }

    [PropertiesMustMatch("Password", "ConfirmPassword", ErrorMessage = "The password and confirmation password do not match.")]
    public class RegisterModel
    {
        [Required]
        [DisplayName("User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Email address")]
        public string Email { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirm password")]
        public string ConfirmPassword { get; set; }
    }
    #endregion

    #region Validation
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class PropertiesMustMatchAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "'{0}' and '{1}' do not match.";

        private readonly object _typeId = new object();

        public PropertiesMustMatchAttribute(string originalProperty, string confirmProperty)
            : base(_defaultErrorMessage)
        {
            OriginalProperty = originalProperty;
            ConfirmProperty = confirmProperty;
        }

        public string ConfirmProperty
        {
            get;
            private set;
        }

        public string OriginalProperty
        {
            get;
            private set;
        }

        public override object TypeId
        {
            get
            {
                return _typeId;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
                OriginalProperty, ConfirmProperty);
        }

        public override bool IsValid(object value)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
            object originalValue = properties.Find(OriginalProperty, true /* ignoreCase */).GetValue(value);
            object confirmValue = properties.Find(ConfirmProperty, true /* ignoreCase */).GetValue(value);
            return Object.Equals(originalValue, confirmValue);
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidatePasswordLengthAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "'{0}' must be at least {1} characters long.";

        private readonly int _minCharacters = Membership.Provider.MinRequiredPasswordLength;

        public ValidatePasswordLengthAttribute()
            : base(_defaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
                name, _minCharacters);
        }

        public override bool IsValid(object value)
        {
            string valueAsString = value as string;
            return (valueAsString != null && valueAsString.Length >= _minCharacters);
        }
    }
    #endregion

}
