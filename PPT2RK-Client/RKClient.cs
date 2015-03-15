using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthGraphNet;
using HealthGraphNet.Models;
using System.Configuration;

namespace PPT2RK_Client
{
    public class RKClient
    {
        private string _clientId;
        private string _clientSecret;
        private string _accessToken;
        private AccessTokenManager _tm;
        private HealthGraphNet.Models.UsersModel _user;

        public bool IsConnected { get; private set; }

        public RKClient(string clientId, string clientSecret, string accessToken = null)
        {
            this._clientId = clientId;
            this._clientSecret = clientSecret;
            this._accessToken = accessToken;
            this.IsConnected = false;
        }

        public Task<bool> Connect()
        {
            try
            {
                this._tm = new AccessTokenManager(this._clientId, this._clientSecret, "https://runkeeper.com/apps/authorize", this._accessToken);
                
                var userRequest = new UsersEndpoint(this._tm);
                this._user = userRequest.GetUser();
                this.IsConnected = true;
                return Task.FromResult<bool>(true);
            }
            catch (Exception ex)
            {
                return Task.FromResult<bool>(false);
            }
        }

        internal void AddExercise(PolarPersonalTrainerLib.PPTExercise PPTExercise)
        {
            if (this.IsConnected == false)
                this.Connect();

            HeartRateModel hrm = new HeartRateModel();
            var newAcitivty = new FitnessActivitiesNewModel()
            {
                Type = PPTExercise.sport ?? "Running",
                StartTime = PPTExercise.time,
                Duration = PPTExercise.duration.TotalSeconds,
                TotalCalories = PPTExercise.calories,
                HeartRate = PPTExercise.heartRate.HeartBeats.Select(e => { 
                    return new HeartRateModel() 
                    { 
                        HeartRate = e.HeartRate, 
                        Timestamp = (e.Time - PPTExercise.time).TotalSeconds 
                    };
                }).ToList(),
                AverageHeartRate = PPTExercise.heartRate.average,
                TotalDistance = PPTExercise.Distance
            };

            var activitiesRequest = new FitnessActivitiesEndpoint(this._tm, this._user);
            activitiesRequest.CreateActivity(newAcitivty);
        }

        internal bool ExcerciseExists(PolarPersonalTrainerLib.PPTExercise PPTExercise)
        {
            if (this.IsConnected == false)
                this.Connect();

            var activitiesRequest = new FitnessActivitiesEndpoint(this._tm, this._user);
            var acitivities = activitiesRequest.GetFeedPage(noEarlierThan: PPTExercise.time, noLaterThan: PPTExercise.time);
            return acitivities.Items.Count > 0;
        }
    }
}
