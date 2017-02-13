using System;

namespace Estuite.Example.Domain.Events
{
    public class AccountNameCorrected
    {
        public Guid AccountId { get; set; }
        public string Name { get; set; }
    }
}