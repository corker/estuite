using System;
using Estuite.Domain;
using Estuite.Example.Domain.Events;

namespace Estuite.Example.Domain.Aggregates
{
    public class Account : Aggregate<Guid>
    {
        private string _name;

        protected Account(Guid id) : base(id)
        {
        }

        public void Register(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (_name != null) throw new InvalidOperationException("Account was already registered");

            Apply<AccountRegistered>(x =>
            {
                x.AccountId = Id;
                x.Name = name;
            });
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