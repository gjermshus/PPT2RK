using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PPT2RK_Client
{
    public class Client
    {
        private PolarPersonalTrainerLib.PPTExport _polarexporter;
        private RKClient _rkclient;

        public Client(string PolarUsername, string PolarPassword, string RK_ClientId, string RK_ClientSecret)
        {
            try
            {
                this.Initiate(PolarUsername, PolarPassword, RK_ClientId, RK_ClientSecret);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Client()
        {
            var appSettings = ConfigurationManager.AppSettings;
            try
            {
                this.Initiate(appSettings["PPTUsername"], appSettings["PPTPassword"], appSettings["RKClientId"], appSettings["RKClientSecret"], appSettings["RKAccessToken"]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Initiate(string PolarUsername, string PolarPassword, string RK_ClientId, string RK_ClientSecret, string RK_accessToken = null)
        {
            try
            {
                this._polarexporter = new PolarPersonalTrainerLib.PPTExport(PolarUsername, PolarPassword);
                this._rkclient = new RKClient(RK_ClientId, RK_ClientSecret, RK_accessToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets todays activities
        /// </summary>
        /// <returns>Number of excercises transfered</returns>
        public Task<int> TransferTodaysAcitivities()
        {
            DateTime startTime = DateTime.Today;
            DateTime endTime = DateTime.Today.AddDays(1).AddSeconds(-1);
            // Get todays activities from PPT
            XmlDocument doc = this._polarexporter.downloadSessions(startTime, endTime);
            List<PolarPersonalTrainerLib.PPTExercise> excercises = PolarPersonalTrainerLib.PPTExtract.convertXmlToExercises(doc);
            int count = 0;
            foreach (var PPTExercise in excercises)
            {
                if (!this._rkclient.ExcerciseExists(PPTExercise))
                {
                    this._rkclient.AddExercise(PPTExercise);
                    count++;
                }
            }

            return Task.FromResult(count);
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Number of excercises transfered</returns>
        public Task<int> TransferAcitivities(DateTime day)
        {
            DateTime startTime= new DateTime(day.Year, day.Month, day.Day, 0,0,0);
            DateTime endTime = startTime.AddDays(1).AddSeconds(-1);
            // Get todays activities from PPT
            XmlDocument doc = this._polarexporter.downloadSessions(startTime, endTime);
            List<PolarPersonalTrainerLib.PPTExercise> excercises = PolarPersonalTrainerLib.PPTExtract.convertXmlToExercises(doc);
            int count = 0;
            foreach (var PPTExercise in excercises)
            {
                if (!this._rkclient.ExcerciseExists(PPTExercise))
                {
                    this._rkclient.AddExercise(PPTExercise);
                    count++;
                }
            }

            return Task.FromResult(count);
        }

        public Task<int> TransferAcitivities(DateTime from, DateTime To)
        {
            DateTime startTime = from;
            DateTime endTime = To;
            // Get todays activities from PPT
            XmlDocument doc = this._polarexporter.downloadSessions(startTime, endTime);
            List<PolarPersonalTrainerLib.PPTExercise> excercises = PolarPersonalTrainerLib.PPTExtract.convertXmlToExercises(doc);
            int count = 0;
            foreach (var PPTExercise in excercises)
            {
                if (!this._rkclient.ExcerciseExists(PPTExercise))
                {
                    this._rkclient.AddExercise(PPTExercise);
                    count++;
                }
            }

            return Task.FromResult(count);
        }
    }
}
