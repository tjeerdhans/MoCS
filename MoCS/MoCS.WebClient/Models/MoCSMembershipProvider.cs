using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using MoCS.Business.Facade;
using MoCS.Business.Objects;

namespace MoCS.WebClient.Models
{
    public class MoCSMembershipProvider : MembershipProvider
    {
        private string _applicationName;
        private int _maxInvalidPasswordAttempts;
        private int _minRequiredNonAlphanumericCharacters;
        private int _minRequiredPasswordLength = 4;
        private int _passwordAttemptWindow;
        private MembershipPasswordFormat _passwordFormat;
        private string _passwordStrengthRegularExpression;
        private bool _requiresQuestionAndAnswer;
        private bool _requiresUniqueEmail;

        public override string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }
        public override int MaxInvalidPasswordAttempts
        {
            get { return _maxInvalidPasswordAttempts; }
        }
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _minRequiredNonAlphanumericCharacters; }
        }
        public override int MinRequiredPasswordLength
        {
            get { return _minRequiredPasswordLength; }
        }
        public override int PasswordAttemptWindow
        {
            get { return _passwordAttemptWindow; }
        }
        public override MembershipPasswordFormat PasswordFormat
        {
            get { return _passwordFormat; }
        }
        public override string PasswordStrengthRegularExpression
        {
            get { return _passwordStrengthRegularExpression; }
        }
        public override bool RequiresQuestionAndAnswer
        {
            get { return _requiresQuestionAndAnswer; }
        }
        public override bool RequiresUniqueEmail
        {
            get { return _requiresUniqueEmail; }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            Team team = ClientFacade.Instance.GetTeamByName(username);
            if (team.Password == oldPassword)
            {
                team.Password = newPassword;
                ClientFacade.Instance.UpdateTeam(team);
                return true;
            }

            return false;
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public MembershipUser CreateUser(string username, string password, string members, out MembershipCreateStatus status)
        {
            MembershipUser result = null;

            if (ClientFacade.Instance.GetTeamByName(username) != null)
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return result;
            }

            Team team = new Team
            {
                Name = username,
                IsAdmin = false,
                CreateDate = DateTime.Now,
                Password = password,
                Members = members,
                Score = 0
            };

            team = ClientFacade.Instance.SaveTeam(team);


            if (team != null)
            {
                status = MembershipCreateStatus.Success;
                result = new MembershipUser("MoCSMembershipProvider", team.Name, team.Id, "", "", team.Members, true, false, team.CreateDate, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
            }
            else
            {
                status = MembershipCreateStatus.ProviderError;
            }

            return result;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string userName, bool userIsOnline)
        {
            MembershipUser result = null;
            Team team = ClientFacade.Instance.GetTeamByName(userName);

            if (team != null)
            {
                result = new MembershipUser("MoCSMembershipProvider", userName, team.Id, "", "", team.Members, true, false, team.CreateDate, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
            }

            return result;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            MembershipUser result = null;
            Team team = ClientFacade.Instance.GetTeamById((int)providerUserKey);

            if (team != null)
            {
                result = new MembershipUser("MoCSMembershipProvider", team.Name, team.Id, "", "", team.Members, true, false, team.CreateDate, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
            }

            return result;
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string userName, string password)
        {
            Team team = new Team()
            {
                Name = userName,
                Password = password
            };

            team = ClientFacade.Instance.AuthenticateTeam(team);

            if (team != null)
            {
                return true;
            }
            else
            {
                return false;
            }
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
            Team team = new Team()
            {
                Name = userName,
                Password = password
            };

            team = ClientFacade.Instance.AuthenticateTeam(team);

            if (team != null)
            {
                providerUserKey = team.Id;
                return true;
            }
            else
            {
                providerUserKey = null;
                return false;
            }
        }
    }
}
