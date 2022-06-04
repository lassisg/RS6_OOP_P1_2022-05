﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RSGymPT
{

    internal class CLI
    {
        #region Enums

        enum EnumArgumentType
        {
            None,
            Text,
            Date,
            Hour,
            Request
        }
        
        #endregion
        
        #region Structs

        internal struct Command
        {
            internal string TheCommand;     // help, exit, clear, login, request, cancel, finish, message, myrequest, requests
            internal string Description;    // Command description for help
            internal List<CommandOption> Options;
            internal string Example;        // Example of the command use
        }

        internal struct CommandOption
        {
            internal char Option;           // u, p, n, d, h, r, s, a
            internal string Description;    // username, password, name, day, hour, request nº, subject
            internal bool NeedsValue;       // identifica se o parâmetro precisa de um valor (requests -a não precisa)
            internal string Value;          // Reads from command input
        }

        #endregion

        #region Properties

        internal List<Command> Commands { get; set; }
        internal bool Session { get; set; }
        internal User ActiveUser { get; set; }

        #endregion

        #region Constructors & related

        internal CLI()
        {
            Session = false;
            Commands = BuildCommands();
        }

        private List<Command> BuildCommands()
        {

            List<CommandOption> commandOptions = BuildCommandOptions();

            List<Command> commands = new List<Command>(){
                new Command(){
                    TheCommand = "help",
                    Description = "Lista os comandos disponíveis na CLI.",
                    Options = new List<CommandOption>(),
                    Example = "help"
                },
                new Command(){
                    TheCommand = "exit",
                    Description = "Sai da aplicação.",
                    Options = new List<CommandOption>(),
                    Example= "exit"
                },
                new Command(){
                    TheCommand = "clear",
                    Description = "Limpa a consola.",
                    Options = new List<CommandOption>(),
                    Example = "clear"
                },
                new Command(){
                    TheCommand = "login",
                    Description = "Faz o login na aplicação.",
                    Options = new List<CommandOption>{
                        commandOptions.Find(c => c.Option == 'u'),
                        commandOptions.Find(c => c.Option == 'p')
                    },
                    Example = "login -u {username} -p {password}"
                },
                new Command(){
                    TheCommand = "logout",
                    Description = "Faz o login da aplicação.",
                    Options = new List<CommandOption>(),
                    Example = "logout"
                },
                new Command(){
                    TheCommand = "request",
                    Description = "Faz o pedido do PT indicando: nome, data e hora. Não pode haver pedidos duplicados.",
                    Options = new List<CommandOption>{
                        commandOptions.Find(c => c.Option == 'n'),
                        commandOptions.Find(c => c.Option == 'd'),
                        commandOptions.Find(c => c.Option == 'h')
                    },
                    Example = "request -n {nome} -d {data} -h {hora}"
                },
                new Command(){
                    TheCommand = "cancel",
                    Description = "Anula o pedido.",
                    Options = new List<CommandOption>{
                        commandOptions.Find(c => c.Option == 'r')
                    },
                    Example = "cancel -r {nº pedido}"
                },
                new Command(){
                    TheCommand = "finish",
                    Description = "Mensagem automática com estado 'aula concluída', data e horas automáticas.",
                    Options = new List<CommandOption>{
                        commandOptions.Find(c => c.Option == 'r')
                    },
                    Example = "finish -r {nº pedido}"
                },
                new Command(){
                    TheCommand = "message",
                    Description = "Mensagem a informar o motivo, data e horas automáticas.",
                    Options = new List<CommandOption>{
                        commandOptions.Find(c => c.Option == 'r'),
                        commandOptions.Find(c => c.Option == 's')
                    },
                    Example = "message -r {nº pedido} -s {assunto}"
                },
                new Command(){
                    TheCommand = "myrequest",
                    Description = "Lista o pedido efetuado. Validar a existência do pedido.",
                    Options = new List<CommandOption>{
                        commandOptions.Find(c => c.Option == 'r')
                    },
                    Example = "myrequest -r {nº pedido}"
                },
                new Command(){
                    TheCommand = "requests",
                    Description = "Lista todos os pedidos efetuados.",
                    Options = new List<CommandOption>{
                        commandOptions.Find(c => c.Option == 'a')
                    },
                    Example = "requests -a"
                }
            };

            return commands;

        }

        private List<CommandOption> BuildCommandOptions()
        {

            return new List<CommandOption> {
                new CommandOption {
                    Option = 'u',
                    Description = "Utilzador",
                    NeedsValue = true,
                    Value = string.Empty
                },
                new CommandOption {
                    Option = 'p',
                    Description = "Palavra passe",
                    NeedsValue = true,
                    Value = string.Empty
                },
                new CommandOption {
                    Option = 'n',
                    Description = "Nome",
                    NeedsValue = true,
                    Value = string.Empty
                },
                new CommandOption {
                    Option = 'd',
                    Description = "Data",
                    NeedsValue = true,
                    Value = string.Empty
                },
                new CommandOption {
                    Option = 'h',
                    Description = "hora",
                    NeedsValue = true,
                    Value = string.Empty
                },
                new CommandOption {
                    Option = 'r',
                    Description = "nº pedido",
                    NeedsValue = true,
                    Value = string.Empty
                },
                new CommandOption {
                    Option = 's',
                    Description = "Assunto",
                    NeedsValue = true,
                    Value = string.Empty
                },
                new CommandOption {
                    Option = 'a',
                    Description = "Todos",
                    NeedsValue = false,
                    Value = string.Empty
                }
            };

        }

        #endregion

        #region General Methods

        private List<User> GetUsers()
        {
            // TODO: Change values
            List<User> users = new List<User>()
            {
                new User(1, "1", "1"),
                new User(2, "2", "2")
            };

            //List<User> users = new List<User>();
            //string filePath = $"users.csv";
            //StreamReader reader = null;
            //if (File.Exists(filePath))
            //{
            //    reader = new StreamReader(File.OpenRead(filePath));
            //    string line = reader.ReadLine();
            //    List<string> listA = new List<string>();
            //    while (!reader.EndOfStream)
            //    {
            //        line = reader.ReadLine();
            //        string[] values = line.Split(',');
            //        users.Add(new User(
            //            int.Parse(values[0]), 
            //            values[1].Replace("\"",""), 
            //            values[2].Replace("\"", "")));
            //    }
            //}

            return users;

        }

        internal bool Run(string[] args)
        {
            bool commandExists = ValidateCommand(args[0]);
            bool isExit = false;
            string errorMessage;
            Dictionary<EnumArgumentType, string> arguments = new Dictionary<EnumArgumentType, string>();

            if (!commandExists)
                throw new NotSupportedException("Comando inválido!");

            switch (args[0])
            {
                case "help":
                    Help();
                    break;

                case "exit":
                    isExit = true;
                    break;

                case "clear":
                    Clear();
                    break;

                case "login":
                    if (!ValidateArguments(args))
                        throw new ArgumentException("Parâmetros do comando incorretos.");
                    
                    Login(args);
                    break;

                case "logout":
                    if (!SessionActive())
                        throw new UnauthorizedAccessException("Não há utilizador com sessão ativa.");

                    Logout();
                    break;

                case "request":
                    if (!SessionActive())
                        throw new UnauthorizedAccessException();
                    
                    if (!ValidateArguments(args))
                        throw new ArgumentException("Parâmetros do comando incorretos.");

                    arguments.Add(EnumArgumentType.Text, args[1].Split(new[] { ' ' }, 2)[1].Replace("\"", ""));
                    arguments.Add(EnumArgumentType.Date, args[2].Split()[1]);
                    arguments.Add(EnumArgumentType.Hour, args[3].Split()[1]);

                    errorMessage = HasValidArgumentValues(arguments);
                    if (errorMessage != string.Empty)
                        throw new ArgumentException();   
                    
                    // TODO: Code request
                    Console.WriteLine("CreateRequest();");

                    break;

                case "cancel":
                    if (!SessionActive())
                        throw new UnauthorizedAccessException();

                    if (!ValidateArguments(args))
                        throw new ArgumentException("Parâmetros do comando incorretos.");

                    arguments.Add(EnumArgumentType.Request, args[1].Split()[1]);

                    errorMessage = HasValidArgumentValues(arguments);
                    if (errorMessage != string.Empty)
                        throw new ArgumentException();

                    // TODO: Code cancel
                    Console.WriteLine("CancelRequest();");
                    
                    break;

                case "finish":
                    if (!SessionActive())
                        throw new UnauthorizedAccessException();

                    if (!ValidateArguments(args))
                        throw new ArgumentException("Parâmetros do comando incorretos.");

                    arguments.Add(EnumArgumentType.Request, args[1].Split()[1]);

                    errorMessage = HasValidArgumentValues(arguments);
                    if (errorMessage != string.Empty)
                        throw new ArgumentException();

                    // TODO: Code finish
                    Console.WriteLine("FinishRequest();");
                    
                    break;

                case "message":
                    if (!SessionActive())
                        throw new UnauthorizedAccessException();

                    if (!ValidateArguments(args))
                        throw new ArgumentException("Parâmetros do comando incorretos.");

                    arguments.Add(EnumArgumentType.Request, args[1].Split()[1]);
                    arguments.Add(EnumArgumentType.Text, args[2].Split(new[] { ' ' }, 2)[1].Replace("\"", ""));

                    errorMessage = HasValidArgumentValues(arguments);
                    if (errorMessage != string.Empty)
                        throw new ArgumentException();

                    // TODO: Code message
                    Console.WriteLine("AddMessage();");
                    
                    break;

                case "myrequest":
                    if (!SessionActive())
                        throw new UnauthorizedAccessException();

                    if (!ValidateArguments(args))
                        throw new ArgumentException("Parâmetros do comando incorretos.");

                    arguments.Add(EnumArgumentType.Request, args[1].Split()[1]);

                    errorMessage = HasValidArgumentValues(arguments);
                    if (errorMessage != string.Empty)
                        throw new ArgumentException();

                    // TODO: Code myrequest
                    Console.WriteLine("GetRequest();");
                    
                    break;

                case "requests":
                    if (!SessionActive())
                        throw new UnauthorizedAccessException();

                    if (!ValidateArguments(args))
                        throw new ArgumentException("Parâmetros do comando incorretos.");

                    // TODO: Code requests
                    Console.WriteLine("ListRequests();");
                    break;

                default:
                    break;
            }

            return isExit;

        }

        #endregion

        #region Command methods

        private void Help()
        {

            StringBuilder sb = new StringBuilder();

            sb.AppendLine();
            foreach (var command in Commands)
            {
                sb.Append($"{command.TheCommand}".PadRight(12));
                sb.AppendLine($"{command.Description}");

                if (command.Options.Count > 0)
                {

                    sb.AppendLine();
                    sb.Append("".PadRight(12));
                    sb.AppendLine("Parâmetros:");

                    foreach (var option in command.Options)
                    {
                        sb.Append("".PadRight(12));
                        sb.Append($"  -{option.Option}".PadRight(7));
                        sb.AppendLine($"{option.Description}");
                    }
                }

                sb.AppendLine();
                sb.Append("".PadRight(12));
                sb.AppendLine("Examplo de uso:");
                sb.Append("".PadRight(12));
                sb.AppendLine($"  {command.Example}\n");
            }

            Console.WriteLine(sb.ToString());

        }

        private void Clear()
        {
            Console.Clear();
        }

        private void Login(string[] args)
        {
            User currentUser = new User();
            currentUser.UserName = args[1].Split()[1];
            currentUser.Password = args[2].Split()[1];

            if (!ValidateCredentials(currentUser))
                throw new UnauthorizedAccessException("Utilizador ou palavra passe errado. Verfique os dados de login.");

            if (SessionActive())
                throw new UnauthorizedAccessException("Não foi possível efetuar o login. Já há um utilizador com sessão ativa.");

            ActiveUser = currentUser;
            Session = true;
        }

        private void Logout()
        {
            ActiveUser = null;
            Session = false;
        }

        private void Request()
        {

        }

        private void Cancel()
        {

        }

        private void Finish()
        {

        }

        private void Message()
        {

        }

        private void GetRequest()
        {

        }

        private void ListRequests()
        {

        }

        #endregion

        #region Validation methods

        private string HasValidArgumentValues(Dictionary<EnumArgumentType, string> arguments)
        {
            StringBuilder errorMessage = new StringBuilder();

            foreach (KeyValuePair<EnumArgumentType, string> argument in arguments)
            {
                switch (argument.Key)
                {
                    case EnumArgumentType.None:
                        break;

                    case EnumArgumentType.Text:
                        if (!IsValidText(argument.Value))
                            errorMessage.AppendLine("Formato do nome inválido.");
                        break;

                    case EnumArgumentType.Date:
                        if (!IsValidDate(argument.Value))
                            errorMessage.AppendLine("Formato da data inválido.");
                        break;

                    case EnumArgumentType.Hour:
                        if (!IsValidHour(argument.Value))
                            errorMessage.AppendLine("Formato da hora inválido.");
                        break;

                    case EnumArgumentType.Request:
                        if (!IsValidRequest(argument.Value))
                            errorMessage.AppendLine("Formato do nº do pedido inválido.");
                        break;

                    default:
                        break;
                }
            }

            return errorMessage.ToString();

        }

        private bool IsValidText(string inputText)
        {
            string textPattern = "[^\"]*[a-zA-Z]+[^\"]*$";
            Regex rgText = new Regex(textPattern);
            Match textMatch = rgText.Match(inputText);

            return textMatch.Success;
        }

        private bool IsValidDate(string inputDate)
        {
            string datePattern = @"^(0[1-9]|[12][0-9]|3[01])([ /.])(0[1-9]|1[012])([ /.])(19|20)\d\d$";
            Regex rgDate = new Regex(datePattern);
            Match dateMatch = rgDate.Match(inputDate);
            
            return dateMatch.Success;
        }

        private bool IsValidHour(string inputHour)
        {
            string hourPattern = @"^([0-1][0-9]|2[0-3]):[0-5][0-9]$";
            Regex rgHour = new Regex(hourPattern);
            Match hourMatch = rgHour.Match(inputHour);

            return hourMatch.Success;
        }

        private bool IsValidRequest(string inputRequest)
        {
            string requestPattern = @"^\d{1,7}$";
            Regex rgRequest = new Regex(requestPattern);
            Match requestMatch = rgRequest.Match(inputRequest);

            return requestMatch.Success;
        }

        private bool SessionActive()
        {
            if (ActiveUser == null)
                return false;

            return true;
        }

        private bool ValidateCredentials(User user)
        {
            List<User> users = GetUsers();
            bool validCredentials = users.Exists(u => u.UserName == user.UserName && u.Password == user.Password);

            return validCredentials;
        }

        private bool ValidateCommand(string command)
        {
            Command currentCommand = Commands.Find(c => c.TheCommand == command);
            bool isValid = (currentCommand.TheCommand == null) ? false : true;

            return isValid;
        }

        private bool ValidateArguments(string[] commandArgs)
        {
            Command currentCommand = Commands.Find(c => c.TheCommand == commandArgs[0]);
            char currentOption;
            bool hasAllArguments;
            bool isValidArgument = false;

            // Valida se o apenas o comando existe, sem os parâmetros
            if (commandArgs.Length == 1)
                return isValidArgument;
            
            // Valida se possui a quantidade correta de parâmetros
            int currentCommandArguments = currentCommand.Options.Count;
            hasAllArguments = commandArgs.Length - 1 == currentCommandArguments;

            if (!hasAllArguments)
                return isValidArgument;

            for (int i = 1; i < commandArgs.Length; i++)
            {
                // Valida se os parâmetros utilizados correspondem aos parâmetros esperados
                currentOption = char.Parse(commandArgs[i].Split(' ')[0]);
                isValidArgument = currentCommand.Options
                    .Find(o => o.Option == currentOption).Option != 0;

                if (!isValidArgument)
                    return isValidArgument;

                // Valida se os parâmetros utilizados possuem valor associado
                char providedOption = commandArgs[i].Split()[0].ToCharArray()[0];
                bool needsValue = currentCommand.Options.Find(o => o.Option == providedOption).NeedsValue;
                if (needsValue)
                    isValidArgument = commandArgs[i].Split().Length > 1;
                else
                    isValidArgument = commandArgs[i].Split().Length == 1;

                if (!isValidArgument)
                    return isValidArgument;
            }
            
            return isValidArgument;
        }

        #endregion

    }

}
