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
            public TodoItem(int priority, string task)
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
        public static void ReadListFromFile()
        {
            string todoFileName = "todo.lis";
            Console.Write($"Läser från fil {todoFileName} ... ");
            StreamReader sr = new StreamReader(todoFileName);
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
            Console.WriteLine("lista                lista alla 'Active' uppgifter (status, prioritet, namn och beskrivning");
            Console.WriteLine("lista allt           lista alla uppgifter(oavsett status), status, prioritet och namn");
            Console.WriteLine("spara                spara uppgifterna");
            Console.WriteLine("ladda                ladda listan todo.list");
            Console.WriteLine("aktivera /uppgift/   ladda listan todo.list");
            Console.WriteLine("klar /uppgift/       ladda listan todo.list");
            Console.WriteLine("vänta /uppgift/      sätt status på uppgift till 'Waiting'");
            Console.WriteLine("sluta                spara senast laddade filen och avsluta programmet!");
        }

        public static void saveOptions(string save)
        {            
            string folder = $@"{Environment.CurrentDirectory}";
            string fullpath = @$"{folder}\{save}";            

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
            Console.WriteLine($"You have saved your progress. Filename: '{save}'");
        }
    }
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Välkommen till att-göra-listan!");            
            Todo.PrintHelp();            
            string command;
            bool ladda = false;
            do
            {
                command = MyIO.ReadCommand("> ");
                if (MyIO.Equals(command, "hjälp"))
                {
                    Todo.PrintHelp();
                }

                else if (MyIO.Equals(command, "ny"))
                {
                    int prio = Convert.ToInt32(MyIO.ReadCommand("Skriv in prioritet(1 - 4): "));                    
                    string task = MyIO.ReadCommand("Skriv in uppgift: ");
                    Todo.TodoItem item = new Todo.TodoItem(prio, task);
                    Todo.list.Add(item);
                    Console.WriteLine($"Uppgiften '{task}' (prio: {prio}) är nu tillagd!");
                }
                
                else if (MyIO.Equals(command, "beskriv"))
                {
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
                    Todo.saveOptions("todo.lis");
                }

                else if (MyIO.Equals(command, "ladda"))
                {
                    Todo.ReadListFromFile();
                    ladda = true;
                }

                else if (MyIO.Equals(command, "aktivera"))
                {
                    Console.WriteLine("Aktiverar inte än!");
                }

                else if (MyIO.Equals(command, "klar"))
                {
                    Console.WriteLine("Klarar inte än!");
                }

                else if (MyIO.Equals(command, "vänta"))
                {
                    Console.WriteLine("Väntar inte än!");
                }
                
                else if (MyIO.Equals(command, "sluta"))
                {
                    Console.WriteLine("Hej då!");
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