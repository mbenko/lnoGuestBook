using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lnoAzureWebApp.Models
{
    public class GuestBookEntry 
    {

        public string Id { get; set; }
        public string Message { get; set; }
        public string GuestName { get; set; }
        public string PhotoUrl { get; set; }
        public string ThumbUrl { get; set; }
        public DateTime CreateDt { get; set; }
        public GuestBookEntry()
        {
            Id = Guid.NewGuid().ToString();
            CreateDt = DateTime.UtcNow;
        }
    }
}