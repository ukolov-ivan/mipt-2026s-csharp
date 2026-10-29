class CarDescriptionApp
{
    public interface ICar
    {
        string GetDescription();
    }

    // NOTE: More accurate translation. Slightly divergent from the task.

    // Architecturally it would make more sense for these interfaces to
    // be some abstract properties of `ACar` (with enums `EngineType`,
    // `TransmissionType` for example), since they *are* just a part of 
    // the descriptions of concrete cars, and not of a brand/mark.
    // But the task requires to use them as marker interfaces.
    public interface IElectric { }
    public interface IPetrol { }

    public interface IAutomatic { }
    public interface IManual { }

    internal abstract class ACar : ICar
    {
        protected string Brand { get; }
        protected int Horsepower { get; }
        protected string BodyType { get; }

        protected ACar(string brand, int horsepower, string bodyType)
        {
            Brand = brand;
            Horsepower = horsepower;
            BodyType = bodyType;
        }

        public virtual string GetDescription()
        {
            string engine = this switch
            {
                IElectric => "электрический",
                IPetrol => "бензиновый",
                _ => "неизвестного типа"
            };

            string transmission = this switch
            {
                IAutomatic => "автоматической",
                IManual => "механической",
                _ => "неизвестной"
            };
            return $"{Brand}: {engine} автомобиль с {transmission} коробкой передач, мощностью {Horsepower} л.с., тип кузова: {BodyType}";
        }
    }

    // Tesla Model S Plaid
    internal class Tesla : ACar, IElectric, IAutomatic
    {
        public Tesla() : base("Tesla", 1020, "лифтбек") { }
    }

    // BMW M5
    internal class BMW : ACar, IPetrol, IAutomatic
    {
        public BMW() : base("BMW", 600, "седан") { }
    }

    // Toyota Corolla
    internal class Toyota : ACar, IPetrol, IManual
    {
        public Toyota() : base("Toyota", 132, "седан") { }
    }

    // Volkswagen Golf R
    internal class Volkswagen : ACar, IPetrol, IAutomatic
    {
        public Volkswagen() : base("Volkswagen", 320, "хэтчбек") { }
    }

    // Mercedes-Benz G-Class
    internal class Mercedes : ACar, IPetrol, IAutomatic
    {
        public Mercedes() : base("Mercedes-Benz", 577, "внедорожник") { }
    }

    // Volvo V60
    internal class Volvo : ACar, IPetrol, IAutomatic
    {
        public Volvo() : base("Volvo", 250, "универсал") { }
    }

    public enum CarType
    {
        Tesla,
        BMW,
        Toyota,
        Volkswagen,
        Mercedes,
        Volvo,
    }

    public static class CarFactory
    {
        public static ICar CreateCar(CarType carType)
        {
            return carType switch
            {
                CarType.Tesla => new Tesla(),
                CarType.BMW => new BMW(),
                CarType.Toyota => new Toyota(),
                CarType.Volkswagen => new Volkswagen(),
                CarType.Mercedes => new Mercedes(),
                CarType.Volvo => new Volvo(),
                _ => throw new ArgumentException("Машина не найдена.")
            };
        }

        public static bool TryCreateCar(string input, out ICar carType)
        {
            carType = null;
            // For our toy program we assume that all names are just exact no-whitespace strings
            if (Enum.TryParse<CarType>(input, true, out CarType carType) && Enum.IsDefined(typeof(CarType), carType))
            {
                carType = CreateCar(carType);
                return true;
            }
            return false;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Введите марку автомобиля или 'done' для остановки ввода:");

                string input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input)) continue;
                if (input.ToLower() == "done") break;

                if (CarFactory.TryCreateCar(input, out ICar car))
                {
                    Console.WriteLine($"{car.GetDescription()}");
                }
                else
                {
                    Console.WriteLine("Ошибка. Попробуйте снова.");
                }
            }
            Console.WriteLine();
        }
    }
}
