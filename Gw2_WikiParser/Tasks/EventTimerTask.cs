using Gw2_WikiParser.Extensions;
using Gw2_WikiParser.Interfaces;
using Gw2_WikiParser.Model.Input.EventTimerTask;
using Gw2_WikiParser.Model.Output.EventTimerTask;
using Gw2_WikiParser.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gw2_WikiParser.Tasks
{
    public class EventTimerTask : IWikiTask
    {
        private readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string _page;

        public EventTimerTask(string page)
        {
            _page = page;
        }

        private async Task<Dictionary<string, EventGroup>> LoadEvents()
        {
            (_, string pageContent) = await WikiWrapper.Instance.GetPageContent(_page);
            string scriptContent = pageContent.Split("var eventData =")[1].Split(";")[0];

            Regex regexFixProperties = new Regex("([A-Za-z0-9]+(?=: *(\\{|\\\"|\\[|[0-9]+))+)");
            scriptContent = regexFixProperties.Replace(scriptContent, "\"$1\"");

            Regex regexRemoveComments = new Regex(@"(\/\/(.*))");
            scriptContent = regexRemoveComments.Replace(scriptContent, "");

            scriptContent = scriptContent.TrimAndReduceWhitespace();
            Dictionary<string, EventGroup> eventGroups = JsonConvert.DeserializeObject<Dictionary<string, EventGroup>>(scriptContent);

            return eventGroups;
        }

        public string GetProperties()
        {
            return _page;
        }

        public async Task<bool> Run()
        {
            Dictionary<string, EventGroup> events = null;
            List<Event> output = new List<Event>();


            try
            {
                events = await LoadEvents();
                
                foreach (string eventGroupId in events.Keys)
                {
                    EventGroup eventGroup = events[eventGroupId];
                    List<EventSequenceDetails> fullPattern = null;

                    int fillDuration = 60 * 24;
                    int partialDuration = eventGroup.Sequences.Partial.Sum(x => x.Duration);

                    if(partialDuration >= fillDuration)
                    {
                        fullPattern = eventGroup.Sequences.Partial.ToList();
                    }
                    else
                    {
                        fullPattern = new List<EventSequenceDetails>();
                        int patternDuration = eventGroup.Sequences.Pattern.Sum(x => x.Duration);
                        int patternQuantity = (int)Math.Ceiling((fillDuration - partialDuration) / (double) patternDuration);

                        for (int i = 0; i < patternQuantity; i++) fullPattern.AddRange(eventGroup.Sequences.Pattern.DeepCopy());
                    }

                    int cumulative = 0;
                    foreach (EventSequenceDetails eventSequenceDetails in fullPattern)
                    {
                        if (cumulative < fillDuration)
                        {
                            eventSequenceDetails.Start = cumulative;
                            eventSequenceDetails.End = eventSequenceDetails.Start + eventSequenceDetails.Duration;

                            cumulative = eventSequenceDetails.Start + eventSequenceDetails.Duration;
                        }
                    }

                    foreach (EventSequenceDetails eventSequenceDetails in fullPattern)
                    {
                        EventSegment eventSegment = eventGroup.Segments[eventSequenceDetails.R.ToString()];
                        if (eventSegment.Link != null)
                        {
                            (string eventLocation, string eventWikiPage) = await GetEventLocation(eventSegment);

                            Event outputEvent = new Event()
                            {
                                Category = eventGroup.Name,
                                Duration = TimeSpan.FromMinutes(eventSequenceDetails.Duration),
                                Location = eventLocation,
                                Name = eventSegment.Name,
                                Offset = TimeSpan.FromMinutes(eventSequenceDetails.Start),
                                WayPoint = eventSegment.ChatLink,
                                WikiUrl = eventWikiPage
                            };

                            if (!string.IsNullOrWhiteSpace(outputEvent.Name))
                            {
                                output.Add(outputEvent);
                            }
                        }
                    }
                }

                Dictionary<string, Dictionary<string, List<Event>>> dictEvents = output.GroupBy(x => x.Category).ToDictionary(k => k.First().Category, v => v.ToList().GroupBy(x => x.Name).ToDictionary(k2 => k2.First().Name, v2 => v2.ToList()));

                return WriteOutput(dictEvents);
            }
            catch(Exception ex)
            {
                _log.Error(ex);
            }


            return false;
        }

        private async Task<(string, string)> GetEventLocation(EventSegment eventSegment)
        {
            (string pageTitle, string pageContent) = await WikiWrapper.Instance.GetPageContent(eventSegment.Link);
            Regex regexLocation = new Regex(@"(?<=(location *= *))(.)*");
            Match matchLocation = regexLocation.Match(pageContent);

            string pageUrl = WikiWrapper.Instance.GetPageUrl(pageTitle);
            string location = matchLocation.Success ? matchLocation.Value.TrimAndReduceWhitespace() : string.Empty;

            return (location, pageUrl);
        }


        /// <summary>
        /// Writes the output files (separated by ;, from AppConfig key event_timer_output_files)
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private bool WriteOutput(Dictionary<string, Dictionary<string, List<Event>>> items)
        {
            bool wasSuccessful = true;
            string[] paths = ConfigurationManager.AppSettings["event_timer_output_files"].Split(';');

            foreach (string path in paths)
            {
                try
                {
                    File.WriteAllText(path, JsonConvert.SerializeObject(items, Formatting.Indented), Encoding.UTF8);
                }
                catch (IOException ex)
                {
                    _log.Error(ex);
                    wasSuccessful = false;
                }
            }

            return wasSuccessful;
        }

    }
}
