using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AccountEx.Common.Integrations
{
    public class GoogleCalendarEvent
    {

        public string Id { get; set; }
        public string summary { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public GoogleCalendarEventDateTime start { get; set; }
        public GoogleCalendarEventDateTime end { get; set; }
        public List<Reminders> reminders { get; set; }
        public string Ids { get; set; }
    }
    public class GoogleCalendarEventDateTime
    {

        public string dateTime { get; set; }
        public string timeZone { get; set; }
    }


    public partial class InsertEventResponse
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("etag")]
        public string Etag { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("htmlLink")]
        public Uri HtmlLink { get; set; }

        [JsonProperty("created")]
        public DateTimeOffset Created { get; set; }

        [JsonProperty("updated")]
        public DateTimeOffset Updated { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("creator")]
        public Creator Creator { get; set; }

        [JsonProperty("organizer")]
        public Creator Organizer { get; set; }

        [JsonProperty("start")]
        public End Start { get; set; }

        [JsonProperty("end")]
        public End End { get; set; }

        [JsonProperty("iCalUID")]
        public string ICalUid { get; set; }

        [JsonProperty("sequence")]
        public long Sequence { get; set; }

        [JsonProperty("reminders")]
        public Reminders Reminders { get; set; }
    }

    public partial class Creator
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("self")]
        public bool Self { get; set; }
    }

    public partial class End
    {
        [JsonProperty("dateTime")]
        public DateTimeOffset DateTime { get; set; }
    }

    public partial class Reminders
    {
        [JsonProperty("useDefault")]
        public bool UseDefault { get; set; }
        [JsonProperty("overrides")]
        public List<ReminderOverrides> Overrides { get; set; }
    }

    public partial class ReminderOverrides
    {
        [JsonProperty("method")]
        public string Method { get; set; }
        [JsonProperty("minutes")]
        public int Minutes { get; set; }
    }




}
