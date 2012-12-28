namespace OShell.Core.Commands
{
    using System;
    using System.Collections.Generic;

    using OShell.Core.Contracts;

    public class DefaultCommandProvider : ICommandProvider
    {
        private static readonly Dictionary<string, Tuple<Type, Type>> CommandMap
            = new Dictionary<string, Tuple<Type, Type>>
                  {
                      { "newkmap", Tuple.Create(typeof(NewKMapCommand), typeof(NewKMapCommandHandler)) },
                      { "delkmap", Tuple.Create(typeof(DelKMapCommand), typeof(DelKMapCommandHandler)) }
                  };

        public DefaultCommandProvider(
            Func<Type, ICommand> commandFactory, Func<Type, ICommandHandler<ICommand>> commandHandlerFactory)
        {
            this.CommandFactory = commandFactory;
            this.CommandHandlerFactory = commandHandlerFactory;
        }

        private Func<Type, ICommand> CommandFactory { get; set; }

        private Func<Type, ICommandHandler<ICommand>> CommandHandlerFactory { get; set; }

        public ICommand GetCommand(string command)
        {
            return this.CommandFactory(CommandMap[command].Item1);
        }

        public Object GetCommandHandler(string command)
        {
            return this.CommandHandlerFactory(CommandMap[command].Item2);
        }

        public bool HasCommand(string command)
        {
            return CommandMap.ContainsKey(command);
        }
    }
}