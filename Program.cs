using System.Data;
using System.Security.Principal;
using System.Xml.Linq;

namespace dtp15_todolist
{
    public class Todo
    {
        public static List<TodoItem> list = new List<TodoItem>();

        public const int Active = 1;
        public const int Waiting = 2;
        public const int Ready = 3;
        public static string lastFile = "";

        public static string StatusToString(int status)
        {
            switch (status)
            {
                case Active: return "aktiv";
                case Waiting: return "väntande";
                case Ready: return "avklarad";
                default: return "(felaktig)";
            }
        }
        public class TodoItem
        {
            public int status;
            public int priority;
            public string task;
            public string taskDescription;
            public TodoItem(int priority, string task, string taskDescription)
            {
                this.status = Active;
                this.priority = priority;
                this.task = task;
                this.taskDescription = "";
            }
            public TodoItem(string todoLine)
            {
                string[] field = todoLine.Split('|');
                status = Int32.Parse(field[0]);
                priority = Int32.Parse(field[1]);
                task = field[2];
                taskDescription = field[3];
            }
            public void Print(bool verbose = false)
            {
                string statusString = StatusToString(status);
                Console.Write($"|{statusString,-12}|{priority,-6}|{task,-20}|");
                if (verbose)
                    Console.WriteLine($"{taskDescription,-40}|");
                else
                    Console.WriteLine();
            }
        }
        public static void ReadListFromFile(string command)
        {
            string[]cmd = command.Split(' ');
            string filename;

            if(cmd.Length > 1)
            {
                filename = $"{cmd[1]}.lis";
            }
            else
            {
                filename = "todo.lis";
            }           
            
            Console.Write($"Läser från fil {filename} ... ");
            StreamReader sr = new StreamReader(filename);
            int numRead = 0;

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                TodoItem item = new TodoItem(line);
                list.Add(item);
                numRead++;
            }
            sr.Close();
            Console.WriteLine($"Läste {numRead} rader.");
            Todo.lastFile = filename;
        }

        public static void changeStatus(string command)
        {
            string[] cwords = command.Split(' ');
            bool found = false;
            for (int i = 0; i < list.Count; i++)
            {                
                if (list[i].task == $"{cwords[1]} {cwords[2]}" && cwords.Length == 3 && list[i].status != Active && cwords[0] == "aktivera")
                {
                    list[i].status = 1;
                    Console.WriteLine($"Status ändrad på {list[i].task} till 'aktiv'");
                    found = true;
                    break;
                }                
                else if (list[i].task == $"{cwords[1]} {cwords[2]}" && cwords.Length == 3 && list[i].status != Waiting && cwords[0] == "vänta")
                {
                    list[i].status = 2;
                    Console.WriteLine($"Status ändrad på {list[i].task} till 'väntande'");
                    found = true;
                    break;
                }                
                else if (list[i].task == $"{cwords[1]} {cwords[2]}" && cwords.Length == 3 && list[i].status != Ready && cwords[0] == "klar")
                {
                    list[i].status = 3;
                    Console.WriteLine($"Status ändrad på {list[i].task} till 'klar'");
                    found = true;
                    break;
                }
            }
            if (found == false)
            {
                Console.WriteLine("Uppgift finns ej på listan eller så är uppgiften redan aktiv!");
            }
        }

        private static void PrintHeadOrFoot(bool head, bool verbose)
        {
            if (head)
            {
                Console.Write("|status      |prio  |namn                |");
                if (verbose) Console.WriteLine("beskrivning                             |");
                else Console.WriteLine();
            }
            Console.Write("|------------|------|--------------------|");
            if (verbose) Console.WriteLine("----------------------------------------|");
            else Console.WriteLine();
        }
        private static void PrintHead(bool verbose)
        {
            PrintHeadOrFoot(head: true, verbose);
        }
        private static void PrintFoot(bool verbose)
        {
            PrintHeadOrFoot(head: false, verbose);
        }
        public static void PrintTodoList(bool verbose = false, string aktiv = "")
        {
            PrintHead(verbose);            
            foreach (TodoItem item in list)
            {
                if (aktiv == "Active")
                {
                    if (item.status == Active)
                        item.Print(verbose);
                }
                else
                {
                    item.Print(verbose);
                } 
            }
            PrintFoot(verbose);
        }
        public static void PrintHelp()
        {            
            Console.WriteLine("Kommandon:");            
            Console.WriteLine("hjälp                lista denna hjälp");
            Console.WriteLine("ny                   skapa en ny uppgift");
            Console.WriteLine("beskriv              lista alla 'Active' uppgifter (status, prioritet, namn och beskrivning");
            Console.WriteLine("beskriv allt         lista alla uppgifter (status, prioritet, namn och beskrivning");
            Console.WriteLine("lista                lista alla 'Active' uppgifter (status, prioritet och namn");
            Console.WriteLine("lista allt           lista alla uppgifter(oavsett status), status, prioritet och namn");
            Console.WriteLine("spara                spara uppgifterna");
            Console.WriteLine("spara /fil/          spara uppgifterna till /fil/");
            Console.WriteLine("ladda                ladda listan todo.list");
            Console.WriteLine("ladda /fil/          ladda lista från va");
            Console.WriteLine("aktivera /uppgift/   sätt status på uppgift till 'Active'");
            Console.WriteLine("klar /uppgift/       sätt status på uppgift till 'Ready'");
            Console.WriteLine("vänta /uppgift/      sätt status på uppgift till 'Waiting'");
            Console.WriteLine("sluta                spara senast laddade filen och avsluta programmet!");
        }

        public static void spara(string save)
        {
            string[] cwords = save.Split(' ');
            string saveFile;
            if (cwords.Length == 1)
            {
                saveFile = "todo.lis";
            }
            else
            {
                saveFile = cwords[1];                
            }
            
            string fullpath = @$"{Environment.CurrentDirectory}\{saveFile}";            

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null && i == 0)
                {
                    string textprint = $"{list[i].status}|{list[i].priority}|{list[i].task}|{list[i].taskDescription}";
                    File.WriteAllText(fullpath, textprint + Environment.NewLine);                    
                }
                else if (list[i] != null & i > 0)
                {
                    string textprint = $"{list[i].status}|{list[i].priority}|{list[i].task}|{list[i].taskDescription}|";
                    File.AppendAllText(fullpath, textprint + Environment.NewLine);
                }
            }
            Console.WriteLine($"Du har sparat. Filnamn: '{saveFile}'");
        }
        public static void newEntry(string command)
        {
            string[] uppgift = command.Split(' ');
            
            if(uppgift.Length == 1)
            {
                int prio = Convert.ToInt32(MyIO.ReadCommand("Skriv in prioritet(1 - 4): "));
                string task = MyIO.ReadCommand("Skriv in uppgiftens namn: ");
                string taskDescription = MyIO.ReadCommand("Skriv in en uppgiftsbeskrivning");
                Todo.TodoItem item = new Todo.TodoItem(prio, task, taskDescription);
                Todo.list.Add(item);
                Console.WriteLine($"Uppgiften '{task}' (prio: {prio}) är nu tillagd!");
            }
            else
            {
                Console.WriteLine($"Skapa en ny uppgift med namn: '{uppgift[1]}'");
                int prio = Convert.ToInt32(MyIO.ReadCommand("Skriv in prioritet(1 - 4): "));
                string task = uppgift[1];
                string taskDescription = MyIO.ReadCommand("Skriv in en uppgiftsbeskrivning");
                Todo.TodoItem item = new Todo.TodoItem(prio, task, taskDescription);
                Todo.list.Add(item);
                Console.WriteLine($"Uppgiften '{task}' (prio: {prio}) är nu tillagd!");
            }            
        }
    }
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Välkommen till att-göra-listan!");            
            Todo.PrintHelp();            
            string command;            

            do
            {
                command = MyIO.ReadCommand("> ");
                if (MyIO.Equals(command, "hjälp"))
                {
                    Todo.PrintHelp();
                }

                else if (MyIO.Equals(command, "ny"))
                {
                    Todo.newEntry(command);
                }

                else if (MyIO.Equals(command, "beskriv"))
                {
                    if (MyIO.HasArgument(command, "allt"))
                        Todo.PrintTodoList(verbose: true);
                    else
                        Todo.PrintTodoList(verbose: true, "Active");
                }

                else if (MyIO.Equals(command, "lista"))
                {
                    if (MyIO.HasArgument(command, "allt"))
                        Todo.PrintTodoList(verbose: false);
                    else
                        Todo.PrintTodoList(verbose: false, "Active");
                }

                else if (MyIO.Equals(command, "spara"))
                {
                    Todo.spara(command);                    
                }

                else if (MyIO.Equals(command, "ladda"))
                {
                    Todo.ReadListFromFile(command);                   
                }

                else if (MyIO.Equals(command, "aktivera"))
                {
                    Todo.changeStatus(command);                    
                }

                else if (MyIO.Equals(command, "klar"))
                {
                    Todo.changeStatus(command);
                }

                else if (MyIO.Equals(command, "vänta"))
                {
                    Todo.changeStatus(command);
                }
                
                else if (MyIO.Equals(command, "sluta"))
                {                    
                    Todo.spara($"spara {Todo.lastFile}");
                    Console.WriteLine("Programmet avslutas!");
                    break;                                    
                }
                
                else
                {
                    Console.WriteLine($"Okänt kommando: {command}");
                }
            }
            while (true);
        }        
    }    
}