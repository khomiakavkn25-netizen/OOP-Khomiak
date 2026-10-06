using System;

namespace Lab6
{
    public class Person
    {
        private string name;
        private int age;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Age
        {
            get { return age; }
            set { age = value; }
        }

        public Person(string name, int age)
        {
            this.name = name;
            this.age = age;
        }

        public virtual void Greet()
        {
            Console.WriteLine("Вітаю, я " + name + ", мені " + age + " років.");
        }

        public string GetRole()
        {
            return "Людина (Person)";
        }
    }

    public class Student : Person
    {
        public string StudentId { get; set; }

        public Student(string name, int age, string studentId) : base(name, age)
        {
            StudentId = studentId;
        }

        public override void Greet()
        {
            Console.WriteLine($"Привіт! Я студент {Name}, студ. квиток: {StudentId}");
        }

        public void Study()
        {
            Console.WriteLine($"{Name} сидить і вчить лабу з ООП.");
        }

        public new string GetRole()
        {
            return "Студент (Student)";
        }
    }

    public class Professor : Person
    {
        public string Department { get; set; }

        public Professor(string name, int age, string department) : base(name, age)
        {
            Department = department;
        }

        public override void Greet()
        {
            Console.WriteLine($"Доброго дня, я викладач {Name} з кафедри {Department}.");
        }

        public void Teach()
        {
            Console.WriteLine($"Професор {Name} читає лекцію.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Person p1 = new Person("Олексій", 40);
            Student st1 = new Student("Іван", 19, "KB12345");
            Professor prof1 = new Professor("Микола Петрович", 52, "ІПЗ");

            st1.Study();
            prof1.Teach();

            Console.WriteLine();

            Person[] list = new Person[] { p1, st1, prof1 };

            for (int i = 0; i < list.Length; i++)
            {
                list[i].Greet();
            }

            Console.WriteLine();

            Student st2 = new Student("Марія", 18, "KB54321");
            Person pRef = st2;

            st2.Greet();
            Console.WriteLine(st2.GetRole());

            pRef.Greet();
            Console.WriteLine(pRef.GetRole());

            Console.ReadLine();
        }
    }
}