using System;

namespace Estuite.Example.Events
{
    public class AccountRegistered
    {
        public Guid AccountId { get; set; }
        public string Name { get; set; }
    }
}