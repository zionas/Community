using Amazon.DynamoDBv2.Model;
using CommunityNetwork.Common;
using CommunityNetwork.Common.Inerfaces;
using CommunityNetwork.Common.Models;
using CommunityNetWork.Dal;
using CommunityNetWork.Dal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Authentication.BL
{
    public class ProfilesManager : IProfileService
    {

        private readonly IDynamoDB _iDynamoDB;
        private readonly IDBConnectorFactory dBFactory;


        public ProfilesManager(IDBConnectorFactory dBFactory)
        {
            this.dBFactory = dBFactory;

            this._iDynamoDB = (IDynamoDB)dBFactory.Create(false);
        }

        }
        public Profile Login(string email, string password)
        {
            var profileConnected = _iDynamoDB.Get<Profile>(email, true);
            if (profileConnected?.Password == password)
            {
                GenerateToken(profileConnected.Email);
            }
            else
            {
                return null;
            }
            return profileConnected;
        }

        public bool CheckValidationToken(string email)
        {
            var token = _iDynamoDB.Get<TokenModel>(email, true);
            return CheckValidToken(token.TokenCreateTime);
        }

        private void GenerateToken(string email)
        {
            TokenModel tokenModel = new TokenModel()
            {
                Token = Guid.NewGuid(),
                TokenCreateTime = DateTime.Now,
                Email = email
            };
            _iDynamoDB.Add(tokenModel);
        }

        private bool CheckValidToken(DateTime tokenCreateTime)
        {
            return true;
            //return (DateTime.Now-tokenCreateTime).TotalMinutes < 15;
        }

        public Profile Register(Profile profile)
        {
            _iDynamoDB.Add(profile);
            return profile;
        }

        public Profile LoginWithFaceBook(Profile profile)
        {
            Profile profileConnected = null;
            try
            {
                 profileConnected = _iDynamoDB.Get<Profile>(profile.Email, true);
                if (profileConnected!=null)
                {
                    GenerateToken(profileConnected.Email);
                }
            }
            catch (Exception)
            {
                profileConnected = Register(profile);
            }
            return profileConnected;
        }

       
    }
}
