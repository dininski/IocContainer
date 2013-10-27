using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VasContainer;

namespace IoCContainer
{
    public class Program
    {
        static void Main(string[] args)
        {
            var ct = new Container();
            ct.RegisterDependency(typeof(IPowerSource), typeof(PowerOutlet));
            var laptop1 = ct.Resolve<Laptop>();
            laptop1.DisplayPower();

            var ct2 = new Container();
            ct2.RegisterDependency(typeof(IPowerSource), typeof(Battery));
            var laptop2 = ct2.Resolve<Laptop>();
            laptop2.DisplayPower();
        }
    }

    public interface IPowerSource
    {
        int GetPower();
    }

    public class Battery : IPowerSource
    {
        public int GetPower()
        {
            return -1;
        }
    }

    public class PowerOutlet : IPowerSource
    {
        public int GetPower()
        {
            return 100000;
        }
    }

    public class Laptop
    {
        private IPowerSource source;

        public Laptop(IPowerSource powerSource)
        {
            this.source = powerSource;
        }

        public void DisplayPower()
        {
            Console.WriteLine(this.source.GetPower());
        }
    }
}
