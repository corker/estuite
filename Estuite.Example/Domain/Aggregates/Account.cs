using System;
using Estuite.Domain;
using Estuite.Example.Domain.Events;

namespace Estuite.Example.Domain.Aggregates
{
    public class Account : Aggregate<Guid>
    {
        private string _name;

        public Account(Guid id) : base(id)
        {
        }

        public static Account Register(Guid accountId, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            var aggregate = new Account(accountId);
            aggregate.Apply<AccountRegistered>(x =>
            {
                x.AccountId = accountId;
                x.Name = name;
            });
            return aggregate;
        }

        public void ChangeName(string name)
        {
            if (_name == name) return;
            Apply<AccountNameCorrected>(x =>
            {
                x.AccountId = Id;
                x.Name = name;
            });
        }

        private void Handle(AccountRegistered @event)
        {
            _name = @event.Name;
        }

        private void Handle(AccountNameCorrected @event)
        {
            _name = @event.Name;
        }
    }
}