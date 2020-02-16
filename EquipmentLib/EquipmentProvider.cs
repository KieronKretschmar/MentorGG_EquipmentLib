using Newtonsoft.Json;
﻿using EquipmentLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using EquipmentLib.Enums;
using Microsoft.Extensions.Logging;

namespace EquipmentLib
{
    public interface IEquipmentProvider
    {
        List<EquipmentSet> EquipmentSets { get; set; }

        Dictionary<short, EquipmentInfo> GetEquipmentDict(Source source, DateTime matchDate);
        EquipmentSet GetEquipmentSet(Source source, DateTime matchDate);
        void TryUpdateEquipments(object source, ElapsedEventArgs e);
    }

    public class EquipmentProvider : IEquipmentProvider
    {
        public HttpClient Client;
        private readonly ILogger<EquipmentProvider> _logger;
        public readonly string EquipmentEndpoint = "http://mentor.gg/backend/equipment/allequipments";

        //public static readonly string EquipmentEndpoint = "http://localhost:58071/backend/equipment/allequipments";
        public List<EquipmentSet> EquipmentSets { get; set; }

        public EquipmentProvider(ILogger<EquipmentProvider> logger, string csvDirectory, string equipmentEndpoint = null)
        {
            _logger = logger;
            EquipmentEndpoint = equipmentEndpoint;

            // Load data from local files as fallback if loading via REST does not work
            var eqReader = new EquipmentReader();
            EquipmentSets = eqReader.LoadEquipmentsFromFiles(csvDirectory);

            if(EquipmentEndpoint != null)
            {
                _logger.LogInformation($"EquipmentEndpoint [ {EquipmentEndpoint} ] specified. Trying to refresh data from there regularly.");
                // Try to update equipments via REST
                Client = new HttpClient();
                Client.DefaultRequestHeaders.Add("Accept", "application/json");


                //// Create timer to Update equipment data regularly
                var dataRefreshTimer = new Timer();
                // Tell the timer what to do when it elapses
                dataRefreshTimer.Elapsed += TryUpdateEquipments;
                // Set it to go off every 60 minutes
                dataRefreshTimer.Interval = 60 * 60 * 1000;
                // And start it        
                dataRefreshTimer.Enabled = true;
                // Refresh once right away
                TryUpdateEquipments();
            }
            else
            {
                _logger.LogInformation($"No EquipmentEndpoint specified. Using fallback data.");
            }
        }

        public EquipmentSet GetEquipmentSet(Source source, DateTime matchDate)
        {
            // DEBUG
            var candidates = EquipmentSets.Where(x => (byte) x.Source == (byte)source && x.ValidFrom < matchDate).OrderByDescending(x => x.ValidFrom);

            // Return latest applicable dataset with the same source
            if (EquipmentSets.Any(x => (byte)x.Source == (byte)source))
                return EquipmentSets.Where(x => (byte)x.Source == (byte)source && x.ValidFrom < matchDate).OrderByDescending(x => x.ValidFrom).First();

            // Default to newest Valve values if none is found
            return EquipmentSets.Where(x => x.Source == Source.Valve && x.ValidFrom < matchDate).OrderByDescending(x => x.ValidFrom).First();
        }

        public Dictionary<short, EquipmentInfo> GetEquipmentDict(Source source, DateTime matchDate)
        {
            var set = GetEquipmentSet(source, matchDate);
            return set.EquipmentDict;
        }


        private void TryUpdateEquipments()
        {
            try
            {
                var response = Client.GetAsync(EquipmentEndpoint).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        string responseString = response.Content.ReadAsStringAsync().Result;
                        var model = JsonConvert.DeserializeObject<EquipmentModel>(responseString);
                        EquipmentSets = model.EquipmentSets;
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning("Could not read response from Equipment REST provider. Continuing with fallback equipment data.");
                        _logger.LogError(e, "error");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning("Could not load equipment data from Equipment REST provider. Continuing with old equipment data.");
                _logger.LogError(e, "error");
            }
        }


        // Signature to enable usage in Timer
        public void TryUpdateEquipments(object source, ElapsedEventArgs e)
        {
            TryUpdateEquipments();
        }
    }
}
