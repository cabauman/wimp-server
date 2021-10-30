using System.Collections.Generic;

namespace WIMP_Server.Dtos.Users
{
    public class ReadApiKeyDto
    {
        public string Key { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}